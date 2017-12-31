using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PluginLibrary
{
    /// <summary>
    /// General interface of plugin
    /// </summary>
    public interface IPlugin
    {
        /// <summary>
        /// Gets name of plugin
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Set this plugin's configuration
        /// </summary>
        /// <param name="config">Configuration to set</param>
        void SetConfig(object config);
    }
}
