using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using WpfControlsLib.ViewModel;

namespace WpfControlsLib.Controls.Scene
{
    public class VirtualVertexData : NodeViewModel
    {
        private string picture = @"View/Pictures/vertex.png";

        public VirtualVertexData() : base()
        {
            this.IsVirtual = true;
            this.Picture = this.picture;
            this.BorderColor = Brushes.Black;
            this.Color = Brushes.Gray;
        }

        public static bool IsVirtualVertex(NodeViewModel node) => node is VirtualVertexData;
    }
}
