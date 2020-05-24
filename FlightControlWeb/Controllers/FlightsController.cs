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
        private IFlightsManager myFlightsManager;
        private IServersManager serversManager;

        public FlightsController(IFlightsManager myFlightsManagerInput,
            IServersManager serversManagerInput)
        {
            myFlightsManager = myFlightsManagerInput;
            serversManager = serversManagerInput;
        }

        // GET: api/Flights
        [HttpGet("{sync_all?}")]
        public async Task<IActionResult> GetFlights([FromQuery] string relative_to)
        {
            string request = Request.QueryString.Value;
            ArrayList flights = new ArrayList();
            DateTime time;
            try
            {
                // convert relative_to string to DateTime object
                time = DateTime.ParseExact(relative_to, "yyyy-MM-ddTHH:mm:ssZ",
                System.Globalization.CultureInfo.InvariantCulture).ToUniversalTime();
                // getting the inner server flights
                flights = myFlightsManager.GetFlightsByTime(time);
            }
            catch
            {
                return BadRequest("Failed receiving flights");
            }
            // if also need to check the servers
            if (request.Contains("sync_all"))
            {
                // getting all servers flights
                Tuple<bool, ArrayList> result = await Task.Run(
                    () => serversManager.GetExternalFlights(time));
                bool failed = result.Item1;
                ArrayList externalFlights = result.Item2;
                // if the request to the servers failed
                if (failed)
                {
                    return BadRequest("Failed receiving flights from servers");
                }
                flights.AddRange(externalFlights);
            }
            return Ok(flights);
        }

        // DELETE: api/Flights/{id}
        [HttpDelete("{id}")]
        public ActionResult DeleteFlight(string id)
        {
            if (myFlightsManager.DeleteFlightPlan(id))
            {
                return Ok();
            }
            // if the flight was not found
            return NotFound("Flight wasn't found");
        }
    }
}
