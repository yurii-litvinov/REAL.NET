namespace REAL.NET.ViewModels
{
    using REAL.NET.Models;
    using Repo;
    using System.Collections.ObjectModel;
    using System;
    using System.ComponentModel;
    class EditorViewModel : INotifyPropertyChanged
    {
        
        private IRepo repo;
        private AppConsole console = new AppConsole();
        private string modelName = "RobotsMetamodel";
        #region Extern all console members to another class

        public bool ConsoleVisibility => console.VisibilityStatus;

        public ConsoleWindow ErrorConsole { get; private set; }

        public ConsoleWindow MessageConsole { get; private set; }
        #endregion

        public event PropertyChangedEventHandler PropertyChanged;
        public Mediator Mediator { get; }
        //Try to use expression body methods.
        private IModel Model { get; set; }
        public ObservableCollection<Node> Nodes { get; set; }
        public ObservableCollection<Edge> Edges { get; set; }

        public EditorViewModel()
        {
            Mediator = Mediator.CreateMediator();
            repo = RepoFactory.CreateRepo();
            Model = repo.Model(modelName);
            
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

            #region Extern all console members to another class
            //Extern all console members to another class
            this.MessageConsole = console.GetConsoleWindowByName("MessageConsole");
            this.ErrorConsole = console.GetConsoleWindowByName("ErrorConsole");
            ErrorConsole.NewMessage("Error");
            MessageConsole.NewMessage("Message");
            console.ShowConsole();
            #endregion
        }
        #region Extern all console members to another class
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

        public void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        #endregion
    }
}