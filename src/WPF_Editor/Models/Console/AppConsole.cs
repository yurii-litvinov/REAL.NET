using System.Collections.Generic;
using WPF_Editor.Models.Interfaces;

namespace WPF_Editor.Models.Console
{
    public class AppConsole : IAppConsole
    {
        private Dictionary<string, ConsoleWindow> windows = new Dictionary<string, ConsoleWindow>();

        public bool VisibilityStatus { get; private set; }

        public IAppConsoleMediator AppConsoleMediator { get; }

        public AppConsole(IAppConsoleMediator mediator)
        {
            this.AppConsoleMediator = mediator;
        }

        public AppConsole()
        {
            windows.Add("MessageConsole", new ConsoleWindow());
            windows.Add("ErrorConsole", new ConsoleWindow());
            VisibilityStatus = false;
        }

        public ConsoleWindow GetConsoleWindowByName(string name)
        {
            return windows[name];
        }

        public void NewMessageToConsoleWindowByName(string name, string message)
        {
            windows[name].NewMessage(message);
        }

        public void ShowConsole()
        {
            VisibilityStatus = true;
        }

        public void HideConsole()
        {
            VisibilityStatus = false;
        }

        public void ClearConsoleWindowByName(string name)
        {
            windows[name].ClearConsoleWindow();
        }
    }
}
