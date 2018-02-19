using System.Linq;

namespace WpfEditor.Constraints
{
    /// <summary>
    /// Class for methods, working with constraint violations
    /// </summary>
    public class Constraints
    {
        public static int NodesAmount { get; set; } = 100;

        public static int EdgesAmount { get; set; } = 100;

        /// <summary>
        /// Checking if edge is initialized with constraint violation
        /// For now (02.04) checks if generalization is a one-side edge.
        /// </summary>
        public static bool CheckEdge(Repo.IEdge edge, Repo.IRepo repo, string modelName)
        {
            return true; // TODO
        }

        public static bool AllowCreateEdge(Repo.IRepo repo, string modelName)
        {
            return repo.Model(modelName).Edges.Count() < EdgesAmount;
        }

        public static bool AllowCreateNode(Repo.IRepo repo, string modelName)
        {
            return repo.Model(modelName).Nodes.Count() < NodesAmount;
        }
    }
}