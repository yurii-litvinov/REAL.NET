using System;
using System.Collections.Generic;
using System.Windows.Media.Imaging;

namespace EditorPrototype.Models
{
    public static class ImageLoader
    {
        private static readonly List<BitmapImage> Images = new List<BitmapImage>();

        public static BitmapImage GetImageById(int id)
        {
            return Images.Count < id ? null : Images[id];
        }
    }
}
