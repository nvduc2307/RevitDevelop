namespace RevitDevelop.Utils.Window2Ds
{
    public static class Geometries2DUtils
    {
        public static double Distance(this System.Windows.Shapes.Line l)
        {
            var result = 0.0;
            try
            {
                var ps = new System.Windows.Point(l.X1, l.Y1);
                var pe = new System.Windows.Point(l.X2, l.Y2);
                var vt = pe - ps;
                result = Math.Sqrt(vt.X * vt.X + vt.Y * vt.Y);
            }
            catch (Exception)
            {
            }
            return result;
        }
        public static double Distance(this System.Windows.Point ps, System.Windows.Point pe)
        {
            var result = 0.0;
            try
            {
                var vt = pe - ps;
                result = Math.Sqrt(vt.X * vt.X + vt.Y * vt.Y);
            }
            catch (Exception)
            {
            }
            return result;
        }
        public static System.Windows.Point Direction(this System.Windows.Shapes.Line l)
        {
            var result = new System.Windows.Point();
            try
            {
                var ps = new System.Windows.Point(l.X1, l.Y1);
                var pe = new System.Windows.Point(l.X2, l.Y2);
                var distance = l.Distance();
                var vt = pe - ps;
                result.X = vt.X / distance;
                result.Y = vt.Y / distance;
            }
            catch (Exception)
            {
            }
            return result;
        }
        public static System.Windows.Point Direction(this System.Windows.Point ps, System.Windows.Point pe)
        {
            var result = new System.Windows.Point();
            try
            {
                var distance = ps.Distance(pe);
                var vt = pe - ps;
                result.X = vt.X / distance;
                result.Y = vt.Y / distance;
            }
            catch (Exception)
            {
            }
            return result;
        }
        public static double DotProduct(this System.Windows.Point vt1, System.Windows.Point vt2)
        {
            var result = 0.0;
            try
            {
                var vtF1 = new XYZ(vt1.X, -vt1.Y, 0);
                var vtF2 = new XYZ(vt2.X, -vt2.Y, 0);
                result = vtF1.DotProduct(vtF2);
            }
            catch (Exception)
            {
            }
            return result;
        }
        public static System.Windows.Point CrossProduc(
            this System.Windows.Point vt1,
            System.Windows.Point vt2 = new System.Windows.Point(),
            int z = -1)
        {
            var result = new System.Windows.Point();
            try
            {
                var vtF1 = new XYZ(vt1.X, -vt1.Y, 0);
                var vtF2 = vt2.X == 0 && vt2.Y == 0
                    ? new XYZ(0, 0, z)
                    : new XYZ(vt2.X, -vt2.Y, 0);
                var vt = vtF1.CrossProduct(vtF2);
                result.X = vt.X;
                result.Y = -vt.Y;
            }
            catch (Exception)
            {
            }
            return result;
        }
    }
}
