using HcBimUtils;
using RevitDevelop.Utils.RevCurves;
using Utils.CompareElement;
using Utils.RevArcs;
using Utils.RevEllipses;

namespace Utils.RevCurveloops
{
    public static class RevCurveloopUtils
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
            var hasArc = curveLoop.Any(x => x is Arc);
            var hasEllipes = curveLoop.Any(x => x is Ellipse);
            XYZ result = null;
            try
            {
                if (hasArc)
                {
                    var arc = curveLoop.FirstOrDefault(x => x is Arc) as Arc;
                    result = arc.Normal;
                }
                else if (hasEllipes)
                {
                    var arc = curveLoop.FirstOrDefault(x => x is Ellipse) as Ellipse;
                    result = arc.Normal;
                }
                else
                {
                    var center = curveLoop.GetCenter();
                    var curves = curveLoop.Select(x => x).ToList();
                    var l1 = curves[0];
                    var l2 = curves[0].Direction().IsParallel(curves[2].Direction())
                        ? curves[1]
                        : curves[2];

                    var dir1 = l1.Direction().Normalize();
                    var dir2 = l2.Direction().Normalize();

                    result = dir1.CrossProduct(dir2).Normalize();
                    var isFlowClock = curveLoop.IsCounterclockwise(result);
                    result = isFlowClock ? result : -result;
                }
            }
            catch (Exception)
            {
            }
            return result;
        }
        public static List<XYZ> GetCurveLoopPoints(this CurveLoop curves)
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
        public static void CreateModelCurve(this CurveLoop curveLoop, Document document)
        {
            try
            {
                var nor = curveLoop.GetNormal();
                foreach (var item in curveLoop)
                {
                    var plane = Plane.CreateByNormalAndOrigin(nor, item.GetEndPoint(0));
                    var sket = SketchPlane.Create(document, plane);
                    document.Create.NewModelCurve(item, sket);
                }
            }
            catch (Exception)
            {
            }
        }
    }
}
