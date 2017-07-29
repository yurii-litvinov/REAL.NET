using System.Windows.Controls;
using WPF_Editor.ViewModels;

namespace WPF_Editor.Views
{
    /// <summary>
    ///     Interaction logic for PluginbarView.xaml
    /// </summary>
    public partial class PluginbarView : UserControl
    {
        public PluginbarView()
        {
            InitializeComponent();
            DataContext = new PluginbarViewModel();
        }
    }
}