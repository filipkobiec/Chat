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
        public async Task CreateRoom(UserModel user, string roomName)
        {
            if (_roomManager.DoesRoomWithSameNameExist(roomName))
                await SendModalMessageToClient("Room with that name already exists");
            else
            {
                RoomModel room = CreateRoomWithAdmin(user, roomName);
                await SendInformationAboutUserToClient(room, user);
                await SendRoomsToClientsInLobby();
                await SendRoomToClientsInRoom(room);
                await RedirectClientToRoom(user, roomName);
            }
        }

        public async Task JoinRoom(UserModel user, Guid roomId)
        {
            var room = _roomManager.GetRoom(roomId);

            if (_roomManager.DoesUserWithSameNameExist(user.Name, room))
                await SendModalMessageToClient("Name is already taken");
            else
            {
                AssignUserToRoom(user, room);
                await SendInformationAboutUserToClient(room, user);
                _roomManager.AddUserToRoom(roomId, user);
                await SendRoomToClientsInRoom(room);
                await RedirectClientToRoom(user, room.RoomName);
            }
        }

        public async Task SendMessage(UserModel user, string message)
        {   
            await Clients.Group(user.RoomId.ToString())
                .SendAsync("ReceiveMessage", user.Name, message);
        }

        public async Task GetRooms()
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, "Lobby");
            await SendRoomsToClientsInLobby();
        }

        public async Task KickUserFromRoom (Guid roomId, Guid userId)
        {
            var user = _roomManager.GetUserFromRoom(roomId, userId);
            await CloseRoomConnection(roomId, userId);
            await SendModalMessageToClient("Admin kicked you from room", user);
        }

        public async Task CloseRoomConnection(Guid roomId, Guid userId)
        {
            var user = _roomManager.GetUserFromRoom(roomId, userId);

            if (user != null)
            {
                var currentRoom = _roomManager.GetRoom(user.RoomId);
                RemoveAllUserData(user, currentRoom);
                await PlaceUserInLobby(user);

                if (currentRoom.IsRoomEmpty)
                    await RemoveRoom(currentRoom);
                else
                    await HandleUserLeavingRoom(user, currentRoom);

                await RedirectUserToMainMenu(user);
            }
        }

        public override async Task<Task> OnDisconnectedAsync(Exception exception)
        {
            var isUserRegistered = _connections.TryGetValue(Context.ConnectionId, out UserModel user);

            if (isUserRegistered)
            {
                var currentRoom = _roomManager.GetRoom(user.RoomId);
                RemoveAllUserData(user, currentRoom);

                if (currentRoom.IsRoomEmpty)
                    await RemoveRoom(currentRoom);
                else
                    await HandleUserLeavingRoom(user, currentRoom);

            }
            return base.OnDisconnectedAsync(exception);
        }

        private async Task SendRoomToClientsInRoom(RoomModel currentRoom)
        {
            await Clients.Group(currentRoom.Id.ToString()).SendAsync("UpdateRoom", currentRoom);
        }

        private async Task SendRoomsToClientsInLobby()
        {
            await Clients.Group("Lobby")
                .SendAsync("ReceiveRooms", _roomManager.GetAllRooms());
        }

        private async Task SendInformationAboutUserToClient(RoomModel room, UserModel user)
        {
            _connections[Context.ConnectionId] = user;
            await Groups.AddToGroupAsync(Context.ConnectionId, room.Id.ToString());
            await Clients.Client(user.ConnectionId).SendAsync("SetUser", user);   
            await Clients.Group(room.Id.ToString()).SendAsync("ReceiveMessage", _botUser,
                $"{user.Name} has joined {room.RoomName}");
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, "Lobby");
        }

        private async Task HandleUserLeavingRoom(UserModel user, RoomModel currentRoom)
        {
            if (user.IsAdmin)
            {
                UserModel newAdmin = SetupNewAdmin(currentRoom);
                currentRoom.Admin = newAdmin;
                await Clients.Client(newAdmin.ConnectionId).SendAsync("SetUser", newAdmin);

            }

            await Clients.Group(currentRoom.Id.ToString())
               .SendAsync("ReceiveMessage", _botUser, $"{user.Name} has left");
            await SendRoomsToClientsInLobby();
            await Clients.Group(currentRoom.Id.ToString()).SendAsync("UpdateRoom", currentRoom);
        }

        private static UserModel SetupNewAdmin(RoomModel currentRoom)
        {
            var newAdmin = currentRoom.UserModels[0];
            newAdmin.IsAdmin = true;
            currentRoom.Admin = newAdmin;
            return newAdmin;
        }

        private RoomModel CreateRoomWithAdmin(UserModel user, string roomName)
        {
            user.Id = Guid.NewGuid();
            user.ConnectionId = Context.ConnectionId;
            user.IsAdmin = true;
            var room = _roomManager.CreateRoom(roomName, user);
            user.RoomId = room.Id;
            return room;
        }
        private void AssignUserToRoom(UserModel user, RoomModel room)
        {
            user.RoomId = room.Id;
            user.Id = Guid.NewGuid();
            user.ConnectionId = Context.ConnectionId;
        }

        private async Task RemoveRoom(RoomModel currentRoom)
        {
            _roomManager.RemoveRoom(currentRoom.Id);
            await SendRoomsToClientsInLobby();
        }

        private void RemoveAllUserData(UserModel user, RoomModel currentRoom)
        {
            currentRoom.UserModels.Remove(user);
            _connections.Remove(user.ConnectionId);
        }

        private async Task PlaceUserInLobby(UserModel user)
        {
            await Groups.AddToGroupAsync(user.ConnectionId, "Lobby");
        }

        private async Task RedirectUserToMainMenu(UserModel user)
        {
            await Clients.Client(user.ConnectionId).SendAsync("RedirectUserToMainMenu");
        }

        private async Task RedirectClientToRoom(UserModel user, string roomName)
        {
            await Clients.Client(user.ConnectionId).SendAsync("RedirectToRoom", roomName);
        }

        private async Task SendModalMessageToClient(string message, UserModel user)
        {
            await Clients.Client(user.ConnectionId).SendAsync("SetModalMessage", message);
        }

        private async Task SendModalMessageToClient(string message)
        {
            await Clients.Client(Context.ConnectionId).SendAsync("SetModalMessage", message);
        }
    }
}
