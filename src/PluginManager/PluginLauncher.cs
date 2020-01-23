/* Copyright 2017-2018 REAL.NET group
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License. */

namespace PluginManager
{
    using EditorPluginInterfaces;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;

    /// <summary>
    /// Class that is responsible for launching and managing plugins.
    /// </summary>
    public class PluginLauncher<T>
    {
        /// <summary>
        /// Gets list of available plugins 
        /// </summary>
        public IList<IPlugin<T>> Plugins => this.pluginsList.AsReadOnly();

        /// <summary>
        /// Gets list of plugins
        /// </summary>
        private readonly List<IPlugin<T>> pluginsList = new List<IPlugin<T>>();

        /// <summary>
        /// Launch plugins from this directory
        /// </summary>
        /// <param name="path">Directory with plugins</param>
        /// <param name="config">Plugins configuration</param>
        /// <exception cref="DirectoryNotFoundException">This directory does not exist</exception>
        /// <exception cref="ArgumentException">Path is just spaces or null string</exception>
        /// <exception cref="PathTooLongException">Path is more than 260 digits</exception>
        /// <exception cref="FileLoadException">Can't load some assemblies from this directory</exception>
        public void LaunchPlugins(string path, T config)
        {
            var blacklist = new List<string> { "PluginLibrary.dll", "EditorPluginInterfaces.dll" };
            var assemblies = Directory.GetFiles(path, "*Plugin*.dll")
                .Where(f => blacklist.All(lib => !f.Contains(lib)))
                .Select(Assembly.LoadFrom);

            foreach (var assembly in assemblies)
            {
                var types = assembly.GetTypes();
                foreach (var type in types)
                {
                    if (type.IsAbstract
                        || type.GetInterfaces().All(x => x.Name != "IPlugin`1")
                        || type.GetConstructor(Type.EmptyTypes) == null)
                    {
                        continue;
                    }
                    var almostPlugin = Activator.CreateInstance(type);
                    if (!(almostPlugin is IPlugin<T> plugin))
                    {
                        continue;
                    }
                    plugin.SetConfig(config);
                    this.pluginsList.Add(plugin);
                }
            }
        }
    }
}