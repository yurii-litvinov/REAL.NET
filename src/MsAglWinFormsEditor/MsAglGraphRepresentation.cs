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

        private readonly Graph graph = new Graph("graph");
        private readonly IRepo repo = RepoFactory.CreateRepo();
        private const string modelName = "mainModel";

        /// <summary>
        /// Class constructor
        /// </summary>
        public MsAglGraphRepresentation()
        {
            AddEdges();
            AddNodes();
        }

        /// <summary>
        /// Getting collection of node type in Repo
        /// </summary>
        /// <returns> Collection of node types </returns>
        public IEnumerable<NodeInfo> GetNodeTypes()
            => repo.MetamodelNodes(modelName);

        /// <summary>
        /// Getting graph method
        /// </summary>
        /// <returns> MSAGL graph representation </returns>
        public Graph GetGraph()
            => graph;

        /// <summary>
        /// Getting name by Repo type
        /// </summary>
        /// <param name="type"> Type name </param>
        /// <returns> Name of attribute </returns>
        public string GetAttributeName(string type)
            => repo.Node(type).name;

        /// <summary>
        /// Add new node to MSAGL and Repo graphs
        /// </summary>
        /// <param name="typeId"> Id of type in Repo </param>
        /// <returns> MSAGL node for addig to view </returns>
        public Node CreateNewNode(string typeId)
        {
            var newNodeInfo = repo.AddNode(typeId, modelName);

            var newNode = graph.AddNode(graph.NodeCount.ToString());
            newNode.LabelText = "New " + newNodeInfo.nodeType.ToString();
            newNode.UserData = new List<AttributeInfo>();
            switch (newNodeInfo.nodeType)
            {
                case NodeType.Attribute:
                    newNode.Attr.FillColor = Color.IndianRed;
                    newNode.Attr.Shape = Shape.Box;
                    break;
                case NodeType.Node:
                    newNode.Attr.FillColor = Color.ForestGreen;
                    newNode.Attr.Shape = Shape.Octagon;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            return newNode;
        }

        /// <summary>
        /// Add new node to MSAGL and Repo graphs
        /// </summary>
        /// <param name="edgeType"> Repo edge type </param>
        /// <param name="edge"> Added edge </param>
        /// <returns> MSAGL node for addig to view </returns>
        public void CreateNewEdge(EdgeType edgeType, Edge edge)
        {
            //TODO: uncomment it when addEdge will be imlemented 
            //repo.AddEdge(edgeType.ToString(), edge.Source, edge.Target);
            
            graph.AddPrecalculatedEdge(edge);
            edge.LabelText = edgeType.ToString();
            FormatEdge(edgeType, edge);
        }
        
        private void FormatEdge(EdgeType edgeType, Edge edge)
        {
            edge.LabelText = edgeType.ToString();
            switch (edgeType)
            {
                case EdgeType.Association:
                    edge.Attr.Color = Color.Black;
                    break;
                case EdgeType.Attribute:
                    edge.Attr.Color = Color.Green;
                    break;
                case EdgeType.Generalization:
                    edge.Attr.Color = Color.Red;
                    break;
                case EdgeType.Type:
                    edge.Attr.Color = Color.Blue;
                    break;
                case EdgeType.Value:
                    edge.Attr.Color = Color.LightCyan;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void AddEdges()
        {
            foreach (var edge in repo.ModelEdges(modelName))
            {
                var newEdge = graph.AddEdge(edge.source, edge.target);
                FormatEdge(edge.edgeType, newEdge);
            }
        }

        private void AddNodes()
        {
            foreach (var node in repo.ModelNodes(modelName))
            {
                var newNode = graph.FindNode(node.name);
                newNode.UserData = node.attributes;
                newNode.Attr.LabelMargin = Padding.Empty.Left;
                switch (node.nodeType)
                {
                    case NodeType.Attribute:
                        newNode.Attr.FillColor = Color.IndianRed;
                        newNode.Attr.Shape = Shape.Box;
                        break;
                    case NodeType.Node:
                        newNode.Attr.FillColor = Color.ForestGreen;
                        newNode.Attr.Shape = Shape.Ellipse;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }
    }
}
