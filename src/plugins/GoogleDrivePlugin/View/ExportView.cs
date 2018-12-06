namespace GoogleDrivePlugin.View
{
    using System;
    using System.Windows;

    using Model;
    using Controller;

    public class ExportView : ImportExportViewBase
    {
        private ExportDialog dialogWindow;

        private GoogleDriveModel model;

        private GoogleDriveController controller;

        private string username;

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

        protected override Window CreateNewWindowInstance() 
            => new ExportDialog(this.controller);
    }
}
