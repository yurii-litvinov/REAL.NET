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
    /// Creates a new edge between two nodes.
    /// </summary>
    internal class CreateEdgeCommand : ICommand
    {
        private IModel model;
        private Repo.IElement type;
        private Repo.IElement source;
        private Repo.IElement destination;

        /// <summary>
        /// Creates a new instance of <see cref="CreateEdgeCommand"/> class.
        /// </summary>
        /// <param name="model">Model to which a new edge shall be added.</param>
        /// <param name="type">Type of a new edge (element from metamodel).</param>
        /// <param name="source">Source node for an edge.</param>
        /// <param name="destination">Destination node for an edge.</param>
        public CreateEdgeCommand(IModel model, Repo.IElement type, Repo.IElement source, Repo.IElement destination)
        {
            this.model = model;
            this.type = type;
            this.source = source;
            this.destination = destination;
        }

        bool ICommand.CanBeUndone => true;

        void ICommand.Execute() =>
            this.model.CreateEdge(
                this.type as Repo.IEdge,
                this.source,
                this.destination
                );

        void ICommand.Undo() => this.model.RemoveElement(this.type);
    }
}
