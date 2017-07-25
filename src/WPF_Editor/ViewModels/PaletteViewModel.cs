using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Repo;
using WPF_Editor.Models;
using WPF_Editor.Models.Interfaces;

namespace WPF_Editor.ViewModels
{
    public class PaletteViewModel : INotifyPropertyChanged
    {
        private Element _selectedElement;
        private readonly IPalette _palette = Palette.CreatePalette();

        public event PropertyChangedEventHandler PropertyChanged;

        // Must be public because bindings use these properties
        //Create class ViewNode
        public ObservableCollection<Node> Nodes { get; }

        public ObservableCollection<Edge> Edges { get; }

        public ObservableCollection<Element> Elements { get; }

        public Element SelectedElement
        {
            get {
                
                if (_palette.SelectedElement is INode)
                {
                    _selectedElement = new Node((INode) _palette.SelectedElement);
                }
                //Implement later.
                /*
                else if (_palette.SelectedElement is IEdge)
                {
                    _selectedElement = new Edge((IEdge) _palette.SelectedElement);
                }
                */
                return _selectedElement;
            }
            set
            {
                OnPropertyChanged("SelectedElement");
                _palette.SelectedElement = value;
            }
            
        }

        public void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        public PaletteViewModel()
        {
            IRepo repo = RepoFactory.CreateRepo();
            IModel model = repo.Model("RobotsMetamodel");
            model.Metamodel.Nodes.First().Name = "ss";
            
            Nodes = new ObservableCollection<Node>();
            foreach (var inode in _palette.Nodes)
            {
                Nodes.Add(new Node(inode));
            }
            Edges = new ObservableCollection<Edge>();
            foreach (var iedge in _palette.Edges)
            {
                Edges.Add(new Edge(iedge, null, null));
            }
            Elements = new ObservableCollection<Element>();
            foreach (var ielement in _palette.Elements)
            {
                Elements.Add(new Element(ielement));
            }
        }
    }
}
