﻿namespace GoogleDrivePlugin.View
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
            this.OpenButton.Click += async (sender, args) =>
                await controller.RequestFileImport(
                    this.FileExplorer.SelectedItem.ID, 
                    this.FileExplorer.SelectedItem.IsDirectory,
                    this.FileExplorer.CurrentDirectoryID);

            this.LogoutBox.LogoutButton.Click += async (sender, args) =>
                await controller.RequestLoggingOut();

            this.FileExplorer.ItemSelected += async (sender, fileInfo) =>
                await controller.RequestModelExport(fileInfo.ID, fileInfo.IsDirectory);
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
