using System.Collections.Generic;
using Repo;

namespace WPF_Editor.ViewModels
{
    public class Edge : Element, IEdge
    {
        private readonly IEdge _edge;

        public IElement From { get => _edge.From; set => _edge.From = value; }

        public IElement To { get => _edge.To; set => _edge.To = value; }

        public override IElement Class => _edge.Class;

        public override IEnumerable<IAttribute> Attributes => _edge.Attributes;

        public override bool IsAbstract => _edge.IsAbstract;

        public override Metatype Metatype => _edge.Metatype;

        public override Metatype InstanceMetatype => _edge.InstanceMetatype;

        public override string Shape => _edge.Shape;
        public Edge(IEdge iedge) : base(iedge)
        {
            _edge = iedge;
        }
    }
}