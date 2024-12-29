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
        public static  void CreateModelCurve(this Line line, Document document)
        {
            try
            {
                var nor = line.Direction.IsParallel(XYZ.BasisZ)
                    ? line.Direction.CrossProduct(XYZ.BasisX)
                    : line.Direction.CrossProduct(XYZ.BasisZ);
                var plane = Plane.CreateByNormalAndOrigin(nor, line.Midpoint());
                var sket = SketchPlane.Create(document, plane);
                document.Create.NewModelCurve(line, sket);
            }
            catch (Exception)
            {
            }
        }
    }
}
