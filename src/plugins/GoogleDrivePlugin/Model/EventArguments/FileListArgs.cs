using System;
using System.Collections.Generic;

namespace GoogleDrivePlugin.Model
{
    public class FileListArgs : EventArgs
    {
        /// <summary>
        /// Initializes new instance of FileListArgs
        /// </summary>
        /// <param name="parentID">Path to data in the Drive</param>
        /// <param name="itemInfo">Pair [itemName, isDirectory flag] </param>
        public FileListArgs(string parentID, List<FileMetaInfo> itemInfo)
        {
            this.FolderID = parentID;
            this.FileList = itemInfo;
        }

        public string FolderID { get; }

        public List<FileMetaInfo> FileList { get; }
    }
}
