using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Repo;

namespace WPF_Editor.ViewModels
{
    public class MetamodelNode : MetamodelElement, INode
    {
        public MetamodelNode(INode inode) : base(inode)
        {
        }
    }
}
