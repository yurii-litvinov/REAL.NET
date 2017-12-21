using System;
using System.Collections.Generic;

namespace PluginLibrary.MainInterfaces
{
    /// <summary>
    /// Abstraction of editor's console
    /// </summary>
    public interface IConsole
    {
        /// <summary>
        /// Event raised when new message has been sent
        /// </summary>
        event EventHandler<EventArgs> NewMessage;

        /// <summary>
        /// Event raised when new error has been reported
        /// </summary>
        event EventHandler<EventArgs> NewError;

        /// <summary>
        /// Gets list of messages sent to console
        /// </summary>
        IList<string> Messages { get; }

        /// <summary>
        /// Gets list of reported errors 
        /// </summary>
        IList<string> Errors { get; }

        /// <summary>
        /// Send message to console
        /// </summary>
        /// <param name="message">Message to send</param>
        void SendMessage(string message);

        /// <summary>
        /// Report error
        /// </summary>
        /// <param name="error">Error to report</param>
        void ReportError(string error);
    }
}
