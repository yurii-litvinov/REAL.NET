﻿namespace EditorPrototype.FileSerialization
{
    using System;
    using System.Linq;
    using System.Text;
    using GraphX.Measure;
    using YAXLib;

    public sealed class YAXPointArraySerializer : ICustomSerializer<Point[]>
    {
        public Point[] DeserializeFromAttribute(System.Xml.Linq.XAttribute attrib)
        {
            return this.Deserialize(attrib.Value);
        }

        public Point[] DeserializeFromElement(System.Xml.Linq.XElement element)
        {
            return this.Deserialize(element.Value);
        }

        public Point[] DeserializeFromValue(string value)
        {
            return this.Deserialize(value);
        }

        public void SerializeToAttribute(Point[] objectToSerialize, System.Xml.Linq.XAttribute attrToFill)
        {
            attrToFill.Value = this.Serialize(objectToSerialize);
        }

        public void SerializeToElement(Point[] objectToSerialize, System.Xml.Linq.XElement elemToFill)
        {
            elemToFill.Value = this.Serialize(objectToSerialize);
        }

        public string SerializeToValue(Point[] objectToSerialize)
        {
            return this.Serialize(objectToSerialize);
        }

        private Point[] Deserialize(string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                return null;
            }

            var arr = str.Split(new char[] { '~' });
            var ptlist = new Point[arr.Length];
            int cnt = 0;
            foreach (var item in arr)
            {
                var res = item.Split(new char[] { '|' });
                if (res.Length == 2)
                {
                    ptlist[cnt] = new Point(Convert.ToDouble(res[0]), Convert.ToDouble(res[1]));
                }
                else
                {
                    ptlist[cnt] = default(Point);
                }

                cnt++;
            }

            return ptlist;
        }

        private string Serialize(Point[] list)
        {
            var sb = new StringBuilder();
            if (list != null)
            {
                var last = list.Last();
                foreach (var item in list)
                {
                    sb.Append(string.Format("{0}|{1}{2}", item.X.ToString(), item.Y.ToString(), item != last ? "~" : string.Empty));
                }
            }

            return sb.ToString();
        }
    }
}
