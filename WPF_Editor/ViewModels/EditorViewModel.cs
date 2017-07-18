namespace REAL.NET.ViewModels
{
    using System;
    using REAL.NET.Models;
    using REAL.NET.Models.FakeRepo;
    using Repo;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;

    class EditorViewModel
    {
        private IRepo repo;
        public ObservableCollection<NodeInfo> NodeCollection { get; }
        public ObservableCollection<EdgeInfo> EdgeCollection { get; }

        public EditorViewModel()
        {
            repo = new FakeRepo();
            NodeCollection = new ObservableCollection<NodeInfo>(repo.ModelNodes("FakeModel"));
            EdgeCollection = new ObservableCollection<EdgeInfo>(repo.ModelEdges("FakeModel"));
        }

    }
}
