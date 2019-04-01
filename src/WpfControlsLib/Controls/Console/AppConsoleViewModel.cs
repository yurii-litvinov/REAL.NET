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

using System.Collections.ObjectModel;
using EditorPluginInterfaces;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace WpfControlsLib.Controls.Console
{
    /// <summary>
    /// ViewModel for errors and warnings window that is shown below the main scene.
    /// Allows to add new messages.
    /// </summary>
    public class AppConsoleViewModel : INotifyPropertyChanged, IConsole
    {
        public ObservableCollection<string> Messages { get; } = new ObservableCollection<string>();

        public ObservableCollection<string> Errors { get; } = new ObservableCollection<string>();

        public ObservableCollection<string> OclEditor { get; } = new ObservableCollection<string>();

        public void ReportError(string error) => this.Errors.Add(error);

        public void SendMessage(string message) => this.Messages.Add(message);


        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }

        string expr;
        public string Ocl {
            get
            {
                return this.expr;
            }
            set
            {
                this.expr = value;
                //OnPropertyChanged(nameof(Ocl));
            }
        }

    }
}
