using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cards.Hubs
{
    public class Room
    {
        public List<UserModel> UserModels { get; }
        public string roomName { get; }
        public Room(string roomName)
        {
            UserModels = new List<UserModel>();
            this.roomName = roomName;
        }

    }
}
