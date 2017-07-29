using System.Collections.Generic;
using Repo;

namespace WPF_Editor.ViewModels.Interfaces
{
    public interface IPaletteMediatorViewModel
    {
        IEnumerable<IElement> MetamodelElements { get; }
        IEnumerable<INode> MetamodelNodes { get; }
        IEnumerable<IEdge> MetamodelEdges { get; }
    }
}