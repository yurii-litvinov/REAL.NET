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


    /// <summary>
    /// Interaction logic for ConstraintsPanel.xaml
    /// </summary>
    public partial class ConstraintsColumn : UserControl
    {
        private readonly WpfControlsLib.Controller.Controller controller;

        public ConstraintsColumn(PluginConfig config)
        {
            this.controller = new WpfControlsLib.Controller.Controller();
            this.scene = new Scene();
            this.scene.Init((Model)config.Model, this.controller, config.ElementProvider);
            this.InitializeComponent();
        }

        private void NewButtonClick(object sender, System.Windows.RoutedEventArgs e)
        {
            var unit = new ConstraintsUnit();
            unit.DeleteButtonClicked += new Action<ConstraintsUnit>(DeleteConstraintsUnit);
            this.stackPanel.Children.Add(unit);
        }

        private void DeleteConstraintsUnit(ConstraintsUnit i)
        {
            this.stackPanel.Children.Remove(i);
        }
    }
}