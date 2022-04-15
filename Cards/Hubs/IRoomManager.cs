﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cards.Hubs
{
    public interface IRoomManager
    {
        public RoomModel CreateRoom(string roomName, UserModel admin);
        public RoomModel GetRoom(Guid roomId);
        public bool RemoveRoom(Guid roomId);
        public bool IsRoomCreated(Guid roomId);
        public ICollection<RoomModel> GetAllRooms();
        public RoomModel AddUserToRoom(Guid roomId, UserModel user);
        public UserModel GetUserFromRoom(Guid roomId, Guid userId);
        public RoomModel RemoveUserFromRoom(Guid roomId, UserModel user);
        public bool DoesUserWithSameNameExist(string name, RoomModel room);

    }
}
