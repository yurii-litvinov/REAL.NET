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
    using System;

    /// <summary>
    /// Removes node from a model. Does not trigger deletion of connected edges.
    /// </summary>
    class RemoveNodeCommand : ICommand
    {
        private IModel model;
        private Repo.IElement node;

        /// <summary>
        /// Initializes a new instance of <see cref="RemoveNodeCommand"/> class.
        /// </summary>
        /// <param name="model">Model from which a node shall be removed.</param>
        /// <param name="node">Node that shall be removed.</param>
        public RemoveNodeCommand(IModel model, Repo.IElement node)
        {
            this.model = model;
            this.node = node;
        }

        // Can not be undone because of naive implementation involving reading the same node will regenerate node 
        // identity. Some support from repository is needed, but not implemented yet.
        bool ICommand.CanBeUndone => false;

        void ICommand.Execute() => this.model.RemoveElement(this.node);

        void ICommand.Undo() => throw new NotImplementedException();
    }
}
