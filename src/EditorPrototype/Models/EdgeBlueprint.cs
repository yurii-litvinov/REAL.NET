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
        public EdgeBlueprint(VertexControl source, Point targetPos, Brush brush)
        {
            this.EdgePath = new Path() { Stroke = brush, Data = new LineGeometry() };
            this.Source = source;
            this.Source.PositionChanged += this.Source_PositionChanged;
        }

        public VertexControl Source { get; set; }

        public Point TargetPos { get; set; }

        public Path EdgePath { get; set; }

        public void Dispose()
        {
            if (this.Source != null)
            {
                this.Source.PositionChanged -= this.Source_PositionChanged;
                this.Source = null;
            }
        }

        internal void UpdateTargetPosition(Point point)
        {
            this.TargetPos = point;
            if (this.Source != null)
            {
                this.UpdateGeometry(this.Source.GetCenterPosition(), point);
            }
        }

        private void Source_PositionChanged(object sender, VertexPositionEventArgs args)
        {
            this.UpdateGeometry(this.Source.GetCenterPosition(), this.TargetPos);
        }

        private void UpdateGeometry(Point start, Point end)
        {
            this.EdgePath.Data = new LineGeometry(start, end);
            (this.EdgePath.Data as LineGeometry).Freeze();
        }
    }
}
