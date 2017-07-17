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
        private ObservableCollection<NodeInfo> nodeCollection;
        private ObservableCollection<EdgeInfo> edgeCollection;
        
        public EditorViewModel()
        {
            repo = new FakeRepo();
            nodeCollection = new ObservableCollection<NodeInfo>(repo.ModelNodes("FakeModel"));
            edgeCollection = new ObservableCollection<EdgeInfo>(repo.ModelEdges("FakeModel"));
        }

        public ObservableCollection<NodeInfo> NodeCollection { get => nodeCollection; set => nodeCollection = value; }
        public ObservableCollection<EdgeInfo> EdgeCollection { get => edgeCollection; set => edgeCollection = value; }
    }
}
