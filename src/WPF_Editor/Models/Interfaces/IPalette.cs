using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPF_Editor.Models.Interfaces
{
    /// <summary>
    /// This interface is abstraction of Palette
    /// </summary>
    interface IPalette
    {
        /// <summary>
        /// Gets selected item from palette. If there's no such item it will return null.
        /// </summary>
        object SelectedElement { get; }
    }
}
