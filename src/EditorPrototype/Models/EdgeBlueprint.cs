namespace EditorPrototype
{
    using System;
    using System.Windows;
    using System.Windows.Media;
    using System.Windows.Shapes;
    using GraphX.Controls;
    using GraphX.Controls.Models;

    public class EdgeBlueprint : IDisposable
    {
        public VertexControl Source { get; set; }

        public Point TargetPos { get; set; }

        public Path EdgePath { get; set; }

        public EdgeBlueprint(VertexControl source, Point targetPos, Brush brush)
        {
            EdgePath = new Path() { Stroke = brush, Data = new LineGeometry() };
            Source = source;
            Source.PositionChanged += Source_PositionChanged;
        }

        void Source_PositionChanged(object sender, VertexPositionEventArgs args)
        {
            UpdateGeometry(Source.GetCenterPosition(), TargetPos);
        }

        internal void UpdateTargetPosition(Point point)
        {
            TargetPos = point;
            if (Source != null)
            {
                UpdateGeometry(Source.GetCenterPosition(), point);
            }

        }

        private void UpdateGeometry(Point start, Point end)
        {
            EdgePath.Data = new LineGeometry(start, end);
            (EdgePath.Data as LineGeometry).Freeze();
        }

        public void Dispose()
        {
            if (Source != null)
            {
                Source.PositionChanged -= Source_PositionChanged;
                Source = null;
            }

        }

    }

}
