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
        private IServersManager manager;

        public ServersController(IServersManager managerInput)
        {
            manager = managerInput;
        }

        // GET: api/servers
        [HttpGet]
        public ActionResult GetServers()
        {
            return Ok(manager.GetServersList());
        }

        // POST: api/servers
        [HttpPost]
        public ActionResult AddServer([FromBody] Server server)
        {
            manager.AddServer(server);
            return Ok();
        }

        // DELETE: api/servers/{id}
        [HttpDelete("{id}")]
        public ActionResult Delete(string id)
        {
            if (manager.DeleteServer(id))
            {
                return Ok();
            }
            // if the server was not found
            return NotFound("Server wasn't found");
        }
    }
}
