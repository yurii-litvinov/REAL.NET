using System;
using System.Collections.Generic;

namespace REAL.NET.Models
{
    public class AppConsole
    {
        private Dictionary<string, ConsoleWindow> windows = new Dictionary<string, ConsoleWindow>();

        public bool VisibilityStatus { get; private set; }

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

    }
}
