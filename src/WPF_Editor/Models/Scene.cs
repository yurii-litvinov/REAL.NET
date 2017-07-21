using System;
using System.Collections.Generic;
using System.Windows.Input;
using Repo;
using WPF_Editor.Models.Interfaces;
using WPF_Editor.ViewModels;

namespace WPF_Editor.Models
{
	public class Scene : IScene
	{
	    private readonly ISceneMediator _sceneMediator;
        private static IScene _scene;
        private readonly List<INode> _nodeList = new List<INode>();
        public static IScene CreateScene(ISceneMediator sceneMediator)
        {
            if (_scene is null)
            {
                _scene = new Scene(sceneMediator);
            }
            return _scene;
        }

        public void HandleClick(object sender, MouseButtonEventArgs e)
        {
            var mouseCoordinates = Mouse.GetPosition(null);
            var element = _sceneMediator.GetSelectedPaletteItem();

            var node = element as Node;
            if(node != null)
            {
                System.Console.WriteLine(@"Node ""{0}"" has been saved at scene.", node.Name);
                System.Console.WriteLine(@"X={0} Y={1}", mouseCoordinates.X, mouseCoordinates.Y);
                _nodeList.Add(node);
                System.Console.WriteLine(@"Node ""{0}"" has been saved at scene.", node.Name);
                
                System.Console.WriteLine();
                return;
            }
            if (!(element is null)) throw new Exception("Something went wrong...");

            System.Console.WriteLine(@"Element hasn't been selected.");
            System.Console.WriteLine();
        }

        private Scene(ISceneMediator sceneMediator) => _sceneMediator = sceneMediator;
	}
}
