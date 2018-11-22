namespace GoogleDrivePlugin.View
{
    using System;
    using Model;
    using Controller;
    using System.Windows;

    public class ImportView : ImportExportViewBase
    {
        private ImportDialog dialogWindow;

        private GoogleDriveModel model;

        private GoogleDriveController controller;

        private string username;

        public ImportView(GoogleDriveModel model, GoogleDriveController controller)
            : base(model)
        {
            this.model = model;
            this.controller = controller;

            model.ShowImportWindow += (sender, args) =>
            {
                username = args.Username;
                this.dialogWindow = (ImportDialog)this.ShowWindow(this.dialogWindow);
                this.dialogWindow.LogoutBox.UsernameLabel.Content = username;
            };
            model.HideImportWindow += (sender, args) => this.HideWindow(this.dialogWindow);

            model.FileListReceived += (sender, args) =>
            {
                if (this.dialogWindow != null)
                {
                    this.HandleReceivedFileList(this.dialogWindow.FileExplorer, args);
                }
            };
                    
        }

        protected override Window CreateNewWindowInstance() => 
            new ImportDialog(this.controller);
    }
}
