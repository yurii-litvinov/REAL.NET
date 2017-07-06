using System;
using System.Collections.Generic;

namespace EditorPrototype
{
    class Model
    {
        private string modelName;
        public string ModelName
        {
            get
            {
                return modelName;
            }
        }
        private Repo.IRepo modelRepo;
        public Repo.IRepo ModelRepo
        {
            get
            {
                return modelRepo;
            }
        }
        public Model()
        {
            modelRepo = Repo.RepoFactory.CreateRepo();
            modelName = "mainModel";
        }
        public event EventHandler<VertexEventArgs> NewVertexInRepo;
        public event EventHandler<EdgeEventArgs> NewEdgeInRepo;
        
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
            args.name = node.name;
            args.type = nodeType(node.nodeType);
            args.attributes = node.attributes;
            NewVertexInRepo?.Invoke(this, args);
        }
        public void NewNode(string typeId)
        {
            var node = modelRepo.AddNode(typeId, modelName);
            RaiseNewVertexInRepo(node);
        }
        private void RaiseNewEdgeInRepo(string typeId, DataVertex prevVer, DataVertex ctrlVer)
        {
            EdgeEventArgs args = new EdgeEventArgs();
            args.type = typeId;
            args.prevVer = prevVer;
            args.ctrlVer = ctrlVer;
            NewEdgeInRepo?.Invoke(this, args);
        }
        public void NewEdge(string typeId, DataVertex prevVer, DataVertex ctrlVer)
        {
            RaiseNewEdgeInRepo(typeId, prevVer, ctrlVer);
        }
        public class VertexEventArgs : EventArgs
        {
            public string name;
            public DataVertex.VertexTypeEnum type;
            public IList<Repo.AttributeInfo> attributes;
        }
        public class EdgeEventArgs : EventArgs
        {
            public string type;
            public DataVertex prevVer;
            public DataVertex ctrlVer;
        }
    }
}
