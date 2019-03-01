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

using System.Linq;

namespace ConstraintsPlugin
{
    using System;
    using EditorPluginInterfaces;
    using System.Windows.Controls;

    /// <summary>
    /// This class is a sample plugin and is also used in unit tests to check plugin loading.
    /// </summary>
    public class ConstraintsPlugin : IPlugin<PluginConfig>
    {
        /// <summary>
        /// Name of plugin
        /// </summary>
        public string Name => "constraintsPlugin";

        public Grid constraintsPanel;
        /// <summary>
        /// Field that contains reference to editor console.
        /// </summary>
        private IConsole console;

        /// <summary>
        /// Toolbar in the top of the windows
        /// </summary>
        //private IDockPanel dockPanel;

        /// <summary>
        /// Establishes connection with the rest of the system.
        /// </summary>
        /// <param name="config">Configuration from system core.</param>
        public void SetConfig(PluginConfig config)
        {
            if (config == null)
            {
                throw new ArgumentException("This is not correct type of configuration");
            }
            this.console = config.Console;
            this.console.SendMessage("Constraints add-on successfully launched");

            this.constraintsPanel = config.ConstraintsGrid;
            this.constraintsPanel.Children.Add(new ConstraintsColumn(config));
            var model = config.Model;
            var repo = model.Repo;

            var initialNode = repo.Model("RobotsMetamodel").Nodes.FirstOrDefault(x => x.Name == "InitialNode");
            if (initialNode != null)
            {
                model.CreateNode(initialNode);
            }
        }
    }
}
