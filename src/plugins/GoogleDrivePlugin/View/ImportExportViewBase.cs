namespace GoogleDrivePlugin.View
{
    using System.Windows;
    using System.Collections.Generic;

    using Model;
    using Controls.FileExplorer;

    /// <summary>
    /// Base of import and export views
    /// </summary>
    public abstract class ImportExportViewBase 
    {
        /// <summary>
        /// Chain of parents of current folder
        /// </summary>
        private Stack<string> parents = new Stack<string>();

        /// <summary>
        /// Initializes new instance of ImportExportViewBase
        /// </summary>
        /// <param name="model"></param>
        public ImportExportViewBase(GoogleDriveModel model)
        {}
        
        /// <summary>
        /// Makes given window visible
        /// </summary>
        /// <param name="window">Window to show</param>
        /// <returns></returns>
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

        /// <summary>
        /// Hides give window
        /// </summary>
        /// <param name="window">Window to hide</param>
        protected void HideWindow(Window window)
        {
            if (window != null)
            {
                window.Close();
            }
        }

        /// <summary>
        /// Adds received files to file list
        /// </summary>
        /// <param name="fileExplorer">List to add files to</param>
        /// <param name="args">Files to add</param>
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

        /// <summary>
        /// Shows error message
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="args">Error info</param>
        protected void HandleError(object sender, OperationProgressArgs args)
        {
            if (args.OperationType == OperationType.Error)
            {
                MessageBox.Show(
                    args.Info, "Operation error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Creates new instance of import/export window
        /// </summary>
        /// <returns>New window</returns>
        protected abstract Window CreateNewWindowInstance();
    }
}
