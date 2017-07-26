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
using WPF_Editor.ViewModels;

namespace WPF_Editor.Views
{
    /// <summary>
    /// Interaction logic for ConsoleView.xaml
    /// </summary>
    public partial class ConsoleView : UserControl
    {
        
        //private ConsoleViewModel viewModel = new ConsoleViewModel();
        
        public ConsoleView()
        {
            InitializeComponent();
            //DataContext = viewModel;
        }

        //private void OnHideShowButtonClick(object sender, RoutedEventArgs e)
        //{
        //    viewModel.ChangeConsoleVisibility();
        //}
    }
}
