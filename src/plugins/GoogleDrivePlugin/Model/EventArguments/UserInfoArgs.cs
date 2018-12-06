using System;

namespace GoogleDrivePlugin.Model
{
    /// <summary>
    /// Available operation types on windows
    /// </summary>
    public enum OperationType {OpenWindow, CloseWindow, Progress, Error}

    /// <summary>
    /// Contains info about operation on window
    /// </summary>
    public class OperationProgressArgs : EventArgs
    {
        /// <summary>
        /// Initializes new instance of OperationProgressArgs
        /// </summary>
        /// <param name="operationType">Type of operation</param>
        /// <param name="info">Info about operation</param>
        /// <param name="progress">Operation progress</param>
        public OperationProgressArgs(OperationType operationType, string info, int progress = 0)
        {
            this.OperationType = operationType;
            this.Info = info;
            this.Progress = progress;
        }

        /// <summary>
        /// Type of operation
        /// </summary>
        public OperationType OperationType { get; }

        /// <summary>
        /// Info about operation
        /// </summary>
        public string Info { get; }

        /// <summary>
        /// Operation progress
        /// </summary>
        public int Progress { get; }
    }
}
