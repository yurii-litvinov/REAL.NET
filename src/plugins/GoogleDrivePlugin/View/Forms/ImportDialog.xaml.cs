namespace GoogleDrivePlugin.View
{
    using System.Windows;

    /// <summary>
    /// Логика взаимодействия для ImportDialog.xaml
    /// </summary>
    public partial class ImportDialog : Window
    {
        private string selectedFile;
        private string currentPath;

        public ImportDialog(Controller.GoogleDriveController controller)
        {
            InitializeComponent();

            this.CancelButton.Click += (sender, args) =>
                controller.RequestImportWindowHidind();
            this.OpenButton.Click += (sender, args) =>
                controller.RequestFileImport(this.currentPath, this.selectedFile);

            this.LogoutBox.LogoutButton.Click += (sender, args) => controller.RequestLoggingOut();

            // TODO: add integration with model
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
