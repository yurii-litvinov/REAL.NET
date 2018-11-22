namespace GoogleDrivePlugin.Model
{
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

        private UserCredential userCredentials;
        private string username;

        private const string ApplicationName = "REALNET-GoogleDrivePlugin";

        public GoogleDriveModel()
        {

        }

        public async void LogUserOut()
        {
            this.HideExportWindow?.Invoke(this, new UserInfoArgs(this.userCredentials.UserId));
            this.HideImportWindow?.Invoke(this, new UserInfoArgs(this.userCredentials.UserId));

            await this.userCredentials.RevokeTokenAsync(CancellationToken.None);
            this.userCredentials = null;
        }

        public async void RequestUpload()
        {
            if (this.userCredentials == null)
            {
                this.userCredentials = await AuthorizeUser();
                this.username = await GetUserInfo(this.userCredentials);
            }

            this.ShowExportWindow?.Invoke(this, new UserInfoArgs(this.username));
        }

        public void RequestUploadHide()
        {

        }

        public async void RequestDownload()
        {
            if (this.userCredentials == null)
            {
                this.userCredentials = await AuthorizeUser();
                this.username = await GetUserInfo(this.userCredentials);
            }

            this.ShowImportWindow?.Invoke(this, new UserInfoArgs(this.username));
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
                "user3",
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
    }
}
