using HcBimUtils;

namespace RevitDevelop.Utils.RevCurves
{
    public static class RevCurveUtils
    {
        public static List<XYZ> GetPoints(this List<Curve> curves)
        {
            var ps = curves.Select(x => x.SP()).ToList();
            ps.Add(curves.Last().EP());
            return ps;
        }
    }
}
