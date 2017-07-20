using System;
using WPF_Editor.Models.Interfaces;
using WPF_Editor.Models;
using System.Windows.Input;
using System.Windows;

namespace REAL.NET.Models
{
	public class Scene : IScene
    {
        private ISceneMediator sceneMediator { get; }
        private static IScene scene;

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
            
        }

        private Scene(ISceneMediator sceneMediator)
        {
            this.sceneMediator = sceneMediator;
		}

    }
}
