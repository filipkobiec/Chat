using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Cards.Hubs.Models
{
    public class CardModel
    {
        public string Text { get; set; }
        public Guid OwnerId { get; set; }

        [JsonConstructor]
        public CardModel()
        {
        }
        public CardModel(string text, Guid ownerId)
        {
            Text = text;
            OwnerId = ownerId;
        }

        public CardModel(string text)
        {
            Text = text;
        }

    }
}
