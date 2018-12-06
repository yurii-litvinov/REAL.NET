using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace GoogleDrivePlugin.Controls.FileExplorer
{
    /// <summary>
    /// Interaction logic for FileExplorer.xaml
    /// </summary>
    public partial class FileExplorer : UserControl
    {
        /// <summary>
        /// The item that is currently selected
        /// </summary>
        public ItemInfo SelectedItem => (ItemInfo)this.ItemList.SelectedItem;

        /// <summary>
        /// User changed his selection of item
        /// </summary>
        public event EventHandler<ItemInfo> ItemSelected;

        /// <summary>
        /// User requested deletion of selected item
        /// </summary>
        public event EventHandler<ItemInfo> ItemDeletionRequested;

        /// <summary>
        /// Handles events connected with item movement
        /// </summary>
        /// <typeparam name="T">Type of item info</typeparam>
        /// <param name="sender">Event sender</param>
        /// <param name="source">Info about source item</param>
        /// <param name="destination">Info about destination item</param>
        public delegate void MoveEventHandler<T>(object sender, T source, T destination);

        /// <summary>
        /// User requested movement of one item to another
        /// </summary>
        public event MoveEventHandler<ItemInfo> ItemMovementRequested;

        /// <summary>
        /// ID of directory in which user currently is
        /// </summary>
        public string CurrentDirectoryID { get; set; } = Model.GoogleDriveModel.RootFolderName;

        /// <summary>
        /// ID of directory which was requested by user in the last request
        /// </summary>
        public string RequestedDirectoryID { get; set; }

        /// <summary>
        /// Initializes new instance of FileExplorer
        /// </summary>
        public FileExplorer()
        {
            InitializeComponent();

            this.ItemList.PreviewMouseDoubleClick += this.HandleChosenItem;

            this.ItemList.MouseMove += this.InitializeDragDropForItem;
            this.ItemList.Drop += this.EndDragDropOperation;
            this.ItemList.DragEnter += this.HighlightCurrentTarget;
            this.ItemList.DragLeave += this.DeHighlightCurrentTarget;
        }

        /// <summary>
        /// Adds new item to item list
        /// </summary>
        /// <param name="newItem">New item info</param>
        public void AddItemToList(ItemInfo newItem)
        {
            this.ItemList.Items.Add(newItem);
        }

        /// <summary>
        /// Clears file list
        /// </summary>
        public void ClearList()
        {
            this.ItemList.Items.Clear();
        }

        /// <summary>
        /// Handles item which was selected by user
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="args">Some unused info</param>
        private void HandleChosenItem(object sender, EventArgs args)
        {
            if (this.SelectedItem == null)
            {
                return;
            }

            if (this.SelectedItem.IsDirectory)
            {
                this.RequestedDirectoryID = this.SelectedItem.ID;
            }

            this.ItemSelected?.Invoke(this, this.SelectedItem);
        }

        /// <summary>
        /// Handles deletion request
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="args">Some unused info</param>
        private void DeleteItem(object sender, EventArgs args)
        {
            if (this.SelectedItem == null)
            {
                return;
            }

            this.ItemDeletionRequested?.Invoke(this, this.SelectedItem);
        }

        /// <summary>
        /// Initialized drag'n'drop process for selected item
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="args">Info about selection</param>
        private void InitializeDragDropForItem(object sender, MouseEventArgs args)
        {
            var item = this.FindClickedItem((DependencyObject)args.OriginalSource);

            if (item != null && args.LeftButton == MouseButtonState.Pressed)
            {
                DragDrop.DoDragDrop(
                    item,
                    this.ItemList.ItemContainerGenerator.ItemFromContainer(item),
                    DragDropEffects.Copy | DragDropEffects.Move);
            }
        }

        /// <summary>
        /// Finishes drag'n'drop process
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="args">Info about movement target</param>
        private void EndDragDropOperation(object sender, DragEventArgs args)
        {
            var destItem = this.FindClickedItem((DependencyObject)args.OriginalSource);

            if (destItem != null && args.Data.GetDataPresent(typeof(ItemInfo)))
            {
                var srcItemInfo = (ItemInfo)args.Data.GetData(typeof(ItemInfo));
                var destItemInfo =
                    (ItemInfo)this.ItemList.ItemContainerGenerator.ItemFromContainer(destItem);
                this.ItemMovementRequested?.Invoke(this, srcItemInfo, destItemInfo);
            }

            this.DeHighlightCurrentTarget(sender, args);
        }

        /// <summary>
        /// Highlights the item the user is aiming at
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="args">Info about target</param>
        private void HighlightCurrentTarget(object sender, DragEventArgs args)
        {
            var target = this.FindClickedItem((DependencyObject)args.OriginalSource);

            if (target != null)
            {
                target.Background = Brushes.AliceBlue;
            }
        }

        /// <summary>
        /// Dehighlights the item the user is aiming at
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="args">Info about target</param>
        private void DeHighlightCurrentTarget(object sender, DragEventArgs args)
        {
            var target = this.FindClickedItem((DependencyObject)args.OriginalSource);

            if (target != null)
            {
                target.Background = Brushes.Transparent;
            }
        }

        /// <summary>
        /// Find selected item in visual tree
        /// </summary>
        /// <param name="current">Item to find</param>
        /// <returns></returns>
        private ListViewItem FindClickedItem(DependencyObject current) 
        {
            if (current == null)
            {
                return null;
            }

            do
            {
                if (current is ListViewItem)
                {
                    return (ListViewItem)current;
                }
                current = VisualTreeHelper.GetParent(current);
            }
            while (current != null);

            return null;
        }
    }
}
