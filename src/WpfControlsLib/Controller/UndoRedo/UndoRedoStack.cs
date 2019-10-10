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

namespace WpfControlsLib.Controller.UndoRedo
{
    using System.Collections.Generic;
    using EditorPluginInterfaces;

    /// <summary>
    /// Class for undo/redo commands.
    /// </summary>
    internal class UndoRedoStack
    {
        /// <summary>
        /// Stack with undo commands.
        /// </summary>
        private readonly Stack<ICommand> undoStack = new Stack<ICommand>();

        /// <summary>
        /// Stack with redo commands.
        /// </summary>
        private readonly Stack<ICommand> redoStack = new Stack<ICommand>();

        /// <summary>
        /// Gets a value indicating whether undo stack is available.
        /// </summary>
        public bool IsUndoAvailable => this.undoStack.Count != 0;

        /// <summary>
        /// Gets a value indicating whether redo stack is available.
        /// </summary>
        public bool IsRedoAvailable => this.redoStack.Count != 0;

        /// <summary>
        /// Handles a command.
        /// </summary>
        /// <param name="command">Command for handling.</param>
        public void AddCommand(ICommand command)
        {
            if (command.CanBeUndone)
            {
                this.undoStack.Push(command);
                this.redoStack.Clear();
            }
        }

        /// <summary>
        /// Redo a command.
        /// </summary>
        public void Redo()
        {
            if (this.IsRedoAvailable)
            {
                var command = this.redoStack.Pop();
                command.Execute();
                this.undoStack.Push(command);
            }
        }

        /// <summary>
        /// Undo a command.
        /// </summary>
        public void Undo()
        {
            if (this.IsUndoAvailable)
            {
                var command = this.undoStack.Pop();
                command.Undo();
                this.redoStack.Push(command);
            }
        }

        /// <summary>
        /// Resets redo stack.
        /// </summary>
        public void ResetRedo() => this.redoStack.Clear();

        /// <summary>
        /// Resets undo and redo stacks, losing all undo/redo history.
        /// </summary>
        public void Clear()
        {
            this.undoStack.Clear();
            this.redoStack.Clear();
        }
    }
}