using System.Windows.Controls;
using WPF_Editor.ViewModels;

namespace WPF_Editor.Views
{
    /// <summary>
    /// Interaction logic for PaletteView.xaml
    /// </summary>
    public partial class PaletteView : UserControl
    {
        public PaletteView()
        {
            InitializeComponent();
            DataContext = PaletteViewModel.CreatePalette();
        }
    }
}
