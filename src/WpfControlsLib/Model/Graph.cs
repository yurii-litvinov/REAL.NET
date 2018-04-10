﻿/* Copyright 2017-2018 REAL.NET group
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
            this.model.NewVertex += (sender, args) => this.CreateNodeWithPos(args.Node);
            this.model.NewEdge += (sender, args) => this.CreateEdge(args.Edge, args.Source, args.Target);
        }

        public event EventHandler DrawGraph;

        public event EventHandler<ElementAddedEventArgs> ElementAdded;

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
                    EdgeType = EdgeViewModel.EdgeTypeEnum.Association
                };
                this.model.EdgesList.Add(newEdge);

                var attributeInfos = edge.Attributes.Select(x => new AttributeViewModel(x, x.Name, x.Kind.ToString())
                {
                    Value = x.StringValue
                });

                var attributes = attributeInfos as IList<AttributeViewModel> ?? attributeInfos.ToList();
                foreach (var x in attributes)
                {
                    newEdge.Attributes.Add(x);
                    x.OnAttributeChange += (sender, args) => 
                    {
                        this.model.OnEdgeAttributeChanged(newEdge, sender as AttributeViewModel, args.NewValue);
                    };
                }

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

            var newEdge = new EdgeViewModel(sourceViewModel, targetViewModel, Convert.ToDouble(true));
            this.model.EdgesList.Add(newEdge);
            newEdge.IsAllowed = this.model.EdgeIsAllowed(prevVer, ctrlVer);
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
            this.model.NodesList.Add(vertex);
            vertex.IsAllowed = this.model.NodeIsAllowed(vertex.Name);
            var attributeInfos = node.Attributes.Select(x => new AttributeViewModel(x, x.Name, x.Kind.ToString())
            {
                Value = x.StringValue
            });

            foreach (var x in attributeInfos.ToList())
            {
                vertex.Attributes.Add(x);
                x.OnAttributeChange += (sender, args1) =>
                {
                    this.model.OnNodeAttributeChanged(vertex, sender as AttributeViewModel, args1.NewValue);
                };
            }

            var args = new DataVertexArgs
            {
                DataVert = vertex
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
            this.model.NodesList.Add(vertex);
            vertex.IsAllowed = this.model.NodeIsAllowed(vertex.Name);
            var attributeInfos = node.Attributes.Select(x => new AttributeViewModel(x, x.Name, x.Kind.ToString())
            {
                Value = x.StringValue
            });

            foreach (var x in attributeInfos.ToList())
            {
                vertex.Attributes.Add(x);
                x.OnAttributeChange += (sender, args) => 
                {
                    this.model.OnNodeAttributeChanged(vertex, sender as AttributeViewModel, args.NewValue);
                };
            }

            this.DataGraph.AddVertex(vertex);
            this.ElementAdded?.Invoke(this, new ElementAddedEventArgs { Element = node });
        }

        public class DataVertexArgs : EventArgs
        {
            public NodeViewModel DataVert { get; set; }
        }

        public class DataEdgeArgs : EventArgs
        {
            public EdgeViewModel EdgeViewModel { get; set; }
        }

        public class ElementAddedEventArgs : EventArgs
        {
            public IElement Element { get; set; }
        }
    }
}