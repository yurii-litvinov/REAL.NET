using System.Windows;
using System.Windows.Input;
using Repo;
using WPF_Editor.Models.Interfaces;
using WPF_Editor.ViewModels;
using System.Collections.Generic;
namespace WPF_Editor.Models
{
	public class Scene : IScene
	{
	    private readonly ISceneMediator _sceneMediator;
	    private Node _lastCreatedElement;
	    private static IScene _scene;
        //Fix. Initialize from constructor and check if it's a loaded project.
        private List<Node> _nodeList = new List<Node>();
        private List<Edge> _edgeList = new List<Edge>();

	    public IElement LastCreatedElement => _lastCreatedElement;
        
	    //private readonly List<INode> _nodeList = new List<INode>();
	    public static IScene CreateScene(ISceneMediator sceneMediator = null)
        {
            if (_scene is null)
            {
                _scene = new Scene(sceneMediator);
            }
            return _scene;
        }



	    private Scene(ISceneMediator sceneMediator)
        {
            _sceneMediator = sceneMediator;
            
        }

	    public void CreateNode()
	    {
	        IElement element = _sceneMediator.GetSelectedPaletteItem();
	        if (element is INode)
	        {
	            var node = new Node(element as Node);
                _nodeList.Add(node);
	            _lastCreatedElement = node;
	        }
	    }
	}
}
