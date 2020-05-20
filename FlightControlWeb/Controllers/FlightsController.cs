using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
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
        public async Task<ActionResult> GetFlights([FromQuery] string relative_to,
            [FromQuery] string sync_all)
        {
            
            try
            {
                string request = Request.QueryString.Value;
                DateTime time = DateTime.ParseExact(relative_to, "yyyy-MM-ddTHH:mm:ssZ",
                System.Globalization.CultureInfo.InvariantCulture).ToUniversalTime();
                ArrayList flights = myFlightsManager.GetFlightsByTime(time);
                if (request.Contains("sync_all"))
                {
                    ArrayList externals = await Task.Run(() => serversManager.GetExternalFlights(time));
                    flights.AddRange(externals);
                }
                return Ok(flights);
            }
            catch
            {
                return BadRequest();
            }
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public ActionResult DeleteFlight(string id)
        {
            if (myFlightsManager.DeleteFlightPlan(id))
            {
                return Ok();
            }
            return NotFound();
        }
    }
}
