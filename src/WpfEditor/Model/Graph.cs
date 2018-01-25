using System;
using System.Linq;
using QuickGraph;
using Repo;
using WpfEditor.ViewModel;

namespace WpfEditor.Model
{
    public class Graph
    {
        private readonly Model model;

        private readonly BidirectionalGraph<NodeViewModel, EdgeViewModel> dataGraph;

        internal Graph(Model repoModel)
        {
            this.model = repoModel;
            this.dataGraph = new BidirectionalGraph<NodeViewModel, EdgeViewModel>();
            this.model.NewVertexInRepo += (sender, args) => this.CreateNodeWithPos(args.Node);
            this.model.NewEdgeInRepo += (sender, args) => this.CreateEdge(args.Edge, args.PrevVer, args.CtrlVer);
        }

        public event EventHandler DrawGraph;

        public event EventHandler<VertexNameArgs> DrawNewVertex;

        public event EventHandler<SourceTargetArgs> DrawNewEdge;

        public event EventHandler<DataVertexArgs> AddNewVertexControl;

        public event EventHandler<DataEdgeArgs> AddNewEdgeControl;

        public BidirectionalGraph<NodeViewModel, EdgeViewModel> DataGraph => this.dataGraph;

        // Should be replaced
        public void InitModel(string modelName)
        {
            IModel model = this.model.ModelRepo.Model(modelName);
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

                if (this.dataGraph.Vertices.Count(v => v.Node == sourceNode) == 0
                    || this.dataGraph.Vertices.Count(v => v.Node == targetNode) == 0)
                {
                    // Link to an attribute node. TODO: It's ugly.
                    continue;
                }

                var source = this.dataGraph.Vertices.First(v => v.Node == sourceNode);
                var target = this.dataGraph.Vertices.First(v => v.Node == targetNode);

                var newEdge = new EdgeViewModel(source, target, Convert.ToDouble(false)) { EdgeType = EdgeViewModel.EdgeTypeEnum.Association };
                this.dataGraph.AddEdge(newEdge);
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
            this.dataGraph.AddVertex(vertex);
            VertexNameArgs args = new VertexNameArgs();
            args.VertName = node.Name;
            this.DrawNewVertex?.Invoke(this, args);
        }

        public class DataVertexArgs : EventArgs
        {
            private NodeViewModel dataVert;

            public NodeViewModel DataVert
            {
                get => this.dataVert;

                set => this.dataVert = value;
            }
        }

        public class DataEdgeArgs : EventArgs
        {
            private EdgeViewModel edgeViewModel;

            public EdgeViewModel EdgeViewModel
            {
                get => this.edgeViewModel;

                set => this.edgeViewModel = value;
            }
        }

        public class SourceTargetArgs : EventArgs
        {
            private string source;
            private string target;

            public string Source
            {
                get => this.source;

                set => this.source = value;
            }

            public string Target
            {
                get => this.target;

                set => this.target = value;
            }
        }

        public class VertexNameArgs : EventArgs
        {
            public string VertName { get; set; }
        }
    }
}