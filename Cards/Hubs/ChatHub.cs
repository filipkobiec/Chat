using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cards.Hubs
{
    public class ChatHub : Hub
    {
        private readonly string _botUser;
        private readonly IDictionary<string, UserConnection> _connections;
        private readonly IDictionary<string, Room> _rooms;

        public ChatHub(IDictionary<string, UserConnection> _connections, IDictionary<string, Room> rooms)
        {
            _botUser = "MyChat Bot";
            this._connections = _connections;
            this._rooms = rooms;
        }
        public async Task JoinRoom(UserConnection userConnection)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, userConnection.Room);
            _connections[Context.ConnectionId] = userConnection;
            await Clients.Group(userConnection.Room).SendAsync("ReceiveMessage", _botUser ,
                $"{userConnection.User.Name} has joined {userConnection.Room}");
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, "Lobby");

            Room currentRoom;

            if (!_rooms.ContainsKey(userConnection.Room))
            {
                currentRoom= new Room(userConnection.Room);
                var user = userConnection.User;
                user.IsAdmin = true;
                currentRoom.UserModels.Add(user);
                _rooms[userConnection.Room] = currentRoom;
                await Clients.Group("Lobby")
                    .SendAsync("ReceiveRooms", _rooms.Values);
            }
            else
            {
                currentRoom = _rooms[userConnection.Room];
                currentRoom.UserModels.Add(userConnection.User);
            }
            await Clients.Group(userConnection.Room).SendAsync("UpdatePlayers", currentRoom.UserModels);

        }
        public async Task SendMessage(string message)
        {
            if (_connections.TryGetValue(Context.ConnectionId, out UserConnection userConnection))
            {
                await Clients.Group(userConnection.Room)
                    .SendAsync("ReceiveMessage", userConnection.User.Name, message);
            }
        }

        public async Task GetRooms()
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, "Lobby");
            await Clients.Client(Context.ConnectionId).SendAsync("ReceiveRooms", _rooms.Values);
        }

        public async Task CloseRoomConnection()
        {
            var userConnection = _connections[Context.ConnectionId];
            var currentRoom = _rooms[userConnection.Room];
            currentRoom.UserModels.Remove(userConnection.User);
            _connections.Remove(Context.ConnectionId);
            await Groups.AddToGroupAsync(Context.ConnectionId, "Lobby");

            if (currentRoom.UserModels.Count == 0)
            {
                _rooms.Remove(currentRoom.roomName);
                await Clients.Group("Lobby")
                   .SendAsync("ReceiveRooms", _rooms.Values);
            }
            else
            {
                await Clients.Group(userConnection.Room)
                   .SendAsync("ReceiveMessage", _botUser, $"{userConnection.User.Name} has left");
                await Clients.Client(Context.ConnectionId)
                    .SendAsync("ReceiveRooms", _rooms.Values);

                await Clients.Group(userConnection.Room).SendAsync("UpdatePlayers", _rooms[userConnection.Room].UserModels);
            }
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            if(_connections.TryGetValue(Context.ConnectionId, out UserConnection userConnection))
            {
                _connections.Remove(Context.ConnectionId);
                var currentRoom = _rooms[userConnection.Room];
                currentRoom.UserModels.Remove(userConnection.User);
                if (currentRoom.UserModels.Count == 0)
                {
                    _rooms.Remove(currentRoom.roomName);
                    Clients.Group("Lobby")
                    .SendAsync("ReceiveRooms", _rooms.Values);
                }
                Clients.Group(userConnection.Room)
                    .SendAsync("ReceiveMessage", _botUser, $"{userConnection.User.Name} has left");
                Clients.Group(userConnection.Room)
                    .SendAsync("UpdatePlayers", _rooms[userConnection.Room].UserModels);
            }
            return base.OnDisconnectedAsync(exception);
        }
    }
}
