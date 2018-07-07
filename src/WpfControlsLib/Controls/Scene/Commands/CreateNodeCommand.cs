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

namespace WpfControlsLib.Controls.Scene.Commands
{
    using EditorPluginInterfaces;

    /// <summary>
    /// Creates a new node somewhere on a scene.
    /// </summary>
    class CreateNodeCommand : ICommand
    {
        private IModel model;
        private Repo.IElement type;

        /// <summary>
        /// Initializes a new instance of a <see cref="CreateNodeCommand"/> class.
        /// </summary>
        /// <param name="model">Model to which a new node shall be added.</param>
        /// <param name="type">Type of a new node (element from metamodel).</param>
        public CreateNodeCommand(IModel model, Repo.IElement type)
        {
            this.model = model;
            this.type = type;
        }

        bool ICommand.CanBeUndone => true;

        void ICommand.Execute() => this.model.CreateNode(this.type as Repo.IEdge);

        // TODO: This shall also trigger the deletion of connected edges.
        void ICommand.Undo() => this.model.RemoveElement(this.type);
    }
}
