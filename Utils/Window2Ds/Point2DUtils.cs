namespace RevitDevelop.Utils.Window2Ds
{
    public static class Point2DUtils
    {
        public static XYZ RotatePoint(XYZ point, double midX, double midY, double angleRadian)
        {
            // rotate EP1
            var EP1X = point.X - midX;
            var EP1Y = point.Y - midY;
            var x1Rotated = EP1X * Math.Cos(angleRadian) - EP1Y * Math.Sin(angleRadian);
            var y1Rotated = EP1X * Math.Sin(angleRadian) + EP1Y * Math.Cos(angleRadian);
            return new XYZ(x1Rotated + midX, y1Rotated + midY, 0);
        }
    }
}
