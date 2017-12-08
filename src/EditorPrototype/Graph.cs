namespace EditorPrototype
{
    using System;
    using System.Linq;

    public class Graph
    {
        private Model model;

        private GraphExample dataGraph;

        internal Graph(Model repoModel)
        {
            this.model = repoModel;
            this.dataGraph = new GraphExample();
            this.model.NewVertexInRepo += (sender, args) => this.CreateNodeWithPos(args.Node);
            this.model.NewEdgeInRepo += (sender, args) => this.CreateEdge(args.Edge, args.PrevVer, args.CtrlVer);
        }

        public event EventHandler DrawGraph;

        public event EventHandler<VertexNameArgs> DrawNewVertex;

        public event EventHandler<SourceTargetArgs> DrawNewEdge;

        public event EventHandler<DataVertexArgs> AddNewVertexControl;

        public event EventHandler<DataEdgeArgs> AddNewEdgeControl;

        public GraphExample DataGraph
        {
            get
            {
                return this.dataGraph;
            }
        }

        public void InitModel(string modelName) // peremestit'
        {
            Repo.IModel model = this.model.ModelRepo.Model(modelName);
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
                /* var isViolation = Constraints.CheckEdge(edge, this.repo, modelName); */

                var sourceNode = edge.From as Repo.INode;
                var targetNode = edge.To as Repo.INode;
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

                var newEdge = new DataEdge(source, target, Convert.ToDouble(false)) { EdgeType = DataEdge.EdgeTypeEnum.Association };
                this.dataGraph.AddEdge(newEdge);
                SourceTargetArgs args = new SourceTargetArgs();
                args.Source = source.Name;
                args.Target = target.Name;
                this.DrawNewEdge?.Invoke(this, args);
            }

            this.DrawGraph?.Invoke(this, EventArgs.Empty);
        }

        public void CreateEdge(Repo.IEdge edge, DataVertex prevVer, DataVertex ctrlVer)
        {
            if (prevVer == null || ctrlVer == null)
            {
                return;
            }

            var newEdge = new DataEdge(prevVer, ctrlVer, Convert.ToDouble(true));
            DataEdgeArgs args = new DataEdgeArgs();
            args.Edge = newEdge;
            this.AddNewEdgeControl?.Invoke(this, args);
        }

        private void CreateNodeWithPos(Repo.INode node)
        {
            var vertex = new DataVertex(node.Name)
            {
                Node = node,
                VertexType = DataVertex.VertexTypeEnum.Node,
                Picture = node.Class.Shape,
            };

            var attributeInfos = node.Attributes.Select(x => new DataVertex.Attribute(x, x.Name, x.Kind.ToString())
            {
                Value = x.StringValue,
            });

            attributeInfos.ToList().ForEach(x => vertex.Attributes.Add(x));
            DataVertexArgs args = new DataVertexArgs();
            args.DataVert = vertex;
            this.AddNewVertexControl?.Invoke(this, args);
        }

        private void CreateNodeWithoutPos(Repo.INode node)
        {
            var vertex = new DataVertex(node.Name)
            {
                Node = node,
                VertexType = DataVertex.VertexTypeEnum.Node,
                Picture = node.Class.Shape,
            };

            var attributeInfos = node.Attributes.Select(x => new DataVertex.Attribute(x, x.Name, x.Kind.ToString())
            {
                Value = x.StringValue,
            });

            attributeInfos.ToList().ForEach(x => vertex.Attributes.Add(x));
            this.dataGraph.AddVertex(vertex);
            VertexNameArgs args = new VertexNameArgs();
            args.VertName = node.Name;
            this.DrawNewVertex?.Invoke(this, args);
        }

        public class DataVertexArgs : EventArgs
        {
            private DataVertex dataVert;

            public DataVertex DataVert
            {
                get
                {
                    return this.dataVert;
                }

                set
                {
                    this.dataVert = value;
                }
            }
        }

        public class DataEdgeArgs : EventArgs
        {
            private DataEdge edge;

            public DataEdge Edge
            {
                get
                {
                    return this.edge;
                }

                set
                {
                    this.edge = value;
                }
            }
        }

        public class SourceTargetArgs : EventArgs
        {
            private string source;
            private string target;

            public string Source
            {
                get
                {
                    return this.source;
                }

                set
                {
                    this.source = value;
                }
            }

            public string Target
            {
                get
                {
                    return this.target;
                }

                set
                {
                    this.target = value;
                }
            }
        }

        public class VertexNameArgs : EventArgs
        {
            private string vertName;

            public string VertName
            {
                get
                {
                    return this.vertName;
                }

                set
                {
                    this.vertName = value;
                }
            }
        }
    }
}