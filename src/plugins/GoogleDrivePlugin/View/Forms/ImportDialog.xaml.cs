namespace GoogleDrivePlugin.View
{
    using System.Windows;

    /// <summary>
    /// Interaction login for ImportDialog.xaml
    /// </summary>
    public partial class ImportDialog : Window
    {
        /// <summary>
        /// Initializes new instance of ImportDialog
        /// </summary>
        /// <param name="controller">Plugin controller</param>
        public ImportDialog(Controller.GoogleDriveController controller)
        {
            InitializeComponent();

            this.CancelButton.Click += (sender, args) =>
                controller.RequestImportWindowHide();

            this.OpenButton.Click += async (sender, args) =>
            {
                if (this.FileExplorer.SelectedItem != null)
                {
                    await controller.RequestModelImport(
                        this.FileExplorer.SelectedItem.ID,
                        this.FileExplorer.SelectedItem.IsDirectory);
                }
            };

            this.LogoutBox.LogoutButton.Click += async (sender, args) =>
                await controller.RequestLoggingOut();

            this.FileExplorer.ItemSelected += async (sender, fileInfo) =>
                await controller.RequestModelImport(
                    fileInfo.ID, 
                    fileInfo.IsDirectory);

            this.FileExplorer.ItemDeletionRequested += (sender, itemInfo) =>
            {
                var confirmation = System.Windows.MessageBox.Show(
                    this,
                    $"Are you sure to delete {itemInfo.Name}?", "Item deletion",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (confirmation == MessageBoxResult.Yes)
                {
                    controller.RequestFileDeletion(
                        this.FileExplorer.CurrentDirectoryID, itemInfo.ID);
                }
            };

            this.FileExplorer.ItemMovementRequested += (sender, sourceInfo, destInfo) =>
                controller.RequestFileMovement(this.FileExplorer.CurrentDirectoryID, sourceInfo.ID, destInfo.ID);

            this.Loaded += (sender, args) => 
                controller.RequestDirectoryContent(Model.GoogleDriveModel.RootFolderName);
        }

        /// <summary>
        /// Gets new item name from user
        /// </summary>
        /// <param name="itemType">Type of item (folder, file, etc)</param>
        /// <returns>Item name</returns>
        protected string GetNewItemName(string itemType)
        {
            // It is only for development time :)
            return Microsoft.VisualBasic.Interaction.InputBox(
                $"Enter new {itemType} name",
                $"New {itemType}");
        }
    }
}
