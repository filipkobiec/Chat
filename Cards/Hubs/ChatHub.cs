using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cards.Hubs
{
    public class ChatHub : Hub
    {
        private readonly string _botUser;
        private readonly IRoomManager _roomManager;
        private readonly IDictionary<string, UserModel> _connections;

        public ChatHub(IRoomManager roomManager, IDictionary<string, UserModel> connections)
        {
            _connections = connections;
            _botUser = "MyChat Bot";
            _roomManager = roomManager;
  
        }
        public async Task JoinRoom(UserModel user, Guid roomId)
        {
            var room = _roomManager.GetRoom(roomId);
            user.RoomId = room.Id;
            user.Id = Guid.NewGuid();
            user.ConnectionId = Context.ConnectionId;
            await HandleJoiningRoomByClient(room, user);
            _roomManager.AddUserToRoom(roomId, user);
            await UpdateRoom(user, room);
        }

        public async Task CreateRoom(UserModel user, string roomName)
        {
            
            user.Id = Guid.NewGuid();
            user.ConnectionId = Context.ConnectionId;
            var room = _roomManager.CreateRoom(roomName, user);
            user.RoomId = room.Id;
            await HandleJoiningRoomByClient(room, user);
            await SendRoomsToClients();
            await UpdateRoom(user, room);

        }
        public async Task SendMessage(UserModel user, string message)
        {   
            await Clients.Group(user.RoomId.ToString())
                .SendAsync("ReceiveMessage", user.Name, message);
        }

        public async Task GetRooms()
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, "Lobby");
            await SendRoomsToClients();
        }


        public async Task CloseRoomConnection()
        {
            if (_connections.TryGetValue(Context.ConnectionId, out UserModel user))
            {
                var currentRoom = _roomManager.GetRoom(user.RoomId);
                currentRoom.UserModels.Remove(user);
                _connections.Remove(Context.ConnectionId);
                await Groups.AddToGroupAsync(user.ConnectionId, "Lobby");

                if (currentRoom.IsRoomEmpty)
                    await RemoveRoom(currentRoom);
                else
                    await HandleUserLeavingRoom(user, currentRoom);
            }

        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            if (_connections.TryGetValue(Context.ConnectionId, out UserModel user))
            {
                var currentRoom = _roomManager.GetRoom(user.RoomId);
                currentRoom.UserModels.Remove(user);
                _connections.Remove(Context.ConnectionId);

                if (currentRoom.UserModels.Count == 0)
                {
                    _roomManager.RemoveRoom(currentRoom.Id);
                    Clients.Group("Lobby")
                    .SendAsync("ReceiveRooms", _roomManager.GetAllRooms());
                }
                else
                {
                    Clients.Group(currentRoom.Id.ToString())
                        .SendAsync("ReceiveMessage", _botUser, $"{user.Name} has left");
                    Clients.Group(currentRoom.Id.ToString())
                        .SendAsync("UpdateRoom", currentRoom);
                }
            }

            return base.OnDisconnectedAsync(exception);
        }

        private async Task UpdateRoom(UserModel user, RoomModel currentRoom)
        {
            await Clients.Group(currentRoom.Id.ToString()).SendAsync("UpdateRoom", currentRoom);
        }

        private async Task SendRoomsToClients()
        {
            await Clients.Group("Lobby")
                .SendAsync("ReceiveRooms", _roomManager.GetAllRooms());
        }

        private async Task HandleJoiningRoomByClient(RoomModel room, UserModel user)
        {
            _connections[Context.ConnectionId] = user;
            await Groups.AddToGroupAsync(Context.ConnectionId, room.Id.ToString());
            await Clients.Client(user.ConnectionId).SendAsync("SetUser", user);   
            await Clients.Group(room.Id.ToString()).SendAsync("ReceiveMessage", _botUser,
                $"{user.Name} has joined {room.roomName}");
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, "Lobby");
        }

        private async Task HandleUserLeavingRoom(UserModel user, RoomModel currentRoom)
        {
            var newAdmin = currentRoom.UserModels[0];
            newAdmin.IsAdmin = true;
            _roomManager.SaveRoom(currentRoom);
            await Clients.Group(currentRoom.Id.ToString())
               .SendAsync("ReceiveMessage", _botUser, $"{user.Name} has left");
            await SendRoomsToClients();
            await Clients.Client(newAdmin.ConnectionId).SendAsync("SetUser", newAdmin);
            await Clients.Group(currentRoom.Id.ToString()).SendAsync("UpdateRoom", currentRoom);
        }

        private async Task RemoveRoom(RoomModel currentRoom)
        {
            _roomManager.RemoveRoom(currentRoom.Id);
            await SendRoomsToClients();
        }

    }
}
