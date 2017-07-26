using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Repo;
using WPF_Editor.ViewModels.Interfaces;

namespace WPF_Editor.ViewModels
{
    public class MediatorViewModel : ISceneMediatorViewModel, IPaletteMediatorViewModel
    {
        
        private static MediatorViewModel _mediator;
        private ISceneViewModel _scene;
        private IPaletteViewModel _palette;
        private IRepo repo;
        private IModel _currentMetamodel;
        private IModel _currentModel;

        private readonly IRepo _repo = RepoFactory.CreateRepo();


        public static MediatorViewModel CreateMediator()
        {
            if (_mediator is null)
            {
                _mediator = new MediatorViewModel();
            }
            return _mediator;
        }
       

        private MediatorViewModel()
        {
            //Find out, why doesn't _currentModel.Metamodel.Elements contain any elements?
            repo = RepoFactory.CreateRepo();
            _currentMetamodel = _repo.Model("RobotsMetamodel");
            _currentModel = repo.CreateModel("New model based on RobotsMetamodel", _currentMetamodel);
            foreach (var element in _currentMetamodel.Elements)
            {
                System.Console.WriteLine(element.Name);
            }
            _scene = SceneViewModel.CreateScene(this);
            _palette = PaletteViewModel.CreatePalette(this);
        }

        public IEnumerable<IElement> metamodelElements => _currentMetamodel.Elements;
        public IEnumerable<INode> metamodelNodes => _currentMetamodel.Nodes;
        public IEnumerable<IEdge> metamodelEdges => _currentMetamodel.Edges;
    }
}
