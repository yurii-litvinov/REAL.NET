namespace EditorPrototype.FileSerialization
{
    using System;
    using GraphX.Measure;
    using YAXLib;

    public sealed class YAXPointSerializer : ICustomSerializer<Point>
    {
        public Point DeserializeFromAttribute(System.Xml.Linq.XAttribute attrib)
        {
            return this.Deserialize(attrib.Value);
        }

        public Point DeserializeFromElement(System.Xml.Linq.XElement element)
        {
            return this.Deserialize(element.Value);
        }

        public Point DeserializeFromValue(string value)
        {
            return this.Deserialize(value);
        }

        public void SerializeToAttribute(Point objectToSerialize, System.Xml.Linq.XAttribute attrToFill)
        {
            attrToFill.Value = string.Format("{0}|{1}", objectToSerialize.X.ToString(), objectToSerialize.Y.ToString());
        }

        public void SerializeToElement(Point objectToSerialize, System.Xml.Linq.XElement elemToFill)
        {
            elemToFill.Value = string.Format("{0}|{1}", objectToSerialize.X.ToString(), objectToSerialize.Y.ToString());
        }

        public string SerializeToValue(Point objectToSerialize)
        {
            return string.Format("{0}|{1}", objectToSerialize.X.ToString(), objectToSerialize.Y.ToString());
        }

        private Point Deserialize(string str)
        {
            var res = str.Split(new char[] { '|' });
            if (res.Length == 2)
            {
                return new Point(Convert.ToDouble(res[0]), Convert.ToDouble(res[1]));
            }
            else
            {
                return default(Point);
            }
        }
    }
}
