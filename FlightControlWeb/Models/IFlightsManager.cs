using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlightControlWeb.Models
{
    public interface IFlightsManager
    {

        public void AddFlightPlan(FlightPlan flightPlan);

        public bool DeleteFlightPlan(string id);

        public FlightPlan GetFlightPlan(string id);


        public Flight GetFlight(FlightPlan flightPlan, DateTime dateTime);

        public ArrayList GetFlightsByTime(DateTime dateTime);
    }
}
