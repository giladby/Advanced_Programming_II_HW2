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
        private IServerManager serversManager;

        public FlightsController(IFlightsManager myFlightsManagerInput, IServerManager serversManagerInput)
        {
            myFlightsManager = myFlightsManagerInput;
            serversManager = serversManagerInput;
        }

        // GET: api/Flights
        [HttpGet("{sync_all?}")]

        public async Task<IActionResult> GetFlights([FromQuery] string relative_to)
        {
            try
            {
                string request = Request.QueryString.Value;
                DateTime time = DateTime.ParseExact(relative_to, "yyyy-MM-ddTHH:mm:ssZ",
                System.Globalization.CultureInfo.InvariantCulture).ToUniversalTime();
                ArrayList flights = myFlightsManager.GetFlightsByTime(time);
                if (request.Contains("sync_all"))
                {
                    Tuple<bool, ArrayList> result = await Task.Run(() => serversManager.GetExternalFlights(time));
                    if(result.Item1)
                    {
                        return BadRequest("Failed connecting with external server");
                    }
                    flights.AddRange(result.Item2);
                }
                return Ok(flights);
            }
            catch
            {
                return BadRequest("Failed trying to get flights");
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
            return NotFound("Flight wasn't found");
        }
    }
}
