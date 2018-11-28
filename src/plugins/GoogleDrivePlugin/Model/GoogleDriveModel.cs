namespace GoogleDrivePlugin.Model
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    using Google.Apis.Oauth2.v2;
    using Google.Apis.Auth.OAuth2;
    using Google.Apis.Drive.v3;

    using Model.ClientSecret;

    public class GoogleDriveModel
    {
        public delegate void UserInfoHandler(object sender, UserInfoArgs info);
        public event UserInfoHandler ShowImportWindow;
        public event UserInfoHandler ShowExportWindow;
        public event UserInfoHandler HideImportWindow;
        public event UserInfoHandler HideExportWindow;

        public delegate void FileListHandler(object sender, FileListArgs args);
        public event FileListHandler FileListReceived;

        private DriveService drive;
        private UserCredential userCredentials;
        private string username;

        private const string ApplicationName = "REALNET-GoogleDrivePlugin";

        public GoogleDriveModel()
        {
        }

        public async Task LogUserOut()
        {
            this.HideExportWindow?.Invoke(this, new UserInfoArgs(this.userCredentials.UserId));
            this.HideImportWindow?.Invoke(this, new UserInfoArgs(this.userCredentials.UserId));

            await this.userCredentials.RevokeTokenAsync(CancellationToken.None);
            this.userCredentials = null;
        }

        public async Task RequestUpload()
        {
            if (this.userCredentials == null)
            {
                try
                {
                    await this.InitiateNewSessionWithDrive();
                }
                catch (Google.Apis.Auth.OAuth2.Responses.TokenResponseException)
                {
                    return;
                }
            }

            this.ShowExportWindow?.Invoke(this, new UserInfoArgs(this.username));
        }

        public void RequestUploadHide()
        {
            this.HideExportWindow?.Invoke(this, new UserInfoArgs(this.username));
        }

        public async Task RequestDownload()
        {
            if (this.userCredentials == null)
            {
                try
                {
                    await this.InitiateNewSessionWithDrive();
                }
                catch (Google.Apis.Auth.OAuth2.Responses.TokenResponseException)
                {
                    return;
                }
            }

            this.ShowImportWindow?.Invoke(this, new UserInfoArgs(this.username));
        }
        
        public void RequestDownloadHide()
        {
            this.HideImportWindow?.Invoke(this, new UserInfoArgs(this.username));
        }

        public async void CreateNewFile(string folderID, string fileName)
        {
            var newEmptyFile = new Google.Apis.Drive.v3.Data.File();
            newEmptyFile.Name = fileName;

            if (folderID != null)
            {
                newEmptyFile.Parents = new List<string>() { folderID };
            }

            var request = this.drive.Files.Create(newEmptyFile);
            await request.ExecuteAsync();

            await this.RequestFolderContent(folderID);
        }

        public void CreateNewFolder(string parentID, string folderName)
        {

        }

        public void DeleteElement(string itemID)
        {

        }

        public void MoveElement(string sourceID, string destID)
        {

        }

        public void SaveCurrentModelTo(string fileID)
        {

        }

        public void LoadModelFrom(string fileID)
        {

        }   

        public async Task RequestFolderContent(string folderID)
        {
            var request = this.drive.Files.List();

            request.Spaces = "drive";
            request.Fields = "files(id, name, size, mimeType)";
            request.OrderBy = "folder";
            request.Q = $"'{(folderID == null ? "root" : folderID)}' in parents";

            var response = await request.ExecuteAsync();

            var itemList = new List<FileMetaInfo>();
           /* if (folderID != null)
            {
                // Level up folder
                itemList.Add(new FileMetaInfo(parentID, "...", null, true));
            }*/

            foreach (var item in response.Files)
            {
                var isFolder = item.MimeType == "application/vnd.google-apps.folder";
                var itemSize = isFolder ? null : $"{item.Size}B";
                var fileInfo = new FileMetaInfo(item.Id, item.Name, itemSize, isFolder);

                itemList.Add(fileInfo);
            }

            this.FileListReceived?.Invoke(this, new FileListArgs(folderID, itemList));
        }

        private async Task InitiateNewSessionWithDrive()
        {
            this.userCredentials = await AuthorizeUser();
            this.username = await GetUserInfo(this.userCredentials);
            this.drive = InitDriveService(this.userCredentials);
        }

        private static async Task<UserCredential> AuthorizeUser()
        {
            var scopes = new[] 
            {
                Oauth2Service.Scope.UserinfoProfile,
                Oauth2Service.Scope.UserinfoEmail,
                DriveService.Scope.Drive
            };

            return await GoogleWebAuthorizationBroker.AuthorizeAsync(
                new ClientSecrets
                {
                    ClientId = ClientSecretParams.ClientID,
                    ClientSecret = ClientSecretParams.ClientSecret
                },
                scopes,
                "PluginUser",
                CancellationToken.None);
        }

        private static async Task<string> GetUserInfo(UserCredential credential)
        {
            var service = new Oauth2Service(
                new Google.Apis.Services.BaseClientService.Initializer()
                {
                    HttpClientInitializer = credential,
                    ApplicationName = ApplicationName
                });

            var info = await service.Userinfo.Get().ExecuteAsync();
            return info.Email;
        }

        private static DriveService InitDriveService(UserCredential credential)
        {
            return new DriveService(
                new Google.Apis.Services.BaseClientService.Initializer()
                {
                    HttpClientInitializer = credential,
                    ApplicationName = ApplicationName
                });
        }
    }
}
