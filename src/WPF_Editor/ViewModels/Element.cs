using System.Collections.Generic;
using Repo;

namespace WPF_Editor.ViewModels
{
    public class Element : IElement
    {
        private readonly IElement _element;
        public string Name { get => _element.Name; set => _element.Name = value; }
        public virtual IElement Class => _element.Class;
        public virtual IEnumerable<IAttribute> Attributes => _element.Attributes;
        public virtual bool IsAbstract => _element.IsAbstract;
        public virtual Metatype Metatype => _element.Metatype;
        public virtual Metatype InstanceMetatype => _element.InstanceMetatype;
        public virtual string Shape => _element.Shape;

        public Element(IElement element)
        {
            _element = element;
        }

        public override string ToString() => Name;
        
    }
}
