using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPF_Editor.Models.Interfaces
{
    interface IPalette
    {
        IPaletteMediator Palette_mediator { get; }
        /* Gets selected item from palette. If there's no such item it will return null.*/
        object SelectedElement { get; }
    }
}
