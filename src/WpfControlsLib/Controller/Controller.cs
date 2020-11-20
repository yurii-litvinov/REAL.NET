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

namespace WpfControlsLib.Controller
{
    using EditorPluginInterfaces;
    using System;

    /// <summary>
    /// Controller in MVC architecture. Handles commands and manages undo-redo state. Model modification is only
    /// possible via commands and all commands are executed only inside controller.
    /// </summary>
    public class Controller
    {
        private readonly UndoRedo.UndoRedoStack undoStack = new UndoRedo.UndoRedoStack();

        /// <summary>
        /// Event is raised when undo operation becomes available or not available.
        /// </summary>
        public event EventHandler<UndoRedoAvailabilityChangedArgs> UndoAvailabilityChanged;

        /// <summary>
        /// Event is raised when redo operation becomes available or not available.
        /// </summary>
        public event EventHandler<UndoRedoAvailabilityChangedArgs> RedoAvailabilityChanged;

        /// <summary>
        /// Executes given command and adds it into undo stack. May raise events related to undo-redo availability
        /// after the command is executed. Clears redo stack.
        /// </summary>
        /// <param name="command">Command to execute.</param>
        public void Execute(ICommand command)
        {
            this.DoAndCheckUndoRedoStatus(() =>
            {
                this.undoStack.AddCommand(command);
                command.Execute();
            }
            );
        }

        /// <summary>
        /// Undoes last command, modifies undo-redo stack and raises related events after the undo action will 
        /// be executed.
        /// </summary>
        public void Undo() => this.DoAndCheckUndoRedoStatus(this.undoStack.Undo);

        /// <summary>
        /// Redoes last undone command, modifies undo-redo stack and raises related events after the command will 
        /// be executed.
        /// </summary>
        public void Redo() => this.DoAndCheckUndoRedoStatus(this.undoStack.Redo);

        /// <summary>
        /// Clears undo/redo history completely. Useful, for example, when new file is loaded.
        /// </summary>
        public void ClearHistory()
        {
            this.undoStack.Clear();
        }

        private void DoAndCheckUndoRedoStatus(Action action)
        {
            var oldUndoAvailable = this.undoStack.IsUndoAvailable;
            var oldRedoAvailable = this.undoStack.IsRedoAvailable;

            action();

            if (this.undoStack.IsUndoAvailable != oldUndoAvailable)
            {
                UndoAvailabilityChanged?.Invoke(
                    this,
                    new UndoRedoAvailabilityChangedArgs(this.undoStack.IsUndoAvailable));
            }

            if (this.undoStack.IsRedoAvailable != oldRedoAvailable)
            {
                RedoAvailabilityChanged?.Invoke(
                    this,
                    new UndoRedoAvailabilityChangedArgs(this.undoStack.IsRedoAvailable));
            }
        }
    }
}