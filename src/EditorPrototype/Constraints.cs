using System;
using System.Linq;

namespace EditorPrototype
{
    //Class for methods, working with constraint violations
    public class Constraints
    {
        //Checking if Edge is initialized with constraint violation 
        //For now (02.04) checks if generalization is a one-side edge
        public bool CheckEdge(Repo.EdgeInfo edge, Repo.IRepo repo)
        {
            var isViolation = repo.ModelEdges().Any(otherEdge => ((otherEdge.edgeType == Repo.EdgeType.Generalization) && (edge.edgeType == Repo.EdgeType.Generalization) &&(otherEdge.source == edge.target) && (otherEdge.target == edge.source)));
            if (edge.edgeType == Repo.EdgeType.Generalization)
            {
                var source = edge.source;
                var target = edge.target;
                foreach (var otherEdge in repo.ModelEdges())
                {
                    if (otherEdge.edgeType == 0)
                    {
                        if ((otherEdge.source == edge.target) && (otherEdge.target == edge.source))
                        {
                            isViolation = true;
                        }
                    }
                }

            }
            return isViolation;
        }
    }
}
