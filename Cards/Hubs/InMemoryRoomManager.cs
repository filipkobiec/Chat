using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cards.Hubs
{
    public class InMemoryRoomManager : IRoomManager
    {
        private readonly IDictionary<string, Room> _rooms;

        public InMemoryRoomManager()
        {
            _rooms = new ConcurrentDictionary<string, Room>();
        }

        public Room AddUserToRoom(string roomName, UserModel user)
        {
            var room = _rooms[roomName];
            room.UserModels.Add(user);
            return room;
        }

        public Room CreateRoom(string roomName, UserModel admin)
        {
            var newRoom = new Room(roomName, admin);
            _rooms.Add(roomName, newRoom);
            return newRoom;
        }

        public ICollection<Room> GetAllRooms() => _rooms.Values;

        public Room GetRoom(string roomName) => _rooms[roomName];

        public bool IsRoomCreated(string roomName) => _rooms.ContainsKey(roomName);

        public bool RemoveRoom(string roomName) => _rooms.Remove(roomName);

        public bool SaveRoom(Room room)
        {
            return true;
        }
    }
}
