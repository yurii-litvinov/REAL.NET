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
            this.EdgesListEnum = new List<string>() { "Vasya", "Petya", "Evgenii Olegovich" };
        }

        public List<string> EdgesListEnum { get; set; }
    }
}
