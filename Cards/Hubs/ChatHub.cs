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

            Room currentRoom;

            if (!_rooms.ContainsKey(userConnection.Room))
            {
                currentRoom= new Room(userConnection.Room);
                currentRoom.UserModels.Add(userConnection.User);
                _rooms[userConnection.Room] = currentRoom;
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

        public override Task OnDisconnectedAsync(Exception exception)
        {
            if(_connections.TryGetValue(Context.ConnectionId, out UserConnection userConnection))
            {
                _connections.Remove(Context.ConnectionId);
                var currentRoom = _rooms[userConnection.Room];
                currentRoom.UserModels.Remove(userConnection.User);
                Clients.Group(userConnection.Room)
                    .SendAsync("ReceiveMessage", _botUser, $"{userConnection.User} has left");
                Clients.Group(userConnection.Room)
                    .SendAsync("UpdatePlayers", _rooms[userConnection.Room].UserModels);
            }
            return base.OnDisconnectedAsync(exception);
        }
    }
}
