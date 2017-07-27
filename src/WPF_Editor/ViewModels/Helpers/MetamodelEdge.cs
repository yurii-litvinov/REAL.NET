using System;
using Repo;

namespace WPF_Editor.ViewModels.Helpers
{
    public class MetamodelEdge : MetamodelElement, IEdge
    {
        public IEdge IEdge { get; }
        public IElement From
        {
            get => IEdge.From;
            set => throw new NotImplementedException("Cannot change source element of metamodel edge.");
        }

        public IElement To
        {
            get => IEdge.To;
            set => throw new NotImplementedException("Cannot change target element of metamodel edge.");
        }

        public MetamodelEdge(IEdge iedge) : base(iedge)
        {
            IEdge = iedge;
        }
    }
}
