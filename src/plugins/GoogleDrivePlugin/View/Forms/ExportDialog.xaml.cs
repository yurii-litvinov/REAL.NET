namespace GoogleDrivePlugin.View
{
    using System.Windows;

    /// <summary>
    /// Логика взаимодействия для ExportDialog.xaml
    /// </summary>
    public partial class ExportDialog : Window 
    {
        public ExportDialog(Controller.GoogleDriveController controller)
        {
            InitializeComponent();

            this.CancelButton.Click += (sender, args) => 
                controller.RequestExportWindowHiding();
            this.SaveButton.Click += (sender, args) =>
                controller.RequestModelExport(this.FileExplorer.SelectedItem.ID);

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

            this.FileExplorer.ItemSelected += (sender, fileInfo) =>
                controller.RequestModelExport(fileInfo.ID);
            this.FileExplorer.ItemDeletionRequested += (sender, itemInfo) =>
                controller.RequestFileDeletion(itemInfo.ID);
            this.FileExplorer.ItemMovementRequested += (sender, sourceInfo, destInfo) =>
                controller.RequestFileMovement(sourceInfo.ID, destInfo.ID);

            this.Loaded += (sender, args) => controller.RequestDirectoryContent(null);
        }
        
        protected string GetNewItemName(string itemType)
        {
            // It is only for development time :)
            return Microsoft.VisualBasic.Interaction.InputBox(
                $"Enter new {itemType} name",
                $"New {itemType}");
        }
    }
}
