using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cards.Hubs
{
    public class UserModel
    {
        public Guid Id { get; set; }
        public Guid RoomId { get; set; }
        public string ConnectionId { get; set; }
        public string Name { get; set; }
        public bool IsAdmin { get; set; }
    }
}
