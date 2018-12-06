namespace GoogleDrivePlugin.View
{
    using System.Windows;

    /// <summary>
    /// Interaction logic for ExportDialog.xaml
    /// </summary>
    public partial class ExportDialog : Window 
    {
        /// <summary>
        /// Initializes new instance of ExportDialog
        /// </summary>
        /// <param name="controller">Plugin controller</param>
        public ExportDialog(Controller.GoogleDriveController controller)
        {
            InitializeComponent();

            this.CancelButton.Click += (sender, args) => 
                controller.RequestExportWindowHide();
            this.SaveButton.Click += async (sender, args) =>
            {
                if (this.FileExplorer.SelectedItem != null)
                {
                    await controller.RequestModelExport(
                        this.FileExplorer.CurrentDirectoryID,
                        this.FileExplorer.SelectedItem.ID,
                        this.FileExplorer.SelectedItem.IsDirectory);
                }
            };

            this.NewFileButton.Click += (sender, args) =>
                controller.RequestNewFileСreation(
                    this.FileExplorer.CurrentDirectoryID,
                    this.GetNewItemName("file"));

            this.NewFolderButton.Click += (sender, args) =>
                controller.RequestNewFolderCreation(
                    this.FileExplorer.CurrentDirectoryID, 
                    this.GetNewItemName("folder"));

            this.LogoutBox.LogoutButton.Click += async (sender, args) =>
                await controller.RequestLoggingOut();

            this.FileExplorer.ItemSelected += async (sender, fileInfo) =>
                await controller.RequestModelExport(
                    this.FileExplorer.CurrentDirectoryID, 
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
                controller.RequestFileMovement(
                    this.FileExplorer.CurrentDirectoryID, sourceInfo.ID, destInfo.ID);

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
