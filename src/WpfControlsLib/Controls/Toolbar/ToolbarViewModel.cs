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
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using EditorPluginInterfaces.Toolbar;

    /// <summary>
    /// ViewModel for toolbar control, allows to register some commands and show them as buttons. Can be used from
    /// plugins.
    /// </summary>
    public class ToolbarViewModel : IToolbar
    {
        /// <summary>
        /// Gets a list of buttons that should be presented on toolbar
        /// </summary>
        public IList<IButton> Buttons => this.ButtonsToShow as IList<IButton>;

        public ObservableCollection<Button> ButtonsToShow => new ObservableCollection<Button>();

        public void AddButton(IButton button) => this.ButtonsToShow.Add(this.WrapIButton(button));

        public void AddMenu(IMenu menu) => throw new NotImplementedException();

        private Button WrapIButton(IButton button) => new Button(button.Command, button.Description, button.Image);
    }
}
