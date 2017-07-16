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
using Color = Microsoft.Msagl.Drawing.Color;
using Point = Microsoft.Msagl.Point;

namespace MsAglWinFormsEditor
{
    /// <summary>
    /// Main project form
    /// </summary>
    public partial class MainForm : Form
    {
        private readonly RepoExperimental.IRepo repo = RepoExperimental.RepoFactory.CreateRepo();
        private readonly Graph graph = new Graph("graph");
        private readonly GViewer viewer = new GViewer();

        private readonly Hashtable imagesHashtable = new Hashtable();
        private Node selectedNode;

        private const string modelName = "RobotsTestModel";
        private RepoExperimental.IModel currentModel = null;
        private Dictionary<RepoExperimental.IElement, string> ids 
                = new Dictionary<RepoExperimental.IElement, string>();
        private int idCounter;

        /// <summary>
        /// Create form with given graph
        /// </summary>
        public MainForm()
        {
            currentModel = repo.Model(modelName);

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

        private void ViewerOnEdgeAdded(object sender, EventArgs eventArgs)
        {
            var edge = sender as Edge;
            var form = new Form();
            var tableLayout = new TableLayoutPanel { Dock = DockStyle.Fill };
            form.Controls.Add(tableLayout);

            foreach (var type in currentModel.Metamodel.Elements)
            {
                if (type.InstanceMetatype != RepoExperimental.Metatype.Edge || type.IsAbstract)
                {
                    continue;
                }

                var edgeType = type as RepoExperimental.IEdge;

                tableLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 25));
                // TODO: We definitely need names for non-abstract edges. Maybe as attribute?
                var associationButton = new Button { Text = "Link", Dock = DockStyle.Fill };
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

        private void FormatEdge(RepoExperimental.IEdge edgeData, Edge edge)
        {
            // TODO: Add some means to differentiate edges.
        }

        private void AddEdges()
        {
            foreach (var edge in currentModel.Edges)
            {
                var newEdge = graph.AddEdge(id(edge.From), id(edge.To));
                FormatEdge(edge, newEdge);
            }
        }

        private void AddNodes()
        {
            foreach (var node in currentModel.Nodes)
            {
                var newNode = graph.FindNode(id(node));
                newNode.UserData = node.Attributes;
                newNode.Attr.LabelMargin = Left;
                newNode.LabelText = node.Name;

                newNode.Attr.FillColor = Color.IndianRed;
                newNode.Attr.Shape = Shape.Box;
            }
        }

        private void InitPalette()
        {
            foreach (var type in currentModel.Metamodel.Elements)
            {
                if (type.IsAbstract)
                {
                    continue;
                }

                if (type.InstanceMetatype == RepoExperimental.Metatype.Node)
                {
                    var node = type as RepoExperimental.INode;
                    var button = new Button { Text = node.Name, Dock = DockStyle.Bottom };
                    button.Click += (sender, args) => CreateNewNode(node);

                    // TODO: Bind it to Designer, do not do GUI work in C#.
                    paletteGrid.Controls.Add(button, 0, paletteGrid.RowCount - 1);

                    ++paletteGrid.RowCount;
                }
            }
        }

        private void CreateNewNode(RepoExperimental.INode type)
        {
            var newNodeInRepo = currentModel.CreateElement(type) as RepoExperimental.INode;

            var newNode = graph.AddNode(id(newNodeInRepo));
            newNode.LabelText = newNodeInRepo.Name;
            newNode.UserData = new List<RepoExperimental.IAttribute>();

            newNode.Attr.FillColor = Color.IndianRed;
            newNode.Attr.Shape = Shape.Box;

            UpdateGraph();
        }

        private void ViewerMouseClicked(object sender, MouseEventArgs e)
        {
            var selectedObject = viewer.SelectedObject;
            selectedNode = selectedObject as Node;
            if (selectedNode != null)
            {
                var attributes = selectedNode.UserData as List<RepoExperimental.IAttribute>;
                if (attributes != null)
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
                    foreach (var attribute in attributes)
                    {
                        object[] row = { attribute.Name, attribute.Kind.ToString(), attribute.StringValue };
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

        private string id(RepoExperimental.IElement element)
        {
            if (!ids.ContainsKey(element))
            {
                ids.Add(element, (++idCounter).ToString());
            }

            return ids[element];
        }
    }
}
