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
                controller.RequestModelExport(
                    this.FileExplorer.CurrentPath, 
                    this.FileExplorer.SelectedItem.Name);

            this.NewFileButton.Click += (sender, args) =>
                controller.RequestNewFileСreation(
                    this.FileExplorer.CurrentPath,
                    this.GetNewItemName("file"));

            this.NewFolderButton.Click += (sender, args) =>
                controller.RequestNewFolderCreation(
                    this.FileExplorer.CurrentPath, 
                    this.GetNewItemName("folder"));

            this.LogoutBox.LogoutButton.Click += (sender, args) =>
                controller.RequestLoggingOut();

            this.FileExplorer.ItemSelected += (sender, fileInfo) =>
                controller.RequestModelExport(this.FileExplorer.CurrentPath, fileInfo.Name);
            this.FileExplorer.ItemDeletionRequested += (sender, itemInfo) =>
                controller.RequestFileDeletion(this.FileExplorer.CurrentPath, itemInfo.Name);

            this.FileExplorer.ItemMovementRequested += (sender, sourceInfo, destInfo) =>
                controller.RequestFileMovement(
                    this.FileExplorer.CurrentPath, 
                    sourceInfo.Name, 
                    destInfo.Name);
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
