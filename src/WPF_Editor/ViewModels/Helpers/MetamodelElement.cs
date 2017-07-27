using System;
using System.Collections.Generic;
using Repo;

namespace WPF_Editor.ViewModels.Helpers
{
    public abstract class MetamodelElement : IElement
    {
        public IElement IElement { get; }
        public string Name
        {
            get => IElement.Name;
            set => throw new NotImplementedException($"Cannot change name of {Metatype} from metamodel");
        }

        public IElement Class => IElement.Class;

        public IEnumerable<IAttribute> Attributes => IElement.Attributes;

        public bool IsAbstract => IElement.IsAbstract;

        public Metatype Metatype => IElement.Metatype;

        public Metatype InstanceMetatype => IElement.InstanceMetatype;

        public string Shape => IElement.Shape;

        protected MetamodelElement(IElement element)
        {
            IElement = element;
        }
        public override string ToString() => Name;
    }
}
