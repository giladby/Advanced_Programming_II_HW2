using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FlightControlWeb.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FlightControlWeb.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FlightsController : ControllerBase
    {
        private ServerFlightsManager manager = new ServerFlightsManager();

        // GET: api/Flights
        [HttpGet]
        public ArrayList GetFlights([FromQuery] string relativeTo,
            [FromQuery] string syncAll)
        {
            string request = Request.QueryString.Value;
            DateTime time = DateTime.ParseExact(relativeTo, "yyyyMMddTHH:mm:ssZ", 
                System.Globalization.CultureInfo.InvariantCulture);
            ArrayList flights = manager.GetFlightsByTime(time);
            if (request.Contains("sync_all"))
            {
                
            }
            return flights;
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public void DeleteFlight(string id)
        {
            manager.DeleteFlightPlan(id);
        }
    }
}
