namespace GoogleDrivePlugin
{
    using System;
    using EditorPluginInterfaces;
    using EditorPluginInterfaces.Toolbar;
    using WpfControlsLib.Controls.Toolbar;

    /// <summary>
    /// Plugin for uploading models to and downloading them from Google Drive
    /// </summary>
    class GoogleDrivePlugin : IPlugin<PluginConfig>
    {
        /// <summary>
        /// Toolbar in the top of the windows
        /// </summary>
        private IToolbar toolbar;

        /// <summary>
        /// Current opened model
        /// </summary>
        private IModel model;

        /// <summary>
        /// Editor console
        /// </summary>
        private IConsole console;

        /// <summary>
        /// Gets name of plugin
        /// </summary>
        public string Name => "Google Drive Integration Plugin";

        /// <summary>
        /// Sets plugin's configuration
        /// </summary>
        /// <param name="config">Configuration to set</param>
        public void SetConfig(PluginConfig config)
        {
            if (config == null)
            {
                throw new ArgumentException("This is not correct type of configuration");
            }

            this.toolbar = config.Toolbar;
            this.model = config.Model;
            this.console = config.Console;

            this.PlaceButtonsOnToolbar();

            this.console.SendMessage("Google Drive Plugin Successfully Launched");
        }

        private void PlaceButtonsOnToolbar()
        {
            var uploadButton = new Button(
                null,
                "Upload Model To Google Drive",
                "pack://application:,,,/" + "GoogleDrivePlugin;component/ToolbarIcons/CloudUpload.png");
            var downloadButton = new Button(
                null,
                "Download Model From Google Drive",
                "pack://application:,,,/" + "GoogleDrivePlugin;component/ToolbarIcons/CloudDownload.png");

            this.toolbar.AddButton(uploadButton);
            this.toolbar.AddButton(downloadButton);
        }
    }
}
