namespace GoogleDrivePlugin
{
    using System;
    using EditorPluginInterfaces;
    using EditorPluginInterfaces.Toolbar;
    using WpfControlsLib.Controls.Toolbar;

    using Model;
    using Controller;
    using View;

    /// <summary>
    /// Plugin for uploading models to and downloading them from Google Drive
    /// </summary>
    class GoogleDrivePluginCore : IPlugin<PluginConfig>
    {
        /// <summary>
        /// Toolbar in the top of the windows
        /// </summary>
        private IToolbar toolbar;

        /// <summary>
        /// Current opened model
        /// </summary>
        private IModel mainModel;

        /// <summary>
        /// Editor console
        /// </summary>
        private IConsole console;

        private GoogleDriveModel pluginModel;

        private GoogleDriveController pluginController;

        private ExportView exportView;

        private ImportView importView;

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
            this.mainModel = config.Model;
            this.console = config.Console;

            // MVC initialization
            this.pluginModel = new GoogleDriveModel();
            this.pluginController = new GoogleDriveController(this.pluginModel);
            this.importView = new ImportView(
                    this.pluginModel, 
                    this.pluginController);
            this.exportView = new ExportView(
                    this.pluginModel,
                    this.pluginController);

            this.PlaceButtonsOnToolbar();

            this.console.SendMessage("Google Drive Plugin Successfully Launched");
        }

        private void PlaceButtonsOnToolbar()
        {
            var uploadCommand = new Command(this.pluginController.RequestExportWindow);
            var downloadCommand = new Command(this.pluginController.RequestImportWindow);

            var uploadButton = new Button(
                uploadCommand,
                "Upload Model To Google Drive",
                "pack://application:,,,/" + "GoogleDrivePlugin;component/ToolbarIcons/CloudUpload.png");
            var downloadButton = new Button(
                downloadCommand,
                "Download Model From Google Drive",
                "pack://application:,,,/" + "GoogleDrivePlugin;component/ToolbarIcons/CloudDownload.png");

            this.toolbar.AddButton(uploadButton);
            this.toolbar.AddButton(downloadButton);
        }
    }
}
