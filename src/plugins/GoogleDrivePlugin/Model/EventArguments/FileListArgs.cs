using System;
using System.Collections.Generic;

namespace GoogleDrivePlugin.Model
{
    /// <summary>
    /// Contains file list of some folder on Drive
    /// </summary>
    public class FileListArgs : EventArgs
    {
        /// <summary>
        /// Initializes new instance of FileListArgs
        /// </summary>
        /// <param name="parentID">Path to data in the Drive</param>
        /// <param name="itemInfo">Pair [itemName, isDirectory flag] </param>
        public FileListArgs(string parentID, List<ItemMetaInfo> itemInfo)
        {
            this.FolderID = parentID;
            this.FileList = itemInfo;
        }

        /// <summary>
        /// Google Drive ID of folder
        /// </summary>
        public string FolderID { get; }

        /// <summary>
        /// List of files in folder with FolderID
        /// </summary>
        public List<ItemMetaInfo> FileList { get; }
    }
}
