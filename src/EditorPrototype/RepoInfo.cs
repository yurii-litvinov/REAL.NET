namespace EditorPrototype
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class RepoInfo
    {
        private Repo.IRepo repo;
        private string modelName;

        public RepoInfo(Repo.IRepo inputRepo, string inputModelName)
        {
            this.repo = inputRepo;
            this.modelName = inputModelName;
        }

        public IEnumerable<Repo.EdgeInfo> GetEdges()
        {
            return this.repo.ModelEdges(this.modelName);
        }

        public IEnumerable<Repo.NodeInfo> GetNodes()
        {
            return this.repo.ModelNodes(this.modelName);
        }
    }
}
