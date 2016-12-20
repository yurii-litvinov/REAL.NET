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
using System.Windows.Shapes;

namespace EditorPrototype
{
    /// <summary>
    /// Логика взаимодействия для EntitySelectWindow.xaml
    /// </summary>
    public partial class EntitySelectWindow : Window
    {
        public EntitySelectWindow()
        {
            InitializeComponent();
        }

        private void vertexAddBut_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.VertexWindow.Owner = Owner;
            Close();
            MainWindow.VertexWindow.ShowDialog();
        }

        private void edgeAddBut_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.EdgeWindow.Owner = Owner;
            Close();
            MainWindow.EdgeWindow.FillEdgeBoxes();
            MainWindow.EdgeWindow.ShowDialog();
        }
    }
}
