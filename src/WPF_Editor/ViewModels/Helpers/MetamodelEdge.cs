using System;
using Repo;

namespace WPF_Editor.ViewModels.Helpers
{
    public class MetamodelEdge : MetamodelElement, IEdge
    {
        public MetamodelEdge(IEdge iedge) : base(iedge)
        {
            _edge = iedge;
        }

        private readonly IEdge _edge;

        public IElement From
        {
            get => _edge.From;
            set => throw new NotImplementedException("Cannot change source element of metamodel edge.");
        }

        public IElement To
        {
            get => _edge.To;
            set => throw new NotImplementedException("Cannot change target element of metamodel edge.");
        }
    }
}