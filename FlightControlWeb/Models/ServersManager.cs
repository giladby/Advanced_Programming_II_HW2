using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Caching.Memory;
using System.Threading.Tasks;

namespace FlightControlWeb.Models
{
    public class ServersManager
    {
        private IMemoryCache myCache;

        public ServersManager(IMemoryCache cache)
        {
            myCache = cache;
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
            myCache.Set("serversList", serversList);
            return serversList;
        }

        private Dictionary<string, Server> GetExternalFlightIds()
        {
            Dictionary<string, Server> externalFlightIds = new Dictionary<string, Server>();
            if (!myCache.TryGetValue("externalFlightIds", out externalFlightIds))
            {
                if (externalFlightIds == null)
                {
                    externalFlightIds = new Dictionary<string, Server>();
                }
            }
            myCache.Set("externalFlightIds", externalFlightIds);
            return externalFlightIds;
        }

        public void AddServer(Server server) {
            List<Server> serversList = GetServersList();

            serversList.Add(server);

            myCache.Set("serversList", serversList);
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
                serversList.Remove(serverToDelete);
                string flightId = dictInverse[serverToDelete];
                externalFlightIds.Remove(flightId);
            }
        }
    }
}
