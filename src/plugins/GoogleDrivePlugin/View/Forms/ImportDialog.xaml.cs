namespace GoogleDrivePlugin.View
{
    using System.Windows;

    /// <summary>
    /// Логика взаимодействия для ImportDialog.xaml
    /// </summary>
    public partial class ImportDialog : Window
    {
        private string currentPath;

        public ImportDialog(Controller.GoogleDriveController controller)
        {
            InitializeComponent();

            this.CancelButton.Click += (sender, args) =>
                controller.RequestImportWindowHidind();
            this.OpenButton.Click += (sender, args) =>
                controller.RequestFileImport(
                    this.currentPath, 
                    this.FileExplorer.SelectedItem.Name);

            this.LogoutBox.LogoutButton.Click += (sender, args) => controller.RequestLoggingOut();

            this.FileExplorer.ItemSelected += (sender, fileInfo) =>
                controller.RequestModelExport(currentPath, fileInfo.Name);
            this.FileExplorer.ItemDeletionRequested += (sender, itemInfo) =>
                controller.RequestFileDeletion(currentPath, itemInfo.Name);
            this.FileExplorer.ItemMovementRequested += (sender, sourceInfo, destInfo) =>
                controller.RequestFileMovement(currentPath, sourceInfo.Name, destInfo.Name);
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
