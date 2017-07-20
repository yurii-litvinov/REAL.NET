namespace REAL.NET.Models
{
    using System;
    using Repo;
    using WPF_Editor.Models.Interfaces;
    using System.ComponentModel;
    using ViewModels;
    using WPF_Editor.ViewModels;
    class Palette : IPalette, INotifyPropertyChanged
    {
        private static IPalette palette;

        public event PropertyChangedEventHandler PropertyChanged;
        

        /* This property has to be set from EditorView.xaml */
        public Element SelectedElement { get; set; }

        public IPaletteMediator PaletteMediator { get; }

        public static IPalette CreatePalette(IPaletteMediator palette_mediator)
        {
            if(palette is null)
                palette = new Palette(palette_mediator);
            return palette;
        }
        public void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        private Palette(IPaletteMediator palette_mediator)
        {
            PaletteMediator = palette_mediator;
        }
    }
}
