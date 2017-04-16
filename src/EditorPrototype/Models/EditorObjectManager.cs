using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;
using GraphX.Controls;
using GraphX.Controls.Models;

namespace EditorPrototype
{
    public class EditorObjectManager : IDisposable
    {
        private GraphAreaExample _graphArea;
        private ZoomControl _zoomControl;
        private EdgeBlueprint _edgeBp;
        private ResourceDictionary _rd;

        public bool virtualSource;
        public bool virtualTarget;

        public EditorObjectManager(GraphAreaExample graphArea, ZoomControl zc)
        {
            _graphArea = graphArea;
            _zoomControl = zc;
            _rd = new ResourceDictionary
            {
                Source = new Uri("pack://application:,,,/Templates/EditorTemplate.xaml",
                    UriKind.RelativeOrAbsolute)
            };
            virtualSource = false;
            virtualTarget = false;
        }

        public void CreateVirtualEdge(VertexControl source, Point targetPos)
        {
            _zoomControl.MouseMove += _zoomControl_MouseMove;
            _edgeBp = new EdgeBlueprint(source, targetPos, (LinearGradientBrush)_rd["EdgeBrush"]);
            _graphArea.InsertCustomChildControl(0, _edgeBp.EdgePath);
        }        

        void _zoomControl_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (_edgeBp == null) return;
            var pos = _zoomControl.TranslatePoint(e.GetPosition(_zoomControl), _graphArea);
            pos.Offset(2, 2);
            _edgeBp.UpdateTargetPosition(pos);
        }

        private void ClearEdgeBp()
        {
            if (_edgeBp == null) return;
            _graphArea.RemoveCustomChildControl(_edgeBp.EdgePath);//
            _edgeBp.Dispose();
            _edgeBp = null;
        }

        public void Dispose()
        {
            ClearEdgeBp();
            _graphArea = null;
            if (_zoomControl != null)
            {
                _zoomControl.MouseMove -= _zoomControl_MouseMove;
            }
            _zoomControl = null;
            _rd = null;
        }

        public void DestroyVirtualEdge()
        {
            ClearEdgeBp();
        }
    }

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

