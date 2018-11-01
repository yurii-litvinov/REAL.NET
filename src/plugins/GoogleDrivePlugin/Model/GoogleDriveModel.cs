using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoogleDrivePlugin.Model
{
    public class GoogleDriveModel
    {
        public delegate void UserInfoHandler(object sender, UserInfoArgs info);
        public event UserInfoHandler ShowImportWindow;
        public event UserInfoHandler ShowExportWindow;
        public event UserInfoHandler HideImportWindow;
        public event UserInfoHandler HideExportWindow;

        public delegate void FileListHandler(object sender, FileListArgs args);
        public event FileListHandler FileListReceived;

        public GoogleDriveModel()
        {
        }

        public void LogUserOut()
        {

        }

        public void RequestUpload()
        {
            this.ShowExportWindow?.Invoke(this, new UserInfoArgs("TSTibalashov24"));
        }

        public void RequestUploadHide()
        {

        }

        public void RequestDownload()
        {
            this.ShowImportWindow?.Invoke(this, new UserInfoArgs("TSTibalashov24"));
        }
        
        public void RequestDownloadHide()
        {

        }

        public void CreateNewFile(string path, string newFileName)
        {

        }

        public void CreateNewFolder(string path, string newFolderName)
        {

        }

        public void DeleteElement(string path, string elementName)
        {

        }

        public void MoveElement(string currentPath, string elementName, string destPath)
        {

        }

        public void SaveCurrentModelTo(string path, string fileName)
        {

        }

        public void LoadModelFrom(string path, string fileName)
        {

        }

        public void RequestFolderContent(string path, string folderName)
        {

        }

        public void InitiateNewSessionWithDrive()
        {
            // Logging in

            // Request Drive content
        }
    }
}
