namespace WpfControlsLib.Controls.Scene
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Windows;
    using GraphX.Controls;
    using WpfControlsLib.ViewModel;
    using static WpfControlsLib.Model.Graph;

    public class SceneCommands
    {
        private Scene scene;

        public SceneCommands(Scene scene)
        {
            this.scene = scene;
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
            this.scene.Graph.DataGraph.AddVertex(vertex);
            this.scene.SceneX.AddVertex(vertex, control);
        }

        public void DeleteVertexFromScene(Repo.INode node)
        {
            this.scene.SceneX.EdgesList.ToList().Select(x => x.Key)
                .Where(x => x.Edge.From == node || x.Edge.To == node).ToList()
                .ForEach(x => this.scene.SceneX.RemoveEdge(x, true));
            var found = this.scene.SceneX.VertexList.ToList()
                .Where(x => x.Key.Node == node).ToList();
            if (found.Count == 0)
            {
                throw new InvalidOperationException("can't find node like this");
            }

            var nodePair = found[0];
            this.scene.SceneX.RemoveVertex(nodePair.Key, true);
        }

        public void AddEdgeOnScene(Repo.IEdge edge, Point[] routingPoints)
        {
            var from = edge.From;
            var to = edge.To;
            // hack need to be removed
            var found1 = this.scene.SceneX.VertexList.ToList().FindAll(x => x.Key.Node == from as Repo.INode);
            var found2 = this.scene.SceneX.VertexList.ToList().FindAll(x => x.Key.Node == to as Repo.INode);
            if (found1.Count == 0 || found2.Count == 0)
            {
                throw new InvalidOperationException("there is no nodes like this");
            }

            var node1 = found1[0];
            var node2 = found2[0];
            var edgeData = new EdgeViewModel(node1.Key, node2.Key)
            {
                Edge = edge,
                RoutingPoints = routingPoints.ToGraphX()
            };
            var control = new EdgeControl(node1.Value, node2.Value, edgeData);
            this.scene.Graph.DataGraph.AddEdge(edgeData);
            this.scene.SceneX.InsertEdge(edgeData, control);
        }

        public void DeleteEdgeFromScene(Repo.IEdge edge)
        {
            var found = this.scene.SceneX.EdgesList.ToList().FindAll(x => x.Key.Edge == edge);
            if (found.Count == 0)
            {
                throw new InvalidOperationException("can't find edge like this");
            }

            var edgePair = found[0];
            this.scene.SceneX.RemoveEdge(edgePair.Key, true);
        }
    }
}
