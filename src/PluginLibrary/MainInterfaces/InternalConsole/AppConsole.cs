using System;
using System.Collections.Generic;

namespace EditorPrototype.Models.InternalConsole
{
    public class AppConsole : IConsole
    {
        public IList<string> Messages { get; private set; } = new List<string>();

        public IList<string> Errors { get; private set; } = new List<string>();

        public event EventHandler<EventArgs> NewMessage;

        public event EventHandler<EventArgs> NewError;

        public void ReportError(string error)
        {
            Errors.Add(error);
            NewError?.Invoke(this, EventArgs.Empty);
        }

        public void SendMessage(string message)
        {
            Messages.Add(message);
            NewMessage?.Invoke(this, EventArgs.Empty);
        }
    }
}
