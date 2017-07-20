using System;
using System.Collections.Generic;
namespace REAL.NET
{
    using System.Windows;
    using REAL.NET.ViewModels;
    using WPF_Editor.Models.Interfaces;

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
