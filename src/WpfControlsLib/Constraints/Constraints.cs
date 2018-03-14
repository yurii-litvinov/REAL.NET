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

        public bool AllowCreateEdge(IEnumerable<IEdge> edges)
        {
            return this.Check(edges.Count() + 1, this.EdgesAmount);
        }

        public bool AllowCreateNode(IEnumerable<INode> nodes)
        {
            return this.Check(nodes.Count() + 1, this.NodesAmount);
        }

        public bool CheckEdges(IEnumerable<IEdge> edges)
        {
            return this.Check(edges.Count(), this.EdgesAmount);
        }

        public bool CheckNodes(IEnumerable<INode> nodes)
        {
            return this.Check(nodes.Count(), this.NodesAmount);
        }

        private bool Check(int a, int b)
        {
            return a < b;
        }
    }
}