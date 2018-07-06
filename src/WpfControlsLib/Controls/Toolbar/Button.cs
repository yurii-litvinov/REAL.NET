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
    using EditorPluginInterfaces;
    using EditorPluginInterfaces.Toolbar;

    /// <summary>
    /// Class that implements IButton interface
    /// </summary>
    public class Button : IButton
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Button"/> class.
        /// </summary>
        /// <param name="command">Command</param>
        /// <param name="description">Description of command</param>
        /// <param name="image">Image to attach</param
        /// <param name="isEnabled">Is this button should be enabled</param>
        public Button(ICommand command, string description, string image, bool isEnabled)
        {
            this.Command = command;
            this.Description = description;
            this.Image = image;
            this.IsEnabled = isEnabled;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Button"/> class.
        /// Default : isEnabled = true
        /// </summary>
        /// <param name="command">Command</param>
        /// <param name="description">Description of command</param>
        /// <param name="image">Image to attach</param
        public Button(ICommand command, string description, string image)
            : this(command, description, image, true) { }

        /// <summary>
        /// Throws when button's visibility changed
        /// </summary>
        public event EventHandler ButtonEnabledChanged;

        /// <summary>
        /// Gets description of command
        /// </summary>
        public string Description { get; private set; }

        /// <summary>
        /// Gets attached image
        /// </summary>
        public string Image { get; private set; }

        /// <summary>
        /// Gets command attached to button
        /// </summary>
        public ICommand Command { get; private set; }

        /// <summary>
        /// Gets a value indicating whether is this button enabled
        /// </summary>
        public bool IsEnabled { get; private set; }

    /// <summary>
    /// Sets this button enabled
    /// </summary>
    public void SetEnabled()
        {
            this.IsEnabled = true;
            this.ThrowButtonEnabledChanged();
        }

        /// <summary>
        /// Sets this button disabled
        /// </summary>
        public void SetDisabled()
        {
            this.IsEnabled = false;
            this.ThrowButtonEnabledChanged();
        }

        /// <summary>
        /// Executes command attached to button
        /// </summary>
        public void DoAction() => this.Command.Execute();

        /// <summary>
        /// Throws event
        /// </summary>
        private void ThrowButtonEnabledChanged() => this.ButtonEnabledChanged?.Invoke(this, EventArgs.Empty);
    }
}
