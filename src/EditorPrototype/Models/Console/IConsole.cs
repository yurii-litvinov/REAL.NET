using System;
using System.Collections.Generic;

namespace EditorPrototype.Models.Console
{
    public interface IConsole
    {
        event EventHandler<EventArgs> NewMessage;

        event EventHandler<EventArgs> NewError;

        IList<string> Messages { get; }

        IList<string> Errors { get; }

        void SendMessage(string message);

        void ReportError(string error);
    }
}
