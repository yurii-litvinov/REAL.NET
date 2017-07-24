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
using WPF_Editor.Models.Interfaces;
using WPF_Editor.ViewModels;

namespace WPF_Editor.Views
{
    //Logic core class
    /// <summary>
    /// Interaction logic for Scene.xaml
    /// </summary>
    public partial class Scene : UserControl
    {
        public Scene()
        {
            InitializeComponent();
            DataContext = new SceneViewModel(graphArea);
        }

        private void Zoomctrl_OnClick(object sender, RoutedEventArgs e)
        {
            ((SceneViewModel)DataContext).HandleSingleLeftClick();
        }
    }
}
