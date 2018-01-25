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

namespace WpfEditor.Model
{
    using System;
    using System.Linq;
    using QuickGraph;
    using Repo;
    using ViewModel;

    /// <summary>
    /// Represents diagram as GraphX graph. Wraps <see cref="Model"/> and synchronizes changes in repo and in GraphX
    /// graph representation.
    /// </summary>
    public class Graph
    {
        private readonly Model model;

        internal Graph(Model repoModel)
        {
            this.model = repoModel;
            this.DataGraph = new BidirectionalGraph<NodeViewModel, EdgeViewModel>();
            this.model.NewVertexInRepo += (sender, args) => this.CreateNodeWithPos(args.Node);
            this.model.NewEdgeInRepo += (sender, args) => this.CreateEdge(args.Edge, args.PrevVer, args.CtrlVer);
        }

        public event EventHandler DrawGraph;

        public event EventHandler<VertexNameArgs> DrawNewVertex;

        public event EventHandler<SourceTargetArgs> DrawNewEdge;

        public event EventHandler<DataVertexArgs> AddNewVertexControl;

        public event EventHandler<DataEdgeArgs> AddNewEdgeControl;

        public BidirectionalGraph<NodeViewModel, EdgeViewModel> DataGraph { get; }

        // Should be replaced
        public void InitModel(string modelName)
        {
            IModel model = this.model.Repo.Model(modelName);
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
                /* var isViolation = Constraints.CheckEdge(edgeViewModel, this.repo, modelName); */

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

                var newEdge = new EdgeViewModel(source, target, Convert.ToDouble(false)) { EdgeType = EdgeViewModel.EdgeTypeEnum.Association };
                this.DataGraph.AddEdge(newEdge);
                SourceTargetArgs args = new SourceTargetArgs();
                args.Source = source.Name;
                args.Target = target.Name;
                this.DrawNewEdge?.Invoke(this, args);
            }

            this.DrawGraph?.Invoke(this, EventArgs.Empty);
        }

        public void CreateEdge(IEdge edge, NodeViewModel prevVer, NodeViewModel ctrlVer)
        {
            if (prevVer == null || ctrlVer == null)
            {
                return;
            }

            var newEdge = new EdgeViewModel(prevVer, ctrlVer, Convert.ToDouble(true));
            DataEdgeArgs args = new DataEdgeArgs();
            args.EdgeViewModel = newEdge;
            this.AddNewEdgeControl?.Invoke(this, args);
        }

        private void CreateNodeWithPos(INode node)
        {
            var vertex = new NodeViewModel(node.Name)
            {
                Node = node,
                VertexType = NodeViewModel.VertexTypeEnum.Node,
                Picture = node.Class.Shape
            };

            var attributeInfos = node.Attributes.Select(x => new NodeViewModel.Attribute(x, x.Name, x.Kind.ToString())
            {
                Value = x.StringValue
            });

            attributeInfos.ToList().ForEach(x => vertex.Attributes.Add(x));
            DataVertexArgs args = new DataVertexArgs();
            args.DataVert = vertex;
            this.AddNewVertexControl?.Invoke(this, args);
        }

        private void CreateNodeWithoutPos(INode node)
        {
            var vertex = new NodeViewModel(node.Name)
            {
                Node = node,
                VertexType = NodeViewModel.VertexTypeEnum.Node,
                Picture = node.Class.Shape
            };

            var attributeInfos = node.Attributes.Select(x => new NodeViewModel.Attribute(x, x.Name, x.Kind.ToString())
            {
                Value = x.StringValue
            });

            attributeInfos.ToList().ForEach(x => vertex.Attributes.Add(x));
            this.DataGraph.AddVertex(vertex);
            VertexNameArgs args = new VertexNameArgs();
            args.VertName = node.Name;
            this.DrawNewVertex?.Invoke(this, args);
        }

        public class DataVertexArgs : EventArgs
        {
            public NodeViewModel DataVert { get; set; }
        }

        public class DataEdgeArgs : EventArgs
        {
            public EdgeViewModel EdgeViewModel { get; set; }
        }

        public class SourceTargetArgs : EventArgs
        {
            public string Source { get; set; }

            public string Target { get; set; }
        }

        public class VertexNameArgs : EventArgs
        {
            public string VertName { get; set; }
        }
    }
}