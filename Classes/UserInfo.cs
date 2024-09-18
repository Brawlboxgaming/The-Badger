using DSharpPlus.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Badger.Classes
{
    public class UserInfo
    {
        public UserInfo(string name, ulong discordID)
        {
            Name = name;
            DiscordID = discordID;
            LastActive = DateTime.Now;
        }
        [Key] public int ID { get; set; }
        public string Name { get; set; }
        public ulong DiscordID { get; set; }
        public DateTime LastActive { get; set; }
    }
}
