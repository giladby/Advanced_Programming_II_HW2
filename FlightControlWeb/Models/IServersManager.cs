using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlightControlWeb.Models
{
    public interface IServersManager
    {
        public FlightPlan GetExternalPlan(string id);

        public Tuple<bool, ArrayList> GetExternalFlights(DateTime dateTime);


        public List<Server> GetServersList();

        public Dictionary<string, Server> GetExternalFlightIds();

        public void AddServer(Server server);

        public bool DeleteServer(string id);
    }
}
