using System;
using System.Collections.Generic;

namespace WpfControlsLib.Model
{
    /// <summary>
    /// Is used to open and save positions from txt file with same name as model
    /// </summary>
    public static class PositionsLoad
    {
        /// <summary>
        /// Save dictionary with node positions
        /// </summary>
        /// <param name="fileName">Name of the file with model</param>
        public static void SavePositionsTable(string fileName, Dictionary<string, System.Windows.Point> positionsTable)
        {
            var positionsFileName = GetDicFileName(fileName);
            using (System.IO.FileStream fstream = new System.IO.FileStream(positionsFileName, System.IO.FileMode.OpenOrCreate))
            {
                foreach (var nodeName in positionsTable.Keys)
                {
                    byte[] array1 = System.Text.Encoding.Default.GetBytes(
                        nodeName + " " + Convert.ToString(positionsTable[nodeName].X)
                        + " " + Convert.ToString(positionsTable[nodeName].Y + " "));
                    fstream.Write(array1, 0, array1.Length);
                }
            }
        }

        /// <summary>
        /// Opens file with node positions and fills dictionary
        /// </summary>
        /// <param name="fileName">Name of the file with saved model to open</param>
        /// <returns>Whether the positionsFile existed</returns>
        public static Dictionary<string, System.Windows.Point> OpenPositions(string fileName)
        {
            var positionsTable = new Dictionary<string, System.Windows.Point>();
            string positionsFileName = GetDicFileName(fileName);

            if (!System.IO.File.Exists(positionsFileName))
            {
                return new Dictionary<string, System.Windows.Point>();
            }

            using (var reader = new System.IO.StreamReader(positionsFileName))
            {
                var str = reader.ReadLine().Split(' ');
                for (var i = 0; i < str.Length - 1; i += 3)
                {
                    positionsTable.Add(str[i], new System.Windows.Point(
                        Convert.ToDouble(str[i + 1]), Convert.ToDouble(str[i + 2])));
                }
            }

            return positionsTable;
        }

        /// <summary>
        /// Make name for file with node positions
        /// </summary>
        /// <param name="fileName">Name of the file containing model</param>
        /// <returns>the name for file with node positions</returns>
        private static string GetDicFileName(string fileName)
        {
            if (fileName == "")
            {
                return "OpenMode.txt";
            }

            string dicFileName = null;
            for (var i = 0; i <= fileName.LastIndexOf('.'); i++)
            {
                dicFileName += fileName[i];
            }
            dicFileName += "txt";
            return dicFileName;
        }
    }
}
