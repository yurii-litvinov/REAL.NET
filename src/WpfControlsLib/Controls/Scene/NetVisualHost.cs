using System;
using System.Windows;
using System.Windows.Media;

namespace WpfControlsLib.Controls.Scene
{
    /// <summary>
    /// Class that provides net for scene
    /// </summary>
    public class NetVisualHost : FrameworkElement
    {
        /// <summary>
        /// Create a collection of child visual objects
        /// </summary>
        private readonly VisualCollection children;

        /// <summary>
        /// Init parameters and create new visual collection
        /// </summary>
        /// <param name="height">Height of the scene on the screen</param>
        /// <param name="width">Width of the scene on the screen</param>
        /// <param name="scale">The scale of net check</param>
        /// <param name="point">The point that user presses on to move scene</param>
        public NetVisualHost(double height, double width, double scale, Point point)
        {
            children = new VisualCollection(this)
            {
                CreateDrawingVisualLines(height, width, scale, point),
            };
        }

        /// <summary>
        /// Draw the net
        /// </summary>
        /// <param name="height">Height of the scene on the screen</param>
        /// <param name="width">Width of the scene on the screen</param>
        /// <param name="scale">The scale of net check</param>
        /// <param name="point">The point that user presses on to move scene</param>
        /// <returns></returns>
        private DrawingVisual CreateDrawingVisualLines(double height, double width, double scale, Point point)
        {
            DrawingVisual drawingVisual = new DrawingVisual();
            DrawingContext drawingContext = drawingVisual.RenderOpen();

            point = Geometry.RoundPosition(point, scale);

            Pen drawingpen = new Pen(Brushes.WhiteSmoke, 1);
            for (var i = point.Y; i < height + point.Y; i += scale)
            {
                drawingContext.DrawLine(drawingpen, new Point(-width + point.X, i), new Point(width + point.X, i));
            }
            for (var i = point.Y; i > -height + point.Y; i -= scale)
            {
                drawingContext.DrawLine(drawingpen, new Point(-width + point.X, i), new Point(width + point.X, i));
            }
            for (var i = point.X; i < width + point.X; i += scale)
            {
                drawingContext.DrawLine(drawingpen, new Point(i, -height + point.Y), new Point(i, height + point.Y));
            }
            for (var i = point.X; i > -width + point.X; i -= scale)
            {
                drawingContext.DrawLine(drawingpen, new Point(i, -height + point.Y), new Point(i, height + point.Y));
            }

            drawingContext.Close();
            return drawingVisual;
        }

        /// <summary>
        /// Provide a required override for the VisualChildrenCount property
        /// </summary>
        protected override int VisualChildrenCount => children.Count;

        /// <summary>
        /// Provide a required override for the GetVisualChild method
        /// </summary>
        protected override Visual GetVisualChild(int index)
        {
            if (index < 0 || index >= children.Count)
            {
                throw new ArgumentOutOfRangeException();
            }

            return children[index];
        }
    }
}
