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

        public FlightPlanController(IFlightsManager myFlightsManagerInput, IServersManager serversManagerInput)
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
                FlightPlan flightPlan = myFlightsManager.GetFlightPlan(id);
                if (flightPlan == null)
                {
                    flightPlan = await Task.Run(() => serversManager.GetExternalPlan(id));
                    if (flightPlan == null)
                    {
                        return NotFound("Flight plan wasn't found");
                    }
                }
                return Ok(flightPlan);
            }
            catch
            {
                return BadRequest("Failed trying to get flight plan");
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
