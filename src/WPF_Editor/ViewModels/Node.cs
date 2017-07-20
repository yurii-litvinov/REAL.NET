namespace REAL.NET.ViewModels
{
    using Repo;
    using System.Collections.Generic;
    using ViewModels;
    using WPF_Editor.ViewModels;

    class Node : Element, INode
    {
        private INode node { get; }
        public string Name { get => node.Name; set => node.Name = value;}

        public IElement Class => node.Class;

        public IEnumerable<IAttribute> Attributes => node.Attributes;

        public bool IsAbstract => node.IsAbstract;

        public Metatype Metatype => node.Metatype;

        public Metatype InstanceMetatype => node.InstanceMetatype;

        public string Shape => node.Shape;

        public Node(INode inode)
        {
            node = inode;
        }
    }
}