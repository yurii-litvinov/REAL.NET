using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EditorPrototype
{
    class Restrictions
    {
        public void CheckEdge(Repo.EdgeInfo edge, Repo.IRepo repo, ref bool isBreach)
        {
            if (edge.edgeType == 0)
            {
                var source = edge.source;
                var target = edge.target;
                foreach (var otherEdge in repo.ModelEdges())
                {
                    if (otherEdge.edgeType == 0)
                    {
                        if ((otherEdge.source == target) && (otherEdge.target == source))
                        {
                            isBreach = true;
                        }
                    }
                }

            }
        }
    }
}
