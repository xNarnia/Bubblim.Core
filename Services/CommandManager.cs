using System;
using System.Reflection;
using System.Threading.Tasks;
using System.Linq;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Bubblim.Core.Common;

namespace Bubblim.Core.Services
{
    public class CommandManager
    {
        private readonly CommandService _commands;
        private readonly DiscordSocketClient _discord;
        private readonly IServiceProvider _services;

        public CommandManager(
            DiscordSocketClient discord,
            CommandService commands,
            IServiceProvider services)
        {
            _discord = discord;
            _commands = commands;
            _services = services;

            _discord.MessageReceived += MessageReceivedAsync;
        }

        public async Task MessageReceivedAsync(SocketMessage rawMessage)
        {
            // Ignore system messages, or messages from other bots
            if (!(rawMessage is SocketUserMessage message)) return;
            if (message.Source != MessageSource.User) return;

            // This value holds the offset where the prefix ends
            var argPos = 0;
            if (!message.HasCharPrefix(Configurations.CommandPrefix, ref argPos)) return;

            var context = new CommandContext(_discord, message);
            var result = await _commands.ExecuteAsync(context, argPos, _services);

            if (result.Error.HasValue &&
                result.Error.Value != CommandError.UnknownCommand) // it's bad practice to send 'unknown command' errors
            {
                await context.Channel.SendMessageAsync(result.ErrorReason);
                PrettyPrint.WriteLine(result.ToString());
            }
        }

        public async Task ExecuteAsync(CommandContext context, IServiceProvider provider, string input)
        {
            var result = await _commands.ExecuteAsync(context, input, provider);
            await ResultAsync(context, result);
        }

        private async Task ResultAsync(CommandContext context, IResult result)
        {
            if (result.IsSuccess)
                return;

            switch (result)
            {
                case ExecuteResult exr:
                    await context.Channel.SendMessageAsync(exr.Exception?.ToString() ?? exr.ErrorReason);
                    break;
                default:
                    await context.Channel.SendMessageAsync(result.ErrorReason);
                    break;
            }
        }
    }
}