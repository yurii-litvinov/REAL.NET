using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Windows.Forms;
using Microsoft.Msagl.Drawing;
using Microsoft.Msagl.GraphViewerGdi;
using Microsoft.Msagl.Splines;
using Repo;
using Color = Microsoft.Msagl.Drawing.Color;
using Point = Microsoft.Msagl.Point;

namespace MsAglWinFormsEditor
{
    /// <summary>
    /// Main project form
    /// </summary>
    public partial class MainForm : Form
    {
        private readonly IRepo repo = RepoFactory.CreateRepo();
        private readonly Graph graph = new Graph("graph");
        private readonly GViewer viewer = new GViewer();

        private readonly Hashtable imagesHashtable = new Hashtable();
        private Node selectedNode;

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

            viewer.PanButtonPressed = true;
            viewer.ToolBarIsVisible = false;
            viewer.MouseWheel += (sender, args) => viewer.ZoomF += args.Delta * SystemInformation.MouseWheelScrollLines / 4000f;
            viewer.Dock = DockStyle.Fill;
            mainLayout.Controls.Add(viewer, 0, 0);
            ResumeLayout();
            InitPalette();
        }

        private readonly List<EdgeType> edgeTypes = new List<EdgeType>
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
                    //TODO: uncomment it when addEdge will be imlemented 
                    //repo.AddEdge(edgeType.ToString(), edge.Source, edge.Target);
                    form.Close();
                    UpdateGraph();
                };
                tableLayout.Controls.Add(associationButton, 0, tableLayout.RowCount - 1);
                ++tableLayout.RowCount;

            }
            form.Show();
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
                newNode.Attr.LabelMargin = Left;
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
                var button = new Button { Text = type.name, Dock = DockStyle.Bottom };
                button.Click += (sender, args) => CreateNewNode(type.id);

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
            UpdateGraph();
        }

        private void ViewerMouseClicked(object sender, MouseEventArgs e)
        {
            var selectedObject = viewer.SelectedObject;
            selectedNode = selectedObject as Node;
            if (selectedNode != null)
            {
                var attributeInfos = selectedNode.UserData as List<AttributeInfo>;
                if (attributeInfos != null)
                {
                    attributeTable.Visible = true;
                    loadImageButton.Visible = true;
                    viewer.PanButtonPressed = false;
                    var image = imagesHashtable[selectedNode.Id] as Image;
                    if (image != null)
                    {
                        imageLayoutPanel.Visible = true;
                        widthEditor.Value = image.Width;
                        heightEditor.Value = image.Height;
                    }
                    else
                    {
                        imageLayoutPanel.Visible = false;
                    }
                    attributeTable.Rows.Clear();
                    foreach (var info in attributeInfos)
                    {
                        object[] row = { info.name, repo.Node(info.attributeType).name, info.value };
                        attributeTable.Rows.Add(row);
                    }
                }
            }
            else
            {
                attributeTable.Visible = false;
                loadImageButton.Visible = false;
                imageLayoutPanel.Visible = false;
                viewer.PanButtonPressed = true;
            }
        }

        private void LoadImageButtonClick(object sender, EventArgs e)
        {
            var openImageDialog = new OpenFileDialog();
            if (openImageDialog.ShowDialog() == DialogResult.OK)
            {
                imagesHashtable[selectedNode.Id] = new Bitmap(openImageDialog.FileName);
                selectedNode.Attr.Shape = Shape.DrawFromGeometry;
                selectedNode.DrawNodeDelegate = DrawNode;
                selectedNode.NodeBoundaryDelegate = GetNodeBoundary;
                UpdateGraph();
            }
        }

        private void UpdateGraph()
        {
            viewer.Graph = graph;
            viewer.Invalidate();
        }

        private GraphicsPath FillTheGraphicsPath(ICurve iCurve)
        {
            var curve = iCurve as Curve;
            var path = new GraphicsPath();
            if (curve != null)
            {
                foreach (var seg in curve.Segments)
                    AddSegmentToPath(seg, ref path);
            }
            return path;
        }

        private void AddSegmentToPath(ICurve seg, ref GraphicsPath p)
        {
            var line = seg as LineSegment;
            if (line != null)
            {
                p.AddLine(PointF(line.Start), PointF(line.End));
            }
        }

        private ICurve GetNodeBoundary(Node node)
        {
            var image = imagesHashtable[node.Id] as Image;
            double width = image.Width;
            double height = image.Height;

            return CurveFactory.CreateRectangle(width, height, new Point());
        }

        private PointF PointF(Point p)
            => new PointF((float)p.X, (float)p.Y);

        private bool DrawNode(Node node, object graphics)
        {
            var graphic = (Graphics)graphics;
            var image = imagesHashtable[node.Id] as Image;
            using (Matrix matrix = graphic.Transform)
            {
                using (Matrix matrixClone = matrix.Clone())
                {
                    graphic.SetClip(FillTheGraphicsPath(node.Attr.GeometryNode.BoundaryCurve));
                    using (var matrixMult = new Matrix(1, 0, 0, -1, 0, 2 * (float)node.Attr.GeometryNode.Center.Y))
                        matrix.Multiply(matrixMult);

                    graphic.Transform = matrix;
                    graphic.DrawImage(image, new PointF((float)(node.Attr.GeometryNode.Center.X - node.Attr.GeometryNode.Width / 2),
                        (float)(node.Attr.GeometryNode.Center.Y - node.Attr.GeometryNode.Height / 2)));
                    graphic.Transform = matrixClone;
                    graphic.ResetClip();
                }
            }
            return true;//returning false would enable the default rendering
        }
        
        private void RefreshButtonClick(object sender, EventArgs e) 
            => viewer.Graph = graph;

        private void ImageSizeChanged(object sender, EventArgs e)
        {
            var image = imagesHashtable[selectedNode.Id] as Image;
            imagesHashtable[selectedNode.Id] = ResizeImage(image, Convert.ToInt32(widthEditor.Value), Convert.ToInt32(heightEditor.Value));
            UpdateGraph();
        }

        public static Image ResizeImage(Image image, int width, int height)
        {
            var destRect = new Rectangle(0, 0, width, height);
            var destImage = new Bitmap(width, height);

            destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (var graphics = Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                using (var wrapMode = new ImageAttributes())
                {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }

            return destImage;
        }

        private void InsertingEdgeCheckedChanged(object sender, EventArgs e)
        {
            viewer.InsertingEdge = !viewer.InsertingEdge;
        }
    }
}
