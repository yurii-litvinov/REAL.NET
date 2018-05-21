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
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows;
    using EditorPluginInterfaces.UndoRedo;
    using WpfControlsLib.Controls.Scene;
    using WpfControlsLib.Controls.Toolbar;

    /// <summary>
    /// A class that registers events that occur on the scene (adding new edge, vertex, removing)
    /// and adds commands to the stack.
    /// </summary>
    public class SceneActionsRegister
    {
        private GraphArea scene;
        private SceneCommands commands;
        private IUndoRedoStack undoRedoStack;

        /// <summary>
        /// Initializes a new instance of the <see cref="SceneActionsRegister"/> class.
        /// </summary>
        /// <param name="scene">Scene.</param>
        /// <param name="commands">Command of scene.</param>
        /// <param name="undoRedoStack">UndoRedo stack.</param>
        public SceneActionsRegister(GraphArea scene, SceneCommands commands, IUndoRedoStack undoRedoStack)
        {
            this.scene = scene;
            this.commands = commands;
            this.undoRedoStack = undoRedoStack;
        }

        /// <summary>
        /// Registers adding new vertex, adds command to UndoRedo stack.
        /// </summary>
        /// <param name="position">Node position.</param>
        /// <param name="node">Node for adding.</param>
        public void RegisterAddingVertex(Point position, Repo.INode node)
        {
            Action doAction = () => { this.commands.AddVertexOnScene(position, node); };
            Action undoAction = () => { this.commands.RemoveVertexFromScene(node); };
            var command = new Command(doAction, undoAction);
            this.undoRedoStack.AddCommand(command);
        }

        /// <summary>
        /// Registers deleting vertex, adds command to UndoRedo stack.
        /// If the node is not found in the vertex list, exception will be thrown.
        /// </summary>
        /// <param name="node">Node for deleting with infection points.</param>
        /// <param name="edgesFromNode">Edges which are connected to nodes.</param>
        public void RegisterRemovingVertex(Repo.INode node, IEnumerable<Tuple<Repo.IEdge, Point[]>> edgesFromNode)
        {
            var foundVertices = this.scene.VertexList.ToList().FindAll(x => x.Key.Node == node);

            if (foundVertices.Count == 0)
            {
                throw new InvalidOperationException("Can't find this node on scene");
            }

            var vertexToDelete = foundVertices.First();
            var vertexPosition = vertexToDelete.Value.GetPosition();
            Action doAction = () => { this.commands.RemoveVertexFromScene(node); };
            Action undoAction = () =>
            {
                this.commands.AddVertexOnScene(vertexPosition, node);
                this.RestoreEdges(edgesFromNode);
            };
            var command = new Command(doAction, undoAction);
            this.undoRedoStack.AddCommand(command);
        }

        /// <summary>
        /// Registers adding new edge, adds command to UndoRedo stack.
        /// </summary>
        /// <param name="edge">Edge for adding.</param>
        /// <param name="points">Array of infection points.</param>
        public void RegisterAddingEdge(Repo.IEdge edge, Point[] points)
        {
            Action doAction = () => { this.commands.AddEdgeOnScene(edge, points); };
            Action undoAction = () => { this.commands.RemoveEdgeFromScene(edge); };
            var command = new Command(doAction, undoAction);
            this.undoRedoStack.AddCommand(command);
        }

        /// <summary>
        /// Registers deleting edge, adds command to UndoRedo stack.
        /// If the edge is not found in the edges list, exception will be thrown.
        /// </summary>
        /// <param name="edge">Edge for deleting.</param>
        public void RegisterRemovingEdge(Repo.IEdge edge)
        {
            var foundEdges = this.scene.EdgesList.ToList().FindAll(x => x.Key.Edge == edge);

            if (foundEdges.Count == 0)
            {
                throw new InvalidOperationException("can't find this edge on scene");
            }

            var edgePair = foundEdges.First();
            var points = this.ConvertToWinPoints(edgePair.Key.RoutingPoints);
            Action doAction = () => { this.commands.RemoveEdgeFromScene(edge); };
            Action undoAction = () => { this.commands.AddEdgeOnScene(edge, points); };
            var command = new Command(doAction, undoAction);
            this.undoRedoStack.AddCommand(command);
        }

        /// <summary>
        /// Restores edges. It is esential because if we remove vertex from scene, connected edges will be removed to.
        /// </summary>
        /// <param name="pairs">Pairs of edges and infection points.</param>
        private void RestoreEdges(IEnumerable<Tuple<Repo.IEdge, Point[]>> pairs)
        {
            foreach (var pair in pairs)
            {
                this.commands.AddEdgeOnScene(pair.Item1, pair.Item2);
            }
        }

        private Point[] ConvertToWinPoints(GraphX.Measure.Point[] points) => points?.Select(r => new Point(r.X, r.Y)).ToArray();
    }
}