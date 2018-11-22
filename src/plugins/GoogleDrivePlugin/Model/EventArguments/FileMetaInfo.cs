using System;

namespace GoogleDrivePlugin.Model
{
    public class FileMetaInfo
    {
        public FileMetaInfo(string name, string size, bool isDirectory)
        {
            this.Name = name;
            this.Size = size;
            this.IsDirectory = isDirectory;
        }

        public string Name { get; }

        public string Size { get; }

        public bool IsDirectory { get; }
    }
}
