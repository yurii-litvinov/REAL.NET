namespace GoogleDrivePlugin.View
{
    using System;
    using Model;

    public class ExportView : ImportExportViewBase
    {
        private ExportDialog dialogWindow;

        private GoogleDriveModel model;

        private string username;

        public ExportView(GoogleDriveModel model, ExportDialog dialog)
            : base(model, dialog)
        {
            this.model = model;
            this.dialogWindow = dialog;

            model.ShowExportWindow += (sender, args) =>
            {
                username = args.Username;
                this.ShowWindow();
            };
            model.HideExportWindow += (sender, args) => this.HideWindow();

            model.FileListReceived += (sender, args) => this.HandleReceivedFileList(args);
        }

        protected override void HandleReceivedFileList(FileListArgs args)
        {

        }
    }
}
