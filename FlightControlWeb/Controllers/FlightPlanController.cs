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
        public FlightPlan GetFlightPlan(string id)
        {
            FlightPlan fp = myFlightsManager.GetFlightPlan(id);
            if(fp == null)
            {
                fp = serversManager.GetExternalPlan(id);
            }
            return fp;
        }

        // POST: api/FlightPlan
        [HttpPost]
        public FlightPlan AddFlightPlan([FromBody] FlightPlan fp)
        {
            myFlightsManager.AddFlightPlan(fp);
            return fp;
        }
    }
}
