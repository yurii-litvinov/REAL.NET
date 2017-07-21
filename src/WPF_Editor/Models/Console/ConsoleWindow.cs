using System.Collections.Generic;

namespace WPF_Editor.Models.Console
{
    public class ConsoleWindow
    {
        protected List<string> messages;

        public ConsoleWindow()
        {
            messages = new List<string>();
        }

        public override string ToString()
        {
            string resultMessage = "";
            foreach (var message in messages)
            {
                resultMessage += message + "\n";
            }
            return resultMessage;
        }

        public virtual void NewMessage(string message)
        {
            this.messages.Add(message);
        }

        public void ClearConsoleWindow()
        {
            messages.Clear();
        }

        public void SetFocusOnThisElement()
        {

        }
    }
}