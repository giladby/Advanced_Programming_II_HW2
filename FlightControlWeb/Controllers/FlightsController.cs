using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FlightControlWeb.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace FlightControlWeb.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FlightsController : ControllerBase
    {
        private MyFlightsManager myFlightsManager;
        private ServersManager serversManager;

        public FlightsController(MyFlightsManager fm, ServersManager sm)
        {
            myFlightsManager = fm;
            serversManager = sm;

        }

        // GET: api/Flights
        [HttpGet]
        public ArrayList GetFlights([FromQuery] string relative_to,
            [FromQuery] string sync_all)
        {
            ArrayList serverFlights;
            string request = Request.QueryString.Value;
            DateTime time = DateTime.ParseExact(relative_to, "yyyy-MM-ddTHH:mm:ssZ", 
                System.Globalization.CultureInfo.InvariantCulture);
            ArrayList flights = myFlightsManager.GetFlightsByTime(time);
            if (request.Contains("sync_all"))
            {
                List<Server> serversList = serversManager.GetServersList();
                foreach (Server server in serversList)
                {
                    //serverFlights = ..
                    foreach (Flight f in serverFlights)
                    {
                        f.IsExternal = true;
                        flights.Add(f);
                    }


                }
            }
            return flights;
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public void DeleteFlight(string id)
        {
            myFlightsManager.DeleteFlightPlan(id);
        }
    }
}
