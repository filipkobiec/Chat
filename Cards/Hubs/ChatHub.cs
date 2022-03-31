using Cards.Hubs.Models;
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
        private readonly List<CardModel> _blackCards;
        private readonly IDictionary<string, UserModel> _connections;

        public ChatHub(IRoomManager roomManager, IDictionary<string, UserModel> connections)
        {
            _connections = connections;
            _botUser = "MyChat Bot";
            _roomManager = roomManager;
            _blackCards = new List<CardModel>();
            for (int i = 0; i < 10; i++)
            {
                _blackCards.Add(new CardModel(i.ToString()));
            }
        }
        public async Task JoinRoom(UserModel player, Guid roomId)
        {
            var room = _roomManager.GetRoom(roomId);
            player.RoomId = room.Id;
            player.Id = Guid.NewGuid();
            player.ConnectionId = Context.ConnectionId;
            await HandleJoiningRoomByClient(room, player);
            _roomManager.AddUserToRoom(roomId, player);
            await UpdateRoom(player, room);
        }

        public async Task CreateRoom(UserModel player, string roomName)
        {
            
            player.Id = Guid.NewGuid();
            player.ConnectionId = Context.ConnectionId;
            var room = _roomManager.CreateRoom(roomName, player);
            player.RoomId = room.Id;
            await HandleJoiningRoomByClient(room, player);
            await SendRoomsToClients();
            await UpdateRoom(player, room);

        }
        public async Task SendMessage(UserModel player, string message)
        {
            await Clients.Group(player.RoomId.ToString())
                .SendAsync("ReceiveMessage", player.Name, message);
        }

        public async Task GetRooms()
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, "Lobby");
            await SendRoomsToClients();
        }

        public async Task StartGame(UserModel clientPlayer)
        {
            var currentRoom = _roomManager.GetRoom(clientPlayer.RoomId);
            currentRoom.BlackCards = _blackCards;
            currentRoom.BlackCard = currentRoom.BlackCards[0];
            currentRoom.ChosenCards = new List<CardModel>();
            var players = currentRoom.UserModels;

            foreach (var player in players)
            {
                if (player.IsAdmin)
                {
                    currentRoom.CardChar = player;
                    player.IsPlayerCardChar = true;
                    player.IsPlayerTurn = false;
                }
                else
                    player.IsPlayerTurn = true;

                player.Cards = new List<CardModel>();
                for (int i = 0; i < 10; i++)
                {
                    player.Cards.Add(new CardModel(i.ToString(), player.Id));
                }
                await Clients.Client(player.ConnectionId).SendAsync("SetPlayer", player);
            }
            await Clients.Group(currentRoom.Id.ToString()).SendAsync("UpdateRoom", currentRoom);
        }

        public async Task GetChosenCard(UserModel player, CardModel card)
        {
            player.IsPlayerTurn = false;
            var playerRoom = _roomManager.GetRoom(player.RoomId);
            player.Cards.RemoveAll(c => c.Id == card.Id);
            playerRoom.ChosenCards.Add(card);
            if (playerRoom.ChosenCards.Count == playerRoom.UserModels.Count - 1)
            {
                var cardChar = playerRoom.CardChar;
                cardChar.IsPlayerTurn = true;
                await Clients.Client(cardChar.ConnectionId).SendAsync("SetPlayer", cardChar);
            }
            await UpdateRoom(player, playerRoom);
        }

        public async Task HandleWinnerCard(CardModel card, string roomId)
        {
            var room = _roomManager.GetRoom(Guid.Parse(roomId));
            var players = room.UserModels;
            var player = players.FirstOrDefault(p => p.Id == card.OwnerId);
            player.Points++;
            await Clients.Group(room.Id.ToString()).SendAsync("UpdateRoom", room);
        }

        public async Task CloseRoomConnection()
        {
            if (_connections.TryGetValue(Context.ConnectionId, out UserModel player))
            {
                var currentRoom = _roomManager.GetRoom(player.RoomId);
                currentRoom.UserModels.Remove(player);
                _connections.Remove(Context.ConnectionId);
                await Groups.AddToGroupAsync(Context.ConnectionId, "Lobby");

                if (currentRoom.IsRoomEmpty)
                {
                    _roomManager.RemoveRoom(currentRoom.Id);
                    await SendRoomsToClients();
                }
                else
                {
                    var newAdmin = currentRoom.UserModels[0];
                    newAdmin.IsAdmin = true;
                    _roomManager.SaveRoom(currentRoom);
                    await Clients.Group(currentRoom.Id.ToString())
                       .SendAsync("ReceiveMessage", _botUser, $"{player.Name} has left");
                    await SendRoomsToClients();
                    await Clients.Client(newAdmin.ConnectionId).SendAsync("SetPlayer", newAdmin);
                    await Clients.Group(currentRoom.Id.ToString()).SendAsync("UpdateRoom", currentRoom);
                }
            }

        }
        public override Task OnDisconnectedAsync(Exception exception)
        {
            if (_connections.TryGetValue(Context.ConnectionId, out UserModel player))
            {
                var currentRoom = _roomManager.GetRoom(player.RoomId);
                currentRoom.UserModels.Remove(player);
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
                        .SendAsync("ReceiveMessage", _botUser, $"{player.Name} has left");
                    Clients.Group(currentRoom.Id.ToString())
                        .SendAsync("UpdateRoom", currentRoom);
                }
            }

            return base.OnDisconnectedAsync(exception);
        }
        private async Task HandleJoiningRoomByClient(RoomModel room, UserModel player)
        {
            _connections[Context.ConnectionId] = player;
            await Groups.AddToGroupAsync(Context.ConnectionId, room.Id.ToString());
            await Clients.Group(room.Id.ToString()).SendAsync("ReceiveMessage", _botUser,
                $"{player.Name} has joined {room.roomName}");
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, "Lobby");
        }

        private async Task UpdateRoom(UserModel user, RoomModel currentRoom)
        {
            await Clients.Client(Context.ConnectionId).SendAsync("SetPlayer", user);
            await Clients.Group(currentRoom.Id.ToString()).SendAsync("UpdateRoom", currentRoom);
        }
        private async Task SendRoomsToClients()
        {
            await Clients.Group("Lobby")
                .SendAsync("ReceiveRooms", _roomManager.GetAllRooms());
        }
    }
}
