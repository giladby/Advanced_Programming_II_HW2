using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using FlightControlWeb.Models;

namespace FlightControlWeb.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ServersController : ControllerBase
    {
        private ServersManager manager;

        public ServersController(ServersManager managerInput)
        {
            manager = managerInput;
        }

        // GET: api/Servers
        [HttpGet]
        public ActionResult GetServers()
        {
            return Ok(manager.GetServersList());
        }

        // POST: api/Servers
        [HttpPost]
        public ActionResult AddServer([FromBody] Server server)
        {
            manager.AddServer(server);
            return Ok();
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public ActionResult Delete(string id)
        {
            if (manager.DeleteServer(id))
            {
                return Ok();
            }
            return NotFound();
        }
    }
}
