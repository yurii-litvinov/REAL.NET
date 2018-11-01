using System;

namespace GoogleDrivePlugin.Controller
{
    public class GoogleDriveController
    {
        private Model.GoogleDriveModel model;

        public GoogleDriveController(Model.GoogleDriveModel model)
        {
            this.model = model;
        }

        public void RequestImportWindow() => this.model.RequestUpload();

        public void RequestExportWindow() => this.model.RequestDownload();

        public void RequestImportWindowHidind() => this.model.RequestUploadHide();

        public void RequestExportWindowHiding() => this.model.RequestDownloadHide();

        public void RequestDirectoryContent(string path, string folderName) =>
            this.model.RequestFolderContent(path, folderName);

        public void RequestFileImport(string path, string fileName) =>
            this.model.LoadModelFrom(path, fileName);

        public void RequestModelExport(string path, string destinationFileName) =>
            this.model.SaveCurrentModelTo(path, destinationFileName);

        public void RequestNewFileСreation(string path, string fileName) =>
            this.model.CreateNewFile(path, fileName);

        public void RequestNewFolderCreation(string path, string folderName) =>
            this.model.CreateNewFolder(path, folderName);

        public void RequestLoggingOut() => this.model.LogUserOut();

        public void RequestFileDeletion(string path, string item) =>
            this.model.DeleteElement(path, item);

        public void RequestFileMovement(string sourcePath, string fileName, string destPath) =>
            this.model.MoveElement(sourcePath, fileName, destPath);
    }
}
