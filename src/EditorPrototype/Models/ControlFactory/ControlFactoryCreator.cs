using System;
using EditorPluginInterfaces;
using EditorPluginInterfaces.Toolbar;
using EditorPrototype.Models.InternalConsole;

namespace EditorPrototype.Models.ControlFactory
{
    public class ControlFactoryCreator
    {
        private static IControlFactory factory;

        private ControlFactoryCreator()
        { }

        public static IControlFactory CreateControlFactory() => factory ?? new ControlFactory();

        private class ControlFactory : IControlFactory
        {
            private IConsole console;

            public IConsole CreateConsole() => this.console ?? new AppConsole();

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
