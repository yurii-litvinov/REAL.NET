using System;

namespace GoogleDrivePlugin.Model
{
    public class FileMetaInfo
    {
        public FileMetaInfo(string id, string name, string size, bool isDirectory)
        {
            this.ID = id;
            this.Name = name;
            this.Size = size;
            this.IsDirectory = isDirectory;
        }

        public string ID { get; }

        public string Name { get; }

        public string Size { get; }

        public bool IsDirectory { get; }
    }
}
