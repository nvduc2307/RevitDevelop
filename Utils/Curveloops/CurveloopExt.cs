using HcBimUtils;

namespace Utils.Curveloops
{
    public static class CurveloopExt
    {
        public static BoundingBoxXYZ GetBoundingBoxOXY(this CurveLoop curveLoop)
        {
            var result = new BoundingBoxXYZ();
            try
            {
                var ps = curveLoop.GetPoints();
                var minx = ps.Min(x => x.X);
                var miny = ps.Min(x => x.Y);
                var maxx = ps.Max(x => x.X);
                var maxy = ps.Max(x => x.Y);
                result.Min = new XYZ(minx, miny, -1);
                result.Max = new XYZ(maxx, maxy, 0);

            }
            catch (Exception)
            {
            }
            return result;
        }
        public static XYZ GetCenter(this CurveLoop curveLoop)
        {
            XYZ result = null;
            try
            {
                var ps = curveLoop.GetPoints();
                var minx = ps.Min(x => x.X);
                var miny = ps.Min(x => x.Y);
                var maxx = ps.Max(x => x.X);
                var maxy = ps.Max(x => x.Y);

                var pMin = new XYZ(minx, miny, 0);
                var pMax = new XYZ(maxx, maxy, 0);
                result = pMin.Midpoint(pMax);
            }
            catch (Exception)
            {
            }
            return result;
        }
        public static XYZ GetNormal(this CurveLoop curveLoop)
        {
            var center = curveLoop.GetCenter();
            XYZ result = null;
            try
            {
                var curves = curveLoop.Select(x => x).ToList();
                var l1 = curves[0];
                var l2 = curves[2];

                var dir1 = (l1.GetEndPoint(0) - center).Normalize();
                var dir2 = (l1.GetEndPoint(1) - center).Normalize();

                var normal = dir1.CrossProduct(dir2).Normalize();
                var isFlowClock = curveLoop.IsCounterclockwise(normal);
                result = isFlowClock ? normal : -normal;
            }
            catch (Exception)
            {
            }
            return result;
        }
        public static CurveLoop CreateOffset(this CurveLoop loop, double offset, XYZ normal)
        {
            try
            {
                return CurveLoop.CreateViaOffset(loop, offset, normal);
            }
            catch
            {
                return loop;
            }
        }
    }
}
