namespace RevitDevelop.Utils.Units
{
    public static class UnitsUtils
    {
        public static double MmToPixel(this double Mm)
        {
            return Mm * 3.7795275591;
        }
        public static double PixelToMm(this double pixcel)
        {
            return pixcel / 3.7795275591;
        }
    }
}
