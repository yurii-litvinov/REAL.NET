using System;

namespace GoogleDrivePlugin.Model
{
    public class UserInfoArgs : EventArgs
    {
        public UserInfoArgs(string username)
        {
            this.Username = username;
        }

        public string Username { get; }
    }
}
