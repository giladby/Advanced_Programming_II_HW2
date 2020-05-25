using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace FlightControlWeb.Models
{
    public interface IFlightsManager
    {
        // add the given flight plan, return false if failed
        public bool AddFlightPlan(FlightPlan flightPlan);
        // delete a flight plan with the given id, return false if the flight was not found
        public bool DeleteFlightPlan(string id);
        // get a flight plan with the given id
        public FlightPlan GetFlightPlan(string id);
        // create a flight with the given dateTime from the given flight plan
        public Flight GetFlight(FlightPlan flightPlan, DateTime dateTime);
        // get all the flights relative to the given dateTime
        public ArrayList GetFlightsByTime(DateTime dateTime);
        // check if the given flight plan json object is valid
        public bool IsFlightPlanJsonValid(JsonElement FlightPlanJson);
    }
}
