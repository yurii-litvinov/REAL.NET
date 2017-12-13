using System;
using EditorPrototype.Models.InternalConsole;
using EditorPrototype.Models.Palette;
using EditorPrototype.Models.Scene;
using EditorPrototype.Models.Toolbar;

namespace EditorPrototype.Models.ControlFactory
{
    public class ControlFactoryCreator
    {
        private static volatile IControlFactory factory;

        private static object SyncRoot = new Object();

        private ControlFactoryCreator()
        { }

        private class ControlFactory : IControlFactory
        {
            private IConsole console;

            public IConsole CreateConsole() => console ?? new AppConsole();

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

        public static IControlFactory CreateControlFactory()
        {
            if (factory == null)
            {
                lock (SyncRoot)
                {
                    if (factory == null)
                    {
                        return new ControlFactory();
                    }
                }
            }
            return factory;
        }
    }
}
