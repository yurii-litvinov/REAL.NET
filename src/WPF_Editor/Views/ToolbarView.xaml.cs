using System.Windows.Controls;
using WPF_Editor.ViewModels;

namespace WPF_Editor.Views
{
    /// <summary>
    /// Interaction logic for ToolbarView.xaml
    /// </summary>
    public partial class ToolbarView : UserControl
    {
        public ToolbarView()
        {
            InitializeComponent();
            DataContext = new ToolbarViewModel();
        }
    }
}
