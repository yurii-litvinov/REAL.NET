namespace GoogleDrivePlugin.View
{
    using System;
    using System.Windows;
    using System.Collections.Generic;

    using Model;
    using Controls.FileExplorer;

    public abstract class ImportExportViewBase 
    {
//        private GoogleDriveModel model;

        private Stack<string> parents = new Stack<string>();

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

            if (this.parents.Contains(args.FolderID))
            {
                while (this.parents.Pop() != args.FolderID) ;
            }
            else if (args.FolderID != fileExplorer.CurrentDirectoryID)
            {
                this.parents.Push(fileExplorer.CurrentDirectoryID);
            }

            if (args.FolderID != fileExplorer.CurrentDirectoryID)
            {
                fileExplorer.CurrentDirectoryID = fileExplorer.RequestedDirectoryID;
                fileExplorer.RequestedDirectoryID = null;
            }

            fileExplorer.ClearList();

            if (fileExplorer.CurrentDirectoryID != GoogleDriveModel.RootFolderName)
            {
                // Button to move to upper level
                fileExplorer.AddItemToList(new ItemInfo()
                {
                    ID = this.parents.Peek(),
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

        protected void HandleError(object sender, OperationProgressArgs args)
        {
            if (args.OperationType == OperationType.Error)
            {
                MessageBox.Show(
                    args.Info, "Operation error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        protected abstract Window CreateNewWindowInstance();
    }
}
