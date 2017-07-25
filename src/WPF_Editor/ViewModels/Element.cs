using System.Collections.Generic;
using Repo;

namespace WPF_Editor.ViewModels
{
    public class Attribute : IAttribute
    {

        public string Name { get; }
        public AttributeKind Kind { get; }
        public IElement Type { get; }
        public string StringValue { get; set; }
        public IElement ReferenceValue { get; set; }

        public Attribute(string name, AttributeKind kind, IElement type, string stringValue, IElement referenceValue)
        {
            Name = name;
            Kind = kind;
            Type = type;
            StringValue = stringValue;
            ReferenceValue = referenceValue;
        }
    }
    public class Element : IElement
    {
        public string Name { get; set; }
        public IElement Class { get;}
        public IEnumerable<IAttribute> Attributes { get; }
        public bool IsAbstract { get; }
        public Metatype Metatype { get; }
        public Metatype InstanceMetatype { get; }
        public string Shape { get; }

        public Element(IElement element)
        {
            Name = element.Name;
            Class = element.Class;

            var attributeList = new List<IAttribute>();
            foreach (var attribute in element.Attributes)
            {
                attributeList.Add(new Attribute(attribute.Name, attribute.Kind, attribute.Type, attribute.StringValue, this));
            }
            Attributes = attributeList;
            IsAbstract = element.IsAbstract;
            Metatype = element.Metatype;
            InstanceMetatype = element.InstanceMetatype;
            Shape = element.Shape;
        }

        public override string ToString() => Name;
        
    }
}
