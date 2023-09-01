using DSharpPlus.Entities;

namespace Badger.Classes
{
    public class Game
    {
        public string Value { get; set; }
        public string DisplayName { get; set; }
        public ulong ID { get; set; }
        public int MinPlayers { get; set; }
        public List<DiscordUser> Players { get; set; }
        public string Duration { get; set; }
        public Timer? Timer { get; set; }
        public DiscordMessage? Message { get; set; }
        public Game(string value, string displayName, ulong id, int minPlayers = -1, string duration = "N/A")
        {
            Value = value;
            DisplayName = displayName;
            ID = id;
            MinPlayers = minPlayers;
            Timer = null;
            Message = null;
            Players = new List<DiscordUser>();
            Duration = duration;
        }
    }

    public class NewGame
    {
        public Game Game { get; set; }
        public string Name { get; set; }
        public string Duration { get; set; }
        public Timer? Timer { get; set; }
        public DiscordMessage? Message { get; set; }
        public DateTime Created { get; }
        public NewGame(Game game, string name, string duration = "Indefinite")
        {
            Game = game;
            Name = name;
            Duration = duration;
            Timer = null;
            Message = null;
            Created = DateTime.Now;
        }
    }
}
