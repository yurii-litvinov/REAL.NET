using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPF_Editor.Models.Interfaces;

namespace REAL.NET.Models
{
    //if you add a new component, class has to implement IComponentNameMediator and a component has to implement IComponentName.
    //At least there's has to be one connection(interface reference) from one to another.
    //It has to be defined in IComponentName or IComponentNameMediator interface.
    //Each component like palette, for example, has to be defined once.
    //You can do it using private constructor and special static method CreateComponent. See Scene implementation.

    /// <summary>
    ///This class provides connection between components like palette, console, toolbar, etc.
    ///Now there are only palette and scene.
    /// </summary>
    public class Mediator : ISceneMediator, IPaletteMediator, IAppConsoleMediator
    {
        private static Mediator mediator;

        public IScene Scene { get; }

        public IPalette Palette { get; }

        public IAppConsole AppConsole { get; }

        public static Mediator CreateMediator()
        {
            if (mediator is null)
            {
                mediator = new Mediator();
            }
            return mediator;
        }

        private Mediator()
        {
            /* Property this.Scene and class' name are the same. So there's need in full path to class Scene.*/
            Scene = Models.Scene.CreateScene(this);
            /* Property this.Palette and class' name are the same. So there's need in full path to class Palette.*/
            Palette = Models.Palette.CreatePalette(this);
            AppConsole = new Models.AppConsole();
        }
        
    }
}
