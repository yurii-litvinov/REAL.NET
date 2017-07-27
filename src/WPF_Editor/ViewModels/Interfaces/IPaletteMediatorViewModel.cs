using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
