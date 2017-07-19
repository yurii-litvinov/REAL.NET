using System;
using System.Collections.Generic;
namespace REAL.NET
{
    using System.Windows;
    using REAL.NET.ViewModels;
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private EditorViewModel viewModel = new EditorViewModel();

        public MainWindow()
        {
            InitializeComponent();
            DataContext = viewModel;
        }

        private void OnShowOrMinimizeConsoleButtonClick(object sender, RoutedEventArgs e) => viewModel.ChangeConsoleVisibilityStatus();
    }
}
