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
    using System.Windows.Controls;
    using WpfControlsLib.Controls.Scene;
    using WpfControlsLib.Model;
    using System.ComponentModel;


    /// <summary>
    /// Interaction logic for ConstraintsPanel.xaml
    /// </summary>
    public partial class ConstraintsColumn : UserControl, INotifyPropertyChanged
    {
        private readonly WpfControlsLib.Controller.Controller controller;

        public event PropertyChangedEventHandler PropertyChanged;

        public event Action<Boolean> SaveButtonClicked;
        private Boolean enableSaveButton = false;
        public Boolean EnableSaveButton
        {
            get { return enableSaveButton; }
            set
            {
                enableSaveButton = value;
                OnPropertyChanged("EnableSaveButton");
            }
        }

        public event Action<bool> CheckButtonClicked;

        public bool allowSave = true;
        public event Action<Boolean> NewButtonClicked;
        private Boolean enableNewButton = true;
        public Boolean EnableNewButton
        {
            get { return enableNewButton; }
            set
            {
                enableNewButton = value;
                OnPropertyChanged("EnableNewButton");
            }
        }

        public event Action<Boolean> CloseButtonClicked;
        private Boolean enableCloseButton = false;
        public Boolean EnableCloseButton
        {
            get { return enableCloseButton; }
            set
            {
                enableCloseButton = value;
                OnPropertyChanged("EnableCloseButton");
            }
        }

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public ConstraintsColumn(PluginConfig config)
        {
            this.DataContext = this;
            this.controller = new WpfControlsLib.Controller.Controller();
            //this.scene = new Scene();
            //this.scene.Init((Model)config.Model, this.controller, config.ElementProvider);
            this.InitializeComponent();
        }

        private void SaveConstraintClick(object sender, System.Windows.RoutedEventArgs e)
        {
            this.SaveButtonClicked(true);
            if (allowSave)
            {
                this.EnableSaveButton = false;
                this.EnableCloseButton = false;
                this.EnableNewButton = true;
            }
        }

        public void AddUnit(ConstraintsUnit unit)
        {
            this.stackPanel.Children.Add(unit);
        }
        private void CloseConstraintClick(object sender, System.Windows.RoutedEventArgs e)
        {
            this.EnableSaveButton = false;
            this.EnableNewButton = true;
            this.EnableCloseButton = false;
            this.allowSave = true;
            this.CloseButtonClicked(true);
        }

        private void NewConstraintClick(object sender, System.Windows.RoutedEventArgs e)
        {
            this.EnableSaveButton = true;
            this.EnableNewButton = false;
            this.EnableCloseButton = true;
            this.allowSave = true;
            this.NewButtonClicked(true);
        }

        private void CheckConstraintsClick(object sender, System.Windows.RoutedEventArgs e)
        {
            this.CheckButtonClicked(false);
        }
        public void DeleteConstraintUnit(ConstraintsUnit i)
        {
            this.stackPanel.Children.Remove(i);
        }
    }
}