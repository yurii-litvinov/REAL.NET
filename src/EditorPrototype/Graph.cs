using System.Drawing;

namespace EditorPrototype
{
    using System;
    using System.Linq;

    public class Graph
    {
        private readonly Model model;

        public GraphExample DataGraph { get; }

        internal Graph(Model repoModel)
        {
            this.model = repoModel;
            this.DataGraph = new GraphExample();
            //this.model.NewVertexInRepo += (sender, args) => this.CreateNodeWithPos(args.Node);
            //this.model.NewEdgeInRepo += (sender, args) => this.CreateEdge(args.Edge, args.PrevVer, args.CtrlVer);
        }

        public event EventHandler DrawGraph;

        public event EventHandler<VertexNameArgs> DrawNewVertex;

        public event EventHandler<SourceTargetArgs> DrawNewEdge;

        public event EventHandler<DataVertexArgs> AddNewVertexControl;

        public event EventHandler<DataEdgeArgs> AddNewEdgeControl;
        
        // Should be replaced
        public void InitModel(string modelName)
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

                if (this.DataGraph.Vertices.Count(v => v.Node == sourceNode) == 0
                    || this.DataGraph.Vertices.Count(v => v.Node == targetNode) == 0)
                {
                    // Link to an attribute node. TODO: It's ugly.
                    continue;
                }

                var source = this.DataGraph.Vertices.First(v => v.Node == sourceNode);
                var target = this.DataGraph.Vertices.First(v => v.Node == targetNode);

                var newEdge = new DataEdge(source, target) { Text = string.Format("{0} -> {1}", 0, 1), EdgeType = DataEdge.EdgeTypeEnum.Association };

                var attributeInfos = edge.Attributes.Select(x => new DataEdge.Attribute
                {
                    Name = x.Name,
                    Type = x.Kind.ToString(),
                    Value = x.StringValue
                });
                attributeInfos.ToList().ForEach(x => newEdge.Attributes.Add(x));

                this.DataGraph.AddEdge(newEdge);
                SourceTargetArgs args = new SourceTargetArgs
                {
                    Source = source.Name,
                    Target = target.Name
                };
                //this.DrawNewEdge?.Invoke(this, args);
            }
            
        }

        public void Draw()
        {
            this.DrawGraph?.Invoke(this, EventArgs.Empty);
        }

        public void CreateEdge(Repo.IEdge edge, DataVertex prevVer, DataVertex ctrlVer)
        {
            if (prevVer == null || ctrlVer == null)
            {
                return;
            }

            var newEdge = new DataEdge(prevVer, ctrlVer, Convert.ToDouble(true)){Text = "OLOLO"};
            DataEdgeArgs args = new DataEdgeArgs {Edge = newEdge};
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
                Value = x.StringValue
            });

            attributeInfos.ToList().ForEach(x => vertex.Attributes.Add(x));
            DataVertexArgs args = new DataVertexArgs {DataVert = vertex};
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
                Value = x.StringValue
            });

            attributeInfos.ToList().ForEach(x => vertex.Attributes.Add(x));
            this.DataGraph.AddVertex(vertex);
            VertexNameArgs args = new VertexNameArgs {VertName = node.Name};
            this.DrawNewVertex?.Invoke(this, args);
        }

        public class DataVertexArgs : EventArgs
        {
            public DataVertex DataVert { get; set; }
        }

        public class DataEdgeArgs : EventArgs
        { 
            public DataEdge Edge { get; set; }
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