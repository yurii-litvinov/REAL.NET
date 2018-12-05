using System;

namespace GoogleDrivePlugin.Model
{
    public enum OperationType {OpenWindow, CloseWindow, Progress, Error}

    public class OperationProgressArgs : EventArgs
    {
        public OperationProgressArgs(OperationType operationType, string info, int progress = 0)
        {
            this.OperationType = operationType;
            this.Info = info;
            this.Progress = progress;
        }

        public OperationType OperationType { get; }

        public string Info { get; }

        public int Progress { get; }
    }
}
