using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Repo;
using WPF_Editor.ViewModels.Helpers;
using WPF_Editor.ViewModels.Interfaces;

namespace WPF_Editor.ViewModels
{
    public class MediatorViewModel : ISceneMediatorViewModel, IPaletteMediatorViewModel
    {
        
        private static MediatorViewModel _mediator;
        private ISceneViewModel _scene;
        private IPaletteViewModel _palette;
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
            _currentMetamodel = _repo.Model("RobotsMetamodel");
            _currentModel = _repo.CreateModel("New model based on RobotsMetamodel", _currentMetamodel);
            foreach (var element in _currentModel.Metamodel.Elements)
            {
                System.Console.WriteLine(element.Name);
            }
            _scene = SceneViewModel.CreateScene(this);
            _palette = PaletteViewModel.CreatePalette(this);
        }

        public IEnumerable<IElement> MetamodelElements => _currentModel.Metamodel.Elements;
        public IEnumerable<INode> MetamodelNodes => _currentModel.Metamodel.Nodes;
        public IEnumerable<IEdge> MetamodelEdges => _currentModel.Metamodel.Edges;
        public ModelElement GetInstanceOfSelectedType()
        {
            MetamodelElement element = _palette.SelectedElement;
            if (element is null)
                return null;
            IElement instance = _currentModel.CreateElement(element.IElement);

            if (instance is INode)
            {
                return new ModelNode((INode)instance);
            }
            throw new NotImplementedException("Cannot create edge.");
        }
    }
}
