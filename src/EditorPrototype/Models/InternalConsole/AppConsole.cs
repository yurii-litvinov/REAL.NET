using System;
using System.Collections.Generic;
using EditorPluginInterfaces;

namespace EditorPrototype.Models.InternalConsole
{
    public class AppConsole : IConsole
    {
        public event EventHandler<EventArgs> NewMessage;

        public event EventHandler<EventArgs> NewError;

        public IList<string> Messages { get; private set; } = new List<string>();

        public IList<string> Errors { get; private set; } = new List<string>();

        public void ReportError(string error)
        {
            this.Errors.Add(error);
            this.NewError?.Invoke(this, EventArgs.Empty);
        }

        public void SendMessage(string message)
        {
            this.Messages.Add(message);
            this.NewMessage?.Invoke(this, EventArgs.Empty);
        }
    }
}
