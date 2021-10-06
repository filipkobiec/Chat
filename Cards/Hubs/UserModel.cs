using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cards.Hubs
{
    public class UserModel
    {
        public string Name { get; set; }
        public bool IsAdmin { get; set; }
        public int Points { get; set; }
    }
}
