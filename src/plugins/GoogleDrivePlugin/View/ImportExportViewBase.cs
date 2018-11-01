namespace GoogleDrivePlugin.View
{
    using System;
    using System.Windows;
    using Model;
    using Controller;

    public abstract class ImportExportViewBase 
    {
        private GoogleDriveModel model;

        public ImportExportViewBase(GoogleDriveModel model)
        {
            model.FileListReceived += (sender, args) => this.HandleReceivedFileList(args);
        }
        
        protected Window ShowWindow(Window window)
        {
            if (window == null || !window.IsLoaded)
            {
                window = this.CreateNewWindowInstance();
                
            }
            else
            {
                window.Focus();
            }

            window.Show();
            return window;
        }

        protected void HideWindow(Window window)
        {
            if (window != null)
            {
                window.Close();
            }
        }

        protected abstract Window CreateNewWindowInstance();

        protected abstract void HandleReceivedFileList(FileListArgs args);
    }
}
