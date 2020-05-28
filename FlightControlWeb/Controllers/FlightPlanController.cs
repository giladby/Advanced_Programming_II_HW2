using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using FlightControlWeb.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;

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
                    return await TryGetFlightPlanFromServers(id);
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

        // trying to get the flight plan of the given id from the servers
        private async Task<ActionResult> TryGetFlightPlanFromServers(string id)
        {
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
        }

        // POST: api/FlightPlan
        [HttpPost]
        public ActionResult AddFlightPlan([FromBody] JsonElement FlightPlanJson)
        {
            FlightPlan flightPlan;
            var myUtils = new Utils();
            string FlightPlanString = FlightPlanJson.ToString();
            string error = "Received invalid flight plan";
            // check if the flight plan json object is valid
            if (!myUtils.IsFlightPlanJsonValid(FlightPlanJson))
            {
                return BadRequest(error);
            }
            try
            {
                // trying to deserialize the json object into a flight plan object
                flightPlan = JsonConvert.DeserializeObject<FlightPlan>(FlightPlanString);
            }
            catch
            {
                return BadRequest(error);
            }
            // trying to add the flight plan
            if (myFlightsManager.AddFlightPlan(flightPlan))
            {
                return Ok();
            }
            else
            {
                return BadRequest(error);
            }
        }
    }
}
