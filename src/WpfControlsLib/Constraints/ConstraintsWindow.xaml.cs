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
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;

    /// <summary>
    /// Interaction logic for ConstraintsWindow.xaml.
    /// </summary>
    public partial class ConstraintsWindow : Window
    {
        private readonly RepoInfo info;
        private bool isUserChange = true;
        private Model.Model model;
        private Grid prevGrid;

        public ConstraintsWindow(WpfControlsLib.Model.Model model)
        {
            this.model = model;
            this.InitializeComponent();
            this.ConstraintsPanel.ItemsSource = this.model.Constraints.ConstraintsList;
            this.info = new RepoInfo(model.Repo, model.ModelName);
        }

        public void ObjType1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.errorLabel.Visibility = Visibility.Hidden;
            if (this.isUserChange)
            {
                switch (((ComboBoxItem)this.ObjType.SelectedItem).Content.ToString())
                {
                    case "Node":
                        this.isUserChange = false;
                        this.ElementType1.ItemsSource = new List<string>(this.info.GetNodeTypes());

                        this.ElementType2.IsHitTestVisible = true;
                        this.ElementType2.ItemsSource = new List<string>(this.info.GetNodeTypes());
                        this.arrowLabel.Visibility = Visibility.Visible;
                        this.isUserChange = true;
                        break;
                    case "Edge":
                        this.isUserChange = false;
                        this.ElementType1.ItemsSource = new List<string>(this.info.GetEdgeTypes());

                        this.ElementType2.IsHitTestVisible = false;
                        this.ElementType2.Text = string.Empty;
                        this.arrowLabel.Visibility = Visibility.Hidden;
                        this.isUserChange = true;
                        break;
                }
            }
        }

        private void ElementType1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.errorLabel.Visibility = Visibility.Hidden;

            if (this.isUserChange)
            {
                this.isUserChange = false;
                if (this.ObjType.Text == "Node")
                {
                    this.Attributes.ItemsSource
                        = new List<string>(this.info.GetNodeAttributes(this.ElementType1.SelectedItem.ToString()));
                }
                else
                {
                    this.Attributes.ItemsSource
                        = new List<string>(this.info.GetEdgeAttributes(this.ElementType1.SelectedItem.ToString()));
                }

                if (string.IsNullOrEmpty(this.ElementType2.Text))
                {
                    this.Attributes.IsHitTestVisible = true;
                }
                this.isUserChange = true;
            }
        }

        private void ElementType2_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.errorLabel.Visibility = Visibility.Hidden;
            if (this.isUserChange)
            {
                this.isUserChange = false;
                if (!string.IsNullOrEmpty(this.ElementType2.SelectedItem.ToString()))
                {
                    this.isUserChange = false;
                    this.Attributes.SelectedValue = string.Empty;
                    this.Attributes.IsHitTestVisible = false;
                    this.isUserChange = true;
                }
                else
                {
                    this.Attributes.IsHitTestVisible = true;
                }

                this.isUserChange = true;
            }
        }

        private void Attributes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.isUserChange)
            {
                this.isUserChange = false;
                var attributeName = this.Attributes.SelectedItem.ToString();
                if (!string.IsNullOrEmpty(attributeName.ToString()))
                {
                    this.valueLabel.Text = "Value:";
                }

                if (!string.IsNullOrEmpty(attributeName))
                {
                    var kind 
                        = this.info.GetAttributeType(this.ObjType.Text, this.ElementType1.Text, attributeName);
                    switch (kind)
                    {
                        case "Boolean":
                            this.MakeBooleanView();
                            break;
                        case "String":
                            this.MakeStringView();
                            break;
                        case "Int":
                            this.MakeIntView();
                            break;

                    }
                }
                this.isUserChange = true;
            }
        }

        private void BooleanValues_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.errorLabel.Text = string.Empty;
        }

        private void CreateConstraint_Click(object sender, RoutedEventArgs e)
        {
            this.errorLabel.Text = this.GetCheckResult();
            if (!string.IsNullOrEmpty(this.errorLabel.Text))
            {
                this.errorLabel.Visibility = Visibility.Visible;
                this.errorLabel.Foreground = Brushes.Red;
                return;
            }

            var item = new ConstraintItem()
            {
                ElementTypes = (this.ElementType1.Text, this.ElementType2.Text)
            };
            if (string.IsNullOrEmpty(item.ElementTypes.Item2))
            {
                item.ObjectType = this.ObjType.Text;
            }
            else
            {
                item.ObjectType = "Edge";
            }

            var constraint = new Constraints.ConstraintElement
            {
                ObjectType = item.ObjectType,
                ElementType1 = item.ElementTypes.Item1,
                ElementType2 = item.ElementTypes.Item2,
            };
            if (string.IsNullOrEmpty(this.Attributes.Text))
            {
                item.AttributeTypeFlag = ConstraintItem.AttributeTypeEnum.Int;
                item.Value =(Convert.ToInt32(this.fromBox.Text), Convert.ToInt32(this.toBox.Text));
                if (this.model.Constraints.AddAmountConstraint(constraint, item.Value))
                {
                    this.model.Constraints.ConstraintsList.Add(item);
                }
                else
                {
                    foreach (var listItem in this.model.Constraints.ConstraintsList)
                    {
                        if ((listItem.ElementTypes.Item1 == item.ElementTypes.Item1)
                                && (listItem.ElementTypes.Item2 == item.ElementTypes.Item2))
                        {
                            listItem.Value = item.Value;
                        }
                    }
                }
            }
            else
            {
                item.AttributeName = this.Attributes.Text;
                constraint.AttributeName = item.AttributeName;
                if (!string.IsNullOrEmpty(this.Values.Text))
                {
                    item.AttributeTypeFlag = ConstraintItem.AttributeTypeEnum.Boolean;
                    item.Value = Convert.ToBoolean(this.Values.Text);
                }
                else if (!string.IsNullOrEmpty(this.stringBox.Text))
                {
                    item.AttributeTypeFlag = ConstraintItem.AttributeTypeEnum.String;
                    item.Value = this.stringBox.Text;
                }
                else
                {
                    item.AttributeTypeFlag = ConstraintItem.AttributeTypeEnum.Int;
                    item.Value = (Convert.ToInt32(this.fromBox.Text), Convert.ToInt32(this.toBox.Text));
                }

                if (this.model.Constraints.AddAttributeConstraint(constraint, item.Value))
                {
                    this.model.Constraints.ConstraintsList.Add(item);
                }
                else
                {
                    foreach (var listItem in this.model.Constraints.ConstraintsList)
                    {
                        if ((listItem.ElementTypes.Item1 == item.ElementTypes.Item1)
                            && (listItem.ElementTypes.Item2 == item.ElementTypes.Item2)
                            && (listItem.AttributeName == item.AttributeName))
                        {
                            listItem.Value = item.Value;
                        }
                    }
                }
            }

            this.RestartForm();
        }

        private void DeleteItem_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var item = button.DataContext as ConstraintItem;

            var removingElement = new ConstraintItem();
            var constraint = new Constraints.ConstraintElement
            {
                ObjectType = item.ObjectType,
                ElementType1 = item.ElementTypes.Item1,
                ElementType2 = item.ElementTypes.Item2,
                AttributeName = item.AttributeName
            };
            if (string.IsNullOrEmpty(item.AttributeName))
            {
                this.model.Constraints.DeleteAmountConstraint(constraint);
            }
            else
            {
                this.model.Constraints.DeleteAttributeConstraint(constraint);
            }

            foreach (var listItem in this.model.Constraints.ConstraintsList)
            {
                if ((listItem.ObjectType == item.ObjectType) && (listItem.ElementTypes.Item1 == item.ElementTypes.Item1)
                    && (listItem.ElementTypes.Item2 == item.ElementTypes.Item2))
                {
                    removingElement = listItem;
                }
            }

            this.ConstraintsPanel.ItemsSource = null;
            this.ConstraintsPanel.ItemsSource = this.model.Constraints.ConstraintsList;
        }

        private void EditItem_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (this.prevGrid != null)
            {
                this.prevGrid.Background = new SolidColorBrush(Colors.GhostWhite);
            }

            this.prevGrid = button.Parent as Grid;
            this.prevGrid.Background = new SolidColorBrush(Colors.LightGray);

            var item = button.DataContext as ConstraintItem;

            if (string.IsNullOrEmpty(item.ElementTypes.Item2))
            {
                this.ObjType.Text = item.ObjectType;
            }
            else
            {
                this.ObjType.Text = "Edge";
            }

            this.isUserChange = false;
            this.ElementType1.Text = item.ElementTypes.Item1;
            this.ElementType2.Text = item.ElementTypes.Item2;
            this.Attributes.Text = item.AttributeName;
            switch (item.AttributeTypeFlag)
            {
                case ConstraintItem.AttributeTypeEnum.Boolean:
                    this.MakeBooleanView();
                    this.Values.Text = item.Value.ToString().ToLower();
                    break;
                case ConstraintItem.AttributeTypeEnum.Int:
                    this.MakeIntView();
                    this.fromBox.Text = ((ValueTuple<int, int>)item.Value).Item1.ToString();
                    this.toBox.Text = ((ValueTuple<int, int>)item.Value).Item2.ToString();
                    break;
                case ConstraintItem.AttributeTypeEnum.String:
                    this.MakeStringView();
                    this.stringBox.Text = item.Value.ToString();
                    break;
            }

            this.isUserChange = true;
            this.createConstraint.Content = "Save";
        }

        private void RestartForm()
        {
            this.isUserChange = false;
            this.MakeIntView();
            this.Attributes.SelectedValue = string.Empty;
            this.toBox.Text = string.Empty;
            this.fromBox.Text = string.Empty;
            this.ElementType1.SelectedValue = string.Empty;
            this.ElementType2.SelectedValue = string.Empty;
            this.ObjType.SelectedValue = string.Empty;
            this.ConstraintsPanel.ItemsSource = null;
            this.ConstraintsPanel.ItemsSource = this.model.Constraints.ConstraintsList;
            this.createConstraint.Content = "Create";
            this.isUserChange = true;
        }

        private void HideInt()
        {
            this.fromBox.Visibility = Visibility.Hidden;
            this.fromBox.Text = string.Empty;
            this.toBox.Visibility = Visibility.Hidden;
            this.toBox.Text = string.Empty;
            this.rangeLabel.Visibility = Visibility.Hidden;
        }

        private void HideString()
        {
            this.stringBox.Visibility = Visibility.Hidden;
            this.stringBox.Text = string.Empty;
        }

        private void HideBoolean()
        {
            this.Values.Visibility = Visibility.Hidden;
            this.Values.SelectedItem = string.Empty;
            this.Values.Text = string.Empty;
        }

        private void MakeIntView()
        {
            this.HideString();
            this.HideBoolean();

            this.valueLabel.Text = "Value or amount range:";
            this.fromBox.Visibility = Visibility.Visible;
            this.toBox.Visibility = Visibility.Visible;
            this.rangeLabel.Visibility = Visibility.Visible;
        }

        private void MakeBooleanView()
        {
            this.HideInt();
            this.HideString();

            this.Values.Visibility = Visibility.Visible;
            this.valueLabel.Text = "Value";
        }

        private void MakeStringView()
        {
            this.HideInt();
            this.HideBoolean();

            this.valueLabel.Text = "RegExp:";
            this.stringBox.Visibility = Visibility.Visible;
        }

        private string GetCheckResult()
        {
            if (this.fromBox.Visibility == Visibility.Visible)
            {
                    var valueFrom = Convert.ToInt32(this.fromBox.Text);
                    var valueTo = Convert.ToInt32(this.toBox.Text);

                    if ((valueFrom < 0) || (valueTo < 0))
                    {
                        return "The number can not be less than zero.";
                    }

                    if (valueFrom > valueTo)
                    {
                        return "Please, swap values in value boxes (the left should not be more than the right one)";
                    }

                    if (string.IsNullOrEmpty(this.ElementType1.Text))
                    {
                        return "Please, choose element type.";
                    }

                    if (string.IsNullOrEmpty(this.fromBox.Text) || string.IsNullOrEmpty(this.toBox.Text))
                    {
                        return "Please, fill range boxes.";
                    }
            }

            return string.Empty;
        }
    }
}
