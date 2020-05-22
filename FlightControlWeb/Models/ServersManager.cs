using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Caching.Memory;
using System.Threading.Tasks;
using System.Collections;
using System.Net;
using System.IO;
using Newtonsoft.Json;
using System.Globalization;

namespace FlightControlWeb.Models
{
    public class ServersManager : IServersManager
    {
        private IMemoryCache myCache;
        private object listLock;
        private object dictLock;

        public ServersManager(IMemoryCache cache)
        {
            myCache = cache;
            listLock = new object();
            dictLock = new object();
        }

        public FlightPlan GetExternalPlan(string id)
        {
            var externalFlightIds = GetExternalFlightIds();
            if (externalFlightIds.ContainsKey(id))
            {
                Server server = externalFlightIds[id];
                FlightPlan flightPlan = GetFlightPlanFromServer(server, id);
                return flightPlan;
            }
            return null;
        }

        private string GetSerializedObject(string url)
        {
            try
            {
                var requestObject = WebRequest.Create(url);
                requestObject.Method = "GET";
                HttpWebResponse responseObject = null;
                responseObject = (HttpWebResponse)requestObject.GetResponse();
                string result = null;
                using (Stream stream = responseObject.GetResponseStream())
                {
                    var streamReader = new StreamReader(stream);
                    result = streamReader.ReadToEnd();
                    streamReader.Close();
                }
                return result;
            }
            catch
            {
                return "";
            }
        }

        private FlightPlan GetFlightPlanFromServer(Server server, string id)
        {
            string url = server.ServerURL + "/api/FlightPlan/" + id;
            string flightPlan = GetSerializedObject(url);
            if (flightPlan == "")
            {
                return null;
            }
            return JsonConvert.DeserializeObject<FlightPlan>(flightPlan);
        }

        private ArrayList GetFlightsFromServer(Server server, DateTime dateTime)
        {
            string dateTimeString = dateTime.ToString("s", DateTimeFormatInfo.InvariantInfo) + "Z";
            string url = server.ServerURL + "/api/Flights?relative_to=" + dateTimeString;
            string flights = GetSerializedObject(url);
            if (flights == "")
            {
                return new ArrayList();
            }
            var flightsList = JsonConvert.DeserializeObject<List<Flight>>(flights);
            return new ArrayList(flightsList);
        }

        public Tuple<bool, ArrayList> GetExternalFlights(DateTime dateTime)
        {
            var myExternalFlights = new ArrayList();
            var serversList = GetServersList();
            int listSize = serversList.Count;
            var tasks = new Task[listSize];
            int i = 0;
            bool failed = false;
            foreach (Server server in serversList)
            {
                tasks[i] = Task.Factory.StartNew(() =>
                {
                    try
                    {
                        AddFlightsFromServer(server, dateTime, myExternalFlights);
                    }
                    catch
                    {
                        failed = true;
                        myExternalFlights = new ArrayList();
                        return;
                    }
                });
                i++;
            }
            Task.WaitAll(tasks);
            return new Tuple<bool, ArrayList>(failed, myExternalFlights);
        }

        private void AddFlightsFromServer(Server server, DateTime dateTime, ArrayList myExternalFlights)
        {
            var flights = new ArrayList();
            var serverFlights = GetFlightsFromServer(server, dateTime);
            foreach (Flight flight in serverFlights)
            {
                flight.IsExternal = true;
                flights.Add(flight);
                lock (dictLock)
                {
                    var externalFlightIds = GetExternalFlightIds();
                    string id = flight.FlightId;
                    if (!externalFlightIds.ContainsKey(id))
                    {
                        externalFlightIds[id] = server;
                        SaveExternalFlightIds(externalFlightIds);
                    }
                }
            }
            lock (listLock)
            {
                myExternalFlights.AddRange(serverFlights);
            }
        }

        public List<Server> GetServersList()
        {
            List<Server> serversList;
            if (!myCache.TryGetValue("serversList", out serversList))
            {
                if (serversList == null)
                {
                    serversList = new List<Server>();
                }
            }
            SaveServersList(serversList);
            return serversList;
        }

        public Dictionary<string, Server> GetExternalFlightIds()
        {
            Dictionary<string, Server> externalFlightIds;
            if (!myCache.TryGetValue("externalFlightIds", out externalFlightIds))
            {
                if (externalFlightIds == null)
                {
                    externalFlightIds = new Dictionary<string, Server>();
                }
            }
            SaveExternalFlightIds(externalFlightIds);
            return externalFlightIds;
        }

        public void AddServer(Server server)
        {
            var serversList = GetServersList();
            serversList.Add(server);
            SaveServersList(serversList);
        }

        private void SaveServersList(List<Server> serversList)
        {
            myCache.Set("serversList", serversList);
        }

        private void SaveExternalFlightIds(Dictionary<string, Server> externalFlightIds)
        {
            myCache.Set("externalFlightIds", externalFlightIds);
        }

        public bool DeleteServer(string id)
        {
            bool deleted = false;
            var serversList = GetServersList();
            var externalFlightIds = GetExternalFlightIds();
            var dictInverse = externalFlightIds.ToDictionary((i) => i.Value, (i) => i.Key);
            Server serverToDelete = null;
            foreach (Server server in serversList)
            {
                if (server.ServerId == id)
                {
                    serverToDelete = server;
                    break;
                }
            }
            if (serverToDelete != null)
            {
                if (dictInverse.ContainsKey(serverToDelete))
                {
                    string flightId = dictInverse[serverToDelete];
                    externalFlightIds.Remove(flightId);
                    SaveExternalFlightIds(externalFlightIds);
                }
                serversList.Remove(serverToDelete);
                SaveServersList(serversList);
                deleted = true;
            }
            return deleted;
        }
    }
}
