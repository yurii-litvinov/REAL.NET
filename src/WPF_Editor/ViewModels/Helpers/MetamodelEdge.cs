using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Repo;

namespace WPF_Editor.ViewModels
{
    public class MetamodelEdge : MetamodelElement, IEdge
    {
        private readonly IEdge _edgeImplementation;
        public IElement From
        {
            get => _edgeImplementation.From;
            set => throw new NotImplementedException("Cannot change source element of metamodel edge.");
        }

        public IElement To
        {
            get => _edgeImplementation.To;
            set => throw new NotImplementedException("Cannot change target element of metamodel edge.");
        }

        public MetamodelEdge(IEdge iedge) : base(iedge)
        {
            _edgeImplementation = iedge;
        }
    }
}
