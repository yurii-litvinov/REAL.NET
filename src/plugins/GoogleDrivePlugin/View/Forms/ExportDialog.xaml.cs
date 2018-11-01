namespace GoogleDrivePlugin.View
{
    using System.Windows;

    /// <summary>
    /// Логика взаимодействия для ExportDialog.xaml
    /// </summary>
    public partial class ExportDialog : Window 
    {
        private string selectedFile;
        private string currentPath;
        
        public ExportDialog(Controller.GoogleDriveController controller)
        {
            this.CancelButton.Click += (sender, args) => 
                controller.RequestExportWindowHiding();
            this.SaveButton.Click += (sender, args) =>
                controller.RequestModelExport(currentPath, selectedFile);

            this.NewFileButton.Click += (sender, args) =>
                controller.RequestNewFileСreation(currentPath, this.GetNewItemName("file"));
            this.NewFolderButton.Click += (sender, args) =>
                controller.RequestNewFolderCreation(currentPath, this.GetNewItemName("folder"));

            this.LogoutBox.LogoutButton.Click += (sender, args) =>
                controller.RequestLoggingOut();

            // TODO: Handle actions with FileExplorer

            InitializeComponent();
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
