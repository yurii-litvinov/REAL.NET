/* Copyright 2017-2018 REAL.NET group
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License. */

namespace WpfEditor.Controls.Console
{
    using System;
    using System.Collections.Generic;
    using EditorPluginInterfaces;

    /// <summary>
    /// ViewModel for errors and warnings window that is shown below the main scene.
    /// Allows to add new messages.
    /// </summary>
    internal class AppConsole : IConsole
    {
        public event EventHandler<EventArgs> NewMessage;

        public event EventHandler<EventArgs> NewError;

        public IList<string> Messages { get; } = new List<string>();

        public IList<string> Errors { get; } = new List<string>();

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
