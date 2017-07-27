using Repo;

namespace WPF_Editor.ViewModels.Helpers
{
    public class MetamodelNode : MetamodelElement, INode
    {
        public INode INode { get; }
        public MetamodelNode(INode inode) : base(inode)
        {
            INode = inode;
        }
    }
}
