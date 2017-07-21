using System.ComponentModel;
using WPF_Editor.Models.Interfaces;
using WPF_Editor.ViewModels;

namespace WPF_Editor.Models
{
    public class Palette : IPalette, INotifyPropertyChanged
    {
        private static IPalette _palette;

        public event PropertyChangedEventHandler PropertyChanged;
        

        /* This property has to be set from EditorView.xaml */
        public Element SelectedElement { get; set; }

        private IPaletteMediator _paletteMediator;

        public static IPalette CreatePalette(IPaletteMediator paletteMediator)
        {
            if(_palette is null)
                _palette = new Palette(paletteMediator);
            return _palette;
        }
        public void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        private Palette(IPaletteMediator paletteMediator)
        {
            _paletteMediator = paletteMediator;
        }
    }
}
