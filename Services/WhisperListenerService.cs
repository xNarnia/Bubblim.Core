using Bubblim.Core;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Bubblim
{
    public class WhisperListenerService
    {
        private readonly DiscordSocketClient _discord;
        private readonly CommandService _commands;
        private bool _listen;
        private DateTime _timeToSend;
        private string _output;

        public WhisperListenerService(DiscordSocketClient discord, CommandService commands)
        {
            _discord = discord;
            _commands = commands;
            _listen = true;
            _timeToSend = DateTime.Now.AddSeconds(10);
            _output = "";

            _discord.MessageReceived += MessageReceivedAsync;
        }

        public async Task MessageReceivedAsync(SocketMessage rawMsg)
        {
            // Ignore system messages, or messages from other bots
            if (!(rawMsg is SocketUserMessage msg)) return;
            if (msg.Source != MessageSource.User) return;
            if (msg.Channel is IDMChannel && Configurations.OwnerId != 0)
            {
                if (msg.Content == $"{Configurations.CommandPrefix}listen")
                {
                    _listen = !_listen;
                    await msg.Author.SendMessageAsync($"Listening: {_listen}");
                    return;
                }

                _output += $"{msg.Author.Username}#{msg.Author.Discriminator}: {msg.Content}\n";

                TimeSpan duration = DateTime.Now.Subtract(_timeToSend);

                if (duration.TotalSeconds > 0 && _output != "")
                {
                    _timeToSend = DateTime.Now.AddSeconds(10);

                    if (_output.Length > 2000)
                        _output = _output.Substring(0, 1999);

                    await _discord
                        .GetUser(Configurations.OwnerId)
                        .GetOrCreateDMChannelAsync()
                        .Result
                        .SendMessageAsync(_output);

                    _output = "";
                }
            }
        }
    }
}
