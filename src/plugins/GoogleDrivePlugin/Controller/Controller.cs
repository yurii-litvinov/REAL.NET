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

        public async Task RequestImportWindow() => await this.model.RequestUpload();

        public async Task RequestExportWindow() => await this.model.RequestDownload();

        public void RequestImportWindowHidind() => this.model.RequestUploadHide();

        public void RequestExportWindowHiding() => this.model.RequestDownloadHide();

        // folderID == null means root directory
        public async void RequestDirectoryContent(string folderID) =>
            await this.model.RequestFolderContent(folderID);

        public void RequestFileImport(string fileID) =>
            this.model.LoadModelFrom(fileID);

        public void RequestModelExport(string destFileID) =>
            this.model.SaveCurrentModelTo(destFileID);

        public void RequestNewFileСreation(string parentID, string newFileName) =>
            this.model.CreateNewFile(parentID, newFileName);

        public void RequestNewFolderCreation(string parentID, string newFolderName) =>
            this.model.CreateNewFolder(parentID, newFolderName);

        public async Task RequestLoggingOut() => await this.model.LogUserOut();

        public void RequestFileDeletion(string itemID) =>
            this.model.DeleteElement(itemID);

        public void RequestFileMovement(string sourceID, string destID) =>
            this.model.MoveElement(sourceID, destID);
    }
}
