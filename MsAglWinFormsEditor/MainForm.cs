using System;
using System.Windows.Forms;
using Microsoft.Glee.Drawing;
using Repo;

namespace MsAglWinFormsEditor
{
    /// <summary>
    /// Main project form
    /// </summary>
    public partial class MainForm : Form
    {

        private readonly Repo.Repo repo = RepoFactory.CreateRepo();
        private readonly Graph graph = new Graph("graph");

        /// <summary>
        /// Create form with given graph
        /// </summary>
        public MainForm()
        {
            InitializeComponent();
            
            var viewer = new Microsoft.Glee.GraphViewerGdi.GViewer();

            AddEdges();
            AddNodes();
            
            viewer.Graph = graph;
            
            SuspendLayout();
            viewer.Dock = DockStyle.Fill;
            Controls.Add(viewer);
            ResumeLayout();
        }

        private void AddEdges()
        {
            foreach (var edge in repo.ModelEdges())
            {
                var newEdge = graph.AddEdge(edge.source, edge.edgeType.ToString(), edge.target);
                newEdge.Attr.Fontsize = 5;

                switch (edge.edgeType)
                {
                    case EdgeType.Association:
                        newEdge.Attr.Color = Color.Black;
                        break;
                    case EdgeType.Attribute:
                        newEdge.Attr.Color = Color.Green;
                        break;
                    case EdgeType.Generalization:
                        newEdge.Attr.Color = Color.Red;
                        break;
                    case EdgeType.Type:
                        newEdge.Attr.Color = Color.Blue;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        private void AddNodes()
        {
            foreach (var node in repo.ModelNodes())
            {
                var newNode = graph.FindNode(node.name);
                switch (node.nodeType)
                {
                    case NodeType.Attribute:
                        newNode.Attr.Fillcolor = Color.IndianRed;
                        newNode.Attr.Shape = Shape.Box;
                        break;
                    case NodeType.Node:
                        newNode.Attr.Fillcolor = Color.ForestGreen;
                        newNode.Attr.Shape = Shape.Octagon;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }
    }
}
