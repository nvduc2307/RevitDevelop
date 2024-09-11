using Autodesk.Revit.DB.Structure;
using HcBimUtils.RebarUtils;
using Utils.SkipWarnings;

namespace Utils.RebarInRevits.Utils
{
    public static class UtilsRevRebar
    {
        public static Rebar CreateRebarBaseOldRebar(this Rebar oldRebar, Document document, List<Curve> newCurves)
        {
            Rebar newRebar = null;
            try
            {
                XYZ rebarNormalNew = oldRebar.GetNormal();
                var rebarStyle = oldRebar.GetRebarStyle();
                var rebarType = oldRebar.GetRebarBarType();
                RebarHookType startHookType = null;
                RebarHookType endHookType = null;
                var host = document.GetElement(oldRebar.GetHostId());
                var vtNorm = oldRebar.IsRebarShapeDriven()
                    ? oldRebar.GetShapeDrivenAccessor().Normal
                    : rebarNormalNew;
                var startHookOrien = oldRebar.GetHookOrientation(0);
                var EndHookOrien = oldRebar.GetHookOrientation(1);
                Rebar rebarNew = null;
                rebarNew = Rebar.CreateFromCurves(
                    document,
                    rebarStyle,
                    rebarType,
                    startHookType,
                    endHookType,
                    host,
                    vtNorm,
                    newCurves,
                    startHookOrien,
                    EndHookOrien,
                    true,
                    true);
                document.Delete(oldRebar.Id);
            }
            catch (Exception)
            {
                newRebar = null;
            }
            return newRebar;
        }

        public static Rebar CopyRebar(this Rebar rebarBase, Document document, XYZ dir, double spacing)
        {
            Rebar rebar = null;
            using (var ts = new Transaction(document, "CopyRebar"))
            {
                ts.SkipAllWarnings();
                ts.Start();
                rebar = document.GetElement(rebarBase.Copy(dir * spacing).FirstOrDefault()) as Rebar;
                ts.Commit();
            }
            return rebar;
        }

        public static void RotateRebar(this Rebar rebar, Document document, XYZ center, XYZ dir, double angleRad)
        {
            using (var ts = new Transaction(document, "RotateRebar"))
            {
                ts.SkipAllWarnings();
                ts.Start();
                //--------
                ElementTransformUtils.RotateElement(document, rebar.Id, Line.CreateBound(center, center + dir * 1), angleRad);
                //--------
                ts.Commit();
            }
        }

        public static void SetRebarSolid(this Rebar rebar, Document document)
        {
            using (var ts = new Transaction(document, "name transaction"))
            {
                ts.Start();
                //--------
                if (document.ActiveView is View3D view3d)
                {
                    rebar.SetSolidRebarIn3DView(view3d);
                }
                ts.Commit();
            }
        }

        public static int GetQtyRebar(this double length, double spacing, out double phandu)
        {
            phandu = length % spacing;
            var qty = length != 0
                ? int.Parse(Math.Round((length - phandu) / spacing + 1, 0).ToString())
                : 0;
            return qty;
        }

    }
}
