using Badger.Class;
using Badger.Classes;
using FluentScheduler;
using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Badger.Main
{
    public class ScheduledTasks
    {
        public static async Task InitializeTasks()
        {
            var register = new Registry();
            register.Schedule(async () => await ExecuteTimers()).ToRunEvery(1).Days().At(12, 30);
        }

        public static async Task ExecuteTimers()
        {
            try
            {
                await CheckUserInfo();
            }
            catch (Exception ex)
            {
                await Util.ThrowInteractionlessError(ex);
            }
        }

        public static async Task CheckUserInfo()
        {
            var dbCtx = new BadgerContext();
            var users = dbCtx.UserInfo.ToList();

            foreach (var user in users)
            {
                Console.WriteLine($"Check {user.Name}...");
                if (DateTime.Now - user.LastActive >= TimeSpan.FromDays(30))
                {
                    var guild = await Bot.Client.GetGuildAsync(343867882264068098);
                    var discordUser = await guild.GetMemberAsync(user.DiscordID);
                    if (discordUser.Roles.Any(x => x.Id == RoleID.ACTIVE))
                    {
                        var activeRole = guild.GetRole(RoleID.ACTIVE);
                        var inactiveRole = guild.GetRole(RoleID.INACTIVE);
                        await discordUser.RevokeRoleAsync(activeRole);
                        await discordUser.GrantRoleAsync(inactiveRole);
                    }
                }
            }
        }
    }
}
