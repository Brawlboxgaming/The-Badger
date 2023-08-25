using DSharpPlus.Entities;
using DSharpPlus;
using DSharpPlus.SlashCommands;
using Badger.Class;

namespace Badger.Commands
{
    public class Testing : ApplicationCommandModule
    {
        [SlashCommand("test", "This is a test")]
        public async Task Test(InteractionContext ctx)
        {
            try
            {
                await ctx.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource, new DiscordInteractionResponseBuilder() { IsEphemeral = true });

                DiscordEmbedBuilder embed = new()
                {
                    Color = new DiscordColor("#FFFFFF"),
                    Title = "__**Success:**__",
                    Description = $"*This was a successful test.*",
                    Footer = new DiscordEmbedBuilder.EmbedFooter
                    {
                        Text = $"Server Time: {DateTime.Now}"
                    }
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
