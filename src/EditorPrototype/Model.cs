﻿namespace EditorPrototype
{
    using System;

    public class Model
    {
        private Repo.IRepo modelRepo;
        private string modelName;

        public Model()
        {
            this.modelRepo = Repo.RepoFactory.CreateRepo();
        }

        public event EventHandler<VertexEventArgs> NewVertexInRepo;

        public event EventHandler<EdgeEventArgs> NewEdgeInRepo;

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

        public void NewNode(Repo.IElement element, string modelName)
        {
            var model = this.modelRepo.Model(modelName);
            var newNode = model.CreateElement(element) as Repo.INode;
            this.RaiseNewVertexInRepo(newNode);
        }

        public void NewEdge(Repo.IEdge edge, DataVertex prevVer, DataVertex ctrlVer)
        {
            this.RaiseNewEdgeInRepo(edge, prevVer, ctrlVer);
        }

        private void RaiseNewVertexInRepo(Repo.INode node)
        {
            VertexEventArgs args = new VertexEventArgs();
            args.Node = node;
            this.NewVertexInRepo?.Invoke(this, args);
        }

        private void RaiseNewEdgeInRepo(Repo.IEdge edge, DataVertex prevVer, DataVertex ctrlVer)
        {
            EdgeEventArgs args = new EdgeEventArgs();
            args.Edge = edge;
            args.PrevVer = prevVer;
            args.CtrlVer = ctrlVer;
            this.NewEdgeInRepo?.Invoke(this, args);
        }

        public class VertexEventArgs : EventArgs
        {
            private Repo.INode node;

            public Repo.INode Node
            {
                get
                {
                    return this.node;
                }

                set
                {
                    this.node = value;
                }
            }
        }

        public class EdgeEventArgs : EventArgs
        {
            private Repo.IEdge edge;
            private DataVertex prevVer;
            private DataVertex ctrlVer;

            public Repo.IEdge Edge
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

            public DataVertex PrevVer
            {
                get
                {
                    return this.prevVer;
                }

                set
                {
                    this.prevVer = value;
                }
            }

            public DataVertex CtrlVer
            {
                get
                {
                    return this.ctrlVer;
                }

                set
                {
                    this.ctrlVer = value;
                }
            }
        }
    }
}
