/* Copyright 2018 REAL.NET group
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

namespace WpfControlsLib.Controller
{
    using EditorPluginInterfaces;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Command that is composed of elementary commands that shall be executed and undone as one command.
    /// </summary>
    class CompositeCommand : ICommand
    {
        private IList<ICommand> commands = new List<ICommand>();

        /// <summary>
        /// Adds a new command to a composite command.
        /// </summary>
        /// <param name="command">Command to be added. Can also be composite.</param>
        public void Add(ICommand command)
        {
            commands.Add(command);
        }

        bool ICommand.CanBeUndone => commands.All(c => c.CanBeUndone);

        void ICommand.Execute() => commands.ToList().ForEach(c => c.Execute());

        void ICommand.Undo() => commands.Reverse().ToList().ForEach(c => c.Undo());
    }
}
