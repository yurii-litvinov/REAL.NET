namespace GoogleDrivePlugin.View
{
    using System;
    using System.Windows;
    using Model;

    public abstract class ImportExportViewBase 
    {
        private Window dialogWindow;

        private GoogleDriveModel model;

        public ImportExportViewBase(GoogleDriveModel model, Window dialog)
        {
            this.dialogWindow = dialog;
            this.dialogWindow.Visibility = Visibility.Hidden;

            model.FileListReceived += (sender, args) => this.HandleReceivedFileList(args);
        }
        
        protected void ShowWindow()
        {
            if (this.dialogWindow.Visibility == Visibility.Hidden)
            {
                this.dialogWindow.Show();
            }
            else
            {
                this.dialogWindow.Focus();
            }
        }

        protected void HideWindow()
        {
            this.dialogWindow.Hide();
        }

        protected abstract void HandleReceivedFileList(FileListArgs args);
    }
}
