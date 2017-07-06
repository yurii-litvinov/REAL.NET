namespace EditorPrototype
{
    using GraphX.Controls;

    public class Controller
    {
        private Model model;

        internal Controller(Model graphModel)
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
