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

namespace EditorPluginInterfaces.Undo_redo_functions
{
    using EditorPluginInterfaces.Toolbar;

    /// <summary>
    /// Interface for stack of undo/redo commands.
    /// </summary>
    public interface IStack
    {
        /// <summary>
        /// Gets a value indicating whether undo stack is available.
        /// </summary>
        bool IsUndoAvailable { get; }

        /// <summary>
        /// Gets a value indicating whether redo stack is available.
        /// </summary>
        bool IsRedoAvailable { get; }

        /// <summary>
        /// Handles a command.
        /// </summary>
        /// <param name="command">Command for handling.</param>
        void HandleCommand(ICommand command);

        /// <summary>
        /// Undo a command.
        /// </summary>
        void Undo();

        /// <summary>
        /// Redo a command.
        /// </summary>
        void Redo();
    }
}