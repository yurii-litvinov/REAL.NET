using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace PluginLibrary
{
    public class PluginLauncher
    {
        public IList<IPlugin> Plugins => pluginsList;

        public void PrintAvailiblePlugins()
        {
            foreach (var plugin in pluginsList)
            {
                Console.WriteLine(plugin.Name);
            }
        }

        private List<IPlugin> pluginsList;
       
        public void LaunchPlugins(string folder = "plugins")
        {
            var files = Directory.GetFiles(folder, "*Plugin*.dll");
            var assemblies = new List<Assembly>();
            foreach (var file in files)
            {
                try
                {
                    var assembly = Assembly.Load(file);
                    assemblies.Add(assembly);
                }
                catch (FileLoadException)
                {
                    
                }
            }
            foreach (var assembly in assemblies)
            {
                var types = assembly.GetTypes();
                foreach (var type in types)
                {
                    var interfaces = type.GetInterfaces();
                    foreach (var interFace in interfaces)
                    {
                        if (interFace is IPlugin)
                        {
                            var constructor = type.GetConstructor(new Type[]{});
                            if (constructor == null)
                            {
                                continue;
                            }
                            object almostPlugin = constructor.Invoke(new object[] { });
                            var plugin = almostPlugin as IPlugin;
                            pluginsList.Add(plugin);
                        }
                    }
                }
            }
        }
    }
}