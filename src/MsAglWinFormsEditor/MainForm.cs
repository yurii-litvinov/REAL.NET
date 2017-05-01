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
using Point = Microsoft.Msagl.Point;

namespace MsAglWinFormsEditor
{
    /// <summary>
    /// Main project form. Realisation for all user events
    /// </summary>
    public partial class MainForm : Form
    {
        private readonly GViewer viewer = new GViewer();
        private readonly MsAglGraphRepresentation graph = new MsAglGraphRepresentation();
        private readonly Hashtable imagesHashtable = new Hashtable();
        private Node selectedNode;

        /// <summary>
        /// Create form with given graph
        /// </summary>
        public MainForm()
        {
            viewer.MouseClick += ViewerMouseClicked;
            InitializeComponent();
            viewer.EdgeAdded += ViewerOnEdgeAdded;
            SuspendLayout();

            viewer.Graph = graph.GetGraph();
            viewer.PanButtonPressed = true;
            viewer.ToolBarIsVisible = false;
            viewer.MouseDown += ViewerOnMouseDown;
            viewer.MouseWheel += (sender, args) => viewer.ZoomF += args.Delta * SystemInformation.MouseWheelScrollLines / 4000f;
            viewer.Dock = DockStyle.Fill;
            mainLayout.Controls.Add(viewer, 0, 0);
            ResumeLayout();
            InitPalette();
        }

        private void ViewerOnMouseDown(object sender, MouseEventArgs mouseEventArgs)
        {
            var selectedObject = viewer.SelectedObject;
            selectedNode = selectedObject as Node;
            viewer.PanButtonPressed = selectedNode == null;
        }

        private readonly List<EdgeType> edgeTypes = new List<EdgeType>
        {
            EdgeType.Generalization,
            EdgeType.Association,
            EdgeType.Attribute,
            EdgeType.Type,
            EdgeType.Value
        };

        private void ViewerOnEdgeAdded(object sender, EventArgs eventArgs)
        {
            var edge = sender as Edge;
            var form = new Form();
            var tableLayout = new TableLayoutPanel { Dock = DockStyle.Fill };
            form.Controls.Add(tableLayout);
            form.ControlBox = false;

            foreach (var edgeType in edgeTypes)
            {
                tableLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 25));
                var associationButton = new Button { Text = edgeType.ToString(), Dock = DockStyle.Fill };
                associationButton.Click += (o, args) =>
                {
                    graph.FormatEdge(edgeType, edge);
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

        private void InitPalette()
        {
            foreach (var type in graph.GetNodeTypes())
            {
                var button = new Button { Text = type.name, Dock = DockStyle.Bottom };
                button.Click += (sender, args) => { graph.CreateNewNode(type.id); UpdateGraph(); };
                paletteGrid.Controls.Add(button, 0, paletteGrid.RowCount - 1);

                ++paletteGrid.RowCount;
            }
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
                        object[] row = { info.name, graph.GetAttributeName(info.attributeType), info.value };
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
            var openImageDialog = new OpenFileDialog {Filter = @"Image files *.png | *.png"};
            if (openImageDialog.ShowDialog() == DialogResult.OK)
            {
                var newImage = new Bitmap(openImageDialog.FileName);
                imagesHashtable[selectedNode.Id] = newImage;
                selectedNode.Attr.Shape = Shape.DrawFromGeometry;
                selectedNode.DrawNodeDelegate = DrawNode;
                selectedNode.NodeBoundaryDelegate = GetNodeBoundary;
                widthEditor.Value = newImage.Width;
                heightEditor.Value = newImage.Height;
                imageLayoutPanel.Visible = true;
                UpdateGraph();
            }
        }

        private void UpdateGraph()
        {
            viewer.Graph = graph.GetGraph();
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
            var image = (Image) imagesHashtable[node.Id];
            double width = image.Width;
            double height = image.Height;

            return CurveFactory.CreateRectangle(width, height, new Point());
        }

        private PointF PointF(Point p)
            => new PointF((float)p.X, (float)p.Y);

        private bool DrawNode(Node node, object graphics)
        {
            var graphic = (Graphics) graphics;
            var image = (Image) imagesHashtable[node.Id];
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
            return true; // Returning false would enable the default rendering
        }
        
        private void RefreshButtonClick(object sender, EventArgs e) 
            => viewer.Graph = graph.GetGraph();

        private void ImageSizeChanged(object sender, EventArgs e)
        {
            var image = imagesHashtable[selectedNode.Id] as Image;
            imagesHashtable[selectedNode.Id] = ResizeImage(image, Convert.ToInt32(widthEditor.Value), Convert.ToInt32(heightEditor.Value));
            UpdateGraph();
        }

        private Image ResizeImage(Image image, int width, int height)
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
            => viewer.InsertingEdge = !viewer.InsertingEdge;
    }
}
