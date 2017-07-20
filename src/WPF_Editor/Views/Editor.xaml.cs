using System;
using System.Collections.Generic;
namespace REAL.NET
{
    using System.Windows;
    using REAL.NET.ViewModels;
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class Editor : Window
    {
        IScene scene { get; }
        IPalette palette { get; }

        public Editor()
        {
            InitializeComponent();
            DataContext = viewModel;
            scene = ((EditorViewModel)DataContext).Mediator.Scene;
            palette = ((EditorViewModel)DataContext).Mediator.Palette;
        }

        private void Scene_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            scene.HandleClick(sender, e);
        }
    }
}
