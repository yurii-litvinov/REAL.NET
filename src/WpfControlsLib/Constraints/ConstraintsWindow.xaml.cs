/* Copyright 2018 REAL.NET group
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

namespace WpfControlsLib.Constraints
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;

    /// <summary>
    /// Interaction logic for ConstraintsWindow.xaml
    /// </summary>
    public partial class ConstraintsWindow : Window
    {
        private readonly RepoInfo info;

        private string objType;
        private string elementType;

        private bool userChange = true;
        private WpfControlsLib.Model.Model model;

        public ConstraintsWindow(WpfControlsLib.Model.Model model)
        {
            this.model = model;
            this.InitializeComponent();
            this.info = new RepoInfo(model.Repo, model.ModelName);
        }

        public void ObjType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.errorLabel.Visibility = Visibility.Hidden;
            this.ammountLabel.Visibility = Visibility.Hidden;
            this.ammountBox.Visibility = Visibility.Hidden;
            switch (((ComboBoxItem)this.ObjType.SelectedItem).Content.ToString())
            {
                case "Node":
                    this.userChange = false;
                    this.ElementType.ItemsSource = new List<ConstraintsValues>(this.info.GetNodeTypes().Select(x => new ConstraintsValues { ElementType = x }));
                    this.objType = "Node";
                    break;
                case "EdgeViewModel":
                    this.userChange = false;
                    this.ElementType.ItemsSource = new List<ConstraintsValues>(this.info.GetEdgeTypes().Select(x => new ConstraintsValues { ElementType = x }));
                    this.objType = "EdgeViewModel";
                    break;
            }

            this.userChange = true;
        }

        private void ElementType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.errorLabel.Visibility = Visibility.Hidden;
            this.ammountLabel.Visibility = Visibility.Hidden;
            this.ammountBox.Visibility = Visibility.Hidden;
            if (this.userChange)
            {
                switch (((ConstraintsValues)this.ElementType.SelectedItem).ElementType)
                {
                    case "All":
                        this.ammountLabel.Visibility = Visibility.Visible;
                        this.ammountBox.Visibility = Visibility.Visible;
                        this.elementType = "All";
                        break;
                }
            }
        }

        private void CreateConstraint_Click(object sender, RoutedEventArgs e)
        {
            var item = new ConstraintsItem();
            if (!string.IsNullOrEmpty(this.ammountBox.Text))
            {
                try
                {
                    if (this.objType == "Node")
                    {
                        this.model.Constraints.NodesAmount = Convert.ToInt32((string)this.ammountBox.Text);
                    }
                    else
                    {
                        this.model.Constraints.EdgesAmount = Convert.ToInt32((string)this.ammountBox.Text);
                    }

                    item.Ammount = Convert.ToInt32(this.ammountBox.Text);
                }
                catch
                {
                    this.errorLabel.Visibility = Visibility.Visible;
                    this.errorLabel.Foreground = Brushes.Red;
                    this.errorLabel.Text = "Неправильно заполнено поле количества элементов";
                    return;
                }
            }

            item.ElementType = this.elementType;
            item.ObjectType = this.objType;
            item.Initialize();
            this.ConstraintsPanel.Children.Add(item);
        }
    }
}
