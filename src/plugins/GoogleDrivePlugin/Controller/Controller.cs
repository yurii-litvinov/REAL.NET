using System;

namespace GoogleDrivePlugin.Controller
{
    using System.Threading.Tasks;

    public class GoogleDriveController
    {
        private Model.GoogleDriveModel model;

        public GoogleDriveController(Model.GoogleDriveModel model)
        {
            this.model = model;
        }

        public async Task RequestImportWindow() => await this.model.RequestDownload();

        public async Task RequestExportWindow() => await this.model.RequestUpload();

        public void RequestImportWindowHidind() => this.model.RequestUploadHide();

        public void RequestExportWindowHiding() => this.model.RequestDownloadHide();

        public async void RequestDirectoryContent(string folderID) =>
            await this.model.RequestFolderContent(folderID);

        public async Task RequestModelImport(
            string fileID, bool isDirectory = false)
        {
            if (isDirectory)
            {
                await this.model.RequestFolderContent(fileID);
            }
            else
            {
                this.model.LoadModelFrom(fileID);
            }
        }

        public async Task RequestModelExport(
            string parentID, 
            string destFileID, 
            bool isDirectory = false)
        {
            if (isDirectory)
            {
                await this.model.RequestFolderContent(destFileID);
            }
            else
            {
                this.model.SaveCurrentModelTo(parentID, destFileID);
            }
        }

        public void RequestNewFileСreation(string parentID, string newFileName) =>
            this.model.CreateNewFile(parentID, newFileName);

        public void RequestNewFolderCreation(string parentID, string newFolderName) =>
            this.model.CreateNewFolder(parentID, newFolderName);

        public async Task RequestLoggingOut() => await this.model.LogUserOut();

        public void RequestFileDeletion(string parentID, string itemID) =>
            this.model.DeleteElement(parentID, itemID);

        public void RequestFileMovement(string sourceFolderID, string sourceFileID, string destFolderID) =>
            this.model.MoveElement(sourceFolderID, sourceFileID, destFolderID);
    }
}
