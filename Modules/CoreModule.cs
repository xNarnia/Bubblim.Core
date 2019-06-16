using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Bubblim.Core.Common;
using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace Bubblim.Core.Modules
{
    public class CoreModule : BubblimModuleBase
    {
        private readonly DiscordSocketClient _discord;
        private readonly CommandService _command;
        private readonly IServiceProvider _services;

        public CoreModule(
             DiscordSocketClient discord,
             CommandService command,
            IServiceProvider services)
        {
            _discord = discord;
            _command = command;
            _services = services;
        }

        [Command("restart")]
        [Summary("Kills the process and starts a new one.")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task RestartBot(string botName)
        {
            botName = botName.ToLower();
            if (botName != Assembly.GetEntryAssembly().GetName().Name.ToLower() 
                && botName != "all") return;

            await Context.Channel.SendMessageAsync("Restarting...");

            if (_discord != null)
            {
                await _discord.StopAsync();
            }

            Process.Start(new ProcessStartInfo
            {
                FileName = "dotnet",
                Arguments = $@"""{Assembly.GetEntryAssembly().Location}"""
            });
            PrettyPrint.WriteLine($"Restarted the bot.{Environment.NewLine}");
            Process.GetCurrentProcess().Kill();
        }

        [Command("kill")]
        [Summary("Kills the process.")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task KillBot(string botName)
        {
            botName = botName.ToLower();
            if (botName != Assembly.GetEntryAssembly().GetName().Name.ToLower()
                && botName != "all") return;

            await Context.Channel.SendMessageAsync("Terminating...");

            if (_discord != null)
            {
                await _discord.StopAsync();
            }

            PrettyPrint.WriteLine($"Killed the bot.{Environment.NewLine}");
            Process.GetCurrentProcess().Kill();
        }

        [Command("commands")]
        [Alias("cmd", "cmds")]
        [Summary("Shows all commands loaded into the command service.")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task ListCommands(string botName = null)
        {
            if (botName == null) return;

            botName = botName.ToLower();
            if (botName != Assembly.GetEntryAssembly().GetName().Name.ToLower()
                && botName != "all") return;

            string output = "";

            foreach (var module in _command.Modules)
            {
                output += $"\n\n**{module.Name}**";
                foreach (var cmd in module.Commands)
                {
                    output += $"\n{Configurations.CommandPrefix}{cmd.Name} {string.Join(" ", cmd.Parameters)}";
                }
            }

            await Context.Channel.SendMessageAsync(output);
        }
    }
}