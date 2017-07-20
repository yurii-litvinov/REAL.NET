namespace REAL.NET.Models
{
    public interface IAppConsole
    {
        /// <summary>
        /// Console's mediator
        /// </summary>
        IAppConsoleMediator AppConsoleMediator { get; }

        /// <summary>
        /// True, if it's visible, else false
        /// </summary>
        bool IsVisible { get; }

        /// <summary>
        /// Hides console from screen
        /// </summary>
        void HideConsole();

        /// <summary>
        /// Show Console on Screen
        /// </summary>
        void ShowConsole();

        /// <summary>
        /// Sends message to Console's window
        /// </summary>
        /// <param name="name">Name of Window</param>
        /// <param name="message">Message to send</param>
        void NewMessageToConsoleWindowByName(string name, string message);

        /// <summary>
        /// Clear all messages from window titled by this name
        /// </summary>
        /// <param name="name">Name of window</param>
        void ClearConsoleWindowByName(string name);

        /// <summary>
        /// Gets window by name
        /// </summary>
        /// <param name="name">Name of window</param>
        /// <returns></returns>
        IConsoleWindow GetConsoleWindowByName(string name);

        /// <summary>
        /// Adds new Console Window and sets this name to it
        /// </summary>
        /// <param name="window"></param>
        /// <param name="name"></param>
        void AddConsoleWindow(IConsoleWindow window, string name);
    }
}