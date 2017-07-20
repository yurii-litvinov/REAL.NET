using System;
using WPF_Editor.Models.Interfaces;
using WPF_Editor.Models;
using System.Windows.Input;
using System.Windows;
using Repo;
using System.Collections.Generic;
using WPF_Editor.ViewModels;
using REAL.NET.ViewModels;
namespace REAL.NET.Models
{
	class Scene : IScene
    {
        private ISceneMediator sceneMediator { get; }
        private static IScene scene;
        private List<INode> nodeList = new List<INode>();
        public static IScene CreateScene(ISceneMediator scene_mediator)
        {
            if (scene is null)
            {
                scene = new Scene(scene_mediator);
            }
            return scene;
        }

        public void HandleClick(object sender, MouseButtonEventArgs e)
        {
            Point mouseCoordinates = Mouse.GetPosition(null);
            Element element = sceneMediator.GetSelectedPaletteItem();

            if(element is Node)
            {
                INode node = (INode)element;
                System.Console.WriteLine("Create node with name \"{0}\" at position:", node.Name);
                System.Console.WriteLine("X={0} Y={1}", mouseCoordinates.X, mouseCoordinates.Y);
                nodeList.Add(node);
                System.Console.WriteLine("Node \"{0}\" has been saved at scene.", node.Name);
                
                System.Console.WriteLine();
                return;
            }
            if(element is null)
            {
                System.Console.WriteLine("Element hasn't been selected.");
                System.Console.WriteLine();
                return;
            }
            throw new Exception("Something went wrong...");

        }

        private Scene(ISceneMediator sceneMediator)
        {
            this.sceneMediator = sceneMediator;
            
		}

    }
}
