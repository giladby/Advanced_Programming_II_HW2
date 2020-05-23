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
            // create 2 locks
            listLock = new object();
            dictLock = new object();
        }

        // get a flight plan with the given id, return false if failed
        public Tuple<bool, FlightPlan> GetExternalPlan(string id)
        {
            bool failed = false;
            FlightPlan flightPlan;
            var externalFlightIds = GetExternalFlightIds();
            if (externalFlightIds.ContainsKey(id))
            {
                // get the server that has the given flight
                Server server = externalFlightIds[id];
                try
                {
                    flightPlan = GetFlightPlanFromServer(server, id);
                }
                catch
                {
                    failed = true;
                    flightPlan = null;
                }
                return new Tuple<bool, FlightPlan>(failed, flightPlan);
            } else
            {
                return null;
            }
        }

        // get the serialized object from the given url with a 'GET' request
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

        // get the flight plan with the given id from the given server
        private FlightPlan GetFlightPlanFromServer(Server server, string id)
        {
            string url = server.ServerURL + "/api/FlightPlan/" + id;
            string flightPlan = GetSerializedObject(url);
            if (flightPlan == "")
            {
                return null;
            }
            // deserialize the serialized object to a flight plan object
            return JsonConvert.DeserializeObject<FlightPlan>(flightPlan);
        }

        // get all the flights relative to the given dateTime from the given server
        private ArrayList GetFlightsFromServer(Server server, DateTime dateTime)
        {
            string dateTimeString = dateTime.ToString(
                "s", DateTimeFormatInfo.InvariantInfo) + "Z";
            // creates a flights 'GET' request with a relative_to time
            string url = server.ServerURL + "/api/Flights?relative_to=" + dateTimeString;
            string flights = GetSerializedObject(url);
            if (flights == "")
            {
                return new ArrayList();
            }
            // deserialize the serialized object to a flights list
            var flightsList = JsonConvert.DeserializeObject<List<Flight>>(flights);
            return new ArrayList(flightsList);
        }

        // get all the flights relative to the given dateTime, return false if failed
        public Tuple<bool, ArrayList> GetExternalFlights(DateTime dateTime)
        {
            var myExternalFlights = new ArrayList();
            var serversList = GetServersList();
            int listSize = serversList.Count;
            // creates a task array
            var tasks = new Task[listSize];
            int i = 0;
            bool failed = false;
            foreach (Server server in serversList)
            {
                // add a task to the task array
                tasks[i] = Task.Factory.StartNew(() =>
                {
                    try
                    {
                        // the task adds all the flights of that server
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
            // wait for all the tasks to finish
            Task.WaitAll(tasks);
            return new Tuple<bool, ArrayList>(failed, myExternalFlights);
        }

        private void AddFlightsFromServer(Server server, DateTime dateTime,
            ArrayList myExternalFlights)
        {
            var flights = new ArrayList();
            var serverFlights = GetFlightsFromServer(server, dateTime);
            foreach (Flight flight in serverFlights)
            {
                // make the flight external because its from an external server
                flight.IsExternal = true;
                flights.Add(flight);
                // lock the access to the flight ids dictionary
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
            // lock the access to the flights list
            lock (listLock)
            {
                myExternalFlights.AddRange(serverFlights);
            }
        }

        // get the server list from the cache
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

        // get the external flight ids dictionary from the cache
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

        // add the given server, return false if failed
        public bool AddServer(Server server)
        {
            // check if the server is invalid
            if (server == null)
            {
                return false;
            }
            if ((server.ServerId == null) || (server.ServerURL == null))
            {
                return false;
            }
            var serversList = GetServersList();
            serversList.Add(server);
            SaveServersList(serversList);
            return true;
        }

        private void SaveServersList(List<Server> serversList)
        {
            myCache.Set("serversList", serversList);
        }

        private void SaveExternalFlightIds(Dictionary<string, Server> externalFlightIds)
        {
            myCache.Set("externalFlightIds", externalFlightIds);
        }

        // delete a server with the given id, return false if server was not found
        public bool DeleteServer(string id)
        {
            bool deleted = false;
            var serversList = GetServersList();
            var externalFlightIds = GetExternalFlightIds();
            // create a temp dictionary to iterate on
            var tempFlightIds = GetExternalFlightIds();
            Server serverToDelete = null;
            // search for the server to be deleted
            foreach (Server server in serversList)
            {
                if (server.ServerId == id)
                {
                    serverToDelete = server;
                    break;
                }
            }
            // if the server was found
            if (serverToDelete != null)
            {
                // deletes all instances of the server to be deleted from the dictionary
                foreach (KeyValuePair<string, Server> entry in tempFlightIds)
                {
                    if (entry.Value == serverToDelete)
                    {
                        externalFlightIds.Remove(entry.Key);
                    }
                }
                SaveExternalFlightIds(externalFlightIds);
                serversList.Remove(serverToDelete);
                SaveServersList(serversList);
                deleted = true;
            }
            return deleted;
        }
    }
}
