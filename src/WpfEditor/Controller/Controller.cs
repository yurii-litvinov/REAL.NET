using GraphX.Controls;
using WpfEditor.Model;
using WpfEditor.ViewModel;

namespace WpfEditor.Controller
{
    public class Controller
    {
        private readonly Model.Model model;

        internal Controller(Model.Model graphModel)
        {
            this.model = graphModel;
        }

        public void NewNode(Repo.IElement node, string modelName)
        {
            this.model.NewNode(node, modelName);
        }

        public void NewEdge(Repo.IElement edge, VertexControl prevVer, VertexControl ctrlVer)
        {
            this.model.NewEdge(edge as Repo.IEdge, prevVer?.Vertex as NodeViewModel, ctrlVer?.Vertex as NodeViewModel);
        }
    }
}