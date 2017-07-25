using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Repo;

namespace WPF_Editor.ViewModels
{
    public abstract class MetamodelElement : IElement
    {
        private readonly IElement _elementImplementation;
        public string Name
        {
            get => _elementImplementation.Name;
            set => throw new NotImplementedException($"Cannot change name of {Metatype} from metamodel");
        }

        public IElement Class => _elementImplementation.Class;

        public IEnumerable<IAttribute> Attributes => _elementImplementation.Attributes;

        public bool IsAbstract => _elementImplementation.IsAbstract;

        public Metatype Metatype => _elementImplementation.Metatype;

        public Metatype InstanceMetatype => _elementImplementation.InstanceMetatype;

        public string Shape => _elementImplementation.Shape;

        protected MetamodelElement(IElement element)
        {
            _elementImplementation = element;
        }
    }
}
