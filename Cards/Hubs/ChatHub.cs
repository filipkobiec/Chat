﻿using Cards.Hubs.Models;
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
            await HandleJoiningRoomByClient(userConnection);

            Room currentRoom;
            if (_roomManager.IsRoomCreated(roomName))
            {
                currentRoom = _roomManager.AddUserToRoom(roomName, user);
            }
            else
            {
                currentRoom = _roomManager.CreateRoom(roomName, user);
                await SendRoomsToClients();
            }
            await SendPlayersToClient(userConnection, user, currentRoom);

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
                player.Cards = new List<CardModel>();
                for (int i = 0; i < 10; i++)
                {
                    player.Cards.Add(new CardModel(i.ToString(), player.Id));
                }
            }
            await Clients.Client(Context.ConnectionId).SendAsync("SetPlayer", players[0]);
            await Clients.Group(roomName).SendAsync("UpdatePlayers", players);
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
                currentRoom.UserModels[0].IsAdmin = true;
                _roomManager.SaveRoom(currentRoom);
                await Clients.Group(userConnection.Room)
                   .SendAsync("ReceiveMessage", _botUser, $"{userConnection.User.Name} has left");
                await SendRoomsToClients();
                await Clients.Group(userConnection.Room).SendAsync("UpdatePlayers", currentRoom.UserModels);
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

        private async Task SendPlayersToClient(UserConnection userConnection, UserModel user, Room currentRoom)
        {
            await Clients.Client(Context.ConnectionId).SendAsync("SetPlayer", user);
            await Clients.Group(userConnection.Room).SendAsync("UpdatePlayers", currentRoom.UserModels);
        }
        private async Task SendRoomsToClients()
        {
            await Clients.Group("Lobby")
                .SendAsync("ReceiveRooms", _roomManager.GetAllRooms());
        }
    }
}
