using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DSharpPlus.Net.Models;
using System;
using System.Diagnostics;

namespace Badger
{
    public class Interactions
    {
        public async Task AssignAllInteractions()
        {
            Bot.Client.InteractionCreated += LogInteractions;
            Bot.Client.MessageReactionAdded += CheckReaction;
            Bot.Client.VoiceStateUpdated += UpdateVoiceChannels;

            await Task.CompletedTask;
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

        private async Task CheckReaction(DiscordClient client, MessageReactionAddEventArgs eventArgs)
        {
            if (!eventArgs.User.IsBot)
            {
                ulong roleId = 0;
                string addedArg = "";
                if (eventArgs.Message.Id == 1144722214168559636)
                {
                    switch (eventArgs.Emoji.GetDiscordName())
                    {
                        case ":badger:":
                            roleId = 692901690483146762;
                            break;
                        case ":flower_playing_cards:":
                            roleId = 692901782611034173;
                            break;
                        case ":bust_in_silhouette:":
                            roleId = 692901751094902895;
                            break;
                        case ":house_abandoned:":
                            roleId = 692904039620411422;
                            break;
                        case ":trumpet:":
                            roleId = 692905746740019320;
                            break;
                        case ":bulb:":
                            roleId = 786195625300983838;
                            break;
                        case ":game_die:":
                            roleId = 1119710634532143164;
                            break;
                    }
                }
                else if (eventArgs.Message.Id == 1144722259869700256)
                {
                    switch (eventArgs.Emoji.GetDiscordName())
                    {
                        case ":one:":
                            roleId = 1144711085891530803;
                            break;
                        case ":two:":
                            roleId = 1144711171308519436;
                            break;
                        case ":three:":
                            roleId = 1144711212819554426;
                            break;
                        case ":four:":
                            roleId = 1144711874043203594;
                            break;
                        case ":five:":
                            roleId = 1144711984701521950;
                            break;
                        case ":six:":
                            roleId = 1144712139987235008;
                            break;
                        case ":seven:":
                            roleId = 1144712019342278716;
                            break;
                        case ":eight:":
                            roleId = 1144712241137057884;
                            break;
                    }
                }
                if (((DiscordMember)eventArgs.User).Roles.Any(x => x.Id == roleId))
                {
                    await ((DiscordMember)eventArgs.User).RevokeRoleAsync(eventArgs.Guild.GetRole(roleId));
                    addedArg = "removed from";
                }
                else
                {
                    await ((DiscordMember)eventArgs.User).GrantRoleAsync(eventArgs.Guild.GetRole(roleId));
                    addedArg = "given to";
                }
                await eventArgs.Message.DeleteReactionAsync(eventArgs.Emoji, eventArgs.User);

                DiscordChannel channel = Bot.Client.GetGuildAsync(343867882264068098).Result.GetChannel(1144368862507303003);

                DiscordEmbedBuilder embed = new()
                {
                    Color = new DiscordColor("#FFFFFF"),
                    Title = $"__**Notice:**__",
                    Description = $"<@&{eventArgs.Guild.GetRole(roleId).Id}> {addedArg} {eventArgs.User.Mention}.",
                    Footer = new DiscordEmbedBuilder.EmbedFooter
                    {
                        Text = $"Server Time: {DateTime.Now}"
                    }
                };
                await channel.SendMessageAsync(embed);
            }
        }

        private async Task UpdateVoiceChannels(DiscordClient client, VoiceStateUpdateEventArgs eventArgs)
        {
            // If someone is leaving a channel
            if (eventArgs.Before != null && eventArgs.Before.Channel != null)
            {
                if (eventArgs.Before.Channel.Name.Contains("#"))
                {
                    string[] vcInfo = eventArgs.Before.Channel.Name.Split('#');
                    List<DiscordChannel> channelCollection = await GetSimilarVoiceChannelsAsync(vcInfo, eventArgs, eventArgs.Before.Channel);
                    if (channelCollection.Count(x => x.Users.Count == 0) == 2)
                    {
                        channelCollection = channelCollection.OrderBy(x => int.Parse(x.Name.Split('#')[1])).ToList();
                        DiscordChannel highestNumChannel = channelCollection[0];
                        foreach (DiscordChannel channel in channelCollection)
                        {
                            if (int.Parse(channel.Name.Split('#')[1]) > int.Parse(highestNumChannel.Name.Split('#')[1]) && channel.Users.Count == 0)
                            {
                                highestNumChannel = channel;
                            }
                        }
                        await highestNumChannel.DeleteAsync();
                        int offset = 0;
                        for (int i = 0; i < channelCollection.Count; i++)
                        {
                            if (channelCollection[i] == highestNumChannel)
                            {
                                offset = -1;
                            }
                            else
                            {
                                Action<ChannelEditModel> action = new(x => x.Name = vcInfo[0] + $"#{i + offset + 1}");
                                await channelCollection[i].ModifyAsync(action);
                            }
                        }
                    }
                }
            }
            // If someone is joining a channel
            if (eventArgs.After != null && eventArgs.After.Channel != null)
            {
                if (eventArgs.After.Channel.Name.Contains("#"))
                {
                    string[] vcInfo = eventArgs.After.Channel.Name.Split('#');
                    List<DiscordChannel> channelCollection = await GetSimilarVoiceChannelsAsync(vcInfo, eventArgs, eventArgs.After.Channel);
                    if (channelCollection.Count == 1)
                    {
                        await eventArgs.Guild.CreateChannelAsync($"{vcInfo[0]}#2", ChannelType.Voice, parent: channelCollection[0].Parent, position: channelCollection[0].Position);
                    }
                    else
                    {
                        if (eventArgs.After.Channel.Users.Count > 0 && channelCollection.All(x => x.Users.Count > 0))
                        {
                            await eventArgs.Guild.CreateChannelAsync($"{vcInfo[0]}#{channelCollection.Count + 1}", ChannelType.Voice, parent: channelCollection[0].Parent, position: channelCollection[0].Position);
                        }
                    }
                }
            }
        }

        private async Task<List<DiscordChannel>> GetSimilarVoiceChannelsAsync(string[] vcInfo, VoiceStateUpdateEventArgs eventArgs, DiscordChannel originalChannel)
        {
            List<DiscordChannel> similarChannels = new() { originalChannel };
            foreach (DiscordChannel channel in await eventArgs.Guild.GetChannelsAsync())
            {
                if (channel.Name.Contains(vcInfo[0]) && !channel.Name.Contains(vcInfo[1]))
                {
                    similarChannels.Add(channel);
                }
            }
            return similarChannels;
        }
    }
}
