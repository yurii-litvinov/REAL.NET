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

namespace WpfControlsLib.ViewModel
{
    using System.ComponentModel;
    using System.Windows.Media;
    using Repo;

    /// <summary>
    /// View model for an attribute to be displayed in property editor.
    /// </summary>
    public class AttributeViewModel : INotifyPropertyChanged
    {
        private readonly IAttribute attribute;
        private string value;
        private bool hasAllowedValue = true;
        private Brush textColor = Brushes.Black;

        public AttributeViewModel(IAttribute attribute, string name, string type)
        {
            this.attribute = attribute;
            this.Name = name;
            this.Type = type;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public event System.EventHandler<AttributeEventArgs> OnAttributeChange;

        public string Name { get; }

        public string Type { get; }

        public string Value
        {
            get => this.value;

            set
            {
                this.attribute.StringValue = value;
                this.value = value;
                var args = new AttributeEventArgs
                {
                    NewValue = value
                };
                this.OnAttributeChange?.Invoke(this, args);
            }
        }

        public bool HasAllowedValue
        {
            get => this.hasAllowedValue;

            set
            {
                this.hasAllowedValue = value;
                this.TextColor = value ? Brushes.Black : Brushes.Red;
            }
        }

        public Brush TextColor
        {
            get => this.textColor;

            set
            {
                this.textColor = value;
                this.OnPropertyChanged(nameof(this.TextColor));
            }
        }

        public void OnPropertyChanged(string name = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
