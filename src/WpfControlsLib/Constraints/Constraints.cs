namespace WpfControlsLib.Constraints
{
    using Repo;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Class for methods, working with constraint violations
    /// </summary>
    public class Constraints
    {
        private string modelName;

        public Constraints()
        {
            this.NodesAmount = 100;
            this.EdgesAmount = 100;
        }

        public int NodesAmount { get; set; }

        public int EdgesAmount { get; set; }

        public bool AllowCreateEdge(IEnumerable<IEdge> edges, string modelName)
        {
            return edges.Count() < this.EdgesAmount;
        }

        public bool AllowCreateNode(Repo.IRepo repo, string modelName)
        {
            return repo.Model(modelName).Nodes.Count() < this.NodesAmount;
        }
    }
}