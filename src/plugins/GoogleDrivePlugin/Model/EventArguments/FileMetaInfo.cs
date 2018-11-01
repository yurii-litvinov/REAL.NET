using System;

namespace GoogleDrivePlugin.Model
{
    public class FileMetaInfo
    {
        public FileMetaInfo(string name, bool isDirectory)
        {
            this.Name = name;
            this.IsDirectory = isDirectory;
        }

        public string Name { get; }

        public bool IsDirectory { get; }
    }
}
