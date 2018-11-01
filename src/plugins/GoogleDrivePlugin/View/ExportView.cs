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

            model.ShowExportWindow += (sender, args) =>
            {
                username = args.Username;
                this.dialogWindow = (ExportDialog)this.ShowWindow(this.dialogWindow);
            };
            model.HideExportWindow += (sender, args) => this.HideWindow(this.dialogWindow);

            model.FileListReceived += (sender, args) => this.HandleReceivedFileList(args);
        }

        protected override Window CreateNewWindowInstance() 
            => new ExportDialog(this.controller);

        protected override void HandleReceivedFileList(FileListArgs args)
        {

        }
    }
}
