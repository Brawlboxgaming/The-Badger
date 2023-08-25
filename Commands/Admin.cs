using DSharpPlus.Entities;
using DSharpPlus;
using DSharpPlus.SlashCommands;
using Badger.Class;
using DSharpPlus.CommandsNext.Converters;

namespace Badger.Commands
{
    public class Admin : ApplicationCommandModule
    {
        [SlashCommand("sendGameNotifs", "Sends the Game Notifications embed")]
        public async Task SendGameNotifs(InteractionContext ctx)
        {
            try
            {
                await ctx.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource, new DiscordInteractionResponseBuilder() { IsEphemeral = true });

                DiscordChannel channel = ctx.Guild.GetChannel(343870730301210624);

                DiscordEmbedBuilder embed = new()
                {
                    Color = new DiscordColor("#FFFFFF"),
                    Description = "# Game Notifications:\n" +
                    ":one: - <@&692901690483146762> *The game about figuring out the rules.*\n" +
                    ":two: - <@&692901782611034173> *A 4-player strategy board game with cards.*\n" +
                    ":three: - <@&692901751094902895> *e.g. Secret Hitler, etc.*\n" +
                    ":four: - <@&692904039620411422> *at the House on the Hill.*\n" +
                    ":five: - <@&692905746740019320> *e.g. Challenge & Contact, Wikipedia guessing games, etc.*\n" +
                    ":six: - <@&786195625300983838> *e.g. Know-it-All, Jeopardy, Trivial Pursuit, etc.*\n" +
                    ":seven: - <@&1119710634532143164> *Suggest a game not listed above.*",
                };
                DiscordMessage message = await channel.SendMessageAsync(embed);
                await message.CreateReactionAsync(DiscordEmoji.FromName(ctx.Client, ":badger:"));
                await message.CreateReactionAsync(DiscordEmoji.FromName(ctx.Client, ":flower_playing_cards:"));
                await message.CreateReactionAsync(DiscordEmoji.FromName(ctx.Client, ":bust_in_silhouette:"));
                await message.CreateReactionAsync(DiscordEmoji.FromName(ctx.Client, ":house_abandoned:"));
                await message.CreateReactionAsync(DiscordEmoji.FromName(ctx.Client, ":trumpet:"));
                await message.CreateReactionAsync(DiscordEmoji.FromName(ctx.Client, ":bulb:"));
                await message.CreateReactionAsync(DiscordEmoji.FromName(ctx.Client, ":game_die:"));

                embed = new()
                {
                    Color = new DiscordColor("#FFFFFF"),
                    Title = "__**Success:**__",
                    Description = "*Embed sent successfully.*",
                };
                await ctx.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(embed));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                await Util.ThrowError(ctx, ex);
            }
        }

        [SlashCommand("sendColourSelect", "Sends the Colour Selection embed")]
        public async Task SendColourSelect(InteractionContext ctx)
        {
            try
            {
                await ctx.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource, new DiscordInteractionResponseBuilder() { IsEphemeral = true });

                DiscordChannel channel = ctx.Guild.GetChannel(343870730301210624);

                DiscordEmbedBuilder embed = new()
                {
                    Color = new DiscordColor("#FFFFFF"),
                    Description = "# Colour Selection:\n" +
                    ":one: - <@&1144711085891530803>\n" +
                    ":two: - <@&1144711171308519436>\n" +
                    ":three: - <@&1144711212819554426>\n" +
                    ":four: - <@&1144711874043203594>\n" +
                    ":five: - <@&1144711984701521950>\n" +
                    ":six: - <@&1144712139987235008>\n" +
                    ":seven: - <@&1144712019342278716>\n" +
                    ":eight: - <@&1144712241137057884>",
                };
                DiscordMessage message = await channel.SendMessageAsync(embed);
                await message.CreateReactionAsync(DiscordEmoji.FromName(ctx.Client, ":one:"));
                await message.CreateReactionAsync(DiscordEmoji.FromName(ctx.Client, ":two:"));
                await message.CreateReactionAsync(DiscordEmoji.FromName(ctx.Client, ":three:"));
                await message.CreateReactionAsync(DiscordEmoji.FromName(ctx.Client, ":four:"));
                await message.CreateReactionAsync(DiscordEmoji.FromName(ctx.Client, ":five:"));
                await message.CreateReactionAsync(DiscordEmoji.FromName(ctx.Client, ":six:"));
                await message.CreateReactionAsync(DiscordEmoji.FromName(ctx.Client, ":seven:"));
                await message.CreateReactionAsync(DiscordEmoji.FromName(ctx.Client, ":eight:"));

                embed = new()
                {
                    Color = new DiscordColor("#FFFFFF"),
                    Title = "__**Success:**__",
                    Description = "*Embed sent successfully.*",
                };
                await ctx.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(embed));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                await Util.ThrowError(ctx, ex);
            }
        }
    }
}
