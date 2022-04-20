using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cards.Hubs
{
    public class RoomModel
    {
        public Guid Id { get; set; }
        public UserModel Admin { get; set; }
        public bool IsRoomEmpty => UserModels.Count == 0;
        public List<UserModel> UserModels { get; set; }
        public string RoomName { get; }
        public RoomModel(string roomName, UserModel admin)
        {
            Id = Guid.NewGuid();
            RoomName = roomName;
            UserModels = new List<UserModel>();
            UserModels.Add(admin);
            Admin = admin;
        }
    }
}
