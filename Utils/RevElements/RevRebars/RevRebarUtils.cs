using Autodesk.Revit.DB.Structure;
using HcBimUtils;
using HcBimUtils.DocumentUtils;
using HcBimUtils.RebarUtils;
using Newtonsoft.Json;
using RevitDevelop.Utils.RevElementNumberings;
using Utils.CompareElement;
using Utils.Entities;
using Utils.Geometries;

namespace RevitDevelop.Utils.RevElements.RevRebars
{
    public static class RevRebarUtils
    {
        public static void Numbering(List<RevRebar> numberingRevitRebars, List<OptionRebarNumbering> OptionRebarNumberings, SchemaInfo schemaRebarNumberingInfo)
        {
            var rebarsBase = numberingRevitRebars
                .Select(x => (new ElementId(int.Parse(x.ElementId.ToString()))).ToElement(AC.Document) as Rebar)
                .ToList();
            var rebarsBaseGr = rebarsBase
                .GroupBy(x => x.LookupParameter(OptionRebarNumbering.Prefix.ToString()))
                .Select(x => x.ToList())
                .ToList();
            var results = new List<List<RevRebar>>();
            foreach (var rebarsPrefixGr in rebarsBaseGr)
            {
                var rebarsOptionWrap = new List<List<Rebar>>() { rebarsPrefixGr };
                try
                {
                    foreach (var OptionRebarNumbering in OptionRebarNumberings)
                    {
                        var compareRebar = GetCompareRebar(OptionRebarNumbering);
                        var rebarsOptionWrapNumbering = new List<List<Rebar>>();
                        foreach (var rebars in rebarsOptionWrap)
                        {
                            var rebarsOption = rebars
                                        .GroupBy(x => x, compareRebar)
                                        .Select(x => x.ToList())
                                        .ToList();
                            rebarsOptionWrapNumbering.AddRange(rebarsOption);
                        }
                        rebarsOptionWrap = rebarsOptionWrapNumbering;
                    }
                }
                catch (System.Exception)
                {
                    rebarsOptionWrap.Add(rebarsBase);
                }
                var pos = 1;
                foreach (var rebars in rebarsOptionWrap)
                {
                    try
                    {
                        var result = new List<RevRebar>();
                        foreach (var rebar in rebars)
                        {
                            var rebarNumbering = new RevRebar(rebar);
                            rebarNumbering.ElementPosition = string.IsNullOrEmpty(rebarNumbering.Prefix)
                                ? $"{pos}"
                                : $"{rebarNumbering.Prefix}-{pos}";
                            result.Add(rebarNumbering);
                            //write entity info for rebar
                            schemaRebarNumberingInfo.SchemaField.Value = JsonConvert.SerializeObject(rebarNumbering);
                            SchemaInfo.Write(schemaRebarNumberingInfo.SchemaBase, rebar, schemaRebarNumberingInfo.SchemaField);
                        }
                        results.Add(result);
                        pos++;
                    }
                    catch (Exception)
                    {
                    }
                }
            }
            numberingRevitRebars = results.Count == 0 ? numberingRevitRebars : results.Aggregate((a, b) => a.Concat(b).ToList());
        }
        public static CompareRebar GetCompareRebar(OptionRebarNumbering OptionRebarNumbering)
        {
            var result = new CompareRebar(OptionRebarNumbering.Prefix.ToString());
            switch (OptionRebarNumbering)
            {
                case OptionRebarNumbering.Prefix:
                    result = new CompareRebar(OptionRebarNumbering.Prefix.ToString());
                    break;
                case OptionRebarNumbering.Length:
                    result = new CompareRebar(BuiltInParameter.REBAR_ELEM_LENGTH);
                    break;
                case OptionRebarNumbering.RebarShape:
                    result = new CompareRebar(BuiltInParameter.REBAR_SHAPE);
                    break;
                case OptionRebarNumbering.Diameter:
#if REVIT2021
                    result = new CompareRebar(BuiltInParameter.REBAR_BAR_DIAMETER);
#else
                    result = new CompareRebar(BuiltInParameter.REBAR_MODEL_BAR_DIAMETER);
#endif
                    break;
                case OptionRebarNumbering.SCHEDULE_REBAR_ZONE:
                    result = new CompareRebar(OptionRebarNumbering.SCHEDULE_REBAR_ZONE.ToString());
                    break;
                case OptionRebarNumbering.SCHEDULE_REBAR_GROUP_LEVEL:
                    result = new CompareRebar(OptionRebarNumbering.SCHEDULE_REBAR_GROUP_LEVEL.ToString());
                    break;
                case OptionRebarNumbering.StartThread:
                    result = new CompareRebar(OptionRebarNumbering.StartThread.ToString());
                    break;
                case OptionRebarNumbering.EndThread:
                    result = new CompareRebar(OptionRebarNumbering.EndThread.ToString());
                    break;
                case OptionRebarNumbering.Comments:
                    result = new CompareRebar(BuiltInParameter.ALL_MODEL_INSTANCE_COMMENTS);
                    break;
                case OptionRebarNumbering.工区:
                    result = new CompareRebar("工区");
                    break;
            }
            return result;
        }
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
        public static Rebar CreateRebarBaseOldRebar(this Rebar oldRebar, List<Curve> newCurves)
        {
            Rebar result = null;
            try
            {
                var doc = oldRebar.Document;
                var line_active = newCurves.First(x => x.Direction().DotProduct(XYZ.BasisZ).IsAlmostEqual(0));
                var isLengthZero = newCurves.Any(x => x.Length.IsAlmostEqual(0));
                if (!isLengthZero)
                {
                    var rebarStyle = oldRebar.GetRebarStyle();
                    var rebarType = oldRebar.GetRebarBarType();
                    RebarHookType startHookType = null;
                    RebarHookType endHookType = null;
                    var host = doc.GetElement(oldRebar.GetHostId());
                    var vtNorm = oldRebar.GetNormal();
                    var startHookOrien = oldRebar.GetHookOrientation(0);
                    var EndHookOrien = oldRebar.GetHookOrientation(1);
                    result = Rebar.CreateFromCurves(
                            doc,
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
                    if (doc.ActiveView is View3D view3d)
                    {
                        result.SetSolidRebar3DView(view3d);
                        result.SetUnobscuredInView(view3d, true);
                    };
                }
            }
            catch (Exception) { }
            return result;
        }
        public static List<Curve> GetLinesOrigin(Rebar rb)
        {
            var results = new List<Curve>();
            try
            {
                var vty = rb.GetNormal();
                var cs = rb
                    .GetCenterlineCurves(true, false, false, MultiplanarOption.IncludeOnlyPlanarCurves, 0)
                    .Where(x => x is Line)
                    .ToList();
                var cc = cs.Count;
                if (cc == 0) return results;
                if (cc == 1) return results.Concat(cs).ToList();
                var p1 = cs[0].GetEndPoint(0);
                var p2Last = cs[cc - 1].GetEndPoint(1);
                for (var i = 0; i < cc; i++)
                {
                    if (i == cc - 1)
                    {
                        var l = Line.CreateBound(p1, p2Last);
                        results.Add(l);
                    }
                    else
                    {
                        var j = i + 1;
                        var vtx = cs[i].Direction();
                        var vtz = vtx.CrossProduct(vty);
                        var f = new FaceCustom(vtz, cs[i].Midpoint());
                        var p2 = cs[j].Midpoint().RayPointToFace(cs[j].Direction(), f);
                        var l = Line.CreateBound(p1, p2);
                        results.Add(l);
                        p1 = p2;
                    }
                }
            }
            catch (Exception)
            {
            }
            return results;
        }
        public static List<Curve> GetCurvesOrgin(Rebar rb)
        {
            var results = new List<Curve>();
            try
            {
                results = rb.GetCenterlineCurves(true, false, false, MultiplanarOption.IncludeOnlyPlanarCurves, 0).ToList();
            }
            catch (Exception)
            {
            }
            return results;
        }
        public static void SetSolidRebar3DView(this Rebar rebar, Autodesk.Revit.DB.View view)
        {
            if (rebar != null)
            {
                View3D view3D = view as View3D;
#if R21 || R22
                if (view3D != null)
                {
                    rebar.SetSolidInView(view3D, solid: true);
                }
#endif
            }
        }
        public static double GetBarDiameter(this Rebar rebar)
        {

#if (R21 || R20)
         return rebar.get_Parameter(BuiltInParameter.REBAR_BAR_DIAMETER).AsDouble();
#else
            return rebar.get_Parameter(BuiltInParameter.REBAR_MODEL_BAR_DIAMETER).AsDouble();
#endif
        }
    }
}
