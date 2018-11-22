﻿using System;

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
        public async void RequestDirectoryContent(string folderID, string parentID = null) =>
            await this.model.RequestFolderContent(folderID, parentID);

        public async Task RequestFileImport(
            string fileID, bool isDirectory = false, string parentID = null)
        {
            if (isDirectory)
            {
                await this.model.RequestFolderContent(fileID, parentID);
            }
            else
            {
                this.model.LoadModelFrom(fileID);
            }
        }

        public async Task RequestModelExport(
            string destFileID, bool isDirectory = false, string parentID = null)
        {
            if (isDirectory)
            {
                await this.model.RequestFolderContent(destFileID, parentID);
            }
            else
            {
                this.model.SaveCurrentModelTo(destFileID);
            }
        }

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
