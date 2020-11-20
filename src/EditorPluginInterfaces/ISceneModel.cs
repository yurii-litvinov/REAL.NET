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

using Repo;
using Repo.Visual;

namespace EditorPluginInterfaces
{
    using System;

    /// <summary>
    /// Interface of a model in editor that allows to manipulate repository maintaining consistency with editor.
    /// </summary>
    public interface ISceneModel
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
        /// Element was deleted from model.
        /// </summary>
        event EventHandler<ElementEventArgs> ElementRemoved;

        /// <summary>
        /// Node's scene visual representation changed
        /// </summary>
        event EventHandler<VertexEventArgs> NodeVisualChanged;

        /// <summary>
        /// Edge's scene visual representation changed
        /// </summary>
        event EventHandler<EdgeEventArgs> EdgeVisualChanged; 

        /// <summary>
        /// Element that should be checked from model.
        /// </summary>
        event EventHandler<ElementEventArgs> ElementCheck;

        /// <summary>
        /// Repository. Can be used for queries, but shall not be used for direct manipulation of a model.
        /// </summary>
        Repo.IRepo Repo { get; }

        /// <summary>
        /// Creates a new node in a model.
        /// </summary>
        /// <param name="nodeType">Metatype of created node.</param>
        /// <param name="position">Position of node on scene.</param>
        Repo.INode CreateNode(Repo.IElement nodeType, Repo.VisualPoint position);

        /// <summary>
        /// Creates a new edge in a model.
        /// </summary>
        /// <param name="edgeType">Metatype of an edge.</param>
        /// <param name="source">Node that will be a source of an edge.</param>
        /// <param name="destination">Node that will be destination of an edge.</param>
        Repo.IEdge CreateEdge(Repo.IEdge edgeType, Repo.IElement source, Repo.IElement destination);

        /// <summary>
        /// Restores element after this element being deleted.
        /// </summary>
        /// <param name="element">Element to restore.</param>
        void RestoreElement(Repo.IElement element);

        /// <summary>
        /// Removes an edge or node from a model.
        /// </summary>
        /// <param name="element">Element to remove from model.</param>
        void RemoveElement(Repo.IElement element);

        /// <summary>
        /// Notifies that visual representation of this node changed 
        /// </summary>
        /// <param name="node"></param>
        /// <param name="nodeVisual"></param>
        void UpdateNodeVisual(INode node, in IVisualNodeInfo nodeVisual);

        /// <summary>
        /// Notifies that visual representation of this node changed 
        /// </summary>
        /// <param name="edge"></param>
        /// <param name="edgeVisual"></param>
        void UpdateEdgeVisual(IEdge edge, in IVisualEdgeInfo edgeVisual);
    }
}
