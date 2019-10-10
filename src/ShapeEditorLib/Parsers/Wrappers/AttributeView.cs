namespace ShapeEditorLib.Parsers.Wrappers
{
    internal class AttributeView : IAttributeView
    {
        public AttributeView(string attributeName, string exampleValue, int? order, bool isVisible, (int, int)? position)
        {
            this.AttributeName = attributeName;
            this.ExampleValue = exampleValue;
            this.OrderNumber = order;
            this.IsVisible = isVisible;
            this.Position = position;
        }

        public string AttributeName { get; }

        public string ExampleValue { get; set; }

        public int? OrderNumber { get; set; }

        public bool IsVisible { get; set; }

        public (int, int)? Position { get; set; }
    }
}
