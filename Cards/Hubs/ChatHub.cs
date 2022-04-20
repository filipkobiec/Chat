using Cards.Data;
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
        private readonly IRoomManager _roomManager;
        private readonly IDictionary<string, UserModel> _connections;
        private readonly HubManager _hubManager;

        public ChatHub(IRoomManager roomManager, IDictionary<string, UserModel> connections)
        {
            _connections = connections;
            _roomManager = roomManager;
            _hubManager = new HubManager(this, _roomManager, connections);
        }
        public async Task CreateRoom(UserModel user, string roomName)
        {
            if (_roomManager.DoesRoomWithSameNameExist(roomName))
                await _hubManager.SendModalMessageToClient("Room with that name already exists");
            else
            {
                RoomModel room = _hubManager.CreateRoomWithAdmin(user, roomName);
                await _hubManager.SendInformationAboutUserToClient(room, user);
                await _hubManager.SendRoomsToClientsInLobby();
                await _hubManager.SendRoomToClientsInRoom(room);
                await _hubManager.RedirectClientToRoom(user, roomName);
            }
        }

        public async Task JoinRoom(UserModel user, Guid roomId)
        {
            var room = _roomManager.GetRoom(roomId);

            if (_roomManager.DoesUserWithSameNameExist(user.Name, room))
                await _hubManager.SendModalMessageToClient("Name is already taken");
            else
            {
                _hubManager.AssignUserToRoom(user, room);
                await _hubManager.SendInformationAboutUserToClient(room, user);
                _roomManager.AddUserToRoom(roomId, user);
                await _hubManager.SendRoomToClientsInRoom(room);
                await _hubManager.RedirectClientToRoom(user, room.RoomName);
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
            await _hubManager.SendRoomsToClientsInLobby();
        }

        public async Task KickUserFromRoom (Guid roomId, Guid userId)
        {
            var user = _roomManager.GetUserFromRoom(roomId, userId);
            await CloseRoomConnection(roomId, userId);
            await _hubManager.SendModalMessageToClient("Admin kicked you from room", user);
        }

        public async Task CloseRoomConnection(Guid roomId, Guid userId)
        {
            var user = _roomManager.GetUserFromRoom(roomId, userId);

            if (user != null)
            {
                var currentRoom = _roomManager.GetRoom(user.RoomId);
                _hubManager.RemoveAllUserData(user, currentRoom);
                await _hubManager.PlaceUserInLobby(user);

                if (currentRoom.IsRoomEmpty)
                    await _hubManager.RemoveRoom(currentRoom);
                else
                    await _hubManager.HandleUserLeavingRoom(user, currentRoom);

                await _hubManager.RedirectUserToMainMenu(user);
            }
        }

        public override async Task<Task> OnDisconnectedAsync(Exception exception)
        {
            var isUserRegistered = _connections.TryGetValue(Context.ConnectionId, out UserModel user);

            if (isUserRegistered)
            {
                var currentRoom = _roomManager.GetRoom(user.RoomId);
                _hubManager.RemoveAllUserData(user, currentRoom);

                if (currentRoom.IsRoomEmpty)
                    await _hubManager.RemoveRoom(currentRoom);
                else
                    await _hubManager.HandleUserLeavingRoom(user, currentRoom);

            }
            return base.OnDisconnectedAsync(exception);
        }
    }
}
