namespace REAL.NET.ViewModels
{
    using Repo;
    using System.Collections.Generic;
    using WPF_Editor.ViewModels;
    class Edge : Element, IEdge
    {
        private IEdge edge { get; }
        public IElement From { get => edge.From; set => edge.From = value; }
        public IElement To { get => edge.To; set => edge.To = value; }

        public IElement Class => edge.Class;

        public IEnumerable<IAttribute> Attributes => edge.Attributes;

        public bool IsAbstract => edge.IsAbstract;

        public Metatype Metatype => edge.Metatype;

        public Metatype InstanceMetatype => edge.InstanceMetatype;

        public string Shape => edge.Shape;
        public Edge(IEdge iedge)
        {
            edge = iedge;
        }
    }
}