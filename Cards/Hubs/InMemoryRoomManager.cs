using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cards.Hubs
{
    public class InMemoryRoomManager : IRoomManager
    {
        private readonly IDictionary<string, RoomModel> _rooms;

        public InMemoryRoomManager()
        {
            _rooms = new ConcurrentDictionary<string, RoomModel>();
        }

        public RoomModel AddUserToRoom(string roomName, UserModel user)
        {
            var room = _rooms[roomName];
            room.UserModels.Add(user);
            return room;
        }

        public RoomModel CreateRoom(string roomName, UserModel admin)
        {
            var newRoom = new RoomModel(roomName, admin);
            _rooms.Add(roomName, newRoom);
            return newRoom;
        }

        public ICollection<RoomModel> GetAllRooms() => _rooms.Values;

        public RoomModel GetRoom(string roomName) => _rooms[roomName];

        public bool IsRoomCreated(string roomName) => _rooms.ContainsKey(roomName);

        public bool RemoveRoom(string roomName) => _rooms.Remove(roomName);

        public bool SaveRoom(RoomModel room)
        {
            return true;
        }
    }
}
