using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfControlsLib.Controls.Scene
{
    /// <summary>
    /// Class for work with vertices names
    /// </summary>
    public static class VertexName
    {
        /// <summary>
        /// Change new element name so there would be no same names
        /// </summary>
        /// <param name="element">New element</param>
        public static void NewVertexName(Repo.IElement element, List<string> vertexNames)
        {
            var name = element.Name;
            if (element.Name.LastIndexOf('.') != -1)
            {
                name = string.Empty;
                for (var i = 0; i < element.Name.LastIndexOf('.'); i++)
                {
                    name += element.Name[i];
                }
            }

            var counter = 1;
            while (vertexNames.Contains("a" + name + "." + counter))
            {
                counter++;
            }

            element.Name = name + "." + counter;
            vertexNames.Add("a" + element.Name);
        }
    }
}
