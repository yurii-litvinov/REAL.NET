using GraphX.Controls;

namespace EditorPrototype
{
    class Controller
    {
        private Model model;
        public Controller(Model graphModel)
        {
            model = graphModel;
        }
        public void NewNode(string typeId)
        {
            model.NewNode(typeId);
        }
        public void NewEdge(string typeId, VertexControl prevVer, VertexControl ctrlVer)
        {
            model.NewEdge(typeId, prevVer?.Vertex as DataVertex, ctrlVer?.Vertex as DataVertex);
        }    
    }
}
