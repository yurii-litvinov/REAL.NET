using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Microsoft.Msagl.Drawing;
using Microsoft.Msagl.GraphViewerGdi;
using Repo;
using Color = Microsoft.Msagl.Drawing.Color;
using Graph = Microsoft.Msagl.Drawing.Graph;
using Shape = Microsoft.Msagl.Drawing.Shape;

namespace MsAglWinFormsEditor
{
    /// <summary>
    /// Main project form
    /// </summary>
    public partial class MainForm : Form
    {

        private readonly Repo.IRepo repo = RepoFactory.CreateRepo();
        private readonly Graph graph = new Graph("graph");
        private readonly GViewer viewer = new GViewer();

        /// <summary>
        /// Create form with given graph
        /// </summary>
        public MainForm()
        {
            viewer.MouseClick += ViewerMouseClicked;
            InitializeComponent();
            AddEdges();
            AddNodes();
            viewer.Graph = graph;

            SuspendLayout();
            viewer.Dock = DockStyle.Fill;
            mainLayout.Controls.Add(viewer, 0, 0);
            ResumeLayout();
        }

        private void AddEdges()
        {
            foreach (var edge in repo.ModelEdges())
            {
                var newEdge = graph.AddEdge(edge.source, edge.edgeType.ToString(), edge.target);
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
                newNode.UserData = node.attributes;
                switch (node.nodeType)
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
            }
        }

        private void ViewerMouseClicked(object sender, MouseEventArgs e)
        {
            var selectedObject = viewer.SelectedObject;
            var attributeInfos = (selectedObject as Node)?.UserData as List<AttributeInfo>;
            if (attributeInfos != null)
            {
                attributeTable.Visible = true;
                attributeTable.Rows.Clear();
                foreach (var info in attributeInfos)
                {
                    object[] row = { info.name, repo.Node(info.attributeType).name, info.value };
                    attributeTable.Rows.Add(row);
                }
            }
            else
            {
                attributeTable.Visible = false;
            }
        }
        
    }
}
