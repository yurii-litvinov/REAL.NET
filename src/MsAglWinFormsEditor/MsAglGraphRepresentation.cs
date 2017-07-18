using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Microsoft.Msagl.Drawing;
using Repo;

namespace MsAglWinFormsEditor
{
    /// <summary>
    /// Class for converting and representing graph from Repo as MSAGL graph
    /// </summary>
    public class MsAglGraphRepresentation
    {

        public Graph Graph { get; }
        private readonly IRepo repo = RepoFactory.CreateRepo();

        private const string modelName = "RobotsTestModel";
        private IModel currentModel = null;
        private Dictionary<IElement, string> ids = new Dictionary<IElement, string>();
        private int idCounter;

        /// <summary>
        /// Class constructor
        /// </summary>
        public MsAglGraphRepresentation()
        {
            Graph = new Graph("graph");
            currentModel = repo.Model(modelName);
            AddEdges();
            AddNodes();
        }

        /// <summary>
        /// Getting collection of node type in Repo
        /// </summary>
        /// <returns> Collection of node types </returns>
        public IEnumerable<INode> GetNodeTypes()
            => currentModel.Metamodel.Nodes;

        /// <summary>
        /// Getting name by Repo type
        /// </summary>
        /// <param name="type"> Type name </param>
        /// <returns> Name of attribute </returns>
        public string GetAttributeName(INode type)
            => type.Name;

        /// <summary>
        /// Add new node to MSAGL and Repo graphs
        /// </summary>
        /// <param name="type"> Type of a node in Repo </param>
        /// <returns> MSAGL node for addig to view </returns>
        public Node CreateNewNode(INode type)
        {
            var newNodeData = currentModel.CreateElement(type) as INode;
            var newNode = Graph.AddNode(Graph.NodeCount.ToString());
            newNode.LabelText = newNodeData.Name;
            newNode.UserData = new List<IAttribute>();
            FormatNode(newNode, newNodeData);
            return newNode;
        }

        /// <summary>
        /// Add new node to MSAGL and Repo graphs
        /// </summary>
        /// <param name="edgeType"> Repo edge type </param>
        /// <param name="edge"> Added edge </param>
        /// <returns> MSAGL node for addig to view </returns>
        public void CreateNewEdge(IEdge edgeType, Edge edge)
        {
            //TODO: uncomment it when addEdge will be implemented 
            //repo.AddEdge(edgeType.ToString(), edge.Source, edge.Target);
            Graph.AddPrecalculatedEdge(edge);
            FormatEdge(edgeType, edge);
        }

        private void FormatEdge(IEdge edgeData, Edge edge)
        {
            /*switch (edgeData.Shape)
            {
                case "":*/
                    edge.Attr.Color = Color.Black;
                    /*break;
                default:
                    edge.Attr.Color = Color.Chocolate;
                    break;
            }
            */
        }

        private void AddEdges()
        {
            foreach (var edge in currentModel.Edges)
            {
                var newEdge = Graph.AddEdge(Id(edge.From), Id(edge.To));
                FormatEdge(edge, newEdge);
            }
        }

        private void FormatNode(Node newNode, INode nodeData)
        {
            /*switch (nodeData.Shape)
            {
                case "":*/
                    newNode.Attr.FillColor = Color.IndianRed;
                    newNode.Attr.Shape = Shape.Box;
                    /*break;
                default:
                    newNode.Attr.FillColor = Color.ForestGreen;
                    newNode.Attr.Shape = Shape.Ellipse;
                    break;
            }*/
        }

        private void AddNodes()
        {
            foreach (var node in currentModel.Nodes)
            {
                var newNode = Graph.FindNode(Id(node));
                newNode.UserData = node.Attributes;
                newNode.Attr.LabelMargin = Padding.Empty.Left;
                newNode.LabelText = node.Name;
                FormatNode(newNode, node);
            }
        }

        private string Id(IElement element)
        {
            if (!ids.ContainsKey(element))
            {
                ids.Add(element, (++idCounter).ToString());
            }

            return ids[element];
        }
    }
}
