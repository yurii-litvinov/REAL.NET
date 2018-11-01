namespace GoogleDrivePlugin.View
{
    using System;
    using Model;

    public class ImportView : ImportExportViewBase
    {
        private ImportDialog dialogWindow;

        private GoogleDriveModel model;

        private string username;

        public ImportView(GoogleDriveModel model, ImportDialog dialog)
            : base(model, dialog)
        {
            this.model = model;
            this.dialogWindow = dialog;

            model.ShowImportWindow += (sender, args) =>
            {
                username = args.Username;
                this.ShowWindow();
            };
            model.HideImportWindow += (sender, args) => this.HideWindow();
        }

        protected override void HandleReceivedFileList(FileListArgs args)
        {

        }
    }
}
