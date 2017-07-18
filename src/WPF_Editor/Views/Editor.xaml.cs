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
        public MainWindow()
        {
            System.Console.WriteLine("Initialization");
            InitializeComponent();
            DataContext = new EditorViewModel();
        }

    }
}
