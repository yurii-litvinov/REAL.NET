using EditorPluginInterfaces.Toolbar;
using System.Collections.Generic;

namespace EditorPluginInterfaces
{
    /// <summary>
    /// Interface that is abstraction of editor's toolbar 
    /// </summary>
    public interface IToolbar
    {
        /// <summary>
        /// Gets list of available toolbar's buttons 
        /// </summary>
        IList<IButton> Buttons { get; }

        /// <summary>
        /// Add button on toolbar
        /// </summary>
        /// <param name="button">Button to add</param>
        void AddButton(IButton button);

        /// <summary>
        /// Add menu on toolbar
        /// </summary>
        /// <param name="menu">Menu to add</param>
        void AddMenu(IMenu menu);
    }
}
