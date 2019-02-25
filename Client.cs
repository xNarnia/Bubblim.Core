using System;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Discord.Commands;
using Bubblim.Core.Services;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using Bubblim.Core.Common;
using System.Linq;

namespace Bubblim.Core
{
    public class Client
    {
        public async Task<IDiscordClient> Login(
            Action<IServiceCollection> insertServices = null,
            Action<IServiceProvider> insertProvider = null)
        {
            var config = Configurations.LoadConfigFile();

            var srv = new ServiceManager(config.SocketClient, config.Command, insertServices, insertProvider);

            var client = srv.provider.GetRequiredService<DiscordSocketClient>();
            var commands = srv.provider.GetRequiredService<CommandService>();

            string token = config.Tokens.DiscordBotToken; // Remember to keep this private!
            await client.LoginAsync(TokenType.Bot, token);
            await client.StartAsync();

            await commands.AddModulesAsync(Assembly.GetExecutingAssembly(), srv.provider);
            await commands.AddModulesAsync(Assembly.GetEntryAssembly(), srv.provider);

            return client;
        }

        //private Task Log(LogMessage msg)
        //{
        //    Console.WriteLine(msg.ToString());
        //    return Task.CompletedTask;
        //}
    }
}
