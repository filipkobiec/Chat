using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cards.Hubs
{
    public class InMemoryRoomManager : IRoomManager
    {
        private readonly IDictionary<Guid, RoomModel> _rooms;

        public InMemoryRoomManager()
        {
            _rooms = new ConcurrentDictionary<Guid, RoomModel>();
        }

        public RoomModel AddUserToRoom(Guid roomId, UserModel user)
        {
            var room = _rooms[roomId];
            room.UserModels.Add(user);
            return room;
        }

        public RoomModel CreateRoom(string roomName, UserModel admin)
        {
            var newRoom = new RoomModel(roomName, admin);
            _rooms.Add(newRoom.Id, newRoom);
            return newRoom;
        }

        public ICollection<RoomModel> GetAllRooms() => _rooms.Values;

        public RoomModel GetRoom(Guid roomId) => _rooms[roomId];

        public bool IsRoomCreated(Guid roomId) => _rooms.ContainsKey(roomId);

        public bool RemoveRoom(Guid roomId) => _rooms.Remove(roomId);

    }
}
