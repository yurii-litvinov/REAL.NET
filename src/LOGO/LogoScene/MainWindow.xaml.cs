using System.Windows;
using System.Windows.Controls;

namespace LogoScene
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            //Canvas.SetLeft(turtleControl, 10);
            //Canvas.SetTop(turtleControl, 10);
            var a = turtleControl.ActualHeight;
        }
    }
}
