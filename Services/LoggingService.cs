using Bubblim.Core;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Bubblim
{
    public class LoggingService
    {
        private readonly DiscordSocketClient _discord;
        private readonly CommandService _commands;

        public LoggingService(DiscordSocketClient discord, CommandService commands)
        {
            _discord = discord;
            _commands = commands;

            _discord.Log += OnLogAsync;
            _commands.Log += OnLogAsync;

            _discord.MessageReceived += MessageReceivedAsync;
        }

        public async Task MessageReceivedAsync(SocketMessage rawMsg)
        {
            // Ignore system messages, or messages from other bots
            if (!(rawMsg is SocketUserMessage msg)) return;
            if (msg.Source != MessageSource.User) return;

            PrettyPrint.Log((SocketUserMessage)rawMsg);
        }

        public Task LogAsync(object severity, string source, string message)
        {
            return PrettyPrint.LogAsync(severity, source, message);
        }

        private Task OnLogAsync(LogMessage msg)
            => LogAsync(msg.Severity, msg.Source, msg.Exception?.ToString() ?? msg.Message);
    }
}
