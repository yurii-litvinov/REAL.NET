using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GraphX.Controls;

namespace WPF_Editor.ViewModels.Interfaces
{
    public interface ISceneViewModel
    {
        void InitializeScene(ZoomControl zoomControl);
        ModelElement CreateElement();
    }
}
