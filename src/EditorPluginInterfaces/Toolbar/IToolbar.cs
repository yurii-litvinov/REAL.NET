using System.Collections.Generic;

namespace EditorPluginInterfaces.Toolbar
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
        /// <param name="command">Command which will be executed on click</param>
        /// <param name="desription">Summary description</param>
        /// <param name="image">Image which will be shown on this button</param>
        void AddButton(ICommand command, string desription, string image);

        /// <summary>
        /// Add menu on toolbar
        /// </summary>
        /// <param name="menu">Menu to add</param>
        void AddMenu(IMenu menu);
    }
}
