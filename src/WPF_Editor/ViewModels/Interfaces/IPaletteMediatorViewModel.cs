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
        IEnumerable<IElement> metamodelElements { get; }
        IEnumerable<INode> metamodelNodes { get; }
        IEnumerable<IEdge> metamodelEdges { get; }
    }
}
