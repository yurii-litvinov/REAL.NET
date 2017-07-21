using System.Collections.Generic;
using Repo;

namespace WPF_Editor.ViewModels
{
    public class Edge : Element, IEdge
    {
        private readonly IEdge _edge;
        public IElement From { get => _edge.From; set => _edge.From = value; }
        public IElement To { get => _edge.To; set => _edge.To = value; }

        public IElement Class => _edge.Class;

        public IEnumerable<IAttribute> Attributes => _edge.Attributes;

        public bool IsAbstract => _edge.IsAbstract;

        public Metatype Metatype => _edge.Metatype;

        public Metatype InstanceMetatype => _edge.InstanceMetatype;

        public string Shape => _edge.Shape;
        public Edge(IEdge iedge)
        {
            _edge = iedge;
        }
    }
}