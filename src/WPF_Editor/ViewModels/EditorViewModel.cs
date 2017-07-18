namespace REAL.NET.ViewModels
{
    using System;
    using REAL.NET.Models;
    using REAL.NET.Models.FakeRepo;
    using Repo;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;

    class EditorViewModel : INotifyPropertyChanged
    {
        private IRepo repo;

        public ConsoleWindow ErrorConsole { get; private set; }

        public ConsoleWindow MessageConsole { get; private set; }

        public event PropertyChangedEventHandler PropertyChanged;

        public ObservableCollection<NodeInfo> NodeCollection { get; }

        public ObservableCollection<EdgeInfo> EdgeCollection { get; }


        public EditorViewModel()
        {
            repo = new FakeRepo();
            NodeCollection = new ObservableCollection<NodeInfo>(repo.ModelNodes("FakeModel"));
            EdgeCollection = new ObservableCollection<EdgeInfo>(repo.ModelEdges("FakeModel"));
            this.MessageConsole = new ConsoleWindow();
            this.ErrorConsole = new ConsoleWindow();
        }

        public void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    }
}
