using System.Linq;

namespace WpfEditor.Constraints
{
    /// <summary>
    /// Class for methods, working with constraint violations
    /// </summary>
    public class Constraints
    {
        private static int nodesAmmount = 100;
        private static int edgesAmmount = 100;

        public static int NodesAmmount
        {
            get
            {
                return nodesAmmount;
            }

            set
            {
                nodesAmmount = value;
            }
        }

        public static int EdgesAmmount
        {
            get
            {
                return edgesAmmount;
            }

            set
            {
                edgesAmmount = value;
            }
        }

        /// <summary>
        /// Checking if EdgeViewModel is initialized with constraint violation
        /// For now (02.04) checks if generalization is a one-side edgeViewModel
        /// </summary>
        public static bool CheckEdge(Repo.IEdge edge, Repo.IRepo repo, string modelName)
        {
            return true; // TODO
        }

        public static bool AllowCreateEdge(Repo.IRepo repo, string modelName)
        {
            return repo.Model(modelName).Edges.Count() < EdgesAmmount;
        }

        public static bool AllowCreateNode(Repo.IRepo repo, string modelName)
        {
            return repo.Model(modelName).Nodes.Count() < NodesAmmount;
        }
    }
}