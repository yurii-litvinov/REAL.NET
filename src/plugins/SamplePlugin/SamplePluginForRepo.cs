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

namespace SamplePlugin
{
    using System;
    using System.Collections.Generic;
    using EditorPluginInterfaces;
    using Repo;

    /// <summary>
    /// Sample plugin that shows that connection with repo is ok
    /// </summary>
    public class SamplePluginForRepo : IPlugin<PluginConfig>
    {
        /// <summary>
        /// Name of plugin
        /// </summary>
        public string Name => "This plugin prints all elements from model to console";

        /// <summary>
        /// Sets configuration for plugin
        /// </summary>
        /// <param name="config"></param>
        public void SetConfig(PluginConfig config)
        {
            if (config == null)
            {
                throw new ArgumentException("This is not correct type of configuration");
            }
            model = config.Model;
            console = config.Console;
            PrintAllElementsToConsole();
        }

        /// <summary>
        /// Reference to current model of scene
        /// </summary>
        private IModel model;

        /// <summary>
        /// Reference to console
        /// </summary>
        private IConsole console;

        /// <summary>
        /// Prints all elements to editor's console
        /// </summary>
        private void PrintAllElementsToConsole()
        {
            if (model == null)
            {
                return;
            }
            var elements = model.Elements;
            var list = new List<IElement>(elements);
            foreach (var i in elements)
            {
                this.console.SendMessage(i.Name);
            }
        }
    }
}
