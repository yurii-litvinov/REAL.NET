using System;
using System.Windows;
using System.Windows.Media;
using GraphX.Controls;

namespace EditorPrototype
{
    public class EditorObjectManager : IDisposable
    {
        private GraphAreaExample _graphArea;
        private ZoomControl _zoomControl;
        private EdgeBlueprint _edgeBp;
        private ResourceDictionary _rd;

        public EditorObjectManager(GraphAreaExample graphArea, ZoomControl zc)
        {
            _graphArea = graphArea;
            _zoomControl = zc;
            _rd = new ResourceDictionary
            {
                Source = new Uri("pack://application:,,,/Templates/EditorTemplate.xaml",
                    UriKind.RelativeOrAbsolute)
            };
        }

        public void CreateVirtualEdge(VertexControl source, Point targetPos)
        {
            _zoomControl.MouseMove += _zoomControl_MouseMove;
            _edgeBp = new EdgeBlueprint(source, targetPos, (LinearGradientBrush)_rd["EdgeBrush"]);
            _graphArea.InsertCustomChildControl(0, _edgeBp.EdgePath);
        }

        private void _zoomControl_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (_edgeBp == null)
            {
                return;
            }
            var pos = _zoomControl.TranslatePoint(e.GetPosition(_zoomControl), _graphArea);
            pos.Offset(2, 2);
            _edgeBp.UpdateTargetPosition(pos);
        }

        private void ClearEdgeBp()
        {
            if (_edgeBp == null) return;
            _graphArea.RemoveCustomChildControl(_edgeBp.EdgePath);
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
}