using System;
using EditorPrototype.Models.Console;
using EditorPrototype.Models.Palette;
using EditorPrototype.Models.Scene;
using EditorPrototype.Models.Toolbar;

namespace EditorPrototype.Models.ControlFactory
{
    public class ControlFactoryCreator
    {
        private IControlFactory factory;

        private class ControlFactory : IControlFactory
        {
            private IToolbar toolbar;

            public IConsole CreateConsole()
            {
                throw new NotImplementedException();
            }

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
                lock (this)
                {
                    if (toolbar == null)
                    {
                        return new Toolbar.Toolbar();
                    }
                    else
                    {
                        return toolbar;
                    }
                }
            }
        }

        public IControlFactory CreateControlFactory()
        {
            lock (this)
            {
                if (factory == null)
                {
                    return new ControlFactory();
                }
                else
                {
                    return factory;
                }
            }
        }
    }
}
