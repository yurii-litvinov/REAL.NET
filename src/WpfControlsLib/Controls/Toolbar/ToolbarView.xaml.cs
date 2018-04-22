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

namespace WpfControlsLib.Controls.Toolbar
{
    using System;
    using System.Windows.Controls;

    /// <summary>
    /// Interaction logic for ToolbarView.xaml
    /// </summary>
    public partial class ToolbarView : UserControl
    {
        private ToolbarViewModel viewModel;

        public ToolbarView()
        {
            this.InitializeComponent();
            this.viewModel = new ToolbarViewModel();
            this.DataContext = this.viewModel;
        }

        private void RefreshButtonsOnToolbar(object sender, EventArgs args)
        {
            var toolbar = sender as ToolbarViewModel;
            if (toolbar == null)
            {
                // TODO: replace exception to writing to log
                throw new ArgumentException("sender is incorrect type, expected : ToolbarViewModel");
            }
            // TODO : adding buttons on toolbar
        }
    }
}