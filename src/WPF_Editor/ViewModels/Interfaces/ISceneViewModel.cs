using System.Windows;
using GraphX.Controls;

namespace WPF_Editor.ViewModels.Interfaces
{
    public interface ISceneViewModel
    {
        void InitializeScene(ZoomControl zoomControl);
        void HandleSingleLeftClick(Point position);
    }
}