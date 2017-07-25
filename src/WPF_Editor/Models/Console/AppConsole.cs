using System.Collections.Generic;
using WPF_Editor.Models.Interfaces;

namespace WPF_Editor.Models.Console
{
    public class AppConsole : IAppConsole
    {
        private Dictionary<string, ConsoleWindow> windows = new Dictionary<string, ConsoleWindow>();

        public bool IsVisible { get; private set; }

        public IAppConsoleMediator AppConsoleMediator { get; }

        private static IAppConsole console;

        public static IAppConsole CreateConsole()
        {
            if (console == null)
            {
                console = new AppConsole();
            }
            return console;
        }
        
        private AppConsole(IAppConsoleMediator mediator)
        {
            this.AppConsoleMediator = mediator;
        }

        public AppConsole()
        {
            windows.Add("MessageConsole", new ConsoleWindow());
            windows.Add("ErrorConsole", new ConsoleWindow());
            IsVisible = false;
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
            IsVisible = true;
        }

        public void HideConsole()
        {
            IsVisible = false;
        }

        public void ClearConsoleWindowByName(string name)
        {
            windows[name].ClearConsoleWindow();
        }
    }
}
