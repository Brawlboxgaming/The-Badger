using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;

namespace Badger.Class
{
    public class Util
    {
        public static async Task ThrowError(InteractionContext ctx, Exception ex)
        {
            DiscordEmbedBuilder embed = new()
            {
                Color = new DiscordColor("#FFFFFF"),
                Title = $"__**Error:**__",
                Description = $"*{ex.Message}*",
                Footer = new DiscordEmbedBuilder.EmbedFooter
                {
                    Text = $"Server Time: {DateTime.Now}"
                }
            };
            await ctx.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(embed));

            DiscordChannel channel = Bot.Client.GetGuildAsync(343867882264068098).Result.GetChannel(1144368862507303003);

            string options = "";

            if (ctx.Interaction.Data.Options != null)
            {
                foreach (var option in ctx.Interaction.Data.Options)
                {
                    options += $" {option.Name}: *{option.Value}*";
                }
            }

            embed = new DiscordEmbedBuilder
            {
                Color = new DiscordColor("#FFFFFF"),
                Title = $"__**Error:**__",
                Description = $"'/{ctx.Interaction.Data.Name}{options}' was used by {ctx.User.Mention}." +
                $"\n\n{ex}",
                Footer = new DiscordEmbedBuilder.EmbedFooter
                {
                    Text = $"Server Time: {DateTime.Now}"
                }
            };
            var ping = "";
#if RELEASE
    ping = "<@105742694730457088>";
#endif
            await channel.SendMessageAsync(ping, embed);

            Console.WriteLine(ex.ToString());
        }

        public static async Task ThrowCustomError(InteractionContext ctx, string ex)
        {
            DiscordChannel channel = Bot.Client.GetGuildAsync(343867882264068098).Result.GetChannel(1144368862507303003);

            string options = "";

            DiscordEmbedBuilder embed = new();

            if (ctx != null)
            {
                if (ctx.Interaction.Data.Options != null)
                {
                    foreach (var option in ctx.Interaction.Data.Options)
                    {
                        options += $" {option.Name}: *{option.Value}*";
                    }
                }

                embed = new DiscordEmbedBuilder
                {
                    Color = new DiscordColor("#FFFFFF"),
                    Title = $"__**Error:**__",
                    Description = $"'/{ctx.Interaction.Data.Name}{options}' was used by  {ctx.User.Mention}." +
                    $"\n\n{ex}",
                    Footer = new DiscordEmbedBuilder.EmbedFooter
                    {
                        Text = $"Server Time: {DateTime.Now}"
                    }
                };
            }
            else
            {
                embed = new DiscordEmbedBuilder
                {
                    Color = new DiscordColor("#FFFFFF"),
                    Title = $"__**Error:**__",
                    Description = $"{ex}",
                    Footer = new DiscordEmbedBuilder.EmbedFooter
                    {
                        Text = $"Server Time: {DateTime.Now}"
                    }
                };
            }
            var ping = "";
#if RELEASE
    ping = "<@105742694730457088>";
#endif
            await channel.SendMessageAsync(ping, embed);

            Console.WriteLine(ex);
        }

        public static async Task ThrowInteractionlessError(Exception ex)
        {

            DiscordChannel channel = Bot.Client.GetGuildAsync(343867882264068098).Result.GetChannel(1144368862507303003);

            DiscordEmbedBuilder embed = new()
            {
                Color = new DiscordColor("#FFFFFF"),
                Title = $"__**Error:**__",
                Description = $"\n\n{ex}",
                Footer = new DiscordEmbedBuilder.EmbedFooter
                {
                    Text = $"Server Time: {DateTime.Now}"
                }
            };
            var ping = "";
#if RELEASE
    ping = "<@105742694730457088>";
#endif
            await channel.SendMessageAsync(ping, embed);
            Console.WriteLine(ex.ToString());
        }
    }
}
