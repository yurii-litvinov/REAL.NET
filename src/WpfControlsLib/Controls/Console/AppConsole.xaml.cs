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

using System.ComponentModel;
using System.Windows.Controls;

namespace WpfControlsLib.Controls.Console
{
    /// <summary>
    /// Application console --- centralized place where various messages and error reports shall be placed.
    /// </summary>
    public partial class AppConsole : UserControl
    {
        // Объявляем делегат
        public delegate void TextHandler(string message);
        // Событие, возникающее при выводе денег
        public event TextHandler Pushed;
        public AppConsole()
        {
            this.InitializeComponent();
            Loaded += AppConsole_Loaded;
        }

        private void AppConsole_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            INotifyPropertyChanged viewModel = DataContext as INotifyPropertyChanged;
            viewModel.PropertyChanged += ViewModel_PropertyChanged;
        }

        private void ViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            oclBox.Text = (DataContext as AppConsoleViewModel).Ocl;
        }

        private void button1_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Pushed("");
        }
        
    }
}
