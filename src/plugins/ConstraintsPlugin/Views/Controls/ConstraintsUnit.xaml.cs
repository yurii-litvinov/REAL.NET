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

namespace ConstraintsPlugin
{
    using EditorPluginInterfaces;
    using System;
    using System.ComponentModel;
    using System.Windows.Controls;
    using WpfControlsLib.Controls.Scene;


    /// <summary>
    /// Interaction logic for ConstraintsPanel.xaml
    /// </summary>
    public partial class ConstraintsUnit : UserControl, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public event Action<ConstraintsUnit> DeleteButtonClicked;

        public event Action<string> EditButtonClicked;

        public string ConstraintsModelName;
        private String rootName;

        public String RootName
        {
            get { return rootName; }
            set
            {
                rootName = value;
                OnPropertyChanged("RootName");
            }
        }

        public ConstraintsUnit()
        {
            this.DataContext = this;
            this.rootName = rootName;
            this.InitializeComponent();
        }

        //public ConstraintsUnit()
        //{
        //    this.DataContext = this;
        //    this.InitializeComponent();
        //}

        private void DeleteButtonClick(object sender, System.Windows.RoutedEventArgs e)
        {
            DeleteButtonClicked(this);
        }

        private void EditButtonClick(object sender, System.Windows.RoutedEventArgs e)
        {
            EditButtonClicked(this.ConstraintsModelName);
        }

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}