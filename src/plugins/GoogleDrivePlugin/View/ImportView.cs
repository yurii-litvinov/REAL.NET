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
            };
            model.HideImportWindow += (sender, args) => this.HideWindow(this.dialogWindow);
        }

        protected override Window CreateNewWindowInstance() => 
            new ImportDialog(this.controller);

        protected override void HandleReceivedFileList(FileListArgs args)
        {

        }
    }
}
