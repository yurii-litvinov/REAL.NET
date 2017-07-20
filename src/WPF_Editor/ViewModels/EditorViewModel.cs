namespace REAL.NET.ViewModels
{
    using REAL.NET.Models;
    using Repo;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Collections.ObjectModel;
    using System;

    public class EditorViewModel : INotifyPropertyChanged
    {
        private IRepo repo;

        private string modelName = "RobotsMetamodel";

        #region Extern all console members to another class

        private IAppConsole console;

        public bool ConsoleVisibility => console.IsVisible;

        public string MessageConsoleText => console.GetConsoleWindowByName("MessageConsole").ToString();

        public string ErrorConsoleText => console.GetConsoleWindowByName("ErrorConsole").ToString();

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
            console = new AppConsole();
            repo = RepoFactory.CreateRepo();
            Model = repo.Model(modelName);
            var interfaceNodes = Model.Nodes;
            Nodes = new ObservableCollection<Node>();
            foreach(var node in interfaceNodes)
            {
                Nodes.Add(new Node(node));
            }
            #region Extern all console members to another class
            //Extern all console members to another class
            console.NewMessageToConsoleWindowByName("ErrorConsole", "error");
            console.NewMessageToConsoleWindowByName("MessageConsole", "message");
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