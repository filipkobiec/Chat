using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Cards.Hubs.Models
{
    public class CardModel
    {
        public Guid Id { get; set; }
        public Guid OwnerId { get; set; }
        public bool IsVisible { get; set; }
        public string Text { get; set; }

        [JsonConstructor]
        public CardModel()
        {
        }
        public CardModel(string text, Guid ownerId)
        {
            Id = Guid.NewGuid();
            Text = text;
            OwnerId = ownerId;
            IsVisible = true;
        }

        public CardModel(string text)
        {
            Id = Guid.NewGuid();
            Text = text;
            IsVisible = true;
        }

    }
}
