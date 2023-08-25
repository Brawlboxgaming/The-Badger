using Badger.Class;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;

namespace Badger.Commands
{
    public class Info : ApplicationCommandModule
    {
        [SlashCommand("help", "Lists all the commands that can be used.")]
        public async Task Help(InteractionContext ctx)
        {
            await ctx.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource, new DiscordInteractionResponseBuilder() { IsEphemeral = true });

            DiscordEmbedBuilder embed = new()
            {
                Color = new DiscordColor("#FFFFFF"),
                Title = $"__**List of commands:**__",
                Description = "/help\n" +
                "/source\n" +
                "/roles",
                Footer = new DiscordEmbedBuilder.EmbedFooter
                {
                    Text = $"Server Time: {DateTime.Now}"
                }
            };
            await ctx.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(embed));
        }
        [SlashCommand("source", "Links to the GitHub page.")]
        public async Task Source(InteractionContext ctx)
        {
            await ctx.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource, new DiscordInteractionResponseBuilder() { IsEphemeral = true });

            DiscordEmbedBuilder embed = new()
            {
                Color = new DiscordColor("#FFFFFF"),
                Title = $"__**Github:**__",
                Description = "Insert Link",
                Footer = new DiscordEmbedBuilder.EmbedFooter
                {
                    Text = $"Server Time: {DateTime.Now}"
                }
            };
            await ctx.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(embed));
        }
        [SlashCommand("roles", "Lists all of your roles in the server.")]
        public async Task Roles(InteractionContext ctx)
        {
            try
            {
                await ctx.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource, new DiscordInteractionResponseBuilder() { IsEphemeral = true });

                string description = "";

                foreach (DiscordRole role in ctx.Member.Roles)
                {
                    description += $"<@&{role.Id}>\n";
                }

                DiscordEmbedBuilder embed = new()
                {
                    Color = new DiscordColor("#FFFFFF"),
                    Title = $"__**Roles of {ctx.Member.DisplayName}:**__",
                    Description = description,
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
