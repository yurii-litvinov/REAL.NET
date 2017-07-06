namespace EditorPrototype
{
    using System;
    using System.Windows;
    using System.Windows.Media;
    using GraphX.Controls;

    public class EditorObjectManager : IDisposable
    {
        public EditorObjectManager(GraphAreaExample graphArea, ZoomControl zc)
        {
            GraphArea = graphArea;
            ZoomControl = zc;
            Rd = new ResourceDictionary
            {
                Source = new Uri("pack://application:,,,/Templates/EditorTemplate.xaml",
                    UriKind.RelativeOrAbsolute)
            };

        }

        public void CreateVirtualEdge(VertexControl source, Point targetPos)
        {
            ZoomControl.MouseMove += ZoomControl_MouseMove;
            EdgeBp = new EdgeBlueprint(source, targetPos, (LinearGradientBrush)Rd["EdgeBrush"]);
            GraphArea.InsertCustomChildControl(0, EdgeBp.EdgePath);
        }

        public void Dispose()
        {
            ClearEdgeBp();
            GraphArea = null;
            if (ZoomControl != null)
            {
                ZoomControl.MouseMove -= ZoomControl_MouseMove;
            }

            ZoomControl = null;
            Rd = null;
        }

        public void DestroyVirtualEdge()
        {
            ClearEdgeBp();
        }

        private GraphAreaExample GraphArea;
        private ZoomControl ZoomControl;
        private EdgeBlueprint EdgeBp;
        private ResourceDictionary Rd;

        private void ZoomControl_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (EdgeBp == null)
            {
                return;
            }

            var pos = ZoomControl.TranslatePoint(e.GetPosition(ZoomControl), GraphArea);
            pos.Offset(2, 2);
            EdgeBp.UpdateTargetPosition(pos);
        }

        private void ClearEdgeBp()
        {
            if (EdgeBp == null) return;
            GraphArea.RemoveCustomChildControl(EdgeBp.EdgePath);
            EdgeBp.Dispose();
            EdgeBp = null;
        }

    }

}