using Bubblim.Core.Common;
using Bubblim.Core.Services;
using Discord;
using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Bubblim.Core.Modules
{
    public class InfoModule : BubblimModuleBase
    {
        private InfoService _info { get; set; }

        /// <summary>
        /// Command module for InfoService.
        /// </summary>
        /// <param name="info"></param>
        public InfoModule(
            InfoService info)
        {
            _info = info;
        }

        [Command("admininfo")]
        [Summary("Provides a list of info for the specified bot.")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task AdminInfo(string botName)
        {
            botName = botName.ToLower();
            if (botName != Assembly.GetEntryAssembly().GetName().Name.ToLower()) return;

            string prefix = $"**{Configurations.CommandPrefix}admininfo {Assembly.GetEntryAssembly().GetName().Name.ToLower()} ";
            string suffix = $"**";

            await Context.Channel.SendMessageAsync(
                _info.GetList(prefix, suffix));
            PrettyPrint.WriteLine($"Listing admin info.");
        }

        [Command("admininfo")]
        [Summary("Provides contents of selected info for the specified bot.")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task AdminInfo(string botName, [Remainder]string selectedInfo)
        {
            botName = botName.ToLower();
            selectedInfo = selectedInfo.ToLower();
            if (botName != Assembly.GetEntryAssembly().GetName().Name.ToLower()) return;

            string prefix = $"**{Configurations.CommandPrefix}admininfo {Assembly.GetEntryAssembly().GetName().Name.ToLower()} ";
            string suffix = $"**";

            try
            {
                string output = _info.GetInfo(selectedInfo).Content;

                await Context.Channel.SendMessageAsync(
                _info.GetInfo(selectedInfo).Content ?? "Nothing to display.");

                PrettyPrint.WriteLine($"Listing Info: {selectedInfo}");
            }
            catch(KeyNotFoundException e)
            {
                PrettyPrint.WriteLine(e.Message);
                await Context.Channel.SendMessageAsync("Invalid selection.");
            }
        }
    }
}
