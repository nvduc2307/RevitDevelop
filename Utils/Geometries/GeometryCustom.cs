﻿using HcBimUtils;
using Utils.RevArcs;
using Utils.RevEllipses;

namespace Utils.Geometries
{
    public static class GeometryCustom
    {
        public static double Distance(this XYZ p, Line l)
        {
            var d = 0.0;
            try
            {
                d = p.Distance(l.GetEndPoint(0));
                var dir = l.Direction;
                var vt = (l.GetEndPoint(0) - p).Normalize();
                if (dir.DotProduct(vt).IsAlmostEqual(0)) return p.Distance(l.GetEndPoint(0));
                if (Math.Abs(dir.DotProduct(vt)).IsAlmostEqual(1)) return 0;

                var angle = dir.DotProduct(vt) > 0
                    ? vt.AngleTo(dir)
                    : vt.AngleTo(-dir);
                d = Math.Sin(angle) * d;
            }
            catch (Exception)
            {
                d = 0.0;
            }
            return d;
        }

        public static double Distance(this XYZ p)
        {
            var result = 0.0;
            try
            {
                result = Math.Sqrt(p.X * p.X + p.Y * p.Y + p.Z * p.Z);
            }
            catch (Exception)
            {
            }
            return result;
        }

        public static double Distance(this XYZ p1, XYZ p2)
        {
            var x = p1.X - p2.X;
            var y = p1.Y - p2.Y;
            var z = p1.Z - p2.Z;
            return Math.Sqrt(x * x + y * y + z * z);
        }

        public static double Distance(this XYZ p, FaceCustom faceCad)
        {
            var result = 0.0;
            try
            {
                var d = p.Distance(faceCad.BasePoint);
                var vt = (faceCad.BasePoint - p).VectorNormal();
                var angle = faceCad.Normal.DotProduct(vt) >= 0
                    ? faceCad.Normal.AngleTo(vt)
                    : faceCad.Normal.AngleTo(-vt);
                result = Math.Cos(angle) * d;
            }
            catch (Exception)
            {
            }
            return result;
        }

        public static XYZ VectorNormal(this XYZ vt)
        {
            return vt / vt.Distance();
        }

        public static XYZ MidPoint(this XYZ p1, XYZ p2)
        {
            var x = (p1.X + p2.X) * 0.5;
            var y = (p1.Y + p2.Y) * 0.5;
            var z = (p1.Z + p2.Z) * 0.5;
            return new XYZ(x, y, z);
        }

        public static XYZ RayPointToFace(this XYZ p, XYZ vtRay, FaceCustom faceCad)
        {
            XYZ result = p;
            try
            {
                var vt = (faceCad.BasePoint - p).VectorNormal();
                var normalFace = vt.DotProduct(faceCad.Normal) >= 0 ? faceCad.Normal : -faceCad.Normal;
                var angle1 = normalFace.AngleTo(vt);
                var angle2 = normalFace.AngleTo(vtRay);

                var angle1D = normalFace.AngleTo(vt) * 180 / Math.PI;
                var angle2D = normalFace.AngleTo(vtRay) * 180 / Math.PI;

                var dm = p.Distance(faceCad.BasePoint);

                var dd = p.Distance(faceCad);

                var d = Math.Cos(angle1) * p.Distance(faceCad.BasePoint) / Math.Cos(angle2);
                result = p + vtRay * d;
            }
            catch (Exception)
            {
                result = p;
            }
            return result;
        }

        public static XYZ LineIntersectFace(this Line line, FaceCustom faceCad)
        {
            XYZ result = null;
            try
            {
                var p1 = line.GetEndPoint(0);
                var p2 = line.GetEndPoint(1);
                var p = line.Midpoint().RayPointToFace(line.Direction, faceCad);
                var vt1 = p1 - p;
                var vt2 = p2 - p;
                if (vt1.DotProduct(vt2) < 0) result = p;
            }
            catch (Exception)
            {
            }
            return result;
        }

        public static List<XYZ> ArcIntersectFace(this ArcCustom arc, FaceCustom faceCustom)
        {
            var reuslts = new List<XYZ>();
            foreach (Line curve in arc.Curves)
            {
                var p = curve.LineIntersectFace(faceCustom);
                if (p != null) reuslts.Add(p);
            }
            return reuslts;
        }

        public static List<XYZ> CurveIntersectFace(this Curve curve, FaceCustom faceCad)
        {
            var result = new List<XYZ>();
            if (curve is Line line)
            {
                result.Add(line.LineIntersectFace(faceCad));
            }
            if (curve is Arc arc)
            {
                var arcCustom = new ArcCustom(arc);
                var ps = arcCustom.ArcIntersectFace(faceCad);
                if (ps.Count > 0) result.AddRange(ps);
            }
            if (curve is Ellipse ellipse)
            {
                var ellipseCustom = new EllipseCustom(ellipse);
                var ps = ellipseCustom.EllipesIntersectFace(faceCad);
                if (ps.Count > 0) result.AddRange(ps);
            }
            return result;
        }

        public static List<XYZ> EllipesIntersectFace(this EllipseCustom ellipseCustom, FaceCustom faceCustom)
        {
            var reuslts = new List<XYZ>();
            foreach (Line curve in ellipseCustom.Curves)
            {
                var p = curve.LineIntersectFace(faceCustom);
                if (p != null) reuslts.Add(p);
            }
            return reuslts;
        }

        public static double AngleTo(this XYZ vt1, XYZ vt2)
        {
            var result = 0.0;
            try
            {
                var cos = vt1.DotProduct(vt2) / (vt1.Distance() * vt2.Distance());
                result = Math.Acos(cos);
            }
            catch (Exception)
            {
            }
            return result;
        }

        public static XYZ Round(this XYZ p, int n = 4)
        {
            return new XYZ(Math.Round(p.X, n), Math.Round(p.Y, n), Math.Round(p.Z, n));
        }

        public static XYZ PointToLine(this XYZ p, Line l)
        {
            var result = p;
            try
            {
                var dir = (l.GetEndPoint(1) - l.GetEndPoint(0)).Normalize();
                var d = p.Distance(l.GetEndPoint(0));
                var vt = (l.GetEndPoint(0) - p).Normalize();
                if (dir.DotProduct(vt).IsAlmostEqual(0)) return l.GetEndPoint(0);
                if (Math.Abs(dir.DotProduct(vt)).IsAlmostEqual(1, 0.00000001)) return p;

                var normal = dir.CrossProduct(vt);

                var vti = dir.CrossProduct(normal).DotProduct(vt) > 0
                    ? dir.CrossProduct(normal).Normalize()
                    : -dir.CrossProduct(normal).Normalize();

                var angle = dir.DotProduct(vt) > 0
                    ? vt.AngleTo(dir)
                    : vt.AngleTo(-dir);

                d = Math.Sin(angle) * d;

                result = p + vti * d;
            }
            catch (Exception)
            {
                result = p;
            }
            //ddang sai
            return result;
        }

        public static XYZ Mirror(this XYZ p, Line l)
        {
            var pm = p.PointToLine(l);
            return p.Mirror(pm);
        }

        public static XYZ Mirror(this XYZ p, XYZ pc)
        {
            return new XYZ(pc.X * 2 - p.X, pc.Y * 2 - p.Y, pc.Z * 2 - p.Z);
        }

        public static ModelCurve CreateModelLine(this Document doc, Line l, XYZ normal)
        {
            ModelCurve ml = null;
            try
            {
                var pl = Plane.CreateByNormalAndOrigin(normal, l.Midpoint());
                var sk = SketchPlane.Create(doc, pl);
                ml = doc.Create.NewModelCurve(l, sk);
            }
            catch (Exception)
            {
            }
            return ml;
        }

        public static bool IsSeem(this XYZ p1, XYZ p2)
        {
            return p1.X.IsAlmostEqual(p2.X) && p1.Y.IsAlmostEqual(p2.Y) && p1.Z.IsAlmostEqual(p2.Z);
        }
    }
}
