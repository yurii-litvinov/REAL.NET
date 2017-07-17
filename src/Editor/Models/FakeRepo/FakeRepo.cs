namespace REAL.NET.Models.FakeRepo
{
    using Repo;
    using System;
    using System.Collections.Generic;

    class FakeRepo : IRepo
    {
        private Dictionary<string, Model> dictionary = new Dictionary<string, Model>();

        public FakeRepo()
        {
            NodeInfo n = new NodeInfo();

            Model model = new Model();
            dictionary.Add(model.Name, model);
            
        }

        public NodeInfo AddEdge(string typeId, string sourceId, string targetId, string modelName)
        {
            throw new NotImplementedException();
        }

        public NodeInfo AddNode(string typeId, string modelName)
        {
            throw new NotImplementedException();
        }

        public string EdgeType(string id)
        {
            throw new NotImplementedException();
        }

        public bool IsEdgeClass(string typeId)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<NodeInfo> MetamodelNodes(string modelName)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<EdgeInfo> ModelEdges(string modelName)
        {
            return dictionary.ContainsKey(modelName) ? dictionary[modelName].EdgeCollection : throw new ArgumentException($"Model {modelName} wasn't found");
        }

        public IEnumerable<NodeInfo> ModelNodes(string modelName)
        {
            return dictionary.ContainsKey(modelName) ? dictionary[modelName].NodeCollection : throw new ArgumentException($"Model {modelName} wasn't found");
        }

        public IEnumerable<string> Models()
        {
            throw new NotImplementedException();
        }

        public NodeInfo Node(string id)
        {
            throw new NotImplementedException();
        }

        public NodeInfo NodeType(string id)
        {
            throw new NotImplementedException();
        }
    }
}
