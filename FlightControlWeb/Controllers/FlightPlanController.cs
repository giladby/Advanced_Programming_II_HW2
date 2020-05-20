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

        public FlightPlanController(MyFlightsManager fm, ServersManager sm)
        {
            myFlightsManager = fm;
            serversManager = sm;
        }

        // GET: api/FlightPlan/{id}
        [HttpGet("{id}", Name = "Get")]
        public async Task<ActionResult> GetFlightPlan(string id)
        {
            try
            {
                FlightPlan fp = myFlightsManager.GetFlightPlan(id);
                if (fp == null)
                {
                    fp = await Task.Run(() => serversManager.GetExternalPlan(id));
                    if(fp == null)
                    {
                        return NotFound();
                    }
                    
                }

                return Ok(fp);
            }
            catch
            {
                return BadRequest();
            }
        }

        // POST: api/FlightPlan
        [HttpPost]
        public ActionResult AddFlightPlan([FromBody] FlightPlan fp)
        {
            myFlightsManager.AddFlightPlan(fp);
            return Ok();
        }
    }
}
