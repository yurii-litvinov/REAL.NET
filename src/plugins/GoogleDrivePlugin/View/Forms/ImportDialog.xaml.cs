namespace GoogleDrivePlugin.View
{
    using System.Windows;

    /// <summary>
    /// Логика взаимодействия для ImportDialog.xaml
    /// </summary>
    public partial class ImportDialog : Window
    {
        public ImportDialog(Controller.GoogleDriveController controller)
        {
            InitializeComponent();

            this.CancelButton.Click += (sender, args) =>
                controller.RequestImportWindowHidind();
            this.OpenButton.Click += (sender, args) =>
                controller.RequestFileImport(
                    this.FileExplorer.CurrentPath, 
                    this.FileExplorer.SelectedItem.Name);

            this.LogoutBox.LogoutButton.Click += (sender, args) => controller.RequestLoggingOut();

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

        protected string GetNewItemName(string itemType)
        {
            // It is only for development time :)
            return Microsoft.VisualBasic.Interaction.InputBox(
                $"Enter new {itemType} name",
                $"New {itemType}");
        }
    }
}
