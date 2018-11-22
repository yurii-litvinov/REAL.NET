namespace GoogleDrivePlugin.View
{
    using System;
    using System.Windows;
    using Model;
    using Controller;
    using Controls.FileExplorer;

    public abstract class ImportExportViewBase 
    {
        private GoogleDriveModel model;

        public ImportExportViewBase(GoogleDriveModel model)
        {
            //model.FileListReceived += (sender, args) => this.HandleReceivedFileList(args);
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

        protected virtual void HandleReceivedFileList(FileExplorer fileExplorer,  FileListArgs args)
        {
            if (fileExplorer == null ||
                args.FolderPath != fileExplorer.RequestedPath)
            {
                return;
            }

            foreach (var item in args.FileList)
            {
                fileExplorer.ClearList();
                fileExplorer.AddItemToList(item.Name, item.Size, item.IsDirectory);
            }

            fileExplorer.CurrentPath = fileExplorer.RequestedPath;
            fileExplorer.RequestedPath = null;
        }

        protected abstract Window CreateNewWindowInstance();

        //protected abstract void HandleReceivedFileList(FileListArgs args);

        /*protected void HandleReceivedFileList(FileListArgs args)
        {

        }*/
    }
}
