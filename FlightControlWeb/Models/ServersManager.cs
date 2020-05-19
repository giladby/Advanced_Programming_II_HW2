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
    public class ServersManager
    {
        private IMemoryCache myCache;

        public ServersManager(IMemoryCache cache)
        {
            myCache = cache;
        }

        public FlightPlan GetExternalPlan(string id)
        {
            Dictionary<string, Server> externalFlightIds = GetExternalFlightIds();
            if(externalFlightIds.ContainsKey(id))
            {
                Server server = externalFlightIds[id];
                FlightPlan fp = GetPlanFromServer(server, id);
                return fp;
            }
            return null;
        }

        private string GetSerialzedObject(string url)
        {
            WebRequest requestObject = WebRequest.Create(url);
            requestObject.Method = "GET";
            HttpWebResponse responseObject = null;
            responseObject = (HttpWebResponse)requestObject.GetResponse();
            string result = null;
            using (Stream stream = responseObject.GetResponseStream())
            {
                StreamReader sr = new StreamReader(stream);
                result = sr.ReadToEnd();
                sr.Close();
            }
            return result;
        }
        
        private FlightPlan GetPlanFromServer(Server s, string id)
        {
            string url = s.ServerURL + "/api/FlightPlan/" + id;
            string result = GetSerialzedObject(url);
            return JsonConvert.DeserializeObject<FlightPlan>(result); 
        }

        private ArrayList GetFlightsFromServer(DateTime dt, Server s)
        {
            string dtString = dt.ToString("s", DateTimeFormatInfo.InvariantInfo) + "Z";
            string url = s.ServerURL + "/api/Flights?relative_to=" + dtString;
            string result = GetSerialzedObject(url);
            List<Flight> list = JsonConvert.DeserializeObject<List<Flight>>(result);
            return new ArrayList(list);
        }

        public ArrayList GetExternalFlights(DateTime dt)
        {
            ArrayList myExternalFlights = new ArrayList();
            List<Server> serversList = GetServersList();
            Dictionary<string, Server> externalFlightIds = GetExternalFlightIds();
            foreach (Server server in serversList)
            {
                ArrayList serverFlights = GetFlightsFromServer(dt, server);
                foreach (Flight f in serverFlights)
                {
                    f.IsExternal = true;
                    myExternalFlights.Add(f);

                    externalFlightIds[f.FlightId] = server;
                }
            }
            SaveExternalIds(externalFlightIds);
            return myExternalFlights;
        }

        public List<Server> GetServersList()
        {
            List<Server> serversList = new List<Server>();
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
            Dictionary<string, Server> externalFlightIds = new Dictionary<string, Server>();
            if (!myCache.TryGetValue("externalFlightIds", out externalFlightIds))
            {
                if (externalFlightIds == null)
                {
                    externalFlightIds = new Dictionary<string, Server>();
                }
            }
            SaveExternalIds(externalFlightIds);
            return externalFlightIds;
        }

        public void AddServer(Server server) {
            List<Server> serversList = GetServersList();
            serversList.Add(server);
            SaveServersList(serversList);
        }

        private void SaveServersList(List<Server> serversList)
        {
            myCache.Set("serversList", serversList);
        }

        private void SaveExternalIds(Dictionary<string, Server> externalFlightIds)
        {
            myCache.Set("externalFlightIds", externalFlightIds);
        }

        public void DeleteServer(string id) {
            List<Server> serversList = GetServersList();
            Dictionary<string, Server> externalFlightIds = GetExternalFlightIds();
            Dictionary<Server, string> dictInverse = externalFlightIds.ToDictionary((i) => i.Value, (i) => i.Key);

            Server serverToDelete = null;
            foreach (Server s in serversList)
            {
                if (s.ServerId == id)
                {
                    serverToDelete = s;
                    break;
                }
            }
            if (serverToDelete != null)
            {
                if(dictInverse.ContainsKey(serverToDelete))
                {
                    string flightId = dictInverse[serverToDelete];
                    externalFlightIds.Remove(flightId);
                    SaveExternalIds(externalFlightIds);
                }
                serversList.Remove(serverToDelete);
                SaveServersList(serversList);
            }
        }
    }
}
