using System.Collections.Generic;
using WPF_Editor.Models.Interfaces;

namespace WPF_Editor.Models.Console
{
    public class AppConsole : IAppConsole
    {
        private Dictionary<string, IConsoleWindow> windows = new Dictionary<string, IConsoleWindow>();

        public bool IsVisible { get; private set; }

        public IAppConsoleMediator AppConsoleMediator { get; }

        public AppConsole(IAppConsoleMediator mediator)
        {
            this.AppConsoleMediator = mediator;
        }

        public AppConsole()
        {
            windows.Add("MessageConsole", new ConsoleWindow());
            windows.Add("ErrorConsole", new ConsoleWindow());
            IsVisible = false;
        }

        public IConsoleWindow GetConsoleWindowByName(string name)
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

        IConsoleWindow IAppConsole.GetConsoleWindowByName(string name) => windows[name];

        public void AddConsoleWindow(IConsoleWindow window, string name) => windows.Add(name, window);
    }
}
