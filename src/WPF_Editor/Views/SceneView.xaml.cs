using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using GraphX.Controls;
using GraphX.Controls.Models;
using GraphX.PCL.Common.Enums;
using GraphX.PCL.Common.Interfaces;
using GraphX.PCL.Common.Models;
using GraphX.PCL.Logic.Algorithms.LayoutAlgorithms;
using GraphX.PCL.Logic.Algorithms.OverlapRemoval;
using GraphX.PCL.Logic.Models;
using QuickGraph;
using WPF_Editor.ViewModels;
using WPF_Editor.ViewModels.Interfaces;

namespace WPF_Editor.Views
{
    /// <summary>
    /// Interaction logic for SceneView.xaml
    /// </summary>
    public partial class SceneView : UserControl
    {
        private ISceneViewModel _scene;

        public SceneView()
        {
            InitializeComponent();
            _scene = SceneViewModel.CreateScene();
            _scene.InitializeScene(zoomctrl);
            DataContext = _scene;
            
        }

        private void Zoomctrl_OnClick(object sender, MouseButtonEventArgs e)
        {

            var position = Mouse.GetPosition(this);
            position = TranslatePoint(position, graphArea);
            _scene.HandleSingleLeftClick(position);
        }
    }
}
