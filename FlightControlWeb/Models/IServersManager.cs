using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlightControlWeb.Models
{
    public interface IServersManager
    {
        // get a flight plan with the given id, return false if failed
        public Tuple<bool, FlightPlan> GetExternalPlan(string id);
        // get all the flights relative to the given dateTime, return false if failed
        public Tuple<bool, ArrayList> GetExternalFlights(DateTime dateTime);
        // get the server list from the cache
        public List<Server> GetServersList();
        // get the external flight ids dictionary from the cache
        public Dictionary<string, Server> GetExternalFlightIds();
        // add the given server, return false if failed
        public bool AddServer(Server server);
        // delete a server with the given id, return false if server was not found
        public bool DeleteServer(string id);
    }
}
