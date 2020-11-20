using Microsoft.Msagl.Core.Geometry.Curves;
using Microsoft.Msagl.Core.Layout;
using Microsoft.Msagl.Drawing;
using Microsoft.Msagl.GraphViewerGdi;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using System.Windows.Forms;
using Edge = Microsoft.Msagl.Drawing.Edge;
using Node = Microsoft.Msagl.Drawing.Node;
using Point = Microsoft.Msagl.Core.Geometry.Point;
using Rectangle = System.Drawing.Rectangle;

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
        private const int fontSize = 11;

        /// <summary>
        /// Create form with given graph
        /// </summary>
        public MainForm()
        {
            this.viewer.MouseClick += this.ViewerMouseClicked;
            this.InitializeComponent();
            this.viewer.EdgeAdded += this.ViewerOnEdgeAdded;

            this.viewer.Graph = this.graph.Graph;
            this.viewer.PanButtonPressed = true;
            this.viewer.MouseMove += (sender, args) =>
            {
                this.viewer.Graph.GeometryGraph.UpdateBoundingBox();
                this.viewer.Invalidate();
            };
            this.viewer.ToolBarIsVisible = false;
            this.viewer.MouseDown += this.ViewerOnMouseDown;
            this.viewer.MouseWheel += (sender, args) => this.viewer.ZoomF += args.Delta * SystemInformation.MouseWheelScrollLines / 4000f;
            this.SuspendLayout();
            this.viewer.Dock = DockStyle.Fill;
            this.mainLayout.Controls.Add(this.viewer, 0, 0);
            this.ResumeLayout();
            this.InitPalette();
        }

        private void ViewerOnMouseDown(object sender, MouseEventArgs mouseEventArgs)
           => this.viewer.PanButtonPressed = this.viewer.SelectedObject == null;

        private void ViewerOnEdgeAdded(object sender, EventArgs eventArgs)
        {
            var edge = (Edge)sender;
            var form = new Form();
            var tableLayout = new TableLayoutPanel { Dock = DockStyle.Fill };
            form.Controls.Add(tableLayout);

            // TODO: GetEdgeTypes or something
            foreach (var type in this.graph.GetNodeTypes())
            {
                if (type.InstanceMetatype != Repo.Metatype.Edge || type.IsAbstract)
                {
                    continue;
                }

                var edgeType = type as Repo.IEdge;

                tableLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 25));
                // TODO: We definitely need names for non-abstract edges. Maybe as attribute?
                var associationButton = new Button { Text = "Link", Dock = DockStyle.Fill };
                associationButton.Click += (o, args) =>
                {
                    this.graph.CreateNewEdge(edgeType, edge);
                    this.viewer.Graph.AddPrecalculatedEdge(edge);
                    this.viewer.SetEdgeLabel(edge, edge.Label);
                    var ep = new EdgeLabelPlacement(this.viewer.Graph.GeometryGraph);
                    ep.Run();
                    this.viewer.Invalidate();
                    form.DialogResult = DialogResult.OK;
                };
                associationButton.Font = new Font(associationButton.Font.FontFamily, fontSize);
                tableLayout.Controls.Add(associationButton, 0, tableLayout.RowCount - 1);
                ++tableLayout.RowCount;
            }
            var result = form.ShowDialog();
            if (result == DialogResult.Cancel)
            {
                this.viewer.Undo();
                this.viewer.Invalidate();
            }
        }

        private void InitPalette()
        {
            foreach (var type in this.graph.GetNodeTypes())
            {
                var button = new Button { Text = type.Name, Dock = DockStyle.Bottom };
                button.Click += (sender, args) =>
                {
                    var node = this.graph.CreateNewNode(type);
                    node.GeometryNode = GeometryGraphCreator.CreateGeometryNode(this.viewer.Graph, this.viewer.Graph.GeometryGraph, node, ConnectionToGraph.Disconnected);
                    var viewNode = this.viewer.CreateIViewerNode(node, this.viewer.Graph.Nodes.ToList()[0].Pos - new Point(250, 0), null);
                    this.viewer.AddNode(viewNode, true);
                    this.viewer.Graph.AddNode(node);
                    this.viewer.Invalidate();
                };
                button.Font = new Font(button.Font.FontFamily, fontSize);
                button.Size = new Size(button.Width, button.Height + 10);
                this.paletteGrid.Controls.Add(button, 0, this.paletteGrid.RowCount - 1);

                ++this.paletteGrid.RowCount;
            }
        }

        private void ViewerMouseClicked(object sender, MouseEventArgs e)
        {
            this.selectedNode = this.viewer.SelectedObject as Node;
            if (this.selectedNode != null)
            {
                var attributes = this.selectedNode.UserData as List<Repo.IAttribute>;
                if (attributes != null)
                {
                    this.attributeTable.Visible = true;
                    this.loadImageButton.Visible = true;
                    this.viewer.PanButtonPressed = false;
                    var image = this.imagesHashtable[this.selectedNode.Id] as Image;
                    if (image != null)
                    {
                        this.imageLayoutPanel.Visible = true;
                        this.widthEditor.Value = image.Width;
                        this.heightEditor.Value = image.Height;
                    }
                    else
                    {
                        this.imageLayoutPanel.Visible = false;
                    }
                    this.attributeTable.Rows.Clear();
                    foreach (var attribute in attributes)
                    {
                        object[] row = { attribute.Name, attribute.Kind.ToString(), attribute.StringValue };
                        this.attributeTable.Rows.Add(row);
                    }
                }
            }
            else
            {
                this.attributeTable.Visible = false;
                this.loadImageButton.Visible = false;
                this.imageLayoutPanel.Visible = false;
                this.viewer.PanButtonPressed = this.viewer.SelectedObject == null;
            }
        }

        private void LoadImageButtonClick(object sender, EventArgs e)
        {
            var openImageDialog = new OpenFileDialog { Filter = @"Image files *.png | *.png" };
            if (openImageDialog.ShowDialog() == DialogResult.OK)
            {
                var newImage = new Bitmap(openImageDialog.FileName);
                this.imagesHashtable[this.selectedNode.Id] = newImage;
                this.widthEditor.Value = newImage.Width;
                this.heightEditor.Value = newImage.Height;
                this.ImageSizeChanged(null, null);
                this.imageLayoutPanel.Visible = true;
            }
        }

        private ICurve NodeBoundaryDelegate(Node node)
        {
            var image = (Image)this.imagesHashtable[node.Id];
            double width = image.Width;
            double height = image.Height;

            return CurveFactory.CreateRectangle(width, height, new Point());
        }

        private GraphicsPath FillTheGraphicsPath(ICurve iCurve)
        {
            var curve = iCurve as Curve;
            var path = new GraphicsPath();
            if (curve != null)
            {
                foreach (var seg in curve.Segments)
                    this.AddSegmentToPath(seg, ref path);
            }
            return path;
        }

        private void AddSegmentToPath(ICurve seg, ref GraphicsPath p)
        {
            var line = seg as LineSegment;
            if (line != null)
            {
                p.AddLine(this.PointF(line.Start), this.PointF(line.End));
            }
        }

        private readonly Font drawFont = new Font("Arial", 10);
        private readonly SolidBrush drawBrush = new SolidBrush(System.Drawing.Color.Black);

        private readonly StringFormat format = new StringFormat() { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };

        private PointF PointF(Point p)
            => new PointF((float)p.X, (float)p.Y);

        private bool DrawNode(Node node, object graphics)
        {
            var graphic = (Graphics)graphics;
            var image = (Image)this.imagesHashtable[node.Id];
            using (Matrix matrix = graphic.Transform)
            {
                using (Matrix matrixClone = matrix.Clone())
                {
                    graphic.SetClip(this.FillTheGraphicsPath(node.GeometryNode.BoundaryCurve));
                    using (var matrixMult = new Matrix(1, 0, 0, -1, 0, 2 * (float)node.GeometryNode.Center.Y))
                        matrix.Multiply(matrixMult);

                    graphic.Transform = matrix;
                    graphic.DrawImage(image, new PointF((float)(node.GeometryNode.Center.X - node.GeometryNode.Width / 2),
                        (float)(node.GeometryNode.Center.Y - node.GeometryNode.Height / 2)));
                    var centerX = (float)node.GeometryNode.Center.X;
                    var centerY = (float)node.GeometryNode.Center.Y;
                    graphic.DrawString(node.LabelText, this.drawFont, this.drawBrush, new PointF(centerX, centerY), this.format);
                    graphic.Transform = matrixClone;
                    graphic.ResetClip();
                }
            }
            return true; // Returning false would enable the default rendering
        }

        private void RefreshButtonClick(object sender, EventArgs e)
        {
            this.viewer.Graph = this.graph.Graph;
            this.viewer.Invalidate();
        }

        private void ImageSizeChanged(object sender, EventArgs e)
        {
            var center = this.selectedNode.GeometryNode.Center;
            var image = this.imagesHashtable[this.selectedNode.Id] as Image;
            this.imagesHashtable[this.selectedNode.Id] = this.ResizeImage(image, Convert.ToInt32(this.widthEditor.Value), Convert.ToInt32(this.heightEditor.Value));

            this.selectedNode.Attr.Shape = Shape.DrawFromGeometry;
            this.selectedNode.DrawNodeDelegate = this.DrawNode;
            this.selectedNode.NodeBoundaryDelegate = this.NodeBoundaryDelegate;
            this.selectedNode.Edges.ToList()[0].GeometryObject.RaiseLayoutChangeEvent(0);
            this.viewer.CreateIViewerNode(this.selectedNode).Node.GeometryNode.Center = center;
            this.viewer.Graph.GeometryGraph.UpdateBoundingBox();

            this.viewer.Invalidate();
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
            => this.viewer.InsertingEdge = !this.viewer.InsertingEdge;
    }
}
