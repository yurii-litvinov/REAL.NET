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

namespace EditorPluginInterfaces.UndoRedo
{
    using EditorPluginInterfaces.Toolbar;
    using System;

    /// <summary>
    /// Interface for stack of undo/redo commands.
    /// </summary>
    public interface IUndoRedoStack
    {
        /// <summary>
        /// Event is raised when undo stack is updated.
        /// </summary>
        event EventHandler<StackChangedArgs> UndoUpdated;

        /// <summary>
        /// Event is raised when redo stack is updated.
        /// </summary>
        event EventHandler<StackChangedArgs> RedoUpdated;

        /// <summary>
        /// Gets a value indicating whether undo stack is available.
        /// </summary>
        bool IsUndoAvailable { get; }

        /// <summary>
        /// Gets a value indicating whether redo stack is available.
        /// </summary>
        bool IsRedoAvailable { get; }

        /// <summary>
        /// Register a command.
        /// </summary>
        /// <param name="command">Command for register.</param>
        void AddCommand(ICommand command);

        /// <summary>
        /// Undo a command.
        /// </summary>
        void Undo();

        /// <summary>
        /// Redo a command.
        /// </summary>
        void Redo();

        /// <summary>
        /// Resets redo stack
        /// </summary>
        void ResetRedo();
    }
}