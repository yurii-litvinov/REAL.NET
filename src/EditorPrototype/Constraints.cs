namespace EditorPrototype
{
    using System.Linq;

    /// <summary>
    /// Class for methods, working with constraint violations
    /// </summary>
    public class Constraints
    {
        /// <summary>
        /// Checking if Edge is initialized with constraint violation
        /// For now (02.04) checks if generalization is a one-side edge
        /// </summary>
        public static bool CheckEdge(Repo.EdgeInfo edge, Repo.IRepo repo, string modelName)
        {
            return repo.ModelEdges(modelName).Any(otherEdge =>
                    ((otherEdge.edgeType == Repo.EdgeType.Generalization)
                    && (edge.edgeType == Repo.EdgeType.Generalization)
                    && (otherEdge.source == edge.target)
                    && (otherEdge.target == edge.source)));
        }
    }
}
