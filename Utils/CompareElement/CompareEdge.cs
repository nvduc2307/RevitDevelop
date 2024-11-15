using HcBimUtils;
using Utils.Geometries;

namespace RevitDevelop.Utils.CompareElement
{
    public class CompareEdge : IEqualityComparer<Edge>
    {
        private FaceCustom FaceCustom;
        public CompareEdge(FaceCustom faceCustom)
        {
            FaceCustom = faceCustom;
        }
        public bool Equals(Edge x, Edge y)
        {
            var c1 = x.AsCurve();
            var c2 = y.AsCurve();
            var dir1 = c1.Direction();
            var dir2 = c2.Direction();
            if (!dir1.IsSameDirection(dir2)) return false;
            var p1 = c1.Midpoint().RayPointToFace(dir1, FaceCustom);
            var p2 = c2.Midpoint().RayPointToFace(dir2, FaceCustom);
            return p1 == null ? false : p2 == null ? false : p1.IsSeem(p2);
        }

        public int GetHashCode(Edge obj)
        {
            return 0;
        }
    }
}
