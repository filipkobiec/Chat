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
        public string roomName { get; }
        public RoomModel(string roomName, UserModel admin)
        {
            Id = Guid.NewGuid();
            this.roomName = roomName;
            UserModels = new List<UserModel>();
            UserModels.Add(admin);
            Admin = admin;
        }

        public bool RemoveUser(UserModel user) => UserModels.Remove(user);
    }
}
