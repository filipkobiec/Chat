using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cards.Hubs
{
    public interface IRoomManager
    {
        public RoomModel CreateRoom(string roomName, UserModel admin);
        public RoomModel AddUserToRoom(string roomName, UserModel user);
        public RoomModel GetRoom(string roomName);
        public bool RemoveRoom(string roomName);
        public bool SaveRoom(RoomModel room);
        public bool IsRoomCreated(string roomName);
        public ICollection<RoomModel> GetAllRooms();
    }
}
