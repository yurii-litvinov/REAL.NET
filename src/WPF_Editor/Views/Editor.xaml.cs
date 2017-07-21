using System.Windows;
using WPF_Editor.Models.Interfaces;
using WPF_Editor.ViewModels;

namespace WPF_Editor.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class Editor : Window
    {
        IScene scene { get; }

        IPalette palette { get; }
        
        private EditorViewModel viewModel;

        public Editor()
        {
            InitializeComponent();
            viewModel = new EditorViewModel();
            DataContext = viewModel;
            scene = viewModel.Mediator.Scene;
            palette = viewModel.Mediator.Palette;
        }

        private void Scene_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            scene.HandleClick(sender, e);
        }

        private void OnHideShowButtonClick(object sender, RoutedEventArgs e)
        {
            viewModel.ChangeConsoleVisibilityStatus();
        }
    }
}
