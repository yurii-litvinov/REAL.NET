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
    using Repo;
    using System;
    using System.ComponentModel;
    using System.Windows.Controls;
    using WpfControlsLib.Controls.Palette;
    using WpfControlsLib.Controls.Scene;

    /// <summary>
    /// Interaction logic for ConstraintsPanel.xaml
    /// </summary>
    public partial class ConstraintsScene : UserControl
    {
        private WpfControlsLib.Model.Model model;
        private readonly WpfControlsLib.Controller.Controller controller;
        private IElementProvider temporaryElementProvider;
        private WpfControlsLib.Controls.Palette.Palette palette;

        public ConstraintsScene(WpfControlsLib.Model.Model model, WpfControlsLib.Controls.Palette.Palette palette)
        {
            this.InitializeComponent();

            this.model = model;
            this.palette = palette;
            this.controller = new WpfControlsLib.Controller.Controller();
            this.temporaryElementProvider = new PaletteAdapter(this.palette);

            this.scene.Init(this.model, this.controller, this.temporaryElementProvider);
        }

        private class PaletteAdapter : IElementProvider
        {
            private readonly Palette palette;

            internal PaletteAdapter(Palette palette)
            {
                this.palette = palette;
            }

            public IElement Element => this.palette.SelectedElement;
        }
    }
}