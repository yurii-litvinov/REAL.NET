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
    using System;
    using EditorPluginInterfaces;
    using GraphX.Controls;
    using WpfControlsLib.ViewModel;

    /// <summary>
    /// Controller in MVC architecture. Handles commands and manages undo-redo state. Model modification is only
    /// possible via commands and all commands are executed only inside controller.
    /// </summary>
    public class Controller
    {
        private readonly IModel model;
        private readonly UndoRedo.UndoRedoStack undoStack = new UndoRedo.UndoRedoStack();

        public Controller(IModel model)
        {
            this.model = model;
        }

        /// <summary>
        /// Event is raised when undo operation becomes available or not available.
        /// </summary>
        public event EventHandler<UndoRedoAvailabilityChangedArgs> UndoAvailabilityChanged;

        /// <summary>
        /// Event is raised when redo operation becomes available or not available.
        /// </summary>
        public event EventHandler<UndoRedoAvailabilityChangedArgs> RedoAvailabilityChanged;

        public void NewNode(Repo.IElement node)
        {
            this.model.CreateNode(node);
        }

        public void NewEdge(Repo.IElement edge, VertexControl prevVer, VertexControl ctrlVer)
        {
            this.model.CreateEdge(
                edge as Repo.IEdge,
                (prevVer?.Vertex as NodeViewModel)?.Node,
                (ctrlVer?.Vertex as NodeViewModel)?.Node);
        }

        public void RemoveElement(Repo.IElement element) => this.model.RemoveElement(element);

        public void Execute(ICommand command)
        {
            this.DoAndCheckUndoRedoStatus(() =>
            {
                this.undoStack.AddCommand(command);
                command.Execute();
            }
            );
        }

        public void Undo() => this.DoAndCheckUndoRedoStatus(this.undoStack.Undo);

        public void Redo() => this.DoAndCheckUndoRedoStatus(this.undoStack.Redo);

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