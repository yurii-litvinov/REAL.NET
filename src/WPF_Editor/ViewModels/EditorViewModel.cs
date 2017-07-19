namespace REAL.NET.ViewModels
{
    using REAL.NET.Models;
    using REAL.NET.Models.FakeRepo;
    using Repo;
    using System.Collections.ObjectModel;
    using System.ComponentModel;

    class EditorViewModel : INotifyPropertyChanged
    {
        private IRepo repo;
        private AppConsole console = new AppConsole();

        public bool ConsoleVisibility => console.VisibilityStatus;

        public ConsoleWindow ErrorConsole { get; private set; }

        public ConsoleWindow MessageConsole { get; private set; }

        public event PropertyChangedEventHandler PropertyChanged;

        public ObservableCollection<NodeInfo> NodeCollection { get; }
        public ObservableCollection<EdgeInfo> EdgeCollection { get; }
        public Mediator mediator;


        public EditorViewModel()
        {
            mediator = Mediator.CreateMediator();
            repo = new FakeRepo();
            NodeCollection = new ObservableCollection<NodeInfo>(repo.ModelNodes("FakeModel"));
            EdgeCollection = new ObservableCollection<EdgeInfo>(repo.ModelEdges("FakeModel"));
            this.MessageConsole = console.GetConsoleWindowByName("MessageConsole");
            this.ErrorConsole = console.GetConsoleWindowByName("ErrorConsole");
            ErrorConsole.NewMessage("error");
            MessageConsole.NewMessage("message");
            console.ShowConsole();

        }

        public void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    }
}
