using System;
using EditorPluginInterfaces;
using EditorPluginInterfaces.Toolbar;
using EditorPrototype.Models.InternalConsole;

namespace EditorPrototype.Models.ControlFactory
{
    public class ControlFactoryCreator
    {
        private static readonly IControlFactory factory = new ControlFactory();

        private ControlFactoryCreator()
        { }

        public static IControlFactory CreateControlFactory() => factory;

        private class ControlFactory : IControlFactory
        {
            private readonly IConsole console = new AppConsole();

            public IConsole CreateConsole() => this.console;

            public IScene CreateNewScene()
            {
                throw new NotImplementedException();
            }

            public IPalette CreatePalette()
            {
                throw new NotImplementedException();
            }

            public IScene CreateScene()
            {
                throw new NotImplementedException();
            }

            public IToolbar CreateToolbar()
            {
                throw new NotImplementedException();
            }
        }
    }
}
