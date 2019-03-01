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

using EditorPluginInterfaces;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace WpfControlsLib.Controls.Palette
{
    /// <summary>
    /// Palette with elements of a visual language.
    /// </summary>
    public partial class Palette : UserControl, IPalette
    {
        private bool isInDrag;
        private readonly PaletteViewModel paletteViewModel;
        private Point dragOriginPoint;

        /// <summary>
        /// Creates a new instance of the <see cref="Palette"/> class.
        /// </summary>
        public Palette()
        {
            this.InitializeComponent();
            this.paletteViewModel = new PaletteViewModel();
            this.DataContext = this.paletteViewModel;
        }

        /// <summary>
        /// Gets currently selected element, or null if there is none.
        /// </summary>
        public Repo.IElement SelectedElement => this.paletteViewModel.SelectedElement;

        /// <summary>
        /// Sets a model from which palette will be populated.
        /// </summary>
        /// <param name="model">A model with repository from which palette will take elements.</param>
        public void SetModel(WpfControlsLib.Model.Model model)
            => this.paletteViewModel.SetModel(model);

        /// <summary>
        /// Initializes palette with metamodel with given name. Note that a model shall be set before calling this.
        /// </summary>
        /// <param name="metamodelName">Name of the metamodel from which we shall populate the palette.</param>
        public void InitPalette(string metamodelName)
            => this.paletteViewModel.InitPalette(metamodelName);

        /// <summary>
        /// Clears selected element on a palette.
        /// </summary>
        public void ClearSelection()
            => this.paletteViewModel.SelectedElement = null;

        private void OnElementChecked(object sender, RoutedEventArgs e)
        {
            var selectedElement = ((sender as ToggleButton)?.DataContext as PaletteElement)?.Element;
            this.paletteViewModel.SelectedElement = selectedElement;
        }

        private void OnElementUnchecked(object sender, RoutedEventArgs e)
        {
            this.paletteViewModel.SelectedElement = null;
        }

        private void OnElementMouseMove(object sender, MouseEventArgs e)
        {
            bool IsMovedEnough()
            {
                if (this.dragOriginPoint == new Point())
                {
                    return false;
                }

                var currentMousePos = e.GetPosition(sender as ToggleButton);
                var xDistance = this.dragOriginPoint.X - currentMousePos.X;
                var yDistance = this.dragOriginPoint.Y - currentMousePos.Y;
                return xDistance * xDistance + yDistance * yDistance > 100;
            }

            if (this.isInDrag)
            {
                return;
            }

            if (!(sender is ToggleButton button) || e.LeftButton != MouseButtonState.Pressed)
            {
                return;
            }

            if (!IsMovedEnough())
            {
                return;
            }

            var paletteElement = button.DataContext as PaletteElement;

            var draggedElement = paletteElement?.Element;
            if (draggedElement == null)
            {
                return;
            }

            paletteElement.IsDragged = true;

            // Flagging that we are inside drag-and-drop operation. Despite this operation is blocking, it has its own
            // event loop inside, so we can get more events from a button.
            this.isInDrag = true;

            // Initiating blocking drag-and-drop operation.
            DragAndDropHelper.DoDragAndDrop(button, draggedElement, "REAL.NET palette element", paletteElement.Image);

            this.isInDrag = false;

            paletteElement.IsDragged = false;
        }

        private void OnPreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            this.dragOriginPoint = e.GetPosition(sender as ToggleButton);
        }

        private void OnPreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            this.dragOriginPoint = new Point();
        }
    }
}
