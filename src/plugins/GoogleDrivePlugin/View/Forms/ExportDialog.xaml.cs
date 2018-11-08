namespace GoogleDrivePlugin.View
{
    using System.Windows;

    /// <summary>
    /// Логика взаимодействия для ExportDialog.xaml
    /// </summary>
    public partial class ExportDialog : Window 
    {
        private string currentPath;
        
        public ExportDialog(Controller.GoogleDriveController controller)
        {
            InitializeComponent();

            this.CancelButton.Click += (sender, args) => 
                controller.RequestExportWindowHiding();
            this.SaveButton.Click += (sender, args) =>
                controller.RequestModelExport(currentPath, this.FileExplorer.SelectedItem.Name);

            this.NewFileButton.Click += (sender, args) =>
                controller.RequestNewFileСreation(currentPath, this.GetNewItemName("file"));
            this.NewFolderButton.Click += (sender, args) =>
                controller.RequestNewFolderCreation(currentPath, this.GetNewItemName("folder"));

            this.LogoutBox.LogoutButton.Click += (sender, args) =>
                controller.RequestLoggingOut();

            this.FileExplorer.ItemSelected += (sender, fileInfo) =>
                controller.RequestModelExport(currentPath, fileInfo.Name);
            this.FileExplorer.ItemDeletionRequested += (sender, itemInfo) =>
                controller.RequestFileDeletion(currentPath, itemInfo.Name);
            this.FileExplorer.ItemMovementRequested += (sender, sourceInfo, destInfo) =>
                controller.RequestFileMovement(currentPath, sourceInfo.Name, destInfo.Name);
        }

        private void HideWindow() => this.Hide();

        protected string GetNewItemName(string itemType)
        {
            // It is only for development time :)
            return Microsoft.VisualBasic.Interaction.InputBox(
                $"Enter new {itemType} name",
                $"New {itemType}");
        }
    }
}
