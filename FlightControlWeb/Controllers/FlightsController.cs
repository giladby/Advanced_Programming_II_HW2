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
        private MyFlightsManager manager;

        public FlightsController(MyFlightsManager m)
        {
            manager = m;
        }

        // GET: api/Flights
        [HttpGet]
        public ArrayList GetFlights([FromQuery] string relative_to,
            [FromQuery] string sync_all)
        {
            string request = Request.QueryString.Value;
            DateTime time = DateTime.ParseExact(relative_to, "yyyy-MM-ddTHH:mm:ssZ", 
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
