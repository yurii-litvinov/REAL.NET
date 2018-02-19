namespace WpfEditor.Controls.Scene
{
    using GraphX.Controls;

    public class StaticVertexConnectionPointForGH : StaticVertexConnectionPoint
    {
        public StaticVertexConnectionPointForGH()
            : base()
        {
        }

        public bool IsSource
        {
            get;
            set;
        }

        public bool IsOccupied
        {
            get;
            set;
        }
    }
}
