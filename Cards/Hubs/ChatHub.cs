using Cards.Hubs.Models;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cards.Hubs
{
    public class ChatHub : Hub
    {
        private readonly string _botUser;
        private readonly IDictionary<string, UserConnection> _connections;
        private readonly IRoomManager _roomManager;

        public ChatHub(IDictionary<string, UserConnection> _connections, IRoomManager roomManager)
        {
            _botUser = "MyChat Bot";
            this._connections = _connections;
            _roomManager = roomManager;
        }
        public async Task JoinRoom(UserConnection userConnection)
        {
            var roomName = userConnection.Room;
            var user = userConnection.User;
            user.Id = Guid.NewGuid();
            user.ConnectionId = Context.ConnectionId;
            await HandleJoiningRoomByClient(userConnection);

            RoomModel currentRoom;
            if (_roomManager.IsRoomCreated(roomName))
                currentRoom = _roomManager.AddUserToRoom(roomName, user);
            else
            {
                currentRoom = _roomManager.CreateRoom(roomName, user);
                await SendRoomsToClients();
            }
            await SendPlayersToClient(user, currentRoom);

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
            await SendRoomsToClients();
        }

        public async Task StartGame(string roomName, UserModel clientPlayer)
        {
            var currentRoom = _roomManager.GetRoom(roomName);
            var players = currentRoom.UserModels;

            foreach (var player in players)
            {
                if (player.IsAdmin)
                    player.IsPlayerCardChar = true;

                player.Cards = new List<CardModel>();
                for (int i = 0; i < 10; i++)
                {
                    player.Cards.Add(new CardModel(i.ToString(), player.Id));
                }
                await Clients.Client(player.ConnectionId).SendAsync("SetPlayer", player);
            }
            await Clients.Group(roomName).SendAsync("UpdatePlayers", players);
        }

        public async Task GetChosenCard(UserModel player, CardModel card)
        {
            await Clients.Client(player.ConnectionId).SendAsync("SetPlayer", player);
        }

        public async Task CloseRoomConnection()
        {
            var userConnection = _connections[Context.ConnectionId];
            var currentRoom = _roomManager.GetRoom(userConnection.Room);
            currentRoom.UserModels.Remove(userConnection.User);
            _connections.Remove(Context.ConnectionId);
            await Groups.AddToGroupAsync(Context.ConnectionId, "Lobby");

            if (currentRoom.IsRoomEmpty)
            {
                _roomManager.RemoveRoom(currentRoom.roomName);
                await SendRoomsToClients();
            }
            else
            {
                var newAdmin = currentRoom.UserModels[0];
                newAdmin.IsAdmin = true;
                _roomManager.SaveRoom(currentRoom);
                await Clients.Group(userConnection.Room)
                   .SendAsync("ReceiveMessage", _botUser, $"{userConnection.User.Name} has left");
                await SendRoomsToClients();
                await Clients.Client(newAdmin.ConnectionId).SendAsync("SetPlayer", newAdmin);
                await Clients.Group(userConnection.Room).SendAsync("UpdateRoom", currentRoom);
            }
        }
        public override Task OnDisconnectedAsync(Exception exception)
        {
            if(_connections.TryGetValue(Context.ConnectionId, out UserConnection userConnection))
            {
                _connections.Remove(Context.ConnectionId);
                var currentRoom = _roomManager.GetRoom(userConnection.Room);
                currentRoom.UserModels.Remove(userConnection.User);
                if (currentRoom.UserModels.Count == 0)
                {
                    _roomManager.RemoveRoom(currentRoom.roomName);
                    Clients.Group("Lobby")
                    .SendAsync("ReceiveRooms", _roomManager.GetAllRooms());
                }
                Clients.Group(userConnection.Room)
                    .SendAsync("ReceiveMessage", _botUser, $"{userConnection.User.Name} has left");
                Clients.Group(userConnection.Room)
                    .SendAsync("UpdatePlayers", currentRoom.UserModels);
            }
            return base.OnDisconnectedAsync(exception);
        }
        private async Task HandleJoiningRoomByClient(UserConnection userConnection)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, userConnection.Room);
            _connections[Context.ConnectionId] = userConnection;
            await Clients.Group(userConnection.Room).SendAsync("ReceiveMessage", _botUser,
                $"{userConnection.User.Name} has joined {userConnection.Room}");
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, "Lobby");
        }

        private async Task SendPlayersToClient(UserModel user, RoomModel currentRoom)
        {
            await Clients.Client(Context.ConnectionId).SendAsync("SetPlayer", user);
            await Clients.Group(currentRoom.roomName).SendAsync("UpdateRoom", currentRoom);
        }
        private async Task SendRoomsToClients()
        {
            await Clients.Group("Lobby")
                .SendAsync("ReceiveRooms", _roomManager.GetAllRooms());
        }
    }
}
