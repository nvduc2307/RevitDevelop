namespace RevitDevelop.Utils.RevPoints
{
    public class RevPoint
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }
        public XYZ ConvertPoint()
        {
            try
            {
                return new XYZ(X, Y, Z);
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
