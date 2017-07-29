using Repo;

namespace WPF_Editor.ViewModels.Helpers
{
    public class MetamodelNode : MetamodelElement, INode
    {
        public MetamodelNode(INode inode) : base(inode)
        {
            _node = inode;
        }

        private INode _node;
    }
}