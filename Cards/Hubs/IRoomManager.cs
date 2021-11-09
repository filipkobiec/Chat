using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cards.Hubs
{
    public interface IRoomManager
    {
        public Room CreateRoom(string roomName, UserModel admin);
        public Room AddUserToRoom(string roomName, UserModel user);
        public Room GetRoom(string roomName);
        public bool RemoveRoom(string roomName);
        public bool SaveRoom(Room room);
        public bool IsRoomCreated(string roomName);
        public ICollection<Room> GetAllRooms();
    }
}
