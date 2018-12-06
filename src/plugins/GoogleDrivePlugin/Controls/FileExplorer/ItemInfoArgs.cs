namespace GoogleDrivePlugin.Controls.FileExplorer
{
    using System;

    public class ItemInfoArgs : EventArgs
    {
        public ItemInfoArgs(string itemName, string itemSize, bool isDirectory)
        {
            this.Name = itemName;
            this.Size = itemSize;
            this.IsDirectory = isDirectory;
        }

        public string Name { get; }

        public string Size { get; }

        public bool IsDirectory { get; }
    }
}
