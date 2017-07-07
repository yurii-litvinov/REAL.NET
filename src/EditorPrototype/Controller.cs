namespace EditorPrototype
{
    using GraphX.Controls;

    public class Controller
    {
        private Model model;

        internal Controller(Model graphModel)
        {
            this.model = graphModel;
        }

        public void NewNode(string typeId)
        {
            this.model.NewNode(typeId);
        }

        public void NewEdge(string typeId, VertexControl prevVer, VertexControl ctrlVer)
        {
            this.model.NewEdge(typeId, prevVer?.Vertex as DataVertex, ctrlVer?.Vertex as DataVertex);
        }    
    }
}