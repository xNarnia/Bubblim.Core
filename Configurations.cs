using Bubblim.Core.Common;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Bubblim.Core
{
    [OnLoadConfiguration("Core")]
    public class Configurations : ConfigurationBase<Configurations>
    {
        [JsonIgnore]
        public override string FileName { get; set; } = "config/config.json";

        [JsonIgnore]
        public DiscordSocketConfig SocketClient;
        [JsonIgnore]
        public CommandServiceConfig Command;

        [JsonProperty]
        public static char CommandPrefix;
        [JsonProperty]
        public static ulong OwnerId;
        [JsonProperty]
        public static EnabledServices EnabledServices;
        public Tokens Tokens;

        public Configurations()
        {
            Command = new CommandServiceConfig
            {
                CaseSensitiveCommands = false,
                DefaultRunMode = RunMode.Async,
                LogLevel = LogSeverity.Verbose
            };
            SocketClient = new DiscordSocketConfig
            {
                LogLevel = LogSeverity.Verbose,
                MessageCacheSize = 200
            };

            CommandPrefix = '.';
            OwnerId = 0;
            EnabledServices = new EnabledServices();
            Tokens = new Tokens();
        }

        public override IConfigurationBase GetOrCreateConfiguration()
        {
            Configurations config = base.GetOrCreateConfiguration() as Configurations;

            if (config.Tokens.DiscordBotToken == "")
            {
                PrettyPrint.Log(LogSeverity.Warning, "Token", "Please enter your discord bot token: ");
                string intoken = Console.ReadLine();

                Tokens.DiscordBotToken = intoken;
                SaveJson();
            }

            return config;
        }
    }
    public class EnabledServices
    {
        public bool Info = true;
        public bool WhisperListener = true;
        public bool Menu = true;
        public bool PartyRecruiter = true;
    }

    public class Tokens
    {
        public string DiscordBotToken = "";
    }
}
