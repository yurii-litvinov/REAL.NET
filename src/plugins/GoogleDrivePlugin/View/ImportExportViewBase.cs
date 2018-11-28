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

        private string parentID = null;

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
            
            //window.Topmost = true;
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
                args.FolderID != fileExplorer.RequestedDirectoryID &&
                args.FolderID != fileExplorer.CurrentDirectoryID)
            {
                return;
            }

            if (args.FolderID != fileExplorer.CurrentDirectoryID)
            {
                this.parentID = fileExplorer.CurrentDirectoryID;
                fileExplorer.CurrentDirectoryID = fileExplorer.RequestedDirectoryID;
                fileExplorer.RequestedDirectoryID = null;
            }

            fileExplorer.ClearList();

            if (fileExplorer.CurrentDirectoryID != null)
            {
                // Button to move to upper level
                fileExplorer.AddItemToList(new ItemInfo()
                {
                    ID = this.parentID,
                    Name = "...",
                    IsDirectory = true
                });
            }

            foreach (var item in args.FileList)
            {
                fileExplorer.AddItemToList(new ItemInfo()
                {
                    ID = item.ID,
                    Name = item.Name,
                    Size = item.Size,
                    IsDirectory = item.IsDirectory
                });
            }
        }

        protected abstract Window CreateNewWindowInstance();
    }
}
