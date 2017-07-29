using System.Windows.Controls;
using System.Windows.Input;
using WPF_Editor.ViewModels;
using WPF_Editor.ViewModels.Interfaces;

namespace WPF_Editor.Views
{
    /// <summary>
    ///     Interaction logic for SceneView.xaml
    /// </summary>
    public partial class SceneView : UserControl
    {
        private readonly ISceneViewModel _scene;

        public SceneView()
        {
            InitializeComponent();
            _scene = SceneViewModel.CreateScene();
            _scene.InitializeScene(zoomctrl);
            DataContext = _scene;
        }

        private void HandleSingleLeftClick(object sender, MouseButtonEventArgs e)
        {
            var position = Mouse.GetPosition(this);
            position = TranslatePoint(position, graphArea);
            _scene.HandleSingleLeftClick(position);
        }
    }
}