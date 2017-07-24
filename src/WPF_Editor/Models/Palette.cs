using System.Collections.Generic;
using System.ComponentModel;
using Repo;
using WPF_Editor.Models.Interfaces;
using WPF_Editor.ViewModels;

namespace WPF_Editor.Models
{
    public class Palette : IPalette, INotifyPropertyChanged
    {
        private static IPalette _palette;
        private IPaletteMediator _paletteMediator;
        private Element _selectedElement;
        public event PropertyChangedEventHandler PropertyChanged;


        /* This property has to be set from EditorView.xaml */
        public Element SelectedElement
        {
            get => _selectedElement;
            set
            {
                OnPropertyChanged("SelectedItem");
                _selectedElement = value;
                //Simple property test
                //System.Console.WriteLine((_selectedElement is Node)? ((Node)_selectedElement).Name : _selectedElement.ToString());
            }
        }

        public IEnumerable<Node> Nodes { get; }
        public IEnumerable<Edge> Edges { get; }
        public IEnumerable<Element> Elements { get; }

        public static IPalette CreatePalette(IPaletteMediator paletteMediator = null)
        {
            if(_palette is null)
                _palette = new Palette(paletteMediator);
            return _palette;
        }
        public void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        private Palette(IPaletteMediator paletteMediator, string modelName = "RobotsMetamodel")
        {
            var repo = RepoFactory.CreateRepo();
            IModel model = repo.Model(modelName);

            List<Node> nodeList = new List<Node>();
            foreach (var node in model.Nodes)
            {
                nodeList.Add(new Node(node));
            }
            Nodes = nodeList;
            
            List<Edge> edgeList = new List<Edge>();
            foreach (var edge in model.Edges)
            {
                edgeList.Add(new Edge(edge));
            }
            Edges = edgeList;

            List<Element> elementList = new List<Element>();
            foreach (var element in model.Elements)
            {
                elementList.Add(new Element(element));
            }
            Elements = elementList;

            _paletteMediator = paletteMediator;
        }
    }
}
