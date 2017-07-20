namespace REAL.NET.ViewModels
{
    using REAL.NET.Models;
    using Repo;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Collections.ObjectModel;
    using System;

    class Node : INode
    {
        private INode node { get; }
        public string Name { get => node.Name; set => node.Name = value;}

        public IElement Class => node.Class;

        public IEnumerable<IAttribute> Attributes => node.Attributes;

        public bool IsAbstract => node.IsAbstract;

        public Metatype Metatype => node.Metatype;

        public Metatype InstanceMetatype => node.InstanceMetatype;

        public string Shape => node.Shape;

        public Node(INode inode)
        {
            node = inode;
        }
    }
    class Edge : IEdge
    {
        private IEdge edge { get; }
        public IElement From { get => edge.From; set => edge.From = value; }
        public IElement To { get => edge.To; set => edge.To = value; }

        public IElement Class => edge.Class;

        public IEnumerable<IAttribute> Attributes => edge.Attributes;

        public bool IsAbstract => edge.IsAbstract;

        public Metatype Metatype => edge.Metatype;

        public Metatype InstanceMetatype => edge.InstanceMetatype;

        public string Shape => edge.Shape;
        public Edge(IEdge iedge)
        {
            edge = iedge;
        }
    }
    class EditorViewModel : INotifyPropertyChanged
    {
        
        private IRepo repo;
        private AppConsole console = new AppConsole();
        private string modelName = "RobotsMetamodel";
        #region Extern all console members to another class

        public bool ConsoleVisibility => console.VisibilityStatus;

        public ConsoleWindow ErrorConsole { get; private set; }

        public ConsoleWindow MessageConsole { get; private set; }

        public event PropertyChangedEventHandler PropertyChanged;
        #endregion
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