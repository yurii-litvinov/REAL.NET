namespace REAL.NET.Models
{
    using System;
    using WPF_Editor.Models.Interfaces;
    class Palette : IPalette
    {
        private static IPalette palette;
        /* This property has to be set from EditorView.xaml */
        public object SelectedElement { get; }

        public IPaletteMediator Palette_mediator { get; }

        public static IPalette CreatePalette(IPaletteMediator palette_mediator)
        {
            if(palette is null)
                palette = new Palette(palette_mediator);
            return palette;
        }
        private Palette(IPaletteMediator palette_mediator)
        {
            Palette_mediator = palette_mediator;
        }
    }
}
