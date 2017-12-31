namespace PluginLibrary.MainInterfaces
{
    /// <summary>
    /// Plugins' configuration class
    /// </summary>
    public class PluginConfig
    {
        /// <summary>
        /// Scene's reference that should be given to plugin
        /// </summary>
        public IScene Scene { get; private set; }

        /// <summary>
        /// Toolbar's reference that should be given to plugin
        /// </summary>
        public IToolbar Toolbar { get; private set; }

        /// <summary>
        /// Console's reference that should be given to plugin
        /// </summary>
        public IConsole Console { get; private set; }

        /// <summary>
        /// Palette's reference that should be given to plugin
        /// </summary>
        public IPalette Palette { get; private set; }

        /// <summary>
        /// Initializes a new instance of <see cref="PluginConfig"/> 
        /// </summary>
        /// <param name="scene">Scene</param>
        /// <param name="toolbar">Toolbar</param>
        /// <param name="console">Console</param>
        /// <param name="palette">Palette</param>
        public PluginConfig(IScene scene, IToolbar toolbar, IConsole console, IPalette palette)
        {
            this.Scene = scene;
            this.Toolbar = toolbar;
            this.Console = console;
            this.Palette = palette;
        }
    }
}
