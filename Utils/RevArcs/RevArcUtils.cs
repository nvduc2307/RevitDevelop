using Utils.RevPoints;

namespace Utils.RevArcs
{
    public static class RevArcUtils
    {
        public static XYZ GetCenter(Arc arc)
        {
            return arc.Center;
        }
        public static XYZ GetNormal(Arc arc) { return arc.Normal; }
    }
    public class ArcCustom
    {
        public Arc Arc { get; }
        public XYZ Center { get; }
        public XYZ Normal { get; }
        public XYZ Start { get; }
        public XYZ End { get; }
        public XYZ Mid { get; }
        public List<Curve> Curves { get; }

        public ArcCustom(Arc arc)
        {
            Arc = arc;
            Center = arc.Center;
            Normal = arc.Normal;
            var ps = Arc.Tessellate();
            Start = ps.FirstOrDefault();
            End = ps.FirstOrDefault();
            Mid = ps.Count() > 2 ? ps[2] : null;
            Curves = arc.Tessellate().ToList().PointsToCurves();
        }
    }
}
