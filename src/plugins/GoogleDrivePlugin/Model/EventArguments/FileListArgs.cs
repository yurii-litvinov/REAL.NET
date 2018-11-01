using System;
using System.Collections.Generic;

namespace GoogleDrivePlugin.Model
{
    public class FileListArgs : EventArgs
    {
        /// <summary>
        /// Initializes new instance of FileListArgs
        /// </summary>
        /// <param name="path">Path to data in the Drive</param>
        /// <param name="itemInfo">Pair [itemName, isDirectory flag] </param>
        public FileListArgs(string path, List<UserInfoArgs> itemInfo)
        {
            this.FolderPath = path;
            this.FileList = itemInfo;
        }

        public string FolderPath { get; }

        public List<UserInfoArgs> FileList { get; }
    }
}
