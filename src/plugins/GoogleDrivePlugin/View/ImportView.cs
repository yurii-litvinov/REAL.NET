namespace GoogleDrivePlugin.View
{
    using Model;
    using Controller;

    using System.Windows;

    /// <summary>
    /// View (MVC) of import dialog
    /// </summary>
    public class ImportView : ImportExportViewBase
    {
        /// <summary>
        /// Connected dialog
        /// </summary>
        private ImportDialog dialogWindow;

        /// <summary>
        /// Plugin model
        /// </summary>
        private GoogleDriveModel model;

        /// <summary>
        /// Plugin controller
        /// </summary>
        private GoogleDriveController controller;

        /// <summary>
        /// User's nickname
        /// </summary>
        private string username;

        /// <summary>
        /// Initializes new instance of ImportView
        /// </summary>
        /// <param name="model">Plugin model</param>
        /// <param name="controller">Plugin controller</param>
        public ImportView(GoogleDriveModel model, GoogleDriveController controller)
            : base(model)
        {
            this.model = model;
            this.controller = controller;

            this.model.ImportWindowStatusChanged += (sender, args) =>
            {
                if (args.OperationType == OperationType.OpenWindow)
                {
                    username = args.Info;
                    this.dialogWindow = (ImportDialog)this.ShowWindow(this.dialogWindow);
                    this.dialogWindow.LogoutBox.UsernameLabel.Content = username;
                }
            };

            this.model.ImportWindowStatusChanged += (sender, args) =>
            {
                if (args.OperationType == OperationType.CloseWindow)
                {
                    this.HideWindow(this.dialogWindow);
                }
            };

            this.model.FileListReceived += (sender, args) =>
            {
                if (this.dialogWindow != null)
                {
                    this.HandleReceivedFileList(this.dialogWindow.FileExplorer, args);
                }
            };

            this.model.ImportWindowStatusChanged += this.HandleError;
        }

        /// <summary>
        /// Creates new import dialog
        /// </summary>
        /// <returns>New dialog</returns>
        protected override Window CreateNewWindowInstance() => 
            new ImportDialog(this.controller);
    }
}
