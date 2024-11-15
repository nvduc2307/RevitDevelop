using Autodesk.Revit.DB.Structure;
using HcBimUtils.DocumentUtils;
using Newtonsoft.Json;
using RevitDevelop.Utils.RevElementNumberings;
using Utils.CompareElement;
using Utils.Entities;

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
    }
}
