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
        /// <param name="image">Image to attach</param>
        public Button(ICommand command, string description, string image)
        {
            this.Command = command;
            this.WrappedCommand = new CommandXAMLAdapter(this.Command);
            this.Description = description;
            this.Image = image;
        }

        /// <summary>
        /// Gets description of command
        /// </summary>
        public string Description { get; }

        /// <summary>
        /// Gets attached image
        /// </summary>
        public string Image { get; }

        /// <summary>
        /// Gets command attached to button
        /// </summary>
        public ICommand Command { get; }

        /// <summary>
        /// Gets System.Windows.Input.Command which is necessary for correct working with XAML
        /// NOTICE: Added for compatibility with XAML
        /// </summary>
        public System.Windows.Input.ICommand WrappedCommand { get; }

        /// <summary>
        /// Executes command attached to button
        /// </summary>
        public void DoAction() => this.Command.Execute();
    }
}
