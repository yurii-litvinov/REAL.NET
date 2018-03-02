using System;
using System.Collections.Generic;

namespace WpfControlsLib.Constraints
{
    public class RepoInfo
    {
        private readonly Repo.IRepo repo;
        private readonly string modelName;

        public RepoInfo(Repo.IRepo inputRepo, string inputModelName)
        {
            this.repo = inputRepo;
            this.modelName = inputModelName;
        }

        public IEnumerable<Repo.IEdge> GetEdges()
        {
            return this.repo.Model(this.modelName).Edges;
        }

        public IEnumerable<Repo.INode> GetNodes()
        {
            return this.repo.Model(this.modelName).Nodes;
        }

        public List<string> GetNodeTypes()
        {
            var types = new List<string>();
            types.Add("All");
            foreach (var node in this.repo.Model(this.modelName).Nodes)
            {
                var typeName = Convert.ToString(node.Name);
                if (!types.Contains(typeName))
                {
                    types.Add(typeName);
                }
            }

            return types;
        }

        public List<string> GetEdgeTypes()
        {
            var types = new List<string>();
            types.Add("All");
            foreach (var edge in this.repo.Model(this.modelName).Edges)
            {
                var typeName = Convert.ToString(edge.Name);
                if (!types.Contains(typeName))
                {
                    types.Add(typeName);
                }
            }

            return types;
        }
    }
}
