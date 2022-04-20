using Cards.Hubs;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cards.Data
{
    public class HubManager
    {
        private readonly Hub _hub;
        private readonly IRoomManager _roomManager;
        private readonly IDictionary<string, UserModel> _connections;
        private readonly string _botUser;


        public HubManager(Hub hub, IRoomManager roomManager, IDictionary<string, UserModel> connections)
        {
            _hub = hub;
            _roomManager = roomManager;
            _connections = connections;
            _botUser = "MyChat Bot";
        }

        public async Task SendRoomToClientsInRoom(RoomModel currentRoom)
        {
            await _hub.Clients.Group(currentRoom.Id.ToString()).SendAsync("UpdateRoom", currentRoom);
        }

        public async Task SendRoomsToClientsInLobby()
        {
            await _hub.Clients.Group("Lobby")
                .SendAsync("ReceiveRooms", _roomManager.GetAllRooms());
        }

        public async Task SendInformationAboutUserToClient(RoomModel room, UserModel user)
        {
            _connections[_hub.Context.ConnectionId] = user;
            await _hub.Groups.AddToGroupAsync(_hub.Context.ConnectionId, room.Id.ToString());
            await _hub.Clients.Client(user.ConnectionId).SendAsync("SetUser", user);
            await _hub.Clients.Group(room.Id.ToString()).SendAsync("ReceiveMessage", _botUser,
                $"{user.Name} has joined {room.RoomName}");
            await _hub.Groups.RemoveFromGroupAsync(_hub.Context.ConnectionId, "Lobby");
        }

        public async Task HandleUserLeavingRoom(UserModel user, RoomModel currentRoom)
        {
            if (user.IsAdmin)
            {
                UserModel newAdmin = SetupNewAdmin(currentRoom);
                currentRoom.Admin = newAdmin;
                await _hub.Clients.Client(newAdmin.ConnectionId).SendAsync("SetUser", newAdmin);

            }

            await _hub.Clients.Group(currentRoom.Id.ToString())
               .SendAsync("ReceiveMessage", _botUser, $"{user.Name} has left");
            await SendRoomsToClientsInLobby();
            await _hub.Clients.Group(currentRoom.Id.ToString()).SendAsync("UpdateRoom", currentRoom);
        }

        public UserModel SetupNewAdmin(RoomModel currentRoom)
        {
            var newAdmin = currentRoom.UserModels[0];
            newAdmin.IsAdmin = true;
            currentRoom.Admin = newAdmin;
            return newAdmin;
        }

        public RoomModel CreateRoomWithAdmin(UserModel user, string roomName)
        {
            user.Id = Guid.NewGuid();
            user.ConnectionId = _hub.Context.ConnectionId;
            user.IsAdmin = true;
            var room = _roomManager.CreateRoom(roomName, user);
            user.RoomId = room.Id;
            return room;
        }
        public void AssignUserToRoom(UserModel user, RoomModel room)
        {
            user.RoomId = room.Id;
            user.Id = Guid.NewGuid();
            user.ConnectionId = _hub.Context.ConnectionId;
        }

        public async Task RemoveRoom(RoomModel currentRoom)
        {
            _roomManager.RemoveRoom(currentRoom.Id);
            await SendRoomsToClientsInLobby();
        }

        public void RemoveAllUserData(UserModel user, RoomModel currentRoom)
        {
            currentRoom.UserModels.Remove(user);
            _connections.Remove(user.ConnectionId);
        }

        public async Task PlaceUserInLobby(UserModel user)
        {
            await _hub.Groups.AddToGroupAsync(user.ConnectionId, "Lobby");
        }

        public async Task RedirectUserToMainMenu(UserModel user)
        {
            await _hub.Clients.Client(user.ConnectionId).SendAsync("RedirectUserToMainMenu");
        }

        public async Task RedirectClientToRoom(UserModel user, string roomName)
        {
            await _hub.Clients.Client(user.ConnectionId).SendAsync("RedirectToRoom", roomName);
        }

        public async Task SendModalMessageToClient(string message, UserModel user)
        {
            await _hub.Clients.Client(user.ConnectionId).SendAsync("SetModalMessage", message);
        }

        public async Task SendModalMessageToClient(string message)
        {
            await _hub.Clients.Client(_hub.Context.ConnectionId).SendAsync("SetModalMessage", message);
        }
    }
}
