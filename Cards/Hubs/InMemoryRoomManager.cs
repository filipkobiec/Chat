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

        public RoomModel AddUserToRoom(Guid roomId, UserModel user)
        {
            var room = _rooms[roomId];
            room.UserModels.Add(user);
            return room;
        }

        public UserModel GetUserFromRoom(Guid roomId, Guid userId)
        {
            var room = _rooms[roomId];
            var roomUsers = room.UserModels;
            var user = roomUsers.SingleOrDefault(u => u.Id == userId);
            return user;
        }

        public bool DoesUserWithSameNameExist(string name, RoomModel room) => room.UserModels.Any(u => u.Name == name);

        public RoomModel RemoveUserFromRoom(Guid roomId, UserModel user)
        {
            var room = _rooms[roomId];

            if (user != null)
            {
                room.UserModels.Remove(user);
            }

            return room;
        }
    }
}
