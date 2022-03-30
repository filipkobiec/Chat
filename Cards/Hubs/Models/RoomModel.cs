using Cards.Hubs.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cards.Hubs
{
    public class RoomModel
    {
        public UserModel Admin { get; set; }
        public bool IsRoomEmpty => UserModels.Count == 0;
        public List<UserModel> UserModels { get; set; }
        public string roomName { get; }
        public List<CardModel> chosenCards {get; set;}
        public CardModel BlackCard { get; set; }
        public List<CardModel> BlackCards { get; set; }
        public RoomModel(string roomName, UserModel admin)
        {
            this.roomName = roomName;
            UserModels = new List<UserModel>();
            BlackCards = new List<CardModel>();
            UserModels.Add(admin);
            Admin = admin;
            admin.IsAdmin = true;
            admin.IsPlayerTurn = true;
        }

        public bool RemoveUser(UserModel user) => UserModels.Remove(user);
    }
}
