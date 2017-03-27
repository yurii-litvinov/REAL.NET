using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;
using Color = System.Drawing.Color;

namespace MsAglWinFormsEditor
{
    /// <summary>
    /// Form for drawing node shape
    /// </summary>
    public partial class DrawingForm : Form
    {
        private readonly List<Item> items = new List<Item>();

        private class Item
        {
            public Types Type { get; set; }

            public Point Position { get; set; }

            public object Figure { get; set; }
        }

        private enum Types
        {
            Rect,
            Ellipse,
            Image
        }

        public DrawingForm()
        {
            InitializeComponent();
        }

        private void OnCanvasPaint(object sender, PaintEventArgs e)
        {
            canvas.Invalidate();
            foreach (var item in items)
            {
                switch (item.Type)
                {
                    case Types.Rect:
                        e.Graphics.DrawRectangle(new Pen(Color.Green, 10), (Rectangle)item.Figure);
                        break;
                    case Types.Ellipse:
                        e.Graphics.DrawEllipse(new Pen(Color.Green, 10), (Rectangle)item.Figure);
                        break;
                    case Types.Image:
                        e.Graphics.DrawImage((Image)item.Figure, item.Position.X, item.Position.Y);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        private void OnCanvasClick(object sender, EventArgs e)
        {
            if (itemsListBox.SelectedIndex < 0)
                return;
            var cursorPosition = PointToClient(Cursor.Position);
            cursorPosition.X -= canvas.Left;
            cursorPosition.Y -= canvas.Top;
            var item = items[itemsListBox.SelectedIndex];
            if (item.Type == Types.Image)
            {
                items.Add(new Item
                {
                    Figure = items[itemsListBox.SelectedIndex].Figure,
                    Type = items[itemsListBox.SelectedIndex].Type,
                    Position = cursorPosition
                });
                items.RemoveAt(itemsListBox.SelectedIndex);
                return;
            }
            var rect = (Rectangle)item.Figure;
            rect.Location = cursorPosition;
            items.Add(new Item
            {
                Figure = rect,
                Type = items[itemsListBox.SelectedIndex].Type,
                Position = new Point()
            }
            );
            items.RemoveAt(itemsListBox.SelectedIndex);
            itemsListBox.Items.RemoveAt(itemsListBox.SelectedIndex);
            itemsListBox.Items.Add("new Rect");
        }

        private void ShapesComboBoxSelectedIndexChanged(object sender, EventArgs e)
        {
            switch (shapesComboBox.SelectedIndex)
            {
                case 0:
                    items.Add(new Item
                    {
                        Figure = new Rectangle(10, 10, 10, 10),
                        Type = Types.Rect,
                        Position = new Point()
                    });
                    itemsListBox.Items.Add("Rectangle");
                    break;
                case 1:
                    items.Add(new Item
                    {
                        Figure = new Rectangle(10, 10, 10, 10),
                        Type = Types.Ellipse,
                        Position = new Point()
                    });
                    itemsListBox.Items.Add("Ellipse");
                    break;
                default:
                    var openImageDialog = new OpenFileDialog();
                    if (openImageDialog.ShowDialog() == DialogResult.OK)
                    {
                        var imagePath = openImageDialog.FileName;
                        items.Add(new Item
                        {
                            Figure = new Bitmap(imagePath),
                            Type = Types.Image,
                            Position = new Point(10, 10)
                        });
                        itemsListBox.Items.Add("Image");
                    }
                    break;
            }
        }

        private void SaveClick(object sender, EventArgs e)
        {
            var dialog = new SaveFileDialog
            {
                Filter = @"Png Image|*.png",
                Title = @"Save an Image File",
                FileName = "node.png"
            };
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                var width = Convert.ToInt32(canvas.Width);
                var height = Convert.ToInt32(canvas.Height);
                var bmp = new Bitmap(width, height);
                canvas.DrawToBitmap(bmp, new Rectangle(0, 0, width, height));
                bmp.Save(dialog.FileName, ImageFormat.Jpeg);
            }
        }
    }
}
