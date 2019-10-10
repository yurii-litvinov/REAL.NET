using Repo;
using System;
using System.IO;

namespace ShapeEditorLib
{
    public static class ViewFactory
    {
        public static IElementView CreateView(IElement element, string path)
        {
            string link = "";
            TypeOfVisual type = TypeOfVisual.NoFile;
            if (element is INode)
            {
                var node = element as INode;
                var visualInfo = node.VisualInfo;
                type = visualInfo.Type;
                link = visualInfo.LinkToFile;
            }
            else
            {
                var edge = element as IEdge;
                var visualInfo = edge.VisualInfo;
                type = visualInfo.Type;
                link = visualInfo.LinkToFile;
            }
            return CreateViewFromFile(type, link);
        }

        public static IElementView CreateViewFromFile(TypeOfVisual type, string link)
        {
            IViewFileParser parser = null;
            if (type == TypeOfVisual.XML)
            {
                parser = new Parsers.XMLParser();
            }
            else
            {
                throw new NotImplementedException("can't parse this type of file yet");
            }
            parser.LoadFile(link);
            return parser.ParseElementView();
        }
    }
}
