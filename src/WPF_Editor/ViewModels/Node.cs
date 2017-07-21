using System.Collections.Generic;
using Repo;

namespace WPF_Editor.ViewModels
{
    public class Node : Element, INode
    {
        private readonly INode _node;
        public string Name { get => _node.Name; set => _node.Name = value;}

        public IElement Class => _node.Class;

        public IEnumerable<IAttribute> Attributes => _node.Attributes;

        public bool IsAbstract => _node.IsAbstract;

        public Metatype Metatype => _node.Metatype;

        public Metatype InstanceMetatype => _node.InstanceMetatype;

        public string Shape => _node.Shape;

        public Node(INode inode)
        {
            _node = inode;
        }
    }
}