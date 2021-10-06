using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cards.Hubs
{
    public class Room
    {
        public List<UserConnection> userConnections { get; }
        public string roomName { get; }
        public Room(string roomName)
        {
            userConnections = new List<UserConnection>();
            this.roomName = roomName;
        }

    }
}
