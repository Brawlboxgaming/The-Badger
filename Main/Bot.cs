using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Enums;
using DSharpPlus.Interactivity.Extensions;
using DSharpPlus.SlashCommands;
using Microsoft.Extensions.Logging;
using System.Text;
using Badger.Class;
using Badger.Commands;
using Newtonsoft.Json;

namespace Badger
{
    public class Bot
    {
        public Interactions interactions = new();
        public Events events = new();
        public static DiscordClient Client { get; private set; }
        public static SlashCommandsExtension SlashCommands { get; private set; }

        public async Task RunAsync()
        {
            string json = string.Empty;

            using (FileStream fs = File.OpenRead("config.json"))
            using (StreamReader sr = new(fs, new UTF8Encoding(false)))
                json = await sr.ReadToEndAsync().ConfigureAwait(false);

            BotConfig configJson = JsonConvert.DeserializeObject<BotConfig>(json);

            DiscordConfiguration config = new()
            {
                Token = configJson.Token,
                TokenType = TokenType.Bot,
                AutoReconnect = true,
                MinimumLogLevel = LogLevel.Debug
            };

            Client = new DiscordClient(config);

            Client.Ready += OnClientReady;

            Client.UseInteractivity(new InteractivityConfiguration
            {
                PollBehaviour = PollBehaviour.KeepEmojis,
                AckPaginationButtons = true,
                Timeout = TimeSpan.FromSeconds(60)
            });

            CommandsNextConfiguration commandsConfig = new()
            {
                EnableDms = false,
                EnableDefaultHelp = false,
                DmHelp = true
            };

            SlashCommands = Client.UseSlashCommands();
#if DEBUG
            SlashCommands.RegisterCommands<Testing>();
#endif
            SlashCommands.RegisterCommands<Info>();
            SlashCommands.RegisterCommands<Admin>();

            await events.AssignAllEvents();

            await interactions.AssignAllInteractions();

            DiscordActivity activity = new()
            {
                Name = $"figuring out the rules."
            };

            await Client.ConnectAsync(activity);

            await Task.Delay(-1);
        }

        private Task OnClientReady(DiscordClient sender, ReadyEventArgs e)
        {
            return Task.CompletedTask;
        }
    }
}
