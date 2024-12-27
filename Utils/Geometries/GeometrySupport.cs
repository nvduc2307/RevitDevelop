using HcBimUtils;

namespace Utils.Geometries
{
    public static class GeometrySupport
    {
        private const int EPS_DIGITS = 6;
        private const double EPS_DECIMAL = 1.0e-6;
        public static XYZ RayIntersectPlane(this XYZ point, XYZ vecRay, PlanarFace planarFace)
        {
            XYZ p = null;

            var ori = planarFace.Origin;
            var normal = planarFace.FaceNormal;
            if (vecRay.DotProduct(normal) != 0)
            {
                var dist = (ori - point).DotProduct(normal) / (vecRay.DotProduct(normal));
                p = point.Add(dist * vecRay.Normalize());
            }
            return p;
        }
        public static XYZ RayIntersectPlane(this XYZ point, XYZ vecRay, Plane plane)
        {
            XYZ p = null;
            var ori = plane.Origin;
            var normal = plane.Normal.Normalize();
            if (vecRay.DotProduct(normal) != 0)
            {
                var dist = (ori - point).DotProduct(normal) / (vecRay.DotProduct(normal));
                p = point.Add(dist * vecRay.Normalize());
            }
            return p;
        }
        public static XYZ RayIntersectLine2D(this XYZ point, XYZ vecRay, Line line)
        {
            XYZ p = null;
            if (vecRay.DotProduct(line.Direction.CrossProduct(XYZ.BasisZ).Normalize()) != 0)
            {
                var plane = Plane.CreateByNormalAndOrigin(line.Direction.CrossProduct(XYZ.BasisZ).Normalize(), line.GetEndPoint(0));
                p = point.RayIntersectPlane(vecRay, plane);
            }
            return p;
        }
        public static XYZ RayIntersectLineBound2D(this XYZ point, XYZ vecRay, Line line)
        {
            XYZ p = null;
            //Check can ray to line
            var canRay = false;
            var cross = vecRay.CrossProduct(XYZ.BasisZ).Normalize();
            var dot1 = cross.DotProduct(line.GetEndPoint(0) - point);
            var dot2 = cross.DotProduct(line.GetEndPoint(1) - point);
            if (dot1 * dot2 < 0) canRay = true;

            if (vecRay.DotProduct(line.Direction.CrossProduct(XYZ.BasisZ).Normalize()) != 0 && canRay)
            {
                var plane = Plane.CreateByNormalAndOrigin(line.Direction.CrossProduct(XYZ.BasisZ).Normalize(), line.GetEndPoint(0));
                p = point.RayIntersectPlane(vecRay, plane);
            }
            return p;
        }
        public static XYZ IntersectLines2D(Line l1, Line l2, double zElevation = 0)
        {
            var p1s = l1.GetEndPoint(0);
            var p1e = l1.GetEndPoint(1);
            var p2s = l2.GetEndPoint(0);
            var p2e = l2.GetEndPoint(1);
            p1s = new XYZ(p1s.X, p1s.Y, zElevation);
            p1e = new XYZ(p1e.X, p1e.Y, zElevation);
            p2s = new XYZ(p2s.X, p2s.Y, zElevation);
            p2e = new XYZ(p2e.X, p2e.Y, zElevation);
            var dir1 = p1e - p1s;
            var dir2 = p2e - p2s;
            if (!dir1.IsParallel(dir2))
            {
                var plane = Plane.CreateByNormalAndOrigin(dir1.CrossProduct(XYZ.BasisZ).Normalize(), p1s);
                return p2s.RayIntersectPlane(dir2.Normalize(), plane);
            }
            else
            {
                return null;
            }
        }
        public static Line TrimLineToLine2D(Line lineBeTrim, Line lineReference)
        {
            var planeRefer = Plane.CreateByNormalAndOrigin(lineReference.Direction.CrossProduct(XYZ.BasisZ), lineReference.GetEndPoint(0));
            var p1 = lineBeTrim.GetEndPoint(0);
            var p2 = lineBeTrim.GetEndPoint(1);
            var p1OnPlane = p1.RayIntersectPlane(lineBeTrim.Direction.Normalize(), planeRefer);
            var p2OnPlane = p2.RayIntersectPlane(lineBeTrim.Direction.Normalize(), planeRefer);
            return p1.DistanceTo(p1OnPlane) < p2.DistanceTo(p2OnPlane) ? Line.CreateBound(p1OnPlane, p2) : Line.CreateBound(p1, p2OnPlane);
        }
        public static bool IsIntersectWithLineSegmentOnPlane(this Line lineSegment1, Line lineSegment2)
        {
            bool r;

            //Line 1
            var u1 = lineSegment1.Direction.Normalize();
            var n1 = u1.CrossProduct(XYZ.BasisZ).Normalize();
            var start1 = lineSegment1.GetEndPoint(0);
            var end1 = lineSegment1.GetEndPoint(1);

            //Line 2
            var u2 = lineSegment2.Direction.Normalize();
            var n2 = u2.CrossProduct(XYZ.BasisZ).Normalize();
            var start2 = lineSegment2.GetEndPoint(0);
            var end2 = lineSegment2.GetEndPoint(1);

            //If # parallel
            if (!u1.IsParallel(u2))
            {
                bool conditon1;
                bool conditon2;

                var dot11 = (start2 - start1).DotProduct(n1).Round(EPS_DIGITS);
                var dot12 = (end2 - start1).DotProduct(n1).Round(EPS_DIGITS);
                conditon1 = dot11 * dot12 <= 0;

                var dot21 = (start1 - start2).DotProduct(n2).Round(EPS_DIGITS);
                var dot22 = (end1 - start2).DotProduct(n2).Round(EPS_DIGITS);
                conditon2 = dot21 * dot22 <= 0;

                r = conditon1 && conditon2;
            }
            else //If parallel
            {
                //Linesegment lie same Line
                if (start1.DotProduct(n1).IsAlmostEqual(start2.DotProduct(n1), EPS_DECIMAL))
                {
                    bool cond1;
                    bool cond2;
                    bool cond3;

                    var dot1 = (start2 - start1).DotProduct(n1).Round(EPS_DIGITS);
                    var dot2 = (end2 - start1).DotProduct(n1).Round(EPS_DIGITS);
                    cond1 = dot1 * dot2 <= 0;

                    var dot3 = (start2 - end1).DotProduct(n1).Round(EPS_DIGITS);
                    var dot4 = (end2 - end1).DotProduct(n1).Round(EPS_DIGITS);
                    cond2 = dot3 * dot4 <= 0;

                    var dot5 = (start2 - start1).DotProduct(n1).Round(EPS_DIGITS);
                    var dot6 = (end2 - start1).DotProduct(n1).Round(EPS_DIGITS);
                    var dot7 = (end1 - start1).DotProduct(n1).Round(EPS_DIGITS);
                    cond3 = (dot5 <= dot7) && (dot6 <= dot7);

                    r = cond1 || cond2 || cond3;
                }
                else
                {
                    r = false;
                }
            }

            return r;
        }
        public static List<XYZ> FixZ(this List<XYZ> xyzs, double z = 0)
        {
            return xyzs.Select(p => new XYZ(p.X, p.Y, z)).ToList();
        }
        public static XYZ FixZ(this XYZ xyz, double z = 0)
        {
            return new XYZ(xyz.X, xyz.Y, z);
        }
        public static Line FixZ(this Line line, double z = 0)
        {
            Line r = null;
            try
            {
                r = Line.CreateBound(line.GetEndPoint(0).FixZ(z), line.GetEndPoint(1).FixZ(z));
            }
            catch { }
            return r;
        }
    }
}
