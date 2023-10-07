using Badger.Class;
using Badger.Classes;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using System.Text.RegularExpressions;
using System.Threading.Channels;

namespace Badger
{
    public class Interactions
    {
        private static readonly Game[] _games = new[]
        {
            new Game("mao", "Mao", 692901690483146762, 4, "Indefinite"),
            new Game("tac", "TAC", 692901782611034173, 4, "c.1-2 hours"),
            new Game("social", "Social Deduction", 692901751094902895, 4),
            new Game("betrayal", "Betrayal", 692904039620411422, 4, "1+ hours"),
            new Game("talking", "Talking Games", 692905746740019320, 3),
            new Game("trivia", "Trivia Games", 786195625300983838, 3),
            new Game("new", "New Games", 1119710634532143164),
        };

        private static List<NewGame> newGames = new List<NewGame>();

        public async Task AssignAllInteractions()
        {
            Bot.Client.InteractionCreated += LogInteractions;
            Bot.Client.VoiceStateUpdated += UpdateVoiceChannels;

            Bot.Client.ComponentInteractionCreated += async (c, e) =>
            {
                if (e.Id == "plusButton")
                {
                    await AfterJoinGameSelect(e, true);
                }
                if (e.Id == "minusButton")
                {
                    await AfterJoinGameSelect(e, false);
                }
                if (e.Id == "gamemode_select" && _games.Any(x => x.Value == e.Values[0]))
                {
                    await AfterGameSelect(e);
                }
            };

            Bot.Client.ModalSubmitted += async (c, e) =>
            {
                if (e.Interaction.Data.CustomId == "newGameModal")
                {
                    await AfterNewGameSubmit(e);
                }
            };

            await Task.CompletedTask;
        }

        private static void GameTimerCallback(object? obj)
        {
            if (obj is Game)
            {
                Game game = (Game)obj;

                game.Timer.Dispose();
                game.Timer = null;
                game.Message.DeleteAsync().ConfigureAwait(false);
                game.Message = null;
            }
            else if (obj is NewGame)
            {
                NewGame newGame = (NewGame)obj;

                newGame.Timer.Dispose();
                newGame.Message.DeleteAsync().ConfigureAwait(false);
                newGames.Remove(newGame);
            }
        }

        private async Task AfterNewGameSubmit(ModalSubmitEventArgs eventArgs)
        {
            await eventArgs.Interaction.CreateResponseAsync(InteractionResponseType.DeferredMessageUpdate);

            Game game = _games.First(x => x.Value == "new");
            NewGame newGame = new NewGame(game, eventArgs.Values["name"], eventArgs.Values["duration"] == "" ? "Not specified" : eventArgs.Values["duration"]);

            DiscordChannel logChannel = Bot.Client.GetGuildAsync(343867882264068098).Result.GetChannel(1144368862507303003);
            await logChannel.SendMessageAsync(new DiscordEmbedBuilder()
            {
                Color = new DiscordColor("#FFFFFF"),
                Title = $"__**Notice:**__",
                Description = $"{eventArgs.Interaction.User.Mention} is looking to play {game.DisplayName} - {newGame.Name}.",
                Footer = new DiscordEmbedBuilder.EmbedFooter
                {
                    Text = $"Server Time: {DateTime.Now}"
                }
            });

            DiscordEmbedBuilder embed = new()
            {
                Color = new DiscordColor("#FFFFFF"),
                Description = $"# Let's Play {newGame.Name}:" +
                $"\nPress the + button to join up to play." +
                $"\n## Duration: *{newGame.Duration}*" +
                $"\n## Current Players:" +
                $"\n{eventArgs.Interaction.User.Mention}",
                Footer = new DiscordEmbedBuilder.EmbedFooter
                {
                    Text = $"Server Time: {DateTime.Now}"
                }
            };

            DiscordButtonComponent plusButton = new(ButtonStyle.Success, "plusButton", "", false, new DiscordComponentEmoji(DiscordEmoji.FromName(Bot.Client, ":heavy_plus_sign:")));
            DiscordButtonComponent minusButton = new(ButtonStyle.Danger, "minusButton", "", false, new DiscordComponentEmoji(DiscordEmoji.FromName(Bot.Client, ":heavy_minus_sign:")));

            DiscordMessageBuilder messageBuilder = new DiscordMessageBuilder()
            .WithContent($"<@&{game.ID}>")
            .WithAllowedMentions(new IMention[] { new RoleMention(game.ID) })
            .AddEmbed(embed)
            .AddComponents(plusButton, minusButton);

            DiscordChannel ltpChannel = Bot.Client.GetGuildAsync(343867882264068098).Result.GetChannel(1147123572490371072);

            DiscordMessage ltpMessage = await ltpChannel.SendMessageAsync(messageBuilder);
            IReadOnlyList<DiscordMessage> ltpMessages = await ltpChannel.GetMessagesAsync();
            DiscordMessage ogMessage = ltpMessages[ltpMessages.Count - 1];
            await ogMessage.ModifyAsync(ogMessage.Embeds[0]);

            newGame.Message = ltpMessage;
            newGame.Timer = new Timer(GameTimerCallback, newGame, 3600000, 0);

            newGames.Add(newGame);
            newGames.OrderBy(x => x.Created);
        }

        private async Task AfterGameSelect(ComponentInteractionCreateEventArgs eventArgs)
        {
            Game game = _games.First(x => x.Value == eventArgs.Values[0]);
            game.Players.Add(eventArgs.User);

            if (game.Value == "new")
            {
                await eventArgs.Interaction.CreateResponseAsync(InteractionResponseType.Modal, new DiscordInteractionResponseBuilder()
                .WithTitle("New Game Information")
                .WithCustomId("newGameModal")
                .AddComponents(new TextInputComponent(label: "Name", customId: "name", placeholder: "TAC", required: true, style: TextInputStyle.Short))
                .AddComponents(new TextInputComponent(label: "Expected Duration", customId: "duration", placeholder: "1 hour", required: false, style: TextInputStyle.Short))
                );
                await eventArgs.Message.ModifyAsync(eventArgs.Message.Embeds[0]);
                return;
            }
            else
            {
                DiscordChannel logChannel = Bot.Client.GetGuildAsync(343867882264068098).Result.GetChannel(1144368862507303003);
                await logChannel.SendMessageAsync(new DiscordEmbedBuilder()
                {
                    Color = new DiscordColor("#FFFFFF"),
                    Title = $"__**Notice:**__",
                    Description = $"{eventArgs.User.Mention} is looking to play {game.DisplayName}.",
                    Footer = new DiscordEmbedBuilder.EmbedFooter
                    {
                        Text = $"Server Time: {DateTime.Now}"
                    }
                });

                await eventArgs.Interaction.CreateResponseAsync(InteractionResponseType.DeferredMessageUpdate);
                if (game.Timer != null)
                {
                    DiscordFollowupMessageBuilder error = new DiscordFollowupMessageBuilder() { IsEphemeral = true }
                        .AddEmbed(new DiscordEmbedBuilder()
                        {
                            Color = new DiscordColor("#FFFFFF"),
                            Title = "Error:",
                            Description = $"{game.DisplayName} already has a pending notification.",
                            Footer = new DiscordEmbedBuilder.EmbedFooter
                            {
                                Text = $"Server Time: {DateTime.Now}"
                            }
                        });
                    await eventArgs.Interaction.CreateFollowupMessageAsync(error);
                    await eventArgs.Message.ModifyAsync(eventArgs.Message.Embeds[0]);
                    return;
                }
            }

            DiscordEmbedBuilder embed = new()
            {
                Color = new DiscordColor("#FFFFFF"),
                Description = $"# Let's Play {game.DisplayName}:" +
                $"\nPress the + button to join up to play." +
                $"\n## Duration: *{game.Duration}*" +
                $"\n## Current Players:" +
                $"\n{eventArgs.Interaction.User.Mention}",
                Footer = new DiscordEmbedBuilder.EmbedFooter
                {
                    Text = $"Server Time: {DateTime.Now}"
                }
            };

            DiscordButtonComponent plusButton = new(ButtonStyle.Success, "plusButton", "", false, new DiscordComponentEmoji(DiscordEmoji.FromName(Bot.Client, ":heavy_plus_sign:")));
            DiscordButtonComponent minusButton = new(ButtonStyle.Danger, "minusButton", "", false, new DiscordComponentEmoji(DiscordEmoji.FromName(Bot.Client, ":heavy_minus_sign:")));

            DiscordMessageBuilder messageBuilder = new DiscordMessageBuilder()
            .WithContent($"<@&{game.ID}>")
            .WithAllowedMentions(new IMention[] { new RoleMention(game.ID) })
            .AddEmbed(embed)
            .AddComponents(plusButton, minusButton);

            DiscordMessage ltpMessage = await eventArgs.Channel.SendMessageAsync(messageBuilder);
            await eventArgs.Message.ModifyAsync(eventArgs.Message.Embeds[0]);

            game.Message = ltpMessage;
            game.Timer = new Timer(GameTimerCallback, game, 3600000, 0);
        }

        private async Task AfterJoinGameSelect(ComponentInteractionCreateEventArgs eventArgs, bool add)
        {
            await eventArgs.Interaction.CreateResponseAsync(InteractionResponseType.DeferredMessageUpdate);

            Game game = _games.First(x => eventArgs.Message.Content.Contains(x.ID.ToString()));
            string displayName = game.DisplayName;
            string duration = game.Duration;
            if (game.Value == "new")
            {
                displayName = newGames.First(x => x.Game == game).Name;
                duration = newGames.First(x => x.Game == game).Duration;
            }
            if (add)
            {
                if (game.Players.Contains(eventArgs.User)) return;
                game.Players.Add(eventArgs.User);
            }
            else
            {
                if (!game.Players.Contains(eventArgs.User)) return;
                game.Players.Remove(eventArgs.User);
            }
            string playerList = "";
            if (game.Players.Count > 0)
            {
                foreach (var player in game.Players)
                {
                    playerList += $"\n{player.Mention}";
                }
            }
            else
            {
                playerList = "\n*No players*";
            }
            DiscordEmbed embed = new DiscordEmbedBuilder()
            {
                Color = new DiscordColor("#FFFFFF"),
                Description = $"# Let's Play {displayName}:" +
                    $"\nPress the + button to join up to play." +
                    $"\n## Duration: *{duration}*" +
                    $"\n## Current Players:" +
                    $"{playerList}",
                Footer = new DiscordEmbedBuilder.EmbedFooter
                {
                    Text = $"Server Time: {DateTime.Now}"
                }
            };
            if (game.Value == "new")
            {
                NewGame newGame = newGames.First(x => x.Game == game);
                await newGame.Message.ModifyAsync(embed);
            }
            else
            {
                await game.Message.ModifyAsync(embed);

                if (game.Players.Count == game.MinPlayers)
                {
                    await game.Message.DeleteAsync();
                    game.Message = null;

                    game.Timer.Dispose();
                    game.Timer = null;

                    DiscordEmbedBuilder finalEmbed = new()
                    {
                        Color = new DiscordColor("#FFFFFF"),
                        Description = $"# Let's Play {displayName}:" +
                            $"\nYou have reached the minimum/recomended number of players required to play!" +
                            $"\n## Players:" +
                            $"{playerList}",
                        Footer = new DiscordEmbedBuilder.EmbedFooter
                        {
                            Text = $"Server Time: {DateTime.Now}"
                        }
                    };
                    IMention[] userMentions = new IMention[game.Players.Count];
                    for (int i = 0; i < userMentions.Length; i++)
                    {
                        userMentions[i] = new UserMention(game.Players[i].Id);
                    }
                    DiscordMessageBuilder messageBuilder = new DiscordMessageBuilder()
                        .WithContent(playerList)
                        .WithAllowedMentions(userMentions)
                        .AddEmbed(finalEmbed);

                    TimerMessage timerMessage = new TimerMessage(await eventArgs.Channel.SendMessageAsync(messageBuilder));
                }
            }
        }

        private async Task LogInteractions(DiscordClient client, InteractionCreateEventArgs eventArgs)
        {
            DiscordChannel channel = Bot.Client.GetGuildAsync(343867882264068098).Result.GetChannel(1144368862507303003);

            string options = "";

            if (eventArgs.Interaction.Data.Options != null)
            {
                foreach (DiscordInteractionDataOption option in eventArgs.Interaction.Data.Options)
                {
                    options += $" {option.Name}: *{option.Value}*";
                }
            }

            DiscordEmbedBuilder embed = new()
            {
                Color = new DiscordColor("#FFFFFF"),
                Title = $"__**Notice:**__",
                Description = $"'/{eventArgs.Interaction.Data.Name}{options}' was used by {eventArgs.Interaction.User.Mention}.",
                Footer = new DiscordEmbedBuilder.EmbedFooter
                {
                    Text = $"Server Time: {DateTime.Now}"
                }
            };
            await channel.SendMessageAsync(embed);
        }

        private bool _updatingChannels = false;
        private bool _eventDuringChannelUpdate = false;

        private async Task UpdateVoiceChannels(DiscordClient client, VoiceStateUpdateEventArgs eventArgs)
        {
        again:
            if (_updatingChannels)
            {
                _eventDuringChannelUpdate = true;
                return;
            }
            _updatingChannels = true;
            try
            {
                // Check if the channel(s) has/have the format "<name> #<number>"
                string? channelName = null;
                Match m;
                if (eventArgs.Before != null && eventArgs.Before.Channel != null && (m = Regex.Match(eventArgs.Before.Channel.Name, @"^(.*) #\d+$")).Success)
                {
                    channelName = m.Groups[1].Value;
                    await UpdateVoiceChannelCollection(eventArgs.Guild, channelName);
                }
                if (eventArgs.After != null && eventArgs.After.Channel != null &&
                    (m = Regex.Match(eventArgs.After.Channel.Name, @"^(.*) #\d+$")).Success && m.Groups[1].Value != channelName)
                {
                    await UpdateVoiceChannelCollection(eventArgs.Guild, m.Groups[1].Value);
                }

                _updatingChannels = false;
                if (_eventDuringChannelUpdate)
                {
                    _eventDuringChannelUpdate = false;
                    goto again;
                }
            }
            catch (Exception ex)
            {
                await Util.ThrowInteractionlessError(ex);
            }
            finally
            {
                _updatingChannels = false;
                _eventDuringChannelUpdate = false;
            }
        }

        private static async Task UpdateVoiceChannelCollection(DiscordGuild guild, string name)
        {
            // Get a list of all voice channels with this name, their number, and the number of users in them
            var channelInfos = (await guild.GetChannelsAsync())
                .Select(ch => new { Channel = ch, Match = Regex.Match(ch.Name, $@"^{Regex.Escape(name)} #(\d+)$") })
                .Where(inf => inf.Match.Success)
                .Select(inf => new { inf.Channel, UserCount = inf.Channel.Users.Count, Number = int.Parse(inf.Match.Groups[1].Value) })
                .OrderBy(inf => inf.Number)
                .ToList();

            // Delete empty voice channels except for the lowest-numbered one
            bool isFirst = true;
            for (int chIx = 0; chIx < channelInfos.Count; chIx++)
            {
                if (channelInfos[chIx].UserCount == 0)
                {
                    if (isFirst)
                    {
                        isFirst = false;
                    }
                    else
                    {
                        DiscordChannel logChannel = Bot.Client.GetGuildAsync(343867882264068098).Result.GetChannel(1144368862507303003);
                        DiscordEmbedBuilder embed = new()
                        {
                            Color = new DiscordColor("#FFFFFF"),
                            Title = $"__**Notice:**__",
                            Description = $"Deleted VC {channelInfos[chIx].Channel.Name}.",
                            Footer = new DiscordEmbedBuilder.EmbedFooter
                            {
                                Text = $"Server Time: {DateTime.Now}"
                            }
                        };

                        await logChannel.SendMessageAsync(embed);

                        if (channelInfos[chIx].Channel.GetMessagesAsync(1000).Result.Count > 0)
                        {
                            string txtFile = $"Last 1000 Messages from {channelInfos[chIx].Channel.Name}:\r\n";
                            var messages = channelInfos[chIx].Channel.GetMessagesAsync(1000).Result.ToList();
                            messages.Reverse();
                            foreach (var message in messages)
                            {
                                txtFile += $"{message.CreationTimestamp} - {message.Content}";
                                foreach (var attachment in message.Attachments)
                                {
                                    txtFile += $" {attachment.Url}";
                                }
                                txtFile += "\r\n";
                            }
                            string fileName = $"{DateTime.Now.ToString().Replace(":", "").Replace("/", "-")} - {channelInfos[chIx].Channel.Name}.txt";
                            await File.WriteAllTextAsync(fileName, txtFile);
                            Stream stream = File.Open(fileName, FileMode.Open);
                            await logChannel.SendMessageAsync(new DiscordMessageBuilder().AddFile(fileName, stream));
                            stream.Close();
                            await stream.DisposeAsync();
                            File.Delete(fileName);
                        }
                        await channelInfos[chIx].Channel.DeleteAsync();
                        channelInfos.RemoveAt(chIx);
                        chIx--;
                    }
                }
            }

            // Rename channels whose numbers are now out of order
            bool hasEmpty = false;
            int curNum = 1;
            foreach (var ch in channelInfos)
            {
                if (ch.Number != curNum)
                {
                    DiscordChannel logChannel = Bot.Client.GetGuildAsync(343867882264068098).Result.GetChannel(1144368862507303003);
                    DiscordEmbedBuilder embed = new()
                    {
                        Color = new DiscordColor("#FFFFFF"),
                        Title = $"__**Notice:**__",
                        Description = $"Renamed VC {ch.Channel.Name} to {name} #{curNum}.",
                        Footer = new DiscordEmbedBuilder.EmbedFooter
                        {
                            Text = $"Server Time: {DateTime.Now}"
                        }
                    };

                    await logChannel.SendMessageAsync(embed);

                    await ch.Channel.ModifyAsync(cem => cem.Name = $"{name} #{curNum}");
                }
                curNum++;
                hasEmpty = hasEmpty || ch.UserCount == 0;
            }

            // Create a new channel if no channels are empty
            if (!hasEmpty)
            {
                DiscordChannel lastChannel = channelInfos.Last().Channel;
                await guild.CreateChannelAsync($"{name} #{curNum}", ChannelType.Voice, parent: lastChannel.Parent, position: lastChannel.Position);

                DiscordChannel logChannel = Bot.Client.GetGuildAsync(343867882264068098).Result.GetChannel(1144368862507303003);
                DiscordEmbedBuilder embed = new()
                {
                    Color = new DiscordColor("#FFFFFF"),
                    Title = $"__**Notice:**__",
                    Description = $"Created VC {name} #{curNum}.",
                    Footer = new DiscordEmbedBuilder.EmbedFooter
                    {
                        Text = $"Server Time: {DateTime.Now}"
                    }
                };

                await logChannel.SendMessageAsync(embed);
            }
        }
    }
}
