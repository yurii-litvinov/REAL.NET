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
        private const string ModelName = "RobotsMetamodel";
        private Element _selectedElement;
        public event PropertyChangedEventHandler PropertyChanged;

        public ObservableCollection<Node> Nodes { get; }

        public ObservableCollection<Edge> Edges { get; }

        public ObservableCollection<Element> Elements { get; }

        //Test it
        public Element SelectedElement
        {
            get {
                _selectedElement = new Element(_palette.SelectedElement);
                return _selectedElement;
            }
            set
            {
                OnPropertyChanged("SelectedElement");
                _palette.SelectedElement = value;
                if (SelectedElement is Node)
                {
                    System.Console.WriteLine((SelectedElement as INode).Name);
                }
                System.Console.WriteLine(SelectedElement);
            }
            
        }
        public void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        private readonly IPalette _palette = Palette.CreatePalette();

        public PaletteViewModel()
        {
            Nodes = new ObservableCollection<Node>();
            foreach (var inode in _palette.Nodes)
            {
                Nodes.Add(new Node(inode));
            }
            Edges = new ObservableCollection<Edge>();
            foreach (var iedge in _palette.Edges)
            {
                Edges.Add(new Edge(iedge));
            }
            Elements = new ObservableCollection<Element>();
            foreach (var ielement in _palette.Elements)
            {
                Elements.Add(new Element(ielement));
            }
        }
    }
}
