using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace GoogleDrivePlugin.Controls.FileExplorer
{
    /// <summary>
    /// Логика взаимодействия для FileExplorer.xaml
    /// </summary>
    public partial class FileExplorer : UserControl
    {
        public ItemInfo SelectedItem => (ItemInfo)this.ItemList.SelectedItem;

        public event EventHandler<ItemInfo> ItemSelected;

        public event EventHandler<ItemInfo> ItemDeletionRequested;

        public delegate void MoveEventHandler<T>(object sender, T source, T destination);
        public event MoveEventHandler<ItemInfo> ItemMovementRequested;

        public FileExplorer()
        {
            InitializeComponent();

            this.AddItemToList("testmodel.file", "154KB", false);
            this.AddItemToList("dfdfdf.txt", "100TB", true);
            for (int i = 0; i < 10; ++i)
            {
                this.AddItemToList($"la{i}", $"{352 * i}KB", false);
            }

            this.ItemList.PreviewMouseDoubleClick += this.HandleChosenItem;

            this.ItemList.MouseMove += this.InitializeDragDropForItem;
            this.ItemList.Drop += this.EndDragDropOperation;
            this.ItemList.DragEnter += this.HighlightCurrentTarget;
            this.ItemList.DragLeave += this.DeHighlightCurrentTarget;
        }

        public void AddItemToList(string itemName, string itemSize, bool isDirectory)
        {
            this.ItemList.Items.Add(
                new ItemInfo { Name = itemName, Size = itemSize, IsDirectory = isDirectory });   
        }

        public void ClearList()
        {
            this.ItemList.Items.Clear();
        }

        private void HandleChosenItem(object sender, EventArgs args)
        {
            if (this.SelectedItem == null)
            {
                return;
            }

            this.ItemSelected?.Invoke(this, this.SelectedItem);
        }

        private void DeleteItem(object sender, EventArgs args)
        {
            if (this.SelectedItem == null)
            {
                return;
            }

            this.ItemDeletionRequested?.Invoke(this, this.SelectedItem);
        }

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

        private void HighlightCurrentTarget(object sender, DragEventArgs args)
        {
            var target = this.FindClickedItem((DependencyObject)args.OriginalSource);

            if (target != null)
            {
                target.Background = Brushes.AliceBlue;
            }
        }
        private void DeHighlightCurrentTarget(object sender, DragEventArgs args)
        {
            var target = this.FindClickedItem((DependencyObject)args.OriginalSource);

            if (target != null)
            {
                target.Background = Brushes.Transparent;
            }
        }

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
