using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using Repo;
using WPF_Editor.Models;
using WPF_Editor.Models.Console;

namespace WPF_Editor.ViewModels
{
    public class EditorViewModel : INotifyPropertyChanged
    {
        private const string ModelName = "RobotsMetamodel";

        #region Put all console members in separate class
        private AppConsole console = new AppConsole();

        public bool ConsoleVisibility => console.VisibilityStatus;

        public ConsoleWindow ErrorConsole { get; private set; }

        public ConsoleWindow MessageConsole { get; private set; }
        #endregion

        public event PropertyChangedEventHandler PropertyChanged;
        public Mediator Mediator { get; }
        //Try to use expression body methods.
        private IModel Model { get; set; }

        public ObservableCollection<Node> Nodes { get; set; }

        private ObservableCollection<Edge> Edges { get; set; }

        public EditorViewModel()
        {
            Mediator = Mediator.CreateMediator();
            var repo = RepoFactory.CreateRepo();
            Model = repo.Model(ModelName);
            
            var interfaceNodes = new ObservableCollection<INode>(Model.Nodes);
            Nodes = new ObservableCollection<Node>();
            foreach(var node in interfaceNodes)
            {
                Nodes.Add(new Node(node));
            }

            var interfaceEdges = new ObservableCollection<IEdge>(Model.Edges);
            Edges = new ObservableCollection<Edge>();
            foreach (var edge in interfaceEdges)
            {
                Edges.Add(new Edge(edge));
            }

            #region Put all console members in separate class
            //Extern all console members to another class
            this.MessageConsole = console.GetConsoleWindowByName("MessageConsole");
            this.ErrorConsole = console.GetConsoleWindowByName("ErrorConsole");
            ErrorConsole.NewMessage("Error");
            MessageConsole.NewMessage("Message");
            console.ShowConsole();
            #endregion
        }
        #region Put all console members in separate class
        public void ChangeConsoleVisibilityStatus()
        {
            if (ConsoleVisibility)
            {
                console.HideConsole();
            }
            else
            {
                console.ShowConsole();
            }
            OnPropertyChanged("ConsoleVisibility");
        }

        private void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        #endregion
    }
}