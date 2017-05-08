namespace EditorPrototype
{
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

    /// <summary>
    /// Логика взаимодействия для ConstraintsWindow.xaml
    /// </summary>
    public partial class ConstraintsWindow : Window
    {
        private RepoInfo info;

        public ConstraintsWindow(Repo.IRepo repo, string modelName)
        {
            this.InitializeComponent();
            this.info = new RepoInfo(repo, modelName);
        }

        public void ObjType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var a = this.info.GetEdges().ToList();
            var b = a[0].edgeType;
            var c = b.ToString();
            var d = this.info.GetEdges().Select(x => new ConstraintsValues { Nodes = x.edgeType.ToString() }).ToList();
            var v = d[0];
            Console.WriteLine(a);
            switch (((ComboBoxItem)this.ObjType.SelectedItem).Content.ToString())
            {
                case "Node":
                    this.TypeCombobox.ItemsSource = new List<ConstraintsValues>(this.info.GetEdges().Select(x => new ConstraintsValues { Nodes = x.edgeType.ToString() }).ToList());

                    break;
                case "Edge":
                    break;
            }
        }
    }
}
