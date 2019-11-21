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

namespace EditorPluginInterfaces
{
    using System;

    /// <summary>
    /// Interface of a model in editor that allows to manipulate repository maintaining consistency with editor.
    /// </summary>
    public interface IModel
    {
        /// <summary>
        /// Model name.
        /// </summary>
        string ModelName { get; set; }

        /// <summary>
        /// New vertex was added into a model.
        /// </summary>
        event EventHandler<VertexEventArgs> NewVertexAdded;

        /// <summary>
        /// New edge was added into a model.
        /// </summary>
        event EventHandler<EdgeEventArgs> NewEdgeAdded;

        /// <summary>
        /// Repository. Can be used for queries, but shall not be used for direct manipulation of a model.
        /// </summary>
        Repo.IRepo Repo { get; }

        /// <summary>
        /// Creates a new node in a model.
        /// </summary>
        /// <param name="element">Metatype of created node.</param>
        void CreateNode(Repo.IElement element);

        /// <summary>
        /// Creates a new edge in a model.
        /// </summary>
        /// <param name="edge">Metatype of an edge.</param>
        /// <param name="source">Node that will be a source of an edge.</param>
        /// <param name="destination">Node that will be destination of an edge.</param>
        void CreateEdge(Repo.IEdge edge, Repo.IElement source, Repo.IElement destination);

        /// <summary>
        /// Removes an adge or node from a model.
        /// </summary>
        /// <param name="element">Element to remove from model.</param>
        void RemoveElement(Repo.IElement element);
    }
}
