using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QuickGraph;
using Repo;
using WPF_Editor.ViewModels.Interfaces;

namespace WPF_Editor.ViewModels
{
    public class PaletteViewModel : IPaletteViewModel, INotifyPropertyChanged
    {
        private MetamodelElement _selectedElement;
        private IPaletteMediatorViewModel _paletteMediator;
        private static IPaletteViewModel _palette;
        public event PropertyChangedEventHandler PropertyChanged;


        public ObservableCollection<MetamodelNode> Nodes { get; }

        public ObservableCollection<MetamodelEdge> Edges { get; }

        public ObservableCollection<MetamodelElement> Elements { get; }

        public static IPaletteViewModel CreatePalette(IPaletteMediatorViewModel paletteMediator = null)
        {
            if (_palette == null)
            {
                _palette = new PaletteViewModel(paletteMediator);
            }
            return _palette;
        }

        private PaletteViewModel(IPaletteMediatorViewModel paletteMediator)
        {
            Nodes = new ObservableCollection<MetamodelNode>();
            Edges = new ObservableCollection<MetamodelEdge>();
            Elements = new ObservableCollection<MetamodelElement>();
            _paletteMediator = paletteMediator;
            foreach (var inode in _paletteMediator.metamodelNodes)
            {
                if (!inode.IsAbstract)
                {
                    Nodes.Add(new MetamodelNode(inode));
                }
            }
            foreach (var iedge in _paletteMediator.metamodelEdges)
            {
                if (!iedge.IsAbstract)
                {
                    Edges.Add(new MetamodelEdge(iedge));
                }
            }
            foreach (var ielement in _paletteMediator.metamodelElements)
            {
                if (!ielement.IsAbstract)
                {
                    if (ielement is INode)
                    {
                        Elements.Add(new MetamodelNode(ielement as INode));
                    }
                    else
                    {
                        Elements.Add(new MetamodelEdge(ielement as IEdge));
                    }
                }
            }
        }

        public MetamodelElement SelectedElement
        {
            get => _selectedElement;
            set
            {
                _selectedElement = value;
                OnPropertyChanged("SelectedElement");
            }
            
        }

        public void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        
    }
}
