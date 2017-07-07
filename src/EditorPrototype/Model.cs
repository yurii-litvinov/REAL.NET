namespace EditorPrototype
{
    using System;
    using System.Collections.Generic;

    public class Model
    {
        public string ModelName
        {
            get
            {
                return this.modelName;
            }
        }

        public Repo.IRepo ModelRepo
        {
            get
            {
                return this.modelRepo;
            }
        }

        public Model()
        {
            this.modelRepo = Repo.RepoFactory.CreateRepo();
            this.modelName = "mainModel";
        }

        public event EventHandler<VertexEventArgs> NewVertexInRepo;

        public event EventHandler<EdgeEventArgs> NewEdgeInRepo;

        public void NewNode(string typeId)
        {
            var node = this.modelRepo.AddNode(typeId, this.modelName);
            RaiseNewVertexInRepo(node);
        }

        public void NewEdge(string typeId, DataVertex prevVer, DataVertex ctrlVer)
        {
            RaiseNewEdgeInRepo(typeId, prevVer, ctrlVer);
        }

        public class VertexEventArgs : EventArgs
        {
            private string name;
            private DataVertex.VertexTypeEnum type;
            private IList<Repo.AttributeInfo> attributes;

            public string Name
            {
                get
                {
                    return name;
                }

                set
                {
                    name = value;
                }
            }

            public DataVertex.VertexTypeEnum Type
            {
                get
                {
                    return type;
                }

                set
                {
                    type = value;
                }
            }

            public IList<Repo.AttributeInfo> Attributes
            {
                get
                {
                    return attributes;
                }

                set
                {
                    attributes = value;
                }
            }
        }

        public class EdgeEventArgs : EventArgs
        {
            public string Type
            {
                get
                {
                    return type;
                }

                set
                {
                    type = value;
                }
            }

            public DataVertex PrevVer
            {
                get
                {
                    return prevVer;
                }

                set
                {
                    prevVer = value;
                }
            }

            public DataVertex CtrlVer
            {
                get
                {
                    return ctrlVer;
                }

                set
                {
                    ctrlVer = value;
                }
            }

            private string type;
            private DataVertex prevVer;
            private DataVertex ctrlVer;
        }

        private Repo.IRepo modelRepo;
        private string modelName;

        private void RaiseNewVertexInRepo(Repo.NodeInfo node)
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

            VertexEventArgs args = new VertexEventArgs();
            args.Name = node.name;
            args.Type = nodeType(node.nodeType);
            args.Attributes = node.attributes;
            NewVertexInRepo?.Invoke(this, args);
        }

        private void RaiseNewEdgeInRepo(string typeId, DataVertex prevVer, DataVertex ctrlVer)
        {
            EdgeEventArgs args = new EdgeEventArgs();
            args.Type = typeId;
            args.PrevVer = prevVer;
            args.CtrlVer = ctrlVer;
            NewEdgeInRepo?.Invoke(this, args);
        }
    }
}
