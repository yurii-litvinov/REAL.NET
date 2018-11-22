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
            
            window.Topmost = true;
            window.Show();
            window.Focus();

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
                args.FolderID != fileExplorer.RequestedDirectoryID)
            {
                return;
            }

            fileExplorer.ClearList();
            foreach (var item in args.FileList)
            {
                fileExplorer.AddItemToList(item.Name, item.Size, item.IsDirectory);
            }

            fileExplorer.CurrentDirectoryID = fileExplorer.RequestedDirectoryID;
            fileExplorer.RequestedDirectoryID = null;
        }

        protected abstract Window CreateNewWindowInstance();
    }
}
