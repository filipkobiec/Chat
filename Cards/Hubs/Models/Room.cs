using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cards.Hubs
{
    public class Room
    {
        public UserModel Admin { get; }
        public bool IsRoomEmpty => UserModels.Count == 0;
        public List<UserModel> UserModels { get; }
        public string roomName { get; }
        public Room(string roomName, UserModel admin)
        {
            this.roomName = roomName;
            UserModels = new List<UserModel>();
            UserModels.Add(admin);
            Admin = admin;
            admin.IsAdmin = true;
            admin.IsPlayerTurn = true;
        }

        public bool RemoveUser(UserModel user) => UserModels.Remove(user);
    }
}
