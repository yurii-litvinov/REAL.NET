using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Microsoft.Msagl.GraphViewerGdi;
using Repo;
using Color = Microsoft.Msagl.Drawing.Color;
using Edge = Microsoft.Msagl.Drawing.Edge;
using Graph = Microsoft.Msagl.Drawing.Graph;
using Node = Microsoft.Msagl.Drawing.Node;
using Shape = Microsoft.Msagl.Drawing.Shape;

namespace MsAglWinFormsEditor
{
    /// <summary>
    /// Main project form
    /// </summary>
    public partial class MainForm : Form
    {

        private readonly Repo.Repo repo = RepoFactory.CreateRepo();
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
            viewer.EdgeAdded += ViewerOnEdgeAdded;
            viewer.Graph = graph;

            SuspendLayout();
            viewer.Dock = DockStyle.Fill;
            mainLayout.Controls.Add(viewer, 0, 0);
            ResumeLayout();
            InitPalette();
        }

        private List<EdgeType> edgeTypes = new List<EdgeType>
        {
            EdgeType.Generalization,
            EdgeType.Association,
            EdgeType.Attribute,
            EdgeType.Type
        };

        private void ViewerOnEdgeAdded(object sender, EventArgs eventArgs)
        {
            var edge = sender as Edge;
            var form = new Form();
            var tableLayout = new TableLayoutPanel { Dock = DockStyle.Fill };
            form.Controls.Add(tableLayout);

            foreach (var edgeType in edgeTypes)
            {
                tableLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 25));
                var associationButton = new Button { Text = edgeType.ToString(), Dock = DockStyle.Fill };
                associationButton.Click += (o, args) =>
                {
                    FormatEdge(edgeType, edge);
                    form.Close();
                    viewer.Invalidate();
                };
                tableLayout.Controls.Add(associationButton, 0, tableLayout.RowCount - 1);
                ++tableLayout.RowCount;
            }

            form.ShowDialog(this);
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
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void AddEdges()
        {
            foreach (var edge in repo.ModelEdges())
            {
                var newEdge = graph.AddEdge(edge.source, edge.target);
                FormatEdge(edge.edgeType, newEdge);
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

        private void InitPalette()
        {
            foreach (var type in repo.MetamodelNodes())
            {
                var button = new Button { Text = type.name };
                EventHandler createNode = (sender, args) => CreateNewNode(type.id);
                button.Click += createNode;

                // TODO: Bind it to Designer, do not do GUI work in C#.
                paletteGrid.Controls.Add(button, 0, paletteGrid.RowCount - 1);

                ++paletteGrid.RowCount;
            }
        }

        private void CreateNewNode(string typeId)
        {
            var newNodeInfo = repo.AddNode(typeId);

            var newNode = graph.AddNode(graph.NodeCount.ToString());
            newNode.LabelText = "New " + newNodeInfo.nodeType.ToString();
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
            viewer.Graph = graph;
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
