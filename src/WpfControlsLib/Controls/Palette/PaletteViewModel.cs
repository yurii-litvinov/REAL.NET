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

using System.Collections.ObjectModel;
using System.Linq;

namespace WpfControlsLib.Controls.Palette
{
    /// <summary>
    /// Contains data for palette and provides non-visual interaction logic, like selection management.
    /// </summary>
    public class PaletteViewModel
    {
        private WpfControlsLib.Model.Model model;
        private Repo.IElement selectedElement;
        private bool isInSelect;

        /// <summary>
        /// Gets an element currently selected on a palette or null if no element is selected.
        /// </summary>
        public Repo.IElement SelectedElement
        {
            get => this.selectedElement;
            set
            {
                if (this.isInSelect)
                {
                    return;
                }

                this.isInSelect = true;
                this.Select(value);
                this.selectedElement = value;
                this.isInSelect = false;
            }
        }

        /// <summary>
        /// Collection of elements on a palette, supposed to be used in data binding.
        /// </summary>
        public ObservableCollection<PaletteElement> Elements { get; } = new ObservableCollection<PaletteElement>();

        /// <summary>
        /// Initializes palette with metamodel with given name. Metamodel contents are taken from model, so make sure
        /// that it is set before calling this method.
        /// </summary>
        /// <param name="metamodelName">Name of a metamodel whose elements shall be drawn on palette.</param>
        public void InitPalette(string metamodelName)
        {
            this.Elements.Clear();
            var metamodel = this.model?.Repo.Model(metamodelName).Metamodel;
            if (metamodel == null)
            {
                return;
            }

            foreach (var type in metamodel.Elements)
            {
                if (type.IsAbstract)
                {
                    continue;
                }

                var element = new PaletteElement
                {
                    Name = type.Name,
                    Image = "pack://application:,,,/"
                            + (type.Shape == string.Empty ? "View/Pictures/vertex.png" : type.Shape),
                    Element = type
                };

                this.Elements.Add(element);
            }
        }

        /// <summary>
        /// Sets a model for a palette.
        /// </summary>
        /// <param name="model">Model (with repository) to use to populate a palette.</param>
        public void SetModel(WpfControlsLib.Model.Model model) => this.model = model;

        private void Select(Repo.IElement element)
            => this.Elements.ToList().ForEach(e => e.IsSelected = e.Element == element);
    }
}
