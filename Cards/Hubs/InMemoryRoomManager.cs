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

        public RoomModel GetRoom(Guid roomId)
        {
            _rooms.TryGetValue(roomId, out var room);
            return room;
        }

        public bool IsRoomCreated(Guid roomId) => _rooms.ContainsKey(roomId);

        public RoomModel RemoveRoom(Guid roomId)
        {
            _rooms.TryGetValue(roomId, out var room);
            if (room != null)
            {
                _rooms.Remove(roomId);
            }

            return room;
        }

        public RoomModel AddUserToRoom(Guid roomId, UserModel user)
        {
            _rooms.TryGetValue(roomId, out var room);
            if (room != null)
                room.UserModels.Add(user);

            return room;
        }

        public UserModel GetUserFromRoom(Guid roomId, Guid userId)
        {
            _rooms.TryGetValue(roomId, out var room);
            UserModel user = null;
            if (room != null)
            {
                var roomUsers = room.UserModels;
                user = roomUsers.SingleOrDefault(u => u.Id == userId);

            }

            return user;
        }

        public bool DoesUserWithSameNameExist(string name, RoomModel room) => room.UserModels.Any(u => u.Name == name);

        public RoomModel RemoveUserFromRoom(Guid roomId, UserModel user)
        {
            _rooms.TryGetValue(roomId, out var room);

            if (user != null)
            {
                room.UserModels.Remove(user);
            }

            return room;
        }

        public bool DoesRoomWithSameNameExist(string name) => _rooms.Values.Any(r => r.RoomName == name);
    }
}
