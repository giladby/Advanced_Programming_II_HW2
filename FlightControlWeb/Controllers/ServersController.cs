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

        public ServersController(ServersManager m)
        {
            manager = m;
        }


        // GET: api/Servers
        [HttpGet]
        public List<Server> GetServers()
        {
            return manager.GetServersList();
        }

        // POST: api/Servers
        [HttpPost]
        public void AddServer([FromBody] Server server)
        {
            manager.AddServer(server);
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public void Delete(string id)
        {
            manager.DeleteServer(id);
        }
    }
}
