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
    internal class CreateNodeCommand : ICommand
    {
        private ISceneModel model;
        private Repo.IElement type;
        private Repo.INode node;
        private Repo.VisualPoint position;

        /// <summary>
        /// Initializes a new instance of a <see cref="CreateNodeCommand"/> class.
        /// </summary>
        /// <param name="model">Model to which a new node shall be added.</param>
        /// <param name="type">Type of a new node (element from metamodel).</param>
        public CreateNodeCommand(ISceneModel model, Repo.IElement type, Repo.VisualPoint position)
        {
            this.model = model;
            this.type = type;
            this.position = position;
        }

        bool ICommand.CanBeUndone => true;

        void ICommand.Execute()
        {
            if (node == null)
            {
                this.node = this.model.CreateNode(this.type as Repo.INode, this.position);
            }
            else
            {
                this.model.RestoreElement(node);
            }
        }

        // TODO: This shall also trigger the deletion of connected edges.
        void ICommand.Undo() => this.model.RemoveElement(this.node);
    }
}
