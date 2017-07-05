using System;
using System.Windows;
using System.Windows.Media;
using GraphX.Controls;

namespace EditorPrototype
{
    using System;
    using System.Windows;
    using System.Windows.Media;
    using GraphX.Controls;

    public class EditorObjectManager : IDisposable
    {
        private GraphAreaExample graphArea;
        private ZoomControl zoomControl;
        private EdgeBlueprint edgeBp;
        private ResourceDictionary rd;
        
        public EditorObjectManager(GraphAreaExample graphArea, ZoomControl zc)
        {
            this.graphArea = graphArea;
            this.zoomControl = zc;
            this.rd = new ResourceDictionary
            {
                Source = new Uri(
                    "pack://application:,,,/Templates/EditorTemplate.xaml",
                    UriKind.RelativeOrAbsolute)
            };

        }

        public bool VirtualSource { get => this.virtualSource; set => this.virtualSource = value; }


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


        public void CreateVirtualEdge(VertexControl source, Point targetPos)
        {
            if (_edgeBp == null) return;
            _graphArea.RemoveCustomChildControl(_edgeBp.EdgePath);
            _edgeBp.Dispose();
            _edgeBp = null;

        }

        public void Dispose()
        {
            this.ClearEdgeBp();
            this.graphArea = null;
            if (this.zoomControl != null)
            {
                this.zoomControl.MouseMove -= this.ZoomControl_MouseMove;
            }

            this.zoomControl = null;
            this.rd = null;
        }
    }  

}
