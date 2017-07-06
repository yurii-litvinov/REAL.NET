namespace EditorPrototype
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    class Graph
    {
        private Model model;
        private GraphExample dataGraph;
        public GraphExample DataGraph
        {
            get
            {
                return dataGraph;
            }

        }
        public Graph(Model repoModel)
        {
            model = repoModel;
            dataGraph = new GraphExample();
            model.NewVertexInRepo += (sender, args) => CreateNodeWithPos(args.Name, args.Type, args.Attributes);
            model.NewEdgeInRepo += (sender, args) => CreateEdge(args.type, args.prevVer, args.ctrlVer);
        }

        public event EventHandler DrawGraph;
        public event EventHandler<VertexNameArgs> DrawNewVertex;
        public event EventHandler<SourceTargetArgs> DrawNewEdge;
        public event EventHandler<DataVertexArgs> AddNewVertexControl;
        public event EventHandler<DataEdgeArgs> AddNewEdgeControl;
        public void InitModel(string modelName)
        {
            foreach (var node in model.ModelRepo.ModelNodes(modelName))
            {
                Func<Repo.NodeType, DataVertex.VertexTypeEnum> nodeType = n =>
                {
                    switch (n)
                    {
                        case Repo.NodeType.Attribute:
                            return DataVertex.VertexTypeEnum.Attribute;
                        case Repo.NodeType.Node:
                            return DataVertex.VertexTypeEnum.Node;
                    }

                    return DataVertex.VertexTypeEnum.Node;
                };

                CreateNodeWithoutPos(node.name, nodeType(node.nodeType), node.attributes);
            }

            foreach (var edge in model.ModelRepo.ModelEdges(modelName))
            {
                var isViolation = Constraints.CheckEdge(edge, model.ModelRepo, modelName);
                var source = dataGraph.Vertices.First(v => v.Name == edge.source);
                var target = dataGraph.Vertices.First(v => v.Name == edge.target);

                Func<Repo.EdgeType, DataEdge.EdgeTypeEnum> edgeType = e =>
                {
                    switch (e)
                    {
                        case Repo.EdgeType.Generalization:
                            return DataEdge.EdgeTypeEnum.Generalization;
                        case Repo.EdgeType.Association:
                            return DataEdge.EdgeTypeEnum.Association;
                        case Repo.EdgeType.Attribute:
                            return DataEdge.EdgeTypeEnum.Attribute;
                        case Repo.EdgeType.Type:
                            return DataEdge.EdgeTypeEnum.Type;
                    }

                    return DataEdge.EdgeTypeEnum.Generalization;
                };

                var newEdge = new DataEdge(source, target, System.Convert.ToDouble(isViolation)) { EdgeType = edgeType(edge.edgeType) };
                dataGraph.AddEdge(newEdge);
                SourceTargetArgs args = new SourceTargetArgs();
                args.Source = source.Key;
                args.Target = target.Key;
                DrawNewEdge?.Invoke(this, args);
            }

            DrawGraph?.Invoke(this, EventArgs.Empty);
        }

        private void CreateNodeWithoutPos(string name, DataVertex.VertexTypeEnum type, IList<Repo.AttributeInfo> attributes)
        {
            var vertex = new DataVertex(name)
            {
                Key = $"{name}",
                VertexType = type,
            };

            var attributeInfos = attributes.Select(x => new DataVertex.Attribute()
            {
                Name = x.name,
                Type = model.ModelRepo.Node(x.attributeType).name,
                Value = x.value
            });

            attributeInfos.ToList().ForEach(x => vertex.Attributes.Add(x));

            dataGraph.AddVertex(vertex);
            VertexNameArgs args = new VertexNameArgs();
            args.VertName = vertex.Key;
            DrawNewVertex?.Invoke(this, args);
        }
        
        private void CreateNodeWithPos(string name, DataVertex.VertexTypeEnum type, IList<Repo.AttributeInfo> attributes)
        {
            var vertex = new DataVertex(name)
            {
                Key = $"{name}",
                VertexType = type
            };

            var attributeInfos = attributes.Select(x => new DataVertex.Attribute()
            {
                Name = x.name,
                Type = model.ModelRepo.Node(x.attributeType).name,
                Value = x.value
            });

            attributeInfos.ToList().ForEach(x => vertex.Attributes.Add(x));
            DataVertexArgs args = new DataVertexArgs();
            args.dataVert = vertex;
            AddNewVertexControl?.Invoke(this, args);
        }

        public void CreateEdge(string type, DataVertex prevVer, DataVertex ctrlVer)
        {
            var newEdge = new DataEdge(prevVer, ctrlVer)
            {
                Text = type
            };

            DataEdgeArgs args = new DataEdgeArgs();
            args.edge = newEdge;
            AddNewEdgeControl?.Invoke(this, args);
        }

        public class DataVertexArgs : EventArgs
        {
            public DataVertex dataVert;
        }

        public class DataEdgeArgs : EventArgs
        {
            public DataEdge edge;
        }

        public class SourceTargetArgs : EventArgs
        {
            private string source;
            private string target;

            public string Source
            {
                get
                {
                    return source;
                }

                set
                {
                    source = value;
                }

            }

            public string Target
            {
                get
                {
                    return target;
                }

                set
                {
                    target = value;
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
                    return vertName;
                }

                set
                {
                    vertName = value;
                }

            }

        }

    }

}
