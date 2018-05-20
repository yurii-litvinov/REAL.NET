namespace WpfControlsLib.Controls.Scene
{
    using GraphX.Controls;
    using System;
    using System.Collections.Generic;
    using System.Windows;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using WpfControlsLib.ViewModel;
    using static WpfControlsLib.Model.Graph;

    public class SceneCommands
    {
        private Scene scene;

        private Repo.IModel model;

        public SceneCommands(Scene scene, Repo.IModel model)
        {
            this.scene = scene;
            this.model = model;
        }

        public void AddVertexOnScene(Point position, Repo.INode node)
        {
            var vertex = new NodeViewModel(node.Name)
            {
                Node = node,
                Picture = node.Class.Shape
            };
            var attributeInfos = node.Attributes.Select(x => new AttributeViewModel(x, x.Name, x.Kind.ToString())
            {
                Value = x.StringValue
            });

            attributeInfos.ToList().ForEach(x => vertex.Attributes.Add(x));
            var control = new VertexControl(vertex);
            control.SetPosition(position);
            this.scene.Scene.AddVertex(vertex, control);
        }

        public void DeleteVertexFromScene(Repo.INode node)
        {
            // TODO : first removing edges, then vertex
        }

        public void AddEdgeOnScene(Repo.INode node1, Repo.INode node2, Point[] routingPoints)
        {

        }

        private void DeleteEdgeFromScene(Repo.IEdge edge)
        {

        }
    }
}
