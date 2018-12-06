using System;

namespace GoogleDrivePlugin.Model
{
    /// <summary>
    /// Contains information about some item on Drive
    /// </summary>
    public class ItemMetaInfo
    {
        /// <summary>
        /// Initializes new instance of ItemMetaInfo
        /// </summary>
        /// <param name="id">Item ID</param>
        /// <param name="name">Item name</param>
        /// <param name="size">Item size (in bytes)</param>
        /// <param name="isDirectory">Is item a directory</param>
        public ItemMetaInfo(string id, string name, string size, bool isDirectory)
        {
            this.ID = id;
            this.Name = name;
            this.Size = size;
            this.IsDirectory = isDirectory;
        }

        /// <summary>
        /// Item ID on Drive
        /// </summary>
        public string ID { get; }

        /// <summary>
        /// Item name on Drive
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Item size on Drive
        /// </summary>
        public string Size { get; }

        /// <summary>
        /// True if item is directory on Drive
        /// </summary>
        public bool IsDirectory { get; }
    }
}
