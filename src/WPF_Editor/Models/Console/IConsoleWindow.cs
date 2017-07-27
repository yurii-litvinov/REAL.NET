namespace REAL.NET.Models
{
    /// <summary>
    /// Interface that is part of AppConsole
    /// </summary>
    public interface IConsoleWindow
    {
        /// <summary>
        /// Clear this window from messages
        /// </summary>
        void ClearConsoleWindow();

        /// <summary>
        /// Send new message to this window
        /// </summary>
        /// <param name="message">Message to send</param>
        void NewMessage(string message);

        /// <summary>
        /// Override method that returns all messages
        /// </summary>
        /// <returns>All messages</returns>
        string ToString();
    }
}