namespace GoogleDrivePlugin.View
{
    using System.Windows;

    using Model;
    using Controller;

    /// <summary>
    /// View (MVC) of export window
    /// </summary>
    public class ExportView : ImportExportViewBase
    {
        /// <summary>
        /// Connected export dialog
        /// </summary>
        private ExportDialog dialogWindow;

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
        /// Initializes new instance of ExportView
        /// </summary>
        /// <param name="model">Plugin model</param>
        /// <param name="controller">Plugin controller</param>
        public ExportView(GoogleDriveModel model, GoogleDriveController controller)
            : base(model)
        {
            this.model = model;
            this.controller = controller;

            this.model.ExportWindowStatusChanged += (sender, args) =>
            {
                if (args.OperationType == OperationType.OpenWindow)
                {
                    username = args.Info;
                    this.dialogWindow = (ExportDialog)this.ShowWindow(this.dialogWindow);
                    this.dialogWindow.LogoutBox.UsernameLabel.Content = username;
                }
            };

            this.model.ExportWindowStatusChanged += (sender, args) =>
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

            this.model.ExportWindowStatusChanged += this.HandleError;
        }

        /// <summary>
        /// Created new instance of export dialog
        /// </summary>
        /// <returns>New export dialog</returns>
        protected override Window CreateNewWindowInstance() 
            => new ExportDialog(this.controller);
    }
}
