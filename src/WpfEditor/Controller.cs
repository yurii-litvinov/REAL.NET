using GraphX.Controls;
using WpfEditor.Models;

namespace WpfEditor
{
    public class Controller
    {
        private readonly Model model;

        internal Controller(Model graphModel)
        {
            this.model = graphModel;
        }

        public void NewNode(Repo.IElement node, string modelName)
        {
            this.model.NewNode(node, modelName);
        }

        public void NewEdge(Repo.IElement edge, VertexControl prevVer, VertexControl ctrlVer)
        {
            this.model.NewEdge(edge as Repo.IEdge, prevVer?.Vertex as DataVertex, ctrlVer?.Vertex as DataVertex);
        }
    }
}