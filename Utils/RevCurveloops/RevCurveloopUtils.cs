using Autodesk.Revit.DB.IFC;
using HcBimUtils;
using Utils.CompareElement;
using Utils.RevArcs;
using Utils.RevEllipses;

namespace Utils.RevCurveloops
{
    public static class RevCurveloopUtils
    {
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
        public static List<XYZ> GetPoints(this CurveLoop curves)
        {
            List<XYZ> list = new List<XYZ>();
            foreach (Curve curf in curves)
            {
                if (curf is Line l) list.Add(l.SP());
                if (curf is Arc arc) list.AddRange(arc.Tessellate());
                if (curf is Ellipse el) list.AddRange(el.Tessellate());
            }

            return list.Distinct(new ComparePoint()).ToList();
        }
        public static CurveLoop GenerateCurveLoop(this CurveLoop curveLoop)
        {
            var result = new CurveLoop();
            foreach (var curve in curveLoop)
            {
                if (curve is Line l) result.Append(l);
                if (curve is Arc arc)
                {
                    var arcCustom = new ArcCustom(arc);
                    var arcLines = arcCustom.Curves;
                    foreach (Line arcL in arcLines)
                    {
                        result.Append(arcL);
                    }
                }
                if (curve is Ellipse el)
                {
                    var elCustom = new EllipseCustom(el);
                    var elLines = elCustom.Curves;
                    foreach (Line ElL in elLines)
                    {
                        result.Append(ElL);
                    }
                }
            }
            return result;
        }
        public static double GetArea(this CurveLoop loop)
        {
            return ExporterIFCUtils.ComputeAreaOfCurveLoops(new List<CurveLoop>() { loop });
        }
    }
}
