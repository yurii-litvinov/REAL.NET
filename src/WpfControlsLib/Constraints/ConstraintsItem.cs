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
 
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace WpfControlsLib.Constraints
{
    /// <summary>
    /// Grid-like element for constraints panel
    /// </summary>
    public class ConstraintsItem : Grid
    {
        public ConstraintsItem()
        {
        }

        public string ElementType { get; set; }

        public string ObjectType { get; set; }

        public int Ammount { get; set; }

        public void Initialize()
        {
            var gridCol1 = new ColumnDefinition();
            gridCol1.Width = new GridLength(8, GridUnitType.Star);
            var gridCol2 = new ColumnDefinition();
            this.ColumnDefinitions.Add(gridCol1);
            this.ColumnDefinitions.Add(gridCol2);

            var text = this.Text();
            SetColumn(text, 0);
            this.Children.Add(text);
            var buttons = this.ButtonGrid();
            SetColumn(buttons, 1);
            this.Children.Add(buttons);

            this.Margin = new Thickness(2);
            this.Background = Brushes.Beige;
        }

        private DockPanel ButtonGrid()
        {
            var buttons = new DockPanel();
            buttons.Width = 20;
            buttons.Height = 70;
            var buttonPanel = new DockPanel();
            var deleteButton = new Button();
            deleteButton.Content = "[x]";
            DockPanel.SetDock(deleteButton, Dock.Top);
            var editButton = new Button();
            editButton.Content = "[/]";
            DockPanel.SetDock(editButton, Dock.Bottom);
            buttonPanel.Children.Add(deleteButton);
            buttonPanel.Children.Add(editButton);
            buttonPanel.Children.Add(new Grid());
            buttons.Children.Add(buttonPanel);
            return buttons;
        }

        private TextBlock Text()
        {
            var text = new TextBlock();
            var sb = new System.Text.StringBuilder(string.Empty);
            text.Width = 500;
            text.Height = 70;
            sb.Append(this.ObjectType + "; " + this.ElementType + "; ");

            if (this.Ammount > 0)
            {
                sb.Append(" ");
                sb.Append(this.Ammount);
                sb.Append(";");
            }

            text.Text = sb.ToString();
            return text;
        }
    }
}
