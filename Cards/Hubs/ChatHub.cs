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


        public async Task CloseRoomConnection()
        {
            if (_connections.TryGetValue(Context.ConnectionId, out UserModel player))
            {
                var currentRoom = _roomManager.GetRoom(player.RoomId);
                currentRoom.UserModels.Remove(player);
                _connections.Remove(Context.ConnectionId);
                await Groups.AddToGroupAsync(player.ConnectionId, "Lobby");

                if (currentRoom.IsRoomEmpty)
                    await RemoveRoom(currentRoom);
                else
                    await HandlePlayerLeavingRoom(player, currentRoom);
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

        private async Task UpdateRoom(UserModel user, RoomModel currentRoom)
        {
            await Clients.Group(currentRoom.Id.ToString()).SendAsync("UpdateRoom", currentRoom);
        }

        private async Task SendRoomsToClients()
        {
            await Clients.Group("Lobby")
                .SendAsync("ReceiveRooms", _roomManager.GetAllRooms());
        }

        private async Task HandleJoiningRoomByClient(RoomModel room, UserModel player)
        {
            _connections[Context.ConnectionId] = player;
            await Groups.AddToGroupAsync(Context.ConnectionId, room.Id.ToString());
            await Clients.Client(player.ConnectionId).SendAsync("SetPlayer", player);   
            await Clients.Group(room.Id.ToString()).SendAsync("ReceiveMessage", _botUser,
                $"{player.Name} has joined {room.roomName}");
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, "Lobby");
        }

        private async Task HandlePlayerLeavingRoom(UserModel player, RoomModel currentRoom)
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

        private async Task RemoveRoom(RoomModel currentRoom)
        {
            _roomManager.RemoveRoom(currentRoom.Id);
            await SendRoomsToClients();
        }

    }
}
