using Bubblim.Core.Common;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bubblim.Core.Services
{
    public class ServiceManager
    {
        public IServiceCollection services;
        public IServiceProvider provider;

        public ServiceManager(
            DiscordSocketConfig SocketClient,
            CommandServiceConfig Command,
            Action<IServiceCollection> insertServices = null,
            Action<IServiceProvider> insertProvider = null)
        {
            var ConfigurationService = new ConfigurationsService();

            services = new ServiceCollection()
                .AddSingleton(new DiscordSocketClient(SocketClient))
                .AddSingleton(new CommandService(Command))
                .AddSingleton<CommandManager>()
                .AddSingleton<LoggingService>()
                .AddSingleton(ConfigurationService);

            // Optional Services
            if (Configurations.EnabledServices.Info)
                services.AddSingleton<InfoService>();

            if (Configurations.EnabledServices.WhisperListener)
                services.AddSingleton<WhisperListenerService>();

            if (Configurations.EnabledServices.Menu)
                services.AddSingleton<MenuService>();

            if (Configurations.EnabledServices.PartyRecruiter)
                services.AddSingleton<PartyRecruiterService>();

            // Add user provided services
            insertServices?.Invoke(services);

            provider = services.BuildServiceProvider();
            provider.GetRequiredService<DiscordSocketClient>();
            provider.GetRequiredService<CommandService>();
            provider.GetRequiredService<CommandManager>();
            provider.GetRequiredService<LoggingService>();
            provider.GetRequiredService<ConfigurationsService>();

            //Optional Services
            if (Configurations.EnabledServices.Info)
                provider.GetRequiredService<InfoService>();

            if (Configurations.EnabledServices.WhisperListener)
                provider.GetRequiredService<WhisperListenerService>();

            if (Configurations.EnabledServices.Menu)
                provider.GetRequiredService<MenuService>();

            if (Configurations.EnabledServices.PartyRecruiter)
                provider.GetRequiredService<PartyRecruiterService>();

            // Add user provided required services
            insertProvider?.Invoke(provider);
        }
    }
}
