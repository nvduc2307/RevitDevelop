using System.Drawing;
using System.IO;

namespace RevitDevelop.Utils.BitMaps
{
    public static class BitMapUtils
    {
        public static void CreateImage(
            Bitmap bitmap,
            string pathSave,
            string nameImage,
            System.Drawing.Imaging.ImageFormat imageFormat,
            List<Graphics> graphics)
        {
            var fullPath = $"{pathSave}{nameImage}.{imageFormat.ToString()}";
            FileInfo fi = new FileInfo(fullPath);
            FileStream fstr = fi.Create();
            foreach (var item in graphics)
            {
                item.Dispose();
            }
            bitmap.Save(fstr, imageFormat);
            bitmap.Dispose();
        }
    }
}
