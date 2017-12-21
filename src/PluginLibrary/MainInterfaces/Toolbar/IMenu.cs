using System.Collections.Generic;

namespace PluginLibrary.MainInterfaces
{
    /// <summary>
    /// Menu which can be added to toolbar
    /// </summary>
    public interface IMenu
    {
        /// <summary>
        /// Gets is this menu submenu itself 
        /// </summary>
        bool IsMenuType { get; }

        /// <summary>
        /// Gets children of this menu
        /// </summary>
        /// <returns>Children's list</returns>
        IList<IMenu> GetChildren();

        /// <summary>
        /// Does action connected with this element of menu
        /// </summary>
        void DoAction();

        /// <summary>
        /// Gets action connected with this element of menu  
        /// </summary>
        /// <returns>Connected Command</returns>
        ICommand GetAction();
    }
}