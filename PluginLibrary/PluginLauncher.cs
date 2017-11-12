using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace PluginLibrary
{
    public class PluginLauncher
    {
        public IList<IPlugin> Plugins => pluginsList;

        private List<IPlugin> pluginsList = new List<IPlugin>();
       
        public void LaunchPlugins(string folder)
        {
            var files = Directory.GetFiles(folder, "*Plugin*.dll");
            var assemblies = new List<Assembly>();
            foreach (var file in files)
            {
                try
                {
                    var assembly = Assembly.LoadFrom(file);
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
                    if (type.IsAbstract)
                    {
                        continue;
                    }
                    var interfaces = type.GetInterfaces();
                    foreach (var interFace in interfaces)
                    {
                        if (interFace.Name == "IPlugin")
                        {
                            var constructor = type.GetConstructor(Type.EmptyTypes);
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