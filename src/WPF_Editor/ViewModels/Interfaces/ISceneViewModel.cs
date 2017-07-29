using GraphX.Controls;
using System.Windows;
namespace WPF_Editor.ViewModels.Interfaces
{
    public interface ISceneViewModel
    {
        void InitializeScene(ZoomControl zoomControl);
        void HandleSingleLeftClick(Point position);
    }
}
