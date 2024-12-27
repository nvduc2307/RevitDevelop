using Autodesk.Revit.DB.Structure;
using HcBimUtils;
using HcBimUtils.GeometryUtils.Geometry;

namespace Utils.RevPoints
{
    public static class RevPointUtils
    {
        public static bool IsCounterClockWise(this List<XYZ> polygons)
        {
            bool r = false;
            double sum = 0;
            for (int i = 0; i < polygons.Count - 1; i++)
            {
                var x1 = polygons[i].X;
                var x2 = polygons[i + 1].X;
                var y1 = polygons[i].Y;
                var y2 = polygons[i + 1].Y;
                sum += (x2 - x1) * (y2 + y1);
            }
            var sp = polygons[0];
            var ep = polygons[polygons.Count - 1];
            sum += (sp.X - ep.X) * (sp.Y + ep.Y);
            if (sum < 0)
            {
                r = true;
            }
            return r;
        }

        public static XYZ CenterPoint(this List<XYZ> points)
        {
            var x = points.Select(a => a.X).ToList();
            var y = points.Select(a => a.Y).ToList();
            var z = points.Select(a => a.Z).ToList();
            var min = new XYZ(x.Min(), y.Min(), z.Min());
            var max = new XYZ(x.Max(), y.Max(), z.Max());
            var center = max.Midpoint(min);
            return center;
        }

        public static IEnumerable<XYZ> GetPoint(this IEnumerable<Rebar> rebars)
        {
            var results = new List<XYZ>();
            foreach (var rebar in rebars)
            {
                try
                {
                    var paths = rebar.GetCenterlineCurves(true, false, false, MultiplanarOption.IncludeAllMultiplanarCurves, 0);
                    foreach (var curve in paths)
                    {
                        results.Add(curve.GetEndPoint(0));
                        results.Add(curve.GetEndPoint(1));
                    }
                }
                catch (Exception)
                {
                }
            }
            return results;
        }

        public static IEnumerable<XYZ> GetPoint(this IEnumerable<FamilyInstance> familyInstances)
        {
            var results = new List<XYZ>();
            foreach (var familyInstance in familyInstances)
            {
                try
                {
                    var solid = familyInstance.GetSingleSolid();
                    var faces = solid.GetFacesFromSolid();
                    foreach (var face in faces)
                    {
                        var points = face.GetPoints();
                        results.AddRange(points);
                    }
                }
                catch (Exception)
                {
                }
            }
            return results;
        }

        public static List<Curve> PointsToCurves(this List<XYZ> points, bool isClose = false)
        {
            var curves = new List<Curve>();
            var pc = points.Count;
            for (int i = 0; i < pc; i++)
            {
                if (isClose)
                {
                    var j = i == 0 ? pc - 1 : i - 1;
                    curves.Add(points[j].CreateLine(points[i]));
                }
                else
                {
                    if (i < pc - 1)
                    {
                        var sp = points[i];
                        var ep = points[i + 1];
                        curves.Add(sp.CreateLine(ep));
                    }
                }
            }
            return curves;
        }
    }
}
