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

namespace WpfControlsLib.Controls.Scene
{
    using System;
    using System.Linq;
    using System.Windows;
    using GraphX.Controls;
    using WpfControlsLib.Controls.Scene.EventArguments;
    using WpfControlsLib.ViewModel;

    /// <summary>
    /// Class with commands which can be executed on scene.
    /// </summary>
    public class SceneCommands
    {
        private Scene scene;

        /// <summary>
        /// Initializes a new instance of the <see cref="SceneCommands"/> class.
        /// </summary>
        /// <param name="scene">Scene.</param>
        public SceneCommands(Scene scene)
        {
            this.scene = scene;
        }

        /// <summary>
        /// New element added event.
        /// </summary>
        public event EventHandler<ElementAddedEventArgs> ElementAdded;

        /// <summary>
        /// Element removed event.
        /// </summary>
        public event EventHandler<ElementRemovedEventArgs> ElementRemoved;

        /// <summary>
        /// A command which adds new vertex on scene.
        /// </summary>
        /// <param name="position">Vertex position.</param>
        /// <param name="node">Node for adding.</param>
        public void AddVertexOnScene(Point position, Repo.INode node)
        {
            var vertex = new NodeViewModel(node.Name)
            {
                Node = node,
                Picture = node.Class.Shape
            };

            var attributeInfos = node.Attributes.Select(x => new AttributeViewModel(x, x.Name, x.Kind.ToString())
            {
                Value = x.StringValue
            });

            attributeInfos.ToList().ForEach(x => vertex.Attributes.Add(x));
            var control = new VertexControl(vertex);
            control.SetPosition(position);
            this.scene.Graph.DataGraph.AddVertex(vertex);
            this.scene.SceneX.AddVertex(vertex, control);
            this.RaiseElementAddedEvent(node as Repo.IElement);
        }

        /// <summary>
        /// A command which removes vertex from scene.
        /// If the node is not found in the vertex list, exception will be thrown.
        /// </summary>
        /// <param name="node">Node for removing.</param>
        public void RemoveVertexFromScene(Repo.INode node)
        {
            this.scene.SceneX.EdgesList.ToList().Select(x => x.Key)
                .Where(x => x.Edge.From == node || x.Edge.To == node).ToList()
                .ForEach(x => this.scene.SceneX.RemoveEdge(x, true));
            var foundNodes = this.scene.SceneX.VertexList.ToList()
                .Where(x => x.Key.Node == node).ToList();

            if (foundNodes.Count == 0)
            {
                throw new InvalidOperationException("Can't find node like this");
            }

            var nodePair = foundNodes.First();
            this.scene.SceneX.RemoveVertex(nodePair.Key, true);
            this.RaiseElementRemovedEvent(node as Repo.IElement);
        }

        /// <summary>
        /// A command which adds new edge on scene.
        /// </summary>
        /// <param name="edge">Edge for adding.</param>
        /// <param name="routingPoints">Array of routing points.</param>
        public void AddEdgeOnScene(Repo.IEdge edge, Point[] routingPoints)
        {
            var elementFromEdge = edge.From;
            var elementToEdge = edge.To;
            var foundElementsFromEdge = this.scene.SceneX.VertexList.ToList().FindAll(x => x.Key.Node == elementFromEdge as Repo.INode);
            var foundElementsToEdge = this.scene.SceneX.VertexList.ToList().FindAll(x => x.Key.Node == elementToEdge as Repo.INode);

            if (foundElementsFromEdge.Count == 0 || foundElementsToEdge.Count == 0)
            {
                throw new InvalidOperationException("There is no nodes like this");
            }

            var nodeFromEdge = foundElementsFromEdge.First();
            var nodeToEdge = foundElementsToEdge.First();
            var edgeData = new EdgeViewModel(nodeFromEdge.Key, nodeToEdge.Key)
            {
                Edge = edge,
                RoutingPoints = routingPoints.ToGraphX()
            };
            var control = new EdgeControl(nodeFromEdge.Value, nodeToEdge.Value, edgeData);
            this.scene.Graph.DataGraph.AddEdge(edgeData);
            this.scene.SceneX.InsertEdge(edgeData, control);
            this.RaiseElementAddedEvent(edge as Repo.IElement);
        }

        /// <summary>
        /// A command which removed edge from scene.
        /// </summary>
        /// <param name="edge">Edge for removing.</param>
        public void RemoveEdgeFromScene(Repo.IEdge edge)
        {
            var found = this.scene.SceneX.EdgesList.ToList().FindAll(x => x.Key.Edge == edge);

            if (found.Count == 0)
            {
                throw new InvalidOperationException("Can't find edge like this");
            }

            var edgePair = found.First();
            this.scene.SceneX.RemoveEdge(edgePair.Key, true);
            this.RaiseElementRemovedEvent(edge as Repo.IElement);
        }

        /// <summary>
        /// Raises new element added event.
        /// </summary>
        /// <param name="element">Argument.</param>
        private void RaiseElementAddedEvent(Repo.IElement element)
        {
            var args = new ElementAddedEventArgs
            {
                Element = element
            };

            this.ElementAdded?.Invoke(this, args);
        }

        /// <summary>
        /// Raises new element removed event.
        /// </summary>
        /// <param name="element">Argument.</param>
        private void RaiseElementRemovedEvent(Repo.IElement element)
        {
            var args = new ElementRemovedEventArgs
            {
                Element = element
            };

            this.ElementRemoved?.Invoke(this, args);
        }
    }
}