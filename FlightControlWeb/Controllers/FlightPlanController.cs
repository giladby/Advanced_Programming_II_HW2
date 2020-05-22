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
        private MyFlightsManager myFlightsManager;
        private ServersManager serversManager;

        public FlightPlanController(MyFlightsManager myFlightsManagerInput, ServersManager serversManagerInput)
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
                        return NotFound();
                    }
                }
                return Ok(flightPlan);
            }
            catch
            {
                return BadRequest();
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
