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
using System.ComponentModel; // CancelEventArgs

namespace EditorPrototype
{
    /// <summary>
    /// Логика взаимодействия для EdgeCreationWindow.xaml
    /// </summary>
    public partial class EdgeCreationWindow : Window
    {
        public EdgeCreationWindow()
        {
            InitializeComponent();
            FillEdgeBoxes();
            Closing += CancelClosing;
            edgeSourceBox.SelectionChanged += CheckOtherControls;
            edgeTargetBox.SelectionChanged += CheckOtherControls;
        }

        private void CheckOtherControls(object sender, SelectionChangedEventArgs e)
        {
            if (edgeSourceBox.SelectedItem != null && edgeTargetBox.SelectedItem != null)
            {
                addEdgeBut.IsEnabled = true;
            }
        }

        private void CancelClosing(object sender, CancelEventArgs e)
        {
            if (!MainWindow.close)
            {
                e.Cancel = true;
                ClearWindow(sender, e);
            }
        }

        private void ClearWindow(object sender, EventArgs e)
        {
            edgeTextBox.Text = string.Empty;
            edgeSourceBox.Items.Clear();
            edgeTargetBox.Items.Clear();
            addEdgeBut.IsEnabled = false;
            Visibility = Visibility.Hidden;
        }

        public void FillEdgeBoxes()
        {
            var vertices = MainWindow.dataGraph.Vertices.ToList();
            var number_of_vertices = vertices.Count;
            for (int i = 0; i < number_of_vertices; i++)
            {
                edgeSourceBox.Items.Add(vertices[i].Name);
                edgeTargetBox.Items.Add(vertices[i].Name);
            }
        }

        public event EventHandler EdgeAddButClicked;

        private void addEdgeButton_Click(object sender, RoutedEventArgs e)
        {
            int source = edgeSourceBox.SelectedIndex;
            int target = edgeTargetBox.SelectedIndex;
            var vertices = MainWindow.dataGraph.Vertices.ToList();
            MainWindow.newEdge = new DataEdge(vertices[source], vertices[target]) { Text = edgeTextBox.Text };
            MainWindow.dataGraph.AddEdge(MainWindow.newEdge);
            ClearWindow(sender, e);
            EdgeAddButClicked(this, EventArgs.Empty);
        }

        private void edgeCancelBut_Click(object sender, RoutedEventArgs e)
        {
            ClearWindow(sender, e);
        }

    }
}
