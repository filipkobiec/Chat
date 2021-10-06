using Cards.Hubs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cards.Controllers
{
    [Route("/[controller]")]
    [ApiController]
    public class HubsController : ControllerBase
    {
        private readonly IDictionary<string, UserConnection> _connections;

        public HubsController(IDictionary<string, UserConnection> connections)
        {
            this._connections = connections;
        }

        [HttpGet]
        public HashSet<string> GetAllHubs()
        {
            HashSet<string> keyList = new HashSet<string>(_connections.Values.Select(connection => connection.Room));
            return keyList;
        }

    }
}
