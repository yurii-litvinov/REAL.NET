using System;
using PluginLibrary.MainInterfaces;

namespace PluginLibrary
{
    /// <summary>
    /// This class is a sample plugin and also helps to check correct plugins' loading  
    /// </summary>
    public class SamplePlugin : IPlugin
    {
        /// <summary>
        /// Name of plugin
        /// </summary>
        public string Name => "samplePlugin";

        /// <summary>
        /// Field that contains link to editor's console
        /// </summary>
        private IConsole console;

        /// <summary>
        /// Sets configuration by getting configuration
        /// </summary>
        /// <param name="config">Configuration to get</param>
        public void SetConfig(object config)
        {
            var configuration = config as PluginConfig;
            if (configuration == null)
            {
                throw new ArgumentException("This is not correct type of configuration");
            }
            this.console = configuration.Console;
            this.console.SendMessage("Sample add-on successfully launched");
        }
    }
}
