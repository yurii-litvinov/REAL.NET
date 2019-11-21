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

namespace WpfControlsLib.Model
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using GraphX.PCL.Common;
    using QuickGraph;
    using Repo;
    using ViewModel;
    using WpfControlsLib.Controls.Scene.EventArguments;

    /// <summary>
    /// Represents diagram as GraphX graph. Wraps <see cref="Model"/> and synchronizes changes in repo and in GraphX
    /// graph representation.
    ///
    /// Also this class serves as a factory and container for ViewModels for various parts of visual model
    /// (see <see cref="NodeViewModel"/> and <see cref="EdgeViewModel"/> as an example). Those view models can only
    /// be created by this class and their lifecycle is managed here.
    /// </summary>
    public class Graph
    {
        private readonly Model model;

        internal Graph(Model repoModel)
        {
            this.model = repoModel;
            this.DataGraph = new BidirectionalGraph<NodeViewModel, EdgeViewModel>();
            this.model.NewVertexAdded += (sender, args) => this.CreateNodeWithPos(args.Node);
            this.model.NewEdgeAdded += (sender, args) => this.CreateEdge(args.Edge, args.Source, args.Target);
            this.model.ElementRemoved += (sender, args) => this.RemoveElement(args.Element);
            this.model.ElementCheck += (sender, args) => this.CheckElement(args.Element, args.IsAllowed);
        }

        public event EventHandler DrawGraph;

        public event EventHandler<ElementAddedEventArgs> ElementAdded;

        public event EventHandler<ElementRemovedEventArgs> ElementRemoved;

        public event EventHandler<DataVertexArgs> AddNewVertexControl;

        public event EventHandler<DataEdgeArgs> AddNewEdgeControl;

        public BidirectionalGraph<NodeViewModel, EdgeViewModel> DataGraph { get; }

        // Should be replaced
        public void InitModel(string modelName)
        {
            var model = this.model.Repo.Model(modelName);
            if (model == null)
            {
                return;
            }

            foreach (var node in model.Nodes)
            {
                this.CreateNodeWithoutPos(node);
            }

            foreach (var edge in model.Edges)
            {
                var sourceNode = edge.From as INode;
                var targetNode = edge.To as INode;
                if (sourceNode == null || targetNode == null)
                {
                    // Editor does not support edges linked to edges. Yet.
                    continue;
                }

                if (this.DataGraph.Vertices.Count(v => v.Node == sourceNode) == 0
                    || this.DataGraph.Vertices.Count(v => v.Node == targetNode) == 0)
                {
                    // Link to an attribute node. TODO: It's ugly.
                    continue;
                }

                var source = this.DataGraph.Vertices.First(v => v.Node == sourceNode);
                var target = this.DataGraph.Vertices.First(v => v.Node == targetNode);

                var newEdge = new EdgeViewModel(source, target, Convert.ToDouble(false))
                {
                    Edge = edge,
                    EdgeType = EdgeViewModel.EdgeTypeEnum.Association
                };
                var attributeInfos = edge.Attributes.Select(x => new AttributeViewModel(x, x.Name, x.Kind.ToString())
                {
                    Value = x.StringValue
                });
                var attributes = attributeInfos as IList<AttributeViewModel> ?? attributeInfos.ToList();
                attributes.ForEach(x => newEdge.Attributes.Add(x));
                var value = attributes.SingleOrDefault(x => x.Name == "Value");
                if (value != null)
                {
                    newEdge.EdgeType = EdgeViewModel.EdgeTypeEnum.Attribute;
                    newEdge.Text = value.Value;
                }

                this.DataGraph.AddEdge(newEdge);
                this.ElementAdded?.Invoke(this, new ElementAddedEventArgs {Element = edge});
            }

            this.DrawGraph?.Invoke(this, EventArgs.Empty);
        }

        public void CreateEdge(IEdge edge, IElement prevVer, IElement ctrlVer)
        {
            _ = prevVer ?? throw new ArgumentNullException(nameof(prevVer));
            _ = ctrlVer ?? throw new ArgumentNullException(nameof(prevVer));

            var sourceViewModel = this.DataGraph.Vertices.FirstOrDefault(x => x.Node == prevVer);
            var targetViewModel = this.DataGraph.Vertices.FirstOrDefault(x => x.Node == ctrlVer);

            _ = sourceViewModel ?? throw new InvalidOperationException();
            _ = targetViewModel ?? throw new InvalidOperationException();

            var newEdge = new EdgeViewModel(sourceViewModel, targetViewModel, Convert.ToDouble(true))
            {
                Edge = edge
            };

            this.DataGraph.AddEdge(newEdge);

            var args = new DataEdgeArgs
            {
                EdgeViewModel = newEdge
            };

            this.AddNewEdgeControl?.Invoke(this, args);
            this.ElementAdded?.Invoke(this, new ElementAddedEventArgs { Element = edge });
        }

        private void CreateNodeWithPos(INode node)
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
            this.DataGraph.AddVertex(vertex);
            var args = new DataVertexArgs
            {
                DataVertex = vertex
            };
            this.AddNewVertexControl?.Invoke(this, args);
            this.ElementAdded?.Invoke(this, new ElementAddedEventArgs { Element = node });
        }

        private void CreateNodeWithoutPos(INode node)
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
            this.DataGraph.AddVertex(vertex);
            this.ElementAdded?.Invoke(this, new ElementAddedEventArgs { Element = node });
        }

        private void RemoveElement(IElement element)
        {
            if (element.Metatype == Metatype.Edge)
            {
                var edgeViewModel = this.DataGraph.Edges.First(x => x.Edge == element);
                this.DataGraph.RemoveEdge(edgeViewModel);
            }

            if (element.Metatype == Metatype.Node)
            {
                var nodeViewModel = this.DataGraph.Vertices.First(x => x.Node == element);
                this.DataGraph.RemoveVertex(nodeViewModel);
            }

            this.ElementRemoved?.Invoke(this, new ElementRemovedEventArgs { Element = element });
        }

        private void CheckElement(IElement element, bool isAllowed)
        {
            if (element.Metatype == Metatype.Edge)
            {
                var edgeViewModel = this.DataGraph.Edges.First(x => x.Edge == element);
                edgeViewModel.IsAllowed = isAllowed;
            }

            if (element.Metatype == Metatype.Node)
            {
                var nodeViewModel = this.DataGraph.Vertices.First(x => x.Node == element);
                nodeViewModel.IsAllowed = isAllowed;
            }
        }

        public class DataVertexArgs : EventArgs
        {
            public NodeViewModel DataVertex { get; set; }
        }

        public class DataEdgeArgs : EventArgs
        {
            public EdgeViewModel EdgeViewModel { get; set; }
        }
    }
}