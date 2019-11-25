using System;
using System.Collections.Generic;
using System.ComponentModel;
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

namespace LogoScene.Controls
{
    /// <summary>
    /// Логика взаимодействия для TurtleControl.xaml
    /// </summary>
    public partial class TurtleControl : UserControl
    {
        [Bindable(true)]
        public double TurtleViewX
        {
            get => (double)GetValue(TurtleViewXProperty);
            set => SetValue(TurtleViewXProperty, value);
        }

        [Bindable(true)]
        public double TurtleViewY
        {
            get => (double)GetValue(TurtleViewYProperty);
            set => SetValue(TurtleViewYProperty, value);
        }

        public TurtleControl()
        {
            InitializeComponent();
            TurtleViewXProperty = DependencyProperty.Register("TurtleViewX", typeof(double), typeof(TurtleControl));
            TurtleViewYProperty = DependencyProperty.Register("TurtleViewY", typeof(double), typeof(TurtleControl));
        }

        private readonly DependencyProperty TurtleViewXProperty;

        private readonly DependencyProperty TurtleViewYProperty;
    }
}
