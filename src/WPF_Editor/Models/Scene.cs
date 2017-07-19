using System;
using WPF_Editor.Models.Interfaces;
using WPF_Editor.Models;
namespace REAL.NET.Models
{
    class Scene : IScene
    {
        public ISceneMediator Scene_mediator { get; }

        private static IScene scene;

        public static IScene CreateScene(ISceneMediator scene_mediator)
        {
            if (scene is null)
                scene = new Scene(scene_mediator);
            return scene;
        }

        public void HandleLeftSingleClick(object clickInfo)
        {
            throw new NotImplementedException();
        }

        private Scene(ISceneMediator sceneMediator)
        {
            Scene_mediator = sceneMediator;
        }
    }
}
