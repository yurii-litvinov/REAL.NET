using System.Collections.Generic;
using GraphX.PCL.Common.Enums;
using GraphX.PCL.Common.Interfaces;
using GraphX.PCL.Common.Models;
using Repo;

namespace WPF_Editor.ViewModels
{
    public class Node : Element, INode, IGraphXVertex
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

        public bool Equals(IGraphXVertex other)
        {
            if (other is null)
            {
                return false;
            }
            return ID == other.ID;
        }

        public long ID { get; set; }
        public ProcessingOptionEnum SkipProcessing { get; set; }
        public double Angle { get; set; }
        public int GroupId { get; set; }
        public override string ToString()
        {
            return Name;
        }
    }
}