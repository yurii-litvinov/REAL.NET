using EditorPrototype.Models.InternalConsole;
using EditorPrototype.Models.PluginConfig;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PluginLibrary
{
    public class SamplePlugin : IPlugin
    {
        public string Name => "samplePlugin";

        public SamplePlugin()
        { }

        private IConsole console;

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
