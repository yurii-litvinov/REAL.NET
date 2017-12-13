using EditorPrototype.Models.InternalConsole;

namespace EditorPrototype.Models.PluginConfig
{
    public class PluginConfig
    {
        public IConsole Console { get; private set; }

        public PluginConfig(IConsole console) => this.Console = console;
    }
}
