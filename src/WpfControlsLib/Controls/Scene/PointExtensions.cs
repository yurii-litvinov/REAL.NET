using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfControlsLib.Controls.Scene
{
    public static class PointExtensions
    {
        public static System.Windows.Point ToWindowsPoint(this Repo.VisualPoint point) => new System.Windows.Point(point.X, point.Y);

        public static Repo.VisualPoint ToVisualPoint(this System.Windows.Point point) => new Repo.VisualPoint(point.X, point.Y);
    }
}
