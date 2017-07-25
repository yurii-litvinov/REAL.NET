using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Repo;
using WPF_Editor.Models.Interfaces;
using WPF_Editor.ViewModels;

namespace WPF_Editor.Models
{
    public class Palette : IPalette
    {
        private static IPalette _palette;
        private IPaletteMediator _paletteMediator;

        public IEnumerable<INode> Nodes { get; }
        public IEnumerable<IEdge> Edges { get; }
        public IEnumerable<IElement> Elements { get; }

        /* This property has to be set from EditorView.xaml */
        public IElement SelectedElement { get; set; }

        public static IPalette CreatePalette(IPaletteMediator paletteMediator = null)
        {
            if(_palette is null)
                _palette = new Palette(paletteMediator);
            return _palette;
        }

        private Palette(IPaletteMediator paletteMediator, string modelName = "RobotsMetamodel")
        {
            
            //Use lambda expressions
            var repo = RepoFactory.CreateRepo();
            IModel model = repo.Model(modelName);

            var nodeList = new List<INode>();
            foreach (var inode in model.Nodes)
            {
                if (!inode.IsAbstract)
                {
                    nodeList.Add(inode);
                }
            }
            Nodes = nodeList;

            var edgeList = new List<IEdge>();

            foreach (var iedge in model.Edges)
            {
                if (!iedge.IsAbstract)
                {
                    edgeList.Add(iedge);
                }
            }
            Edges = edgeList;

            var elementList = new List<IElement>();
            foreach (var ielement in model.Elements)
            {
                if (!ielement.IsAbstract)
                {
                    elementList.Add(ielement);
                }
            }
            Elements = elementList;
            _paletteMediator = paletteMediator;
        }
    }
}
