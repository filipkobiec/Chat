using Cards.Hubs.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cards.Hubs
{
    public class RoomModel
    {
        public Guid Id { get; set; }
        public UserModel Admin { get; set; }
        public UserModel CardChar { get; set; }
        public bool IsRoomEmpty => UserModels.Count == 0;
        public List<UserModel> UserModels { get; set; }
        public string roomName { get; }
        public List<CardModel> ChosenCards {get; set;}
        public CardModel BlackCard { get; set; }
        public List<CardModel> BlackCards { get; set; }
        public RoomModel(string roomName, UserModel admin)
        {
            Id = Guid.NewGuid();
            this.roomName = roomName;
            UserModels = new List<UserModel>();
            BlackCards = new List<CardModel>();
            ChosenCards = new List<CardModel>();
            UserModels.Add(admin);
            Admin = admin;
            admin.IsAdmin = true;
            admin.IsPlayerTurn = true;
        }

        public bool RemoveUser(UserModel user) => UserModels.Remove(user);
    }
}
