using Bubblim.Core.Common;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System.Text;
using Discord;
using Discord.Commands;
using System.Threading.Tasks;

namespace Bubblim.Core.Services
{
    public class ConfigurationsService
    {
        public List<StoredConfig> configs;
        public ConfigurationsService()
        {
            configs = new List<StoredConfig>();

            foreach(Assembly asm in AppDomain.CurrentDomain.GetAssemblies())
            {
                var OnLoadConfigAssemblies = asm.GetTypes()
                    .Where(type => !type.IsInterface && !type.IsAbstract)
                    .Where(type => typeof(IConfigurationBase).IsAssignableFrom(type))
                    .Where(type => type.GetCustomAttribute<OnLoadConfiguration>() != null);

                if(OnLoadConfigAssemblies.Count() > 0)
                {
                    foreach(Type type in OnLoadConfigAssemblies)
                    {
                        // Get the constructor and create an instance of Config
                        ConstructorInfo constructor = type.GetConstructor(Type.EmptyTypes);
                        IConfigurationBase config = constructor.Invoke(new object[] { }) as IConfigurationBase;

                        configs.Add(new StoredConfig
                        {
                            Namespace = type.Namespace,
                            Name = type.Name,
                            Config = config
                        });
                    }
                }
            }

            LoadConfigs();
        }

        public void LoadConfigs()
        {
            foreach (StoredConfig data in configs)
            {
                data.Config.GetOrCreateConfiguration();
                PrettyPrint.Log(LogSeverity.Info, "ConfigurationService", $"{data.Namespace} {data.Name} - Config loaded.");
            }
        }

        public bool LoadConfig(string name)
        {
            var dataSet = configs.Where(data => data.Name == name);
            if(dataSet.Count() == 1)
            {
                var data = dataSet.First();
                data.Config.GetOrCreateConfiguration();
                PrettyPrint.Log(LogSeverity.Info, "ConfigurationService", $"{data.Namespace} {data.Name} - Config loaded.");
                return true;
            }
            else if(dataSet.Count() > 1)
            {
                PrettyPrint.Log(LogSeverity.Error, "ConfigurationService", $"More than one config found. No config loaded.");
                return false;
            }
            else
            {
                PrettyPrint.Log(LogSeverity.Error, "ConfigurationService", $"No config found.");
                return false;
            }
        }

        public class StoredConfig
        {
            public string Namespace;
            public string Name;
            public IConfigurationBase Config;
        }
    }
}