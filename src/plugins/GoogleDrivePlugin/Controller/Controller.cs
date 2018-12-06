using System;

namespace GoogleDrivePlugin.Controller
{
    using System.Threading.Tasks;

    /// <summary>
    /// Controller
    /// </summary>
    public class GoogleDriveController
    {
        /// <summary>
        /// Model of plugin
        /// </summary>
        private Model.GoogleDriveModel model;

        /// <summary>
        /// Initializes new instance of GoogleDriveController
        /// </summary>
        /// <param name="model">Plugin model</param>
        public GoogleDriveController(Model.GoogleDriveModel model)
        {
            this.model = model;
        }

        /// <summary>
        /// Request import window appearance
        /// </summary>
        public async Task RequestImportWindow() => await this.model.RequestDownload();

        /// <summary>
        /// Request export window appearance
        /// </summary>
        public async Task RequestExportWindow() => await this.model.RequestUpload();

        /// <summary>
        /// Request import window hide
        /// </summary>
        public void RequestImportWindowHide() => this.model.RequestDownloadHide();

        /// <summary>
        /// Request export window hide
        /// </summary>
        public void RequestExportWindowHide() => this.model.RequestUploadHide();

        /// <summary>
        /// Request item list of given folder
        /// </summary>
        /// <param name="folderID">ID of folder on Drive</param>
        public async void RequestDirectoryContent(string folderID) =>
            await this.model.RequestFolderContent(folderID);

        /// <summary>
        /// Request import of the model
        /// </summary>
        /// <param name="fileID">ID of file with model</param>
        /// <param name="isDirectory"></param>
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

        /// <summary>
        /// Request export of the model
        /// </summary>
        /// <param name="parentID">ID of containing folder</param>
        /// <param name="destFileID">ID of file to save model to</param>
        /// <param name="isDirectory">Is file a directory</param>
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

        /// <summary>
        /// Request creation of new file on Drive
        /// </summary>
        /// <param name="parentID">ID of containing folder</param>
        /// <param name="newFileName">Name of new file</param>
        public void RequestNewFileСreation(string parentID, string newFileName) =>
            this.model.CreateNewFile(parentID, newFileName);

        /// <summary>
        /// Request creation of new folder on Drive
        /// </summary>
        /// <param name="parentID">ID of containing folder</param>
        /// <param name="newFolderName">New folder name</param>
        public void RequestNewFolderCreation(string parentID, string newFolderName) =>
            this.model.CreateNewFolder(parentID, newFolderName);

        /// <summary>
        /// Reqest logging out of Google Account
        /// </summary>
        /// <returns></returns>
        public async Task RequestLoggingOut() => await this.model.LogUserOut();

        /// <summary>
        /// Request deletion of file on Drive
        /// </summary>
        /// <param name="parentID">ID of containing folder</param>
        /// <param name="itemID">ID of item to delete</param>
        public void RequestFileDeletion(string parentID, string itemID) =>
            this.model.DeleteElement(parentID, itemID);

        /// <summary>
        /// Request movement of item on Drive
        /// </summary>
        /// <param name="sourceFolderID">ID of containing folder</param>
        /// <param name="sourceFileID">ID of item to move</param>
        /// <param name="destFolderID">ID of folder to move to</param>
        public void RequestFileMovement(string sourceFolderID, string sourceFileID, string destFolderID) =>
            this.model.MoveElement(sourceFolderID, sourceFileID, destFolderID);
    }
}
