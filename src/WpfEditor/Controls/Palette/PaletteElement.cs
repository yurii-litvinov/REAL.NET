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

namespace WpfEditor.Controls.Palette
{
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    
    /// <summary>
    /// Element of a palette. Used for data binding in main palette view.
    /// </summary>
    public class PaletteElement : INotifyPropertyChanged
    {
        private bool isSelected;
        private bool isDragged;

        /// <summary>
        /// Name of an element.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Icon of an element to be shown in palette.
        /// </summary>
        public string Image { get; set; }

        /// <summary>
        /// An element itself, from the repository.
        /// </summary>
        public Repo.IElement Element { get; set; }

        /// <summary>
        /// Gets or sets element selection status.
        /// </summary>
        public bool IsSelected
        {
            get => this.isSelected;
            set
            {
                if (this.isSelected == value) return;
                this.isSelected = value;
                this.OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets element dragging status. True when element is in progress of drag-and-drop operation.
        /// </summary>
        public bool IsDragged
        {
            get => this.isDragged;
            set
            {
                if (this.isDragged == value) return;
                this.isDragged = value;
                this.OnPropertyChanged();
            }
        }

        /// <summary>
        /// Event to implement <see cref="INotifyPropertyChanged"/> interface.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
            => this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
