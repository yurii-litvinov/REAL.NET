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

    /// <summary>
    /// Class that implements pattern Command
    /// </summary>
    public class Command : ICommand
    {
        /// <summary>
        /// Action wrapped in command
        /// </summary>
        private Action commandToExecute;

        /// <summary>
        /// Undo action of command
        /// </summary>
        private Action commandToUndo;

        /// <summary>
        /// Initializes a new instance of the <see cref="Command"/> class.
        /// </summary>
        /// <param name="commandToExecute">Command to execute</param>
        public Command(Action commandToExecute)
            : this(commandToExecute, null, false) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="Command"/> class.
        /// </summary>
        /// <param name="commandToExecute">Command to execute></param>
        /// <param name="commandToUndo">Command to execute to undo this command</param>
        public Command(Action commandToExecute, Action commandToUndo)
            : this(commandToExecute, commandToUndo, true) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="Command"/> class.
        /// </summary>
        /// <param name="commandToExecute">Command to execute></param>
        /// <param name="commandToUndo">Command to execute to undo this command</param>
        /// <param name="canBeUndone">Can this command be undone</param>
        private Command(Action commandToExecute, Action commandToUndo, bool canBeUndone)
        {
            this.CanBeUndone = canBeUndone;
            this.commandToExecute = commandToExecute;
            this.commandToUndo = commandToUndo;
        }

        /// <summary>
        /// Gets a value indicating whether can this command be undone
        /// </summary>
        public bool CanBeUndone { get; }

        /// <summary>
        /// Executes this command
        /// </summary>
        public void Execute() => this.commandToExecute?.Invoke();

        /// <summary>
        /// Undo this command
        /// </summary>
        public void Undo() => this.commandToUndo?.Invoke();
    }
}
