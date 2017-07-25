using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Repo;

namespace WPF_Editor.ViewModels
{
    public class MetamodelNode : INode
    {
        private readonly INode _nodeImplementation;
        public string Name
        {
            get => _nodeImplementation.Name;
            set => throw new NotImplementedException("Cannot change name of element from metamodel");
        }

        public IElement Class => _nodeImplementation.Class;

        public IEnumerable<IAttribute> Attributes => _nodeImplementation.Attributes;

        public bool IsAbstract => _nodeImplementation.IsAbstract;

        public Metatype Metatype => _nodeImplementation.Metatype;

        public Metatype InstanceMetatype => _nodeImplementation.InstanceMetatype;

        public string Shape => _nodeImplementation.Shape;

        public MetamodelNode(INode inode)
        {
            _nodeImplementation = inode;
        }
    }
}
