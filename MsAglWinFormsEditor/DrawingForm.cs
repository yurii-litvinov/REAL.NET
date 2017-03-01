using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;
using Color = System.Drawing.Color;

namespace MsAglWinFormsEditor
{
    public partial class DrawingForm : Form
    {
        private readonly List<Tuple<object, Types, Point>> items = new List<Tuple<object, Types, Point>>();


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

        private void canvas_Paint(object sender, PaintEventArgs e)
        {
            canvas.Invalidate();
            foreach (var item in items)
            {
                switch (item.Item2)
                {
                    case Types.Rect:
                        e.Graphics.DrawRectangle(new Pen(Color.Green, 10), (Rectangle)item.Item1);
                        break;
                    case Types.Ellipse:
                        e.Graphics.DrawEllipse(new Pen(Color.Green, 10), (Rectangle)item.Item1);
                        break;
                    case Types.Image:
                        e.Graphics.DrawImage((Image)item.Item1, item.Item3.X, item.Item3.Y);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        private void itemsListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
        }

        private void canvas_Click(object sender, EventArgs e)
        {
            if (itemsListBox.SelectedIndex < 0)
                return;
            var cursorPosition = PointToClient(Cursor.Position);
            cursorPosition.X -= canvas.Left;
            cursorPosition.Y -= canvas.Top;
            var item = items[itemsListBox.SelectedIndex];
            if (item.Item2 == Types.Image)
            {
                items.Add(new Tuple<object, Types, Point>(items[itemsListBox.SelectedIndex].Item1, 
                    items[itemsListBox.SelectedIndex].Item2, cursorPosition));
                items.RemoveAt(itemsListBox.SelectedIndex);
                return;
            }
            var rect = (Rectangle) item.Item1;
            rect.Location = cursorPosition;
            items.Add(new Tuple<object, Types, Point>(rect, items[itemsListBox.SelectedIndex].Item2, new Point()));
            items.RemoveAt(itemsListBox.SelectedIndex);
            itemsListBox.Items.RemoveAt(itemsListBox.SelectedIndex);
            itemsListBox.Items.Add("new Rect");
        }

        private void ShapesComboBoxSelectedIndexChanged(object sender, EventArgs e)
        {
            switch (shapesComboBox.SelectedIndex)
            {
                case 0:
                    items.Add(new Tuple<object, Types, Point>(new Rectangle(10, 10, 10, 10), Types.Rect, new Point()));
                    itemsListBox.Items.Add("Rectangle");
                    break;
                case 1:
                    items.Add(new Tuple<object, Types, Point>(new Rectangle(10, 10, 10, 10), Types.Ellipse, new Point()));
                    itemsListBox.Items.Add("Ellipse");
                    break;
                default:
                    var openImageDialog = new OpenFileDialog();
                    if (openImageDialog.ShowDialog() == DialogResult.OK)
                    {
                        var imagePath = openImageDialog.FileName;
                        items.Add(new Tuple<object, Types, Point>(new Bitmap(imagePath), Types.Image, new Point(10, 10)));
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
