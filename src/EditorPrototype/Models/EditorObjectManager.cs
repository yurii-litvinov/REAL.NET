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

        public void CreateVirtualEdge(VertexControl source, Point targetPos)
        {
            this.zoomControl.MouseMove += ZoomControl_MouseMove;
            this.edgeBp = new EdgeBlueprint(source, targetPos, (LinearGradientBrush)this.rd["EdgeBrush"]);
            this.graphArea.InsertCustomChildControl(0, this.edgeBp.EdgePath);
        }

        public void Dispose()
        {
            ClearEdgeBp();
            this.graphArea = null;
            if (this.zoomControl != null)
            {
                this.zoomControl.MouseMove -= ZoomControl_MouseMove;
            }

            this.zoomControl = null;
            this.rd = null;
        }

        public void DestroyVirtualEdge()
        {
            ClearEdgeBp();
        }
        
        private void ZoomControl_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (this.edgeBp == null)
            {
                return;
            }

            var pos = zoomControl.TranslatePoint(e.GetPosition(this.zoomControl), this.graphArea);
            pos.Offset(2, 2);
            this.edgeBp.UpdateTargetPosition(pos);
        }

        private void ClearEdgeBp()
        {
            if (this.edgeBp == null) return;
            this.graphArea.RemoveCustomChildControl(this.edgeBp.EdgePath);
            this.edgeBp.Dispose();
            this.edgeBp = null;
        }

        public EditorObjectManager(GraphAreaExample graphAreaEx, ZoomControl zc)
        {
            this.graphArea = graphAreaEx;
            this.zoomControl = zc;
            this.rd = new ResourceDictionary
            {
                Source = new Uri("pack://application:,,,/Templates/EditorTemplate.xaml", UriKind.RelativeOrAbsolute)
            };
        }
    }
}