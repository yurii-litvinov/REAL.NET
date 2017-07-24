using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Repo;
using WPF_Editor.Models;
using WPF_Editor.Models.Interfaces;

namespace WPF_Editor.ViewModels
{
    public class PaletteViewModel
    {
        private const string ModelName = "RobotsMetamodel";

        public ObservableCollection<Node> Nodes { get; }

        public ObservableCollection<Edge> Edges { get; }

        public ObservableCollection<Element> Elements { get; }

        public Element SelectedElement
        {
            get => _palette.SelectedElement;
            set => _palette.SelectedElement = value;
        }

        private readonly IPalette _palette = Palette.CreatePalette();

        public PaletteViewModel()
        {
            Nodes = new ObservableCollection<Node>(_palette.Nodes);
            Edges = new ObservableCollection<Edge>(_palette.Edges);
            Elements = new ObservableCollection<Element>(_palette.Elements);
        }
    }
}
