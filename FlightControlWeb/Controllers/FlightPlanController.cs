using System;
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
    public class FlightPlanController : ControllerBase
    {
        private IFlightsManager myFlightsManager;
        private IServersManager serversManager;

        public FlightPlanController(IFlightsManager myFlightsManagerInput,
            IServersManager serversManagerInput)
        {
            myFlightsManager = myFlightsManagerInput;
            serversManager = serversManagerInput;
        }

        // GET: api/FlightPlan/{id}
        [HttpGet("{id}", Name = "Get")]
        public async Task<ActionResult> GetFlightPlan(string id)
        {
            try
            {
                // trying to get the flight plan from the inner server
                FlightPlan flightPlan = myFlightsManager.GetFlightPlan(id);
                // if the flight plan was not found
                if (flightPlan == null)
                {
                    // trying to get the flight plan from the servers
                    Tuple<bool, FlightPlan> result = await Task.Run(
                        () => serversManager.GetExternalPlan(id));
                    // if the flight plan was not found 
                    if (result == null)
                    {
                        return NotFound("Flight plan wasn't found");
                    }
                    bool failed = result.Item1;
                    FlightPlan externalFlightPlan = result.Item2;
                    // if the request to the servers failed
                    if (failed)
                    {
                        return BadRequest("Failed receiving flight plan from server");
                    }
                    // if the flight plan was not found 
                    if (externalFlightPlan == null)
                    {
                        return NotFound("Flight plan wasn't found");
                    }
                    return Ok(externalFlightPlan);
                } else
                {
                    return Ok(flightPlan);
                }
            }
            catch
            {
                return BadRequest("Failed receiving flight plan");
            }
        }

        // POST: api/FlightPlan
        [HttpPost]
        public ActionResult AddFlightPlan([FromBody] FlightPlan flightPlan)
        {
            myFlightsManager.AddFlightPlan(flightPlan);
            return Ok();
        }
    }
}
