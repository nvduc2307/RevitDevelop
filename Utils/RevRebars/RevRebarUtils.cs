using Autodesk.Revit.DB.Structure;
using Utils.CompareElement;

namespace RevitDevelop.Utils.RevRebars
{
    public static class RevRebarUtils
    {
        public static XYZ GetNormal(this Rebar rebar)
        {
            XYZ result = null;
            try
            {
                result = rebar.GetShapeDrivenAccessor().Normal;
            }
            catch (Exception)
            {
            }
            return result;
        }
        public static List<XYZ> GetRebarPoints(this Rebar rebar)
        {
            var result = new List<XYZ>();
            try
            {
                var curves = rebar
                    .GetCenterlineCurves(false, false, false, MultiplanarOption.IncludeOnlyPlanarCurves, 0);
                foreach (var curve in curves)
                {
                    result.Add(curve.GetEndPoint(0));
                    result.Add(curve.GetEndPoint(1));
                }
            }
            catch (Exception)
            {
            }
            return result.Distinct(new ComparePoint()).ToList();
        }
    }
}
