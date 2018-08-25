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
        private string elementType1;
        private string elementType2;

        private bool userChange = true;
        private WpfControlsLib.Model.Model model;
        private List<ConstraintItem> constraintsList;

        public ConstraintsWindow(WpfControlsLib.Model.Model model)
        {
            this.constraintsList = new List<ConstraintItem>();
            this.model = model;
            this.InitializeComponent();
            this.info = new RepoInfo(model.Repo, model.ModelName);
        }

        public string SelectedType1 { get; set; }

        public string SelectedType2 { get; set; }

        public void ObjType1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.errorLabel.Visibility = Visibility.Hidden;
            switch (((ComboBoxItem)this.ObjType1.SelectedItem).Content.ToString())
            {
                case "Node":
                    this.userChange = false;
                    this.ElementType1.ItemsSource = new List<ConstraintsValues>(this.info.GetNodeTypes().Select(x => new ConstraintsValues { ElementType = x }));
                    this.objType = "Node";
                    this.ElementType2.IsHitTestVisible = true;
                    this.ElementType2.ItemsSource = new List<ConstraintsValues>(this.info.GetNodeTypes().Select(x => new ConstraintsValues { ElementType = x }));
                    this.arrowLabel.Visibility = Visibility.Visible;
                    break;
                case "EdgeViewModel":
                    this.userChange = false;
                    this.ElementType1.ItemsSource = new List<ConstraintsValues>(this.info.GetEdgeTypes().Select(x => new ConstraintsValues { ElementType = x }));
                    this.objType = "EdgeViewModel";

                    this.ElementType2.IsHitTestVisible = false;
                    this.userChange = false;
                    this.ElementType2.Text = string.Empty;
                    this.userChange = true;
                    this.arrowLabel.Visibility = Visibility.Hidden;

                    // TODO actually list with node types should be here so user could create an edge constraint from one node to another
                    break;
            }

            this.userChange = true;
        }

        private void ElementType1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.errorLabel.Visibility = Visibility.Hidden;

            if (this.userChange)
            {
                this.elementType1 = ((ConstraintsValues)this.ElementType1.SelectedItem).ElementType;
            }
        }

        private void ElementType2_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.errorLabel.Visibility = Visibility.Hidden;
           if (this.userChange)
            {
                this.elementType2 = ((ConstraintsValues)this.ElementType2.SelectedItem).ElementType;
            }
        }

        private void CreateConstraint_Click(object sender, RoutedEventArgs e)
        {
            var item = new ConstraintItem();
            item.Elements = this.GetElements();
            item.ObjectType = this.objType;
            if (!string.IsNullOrEmpty(this.amountBox.Text))
            {
                try
                {
                    item.Amount = Convert.ToInt32(this.amountBox.Text);
                    if (item.Amount < 0)
                    {
                        throw new Exception("The number can not be less than zero.");
                    }

                    if (item.Elements.Item1 == string.Empty)
                    {
                        throw new Exception("Please, choose element type.");
                    }
                }
                catch (Exception ex)
                {
                    this.errorLabel.Visibility = Visibility.Visible;
                    this.errorLabel.Foreground = Brushes.Red;
                    this.errorLabel.Text = ex.Message;
                    return;
                }
            }

            if (this.model.Constraints.Add(item.Elements, item.Amount))
            {
                this.constraintsList.Add(item);
            }
            else
            {
                foreach (var listItem in this.constraintsList)
                {
                    if ((listItem.Elements.Item1 == item.Elements.Item1) && (listItem.Elements.Item2 == item.Elements.Item2))
                    {
                        listItem.Amount = item.Amount;
                    }
                }
            }

            this.ConstraintsPanel.ItemsSource = null;
            this.ConstraintsPanel.ItemsSource = this.constraintsList;
        }

        private void DeleteItem_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var item = button.DataContext as ConstraintItem;

            var removingElement = new ConstraintItem();
            foreach (var listItem in this.constraintsList)
            {
                if ((listItem.Elements.Item1 == item.Elements.Item1) && (listItem.Elements.Item2 == item.Elements.Item2) && (listItem.Elements.Item3 == item.Elements.Item3))
                {
                    removingElement = listItem;
                }
            }
            this.constraintsList.Remove(removingElement);
            this.ConstraintsPanel.ItemsSource = null;
            this.ConstraintsPanel.ItemsSource = this.constraintsList;
        }

        private void EditItem_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var item = button.DataContext as ConstraintItem;

            this.userChange = false;
            this.ObjType1.Text = item.ObjectType;
            this.SelectedType1 = item.Elements.Item2;
            this.SelectedType2 = item.Elements.Item3;
            this.amountBox.Text = Convert.ToString(item.Amount);
            this.userChange = true;
        }

        private ValueTuple<string, string, string> GetElements()
        {
            if (this.objType == "EdgeViewModel")
            {
                return new ValueTuple<string, string, string>(this.objType, this.elementType1, string.Empty);
            }

            if (this.ElementType2 == null)
            {
                return new ValueTuple<string, string, string>(this.objType, this.elementType1, string.Empty);
            }
            else
            {
                return new ValueTuple<string, string, string>(this.objType, this.elementType1, this.elementType2);
            }
        }
    }
}
