using Utils.CompareElement;
using Utils.RevPoints;

namespace Utils.RevEllipses
{
    public class RevEllipseUtils
    {
    }
    public class EllipseCustom
    {
        public Ellipse Ellipse { get; }
        public XYZ Center { get; }
        public XYZ Normal { get; }
        public XYZ Start { get; }
        public XYZ End { get; }
        public XYZ Mid { get; }
        public List<Curve> Curves { get; }

        public EllipseCustom(Ellipse ellipse)
        {
            Ellipse = ellipse;
            Center = ellipse.Center;
            Normal = ellipse.Normal;
            var ps = ellipse.Tessellate();
            Start = ps.FirstOrDefault();
            End = ps.FirstOrDefault();
            Mid = ps.Count() > 2 ? ps[2] : null;
            Curves = ellipse
                .Tessellate()
                .Distinct(new ComparePoint())
                .ToList()
                .PointsToCurves();
        }
    }
}
