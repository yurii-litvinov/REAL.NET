namespace GoogleDrivePlugin.Controls.FileExplorer
{
    /// <summary>
    /// Info about item in FileExplorer list
    /// </summary>
    public class ItemInfo
    {
        /// <summary>
        /// Item ID
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// Item name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Item size
        /// </summary>
        public string Size { get; set; }

        /// <summary>
        /// Is item a directory
        /// </summary>
        public bool IsDirectory { get; set; }
    }
}
