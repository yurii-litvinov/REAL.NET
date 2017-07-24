using System.Collections.Generic;
using Repo;

namespace WPF_Editor.ViewModels
{
    public class Node : Element, INode
    {
        private readonly INode _node;

        public string Name { get => _node.Name; set => _node.Name = value;}

        public override IElement Class => _node.Class;

        public override IEnumerable<IAttribute> Attributes => _node.Attributes;

        public override bool IsAbstract => _node.IsAbstract;

        public override Metatype Metatype => _node.Metatype;

        public override Metatype InstanceMetatype => _node.InstanceMetatype;

        public override string Shape => _node.Shape;

        public Node(INode inode) : base(inode)
        {
            _node = inode;
        }
    }
}