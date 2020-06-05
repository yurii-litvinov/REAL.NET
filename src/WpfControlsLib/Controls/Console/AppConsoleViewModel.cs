/* Copyright 2017-2019 REAL.NET group
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

using EditorPluginInterfaces;
using System.Collections.ObjectModel;

namespace WpfControlsLib.Controls.Console
{
    /// <summary>
    /// ViewModel for errors and warnings window that is shown below the main scene.
    /// Allows to add new messages.
    /// </summary>
    public class AppConsoleViewModel : IConsole
    {
        public ObservableCollection<string> Messages { get; } = new ObservableCollection<string>();

        public ObservableCollection<string> Errors { get; } = new ObservableCollection<string>();

        public void ReportError(string error) => this.Errors.Add(error);

        public void SendMessage(string message) => this.Messages.Add(message);

        public string ErrorsName => ConsoleLanguageResource.Errors;

        public string MessagesName => ConsoleLanguageResource.Messages;
    }
}
