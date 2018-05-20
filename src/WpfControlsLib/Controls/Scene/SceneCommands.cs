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
            this.scene.SceneX.AddVertex(vertex, control);
        }

        public void DeleteVertexFromScene(Repo.INode node)
        {
            // TODO : first removing edges, then vertex
        }

        public void AddEdgeOnScene(Repo.IEdge edge, Point[] routingPoints)
        {
            var from = edge.From;
            var to = edge.To;
            var found1 = this.scene.SceneX.VertexList.ToList().FindAll(x => x.Key.Node == from);
            var found2 = this.scene.SceneX.VertexList.ToList().FindAll(x => x.Key.Node == to);
            if (found1.Count == 0 || found2.Count == 0)
            {
                throw new InvalidOperationException("there is no nodes like this");
            }

            var node1 = found1[0];
            var node2 = found2[0];
            var edgeData = new EdgeViewModel(node1.Key, node2.Key);
            var control = new EdgeControl(node1.Value, node2.Value, edgeData);
            this.scene.SceneX.AddEdge(edgeData, control);
        }

        private void DeleteEdgeFromScene(Repo.IEdge edge)
        {
            var found = this.scene.SceneX.EdgesList.ToList().FindAll(x => x.Key.Edge == edge);
            if (found.Count == 0)
            {
                throw new InvalidOperationException("there is no edge like this");
            }

            var edgePair = found[0];
            this.scene.SceneX.RemoveEdge(edgePair.Key, true);
        }
    }
}
