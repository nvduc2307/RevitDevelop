using Autodesk.Revit.DB.Structure;
using HcBimUtils;
using HcBimUtils.DocumentUtils;
using HcBimUtils.RebarUtils;
using Newtonsoft.Json;
using Utils.CompareElement;
using Utils.Entities;
using Utils.NumberingRevitElements;
using Utils.RebarInRevits.Utils;
using Utils.Units;

namespace RevitDevelop.Utils.NumberingRevitElements
{
    public class NumberingRevitRebar : NumberingRevitElement
    {
        public int RebarLayer { get; set; }
        public string GroupElevation { get; set; }
        public double Diameter { get; set; } //[mm]
        public double LengthPerOne { get; set; }// [mm]
        public double LengthOrder { get; set; }// [mm]
        public double TotalLength { get; set; }// [mm]
        public double WeightPerOne { get; set; }// [kg]
        public double TotalWeight { get; set; }// [kg]
        public int Quantity { get; set; }
        public bool StartThread { get; set; } // Ren
        public bool EndThread { get; set; } // Ren
        public int CouplerCount { get; set; }
        public int WeldCount { get; set; }
        public string Image { get; set; }
        public string PathSaveImage { get; set; }
        public int Unit { get; set; }
        public NumberingRevitRebar(Rebar rebar)
        {
            ElementId = int.Parse(rebar.Id.ToString());
#if REVIT2021
            Diameter = rebar.get_Parameter(BuiltInParameter.REBAR_BAR_DIAMETER).AsDouble().FootToMm();
#else
            Diameter = rebar.get_Parameter(BuiltInParameter.REBAR_MODEL_BAR_DIAMETER).AsDouble().FootToMm();
#endif
            Name = rebar.GetRebarBarType().Name;
            GroupElevation = GetGroupElevation(rebar);
            Prefix = GetPrefix(rebar);
            Zone = GetZone(rebar);
            Quantity = rebar.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).AsInteger();
            LengthPerOne = Math.Round(rebar.get_Parameter(BuiltInParameter.REBAR_ELEM_LENGTH).AsDouble().FootToMm(), 0);
            LengthOrder = RebarLengthUtils.OptimizeRebarLength(LengthPerOne);
            TotalLength = LengthPerOne * Quantity;
            WeightPerOne = GetWeightPerOne();
            TotalWeight = WeightPerOne * Quantity;
            StartThread = GetStartThread(rebar);
            EndThread = GetEndThread(rebar);
            CouplerCount = GetCouplerCount();
            Unit = (int)UnitEnum.cm;
        }
        public NumberingRevitRebar() { }
        private int GetCouplerCount()
        {
            var result = 0;
            if (StartThread) result++;
            if (EndThread) result++;
            return result;
        }
        private bool GetStartThread(Rebar rebar)
        {
            var hasParam = ElementUtils.HasParameter(rebar, OptionNumberingTypeRebar.StartThread.ToString());
            return hasParam ? rebar.LookupParameter(OptionNumberingTypeRebar.StartThread.ToString()).AsInteger() == 1 : false;
        }
        private bool GetEndThread(Rebar rebar)
        {
            var hasParam = ElementUtils.HasParameter(rebar, OptionNumberingTypeRebar.EndThread.ToString());
            return hasParam ? rebar.LookupParameter(OptionNumberingTypeRebar.EndThread.ToString()).AsInteger() == 1 : false;
        }
        private double GetWeightPerOne()
        {
            var tlr = 7850.0; //[kg/m3]
            var d = Diameter * 1e-3; //[m]
            var s = 0.25 * Math.PI * d * d; //[m2]
            var v = s * LengthPerOne * 1e-3; //[m3]
            return v * tlr; //[kg]
        }
        private string GetGroupElevation(Rebar rebar)
        {
            var hasParam = ElementUtils.HasParameter(rebar, OptionNumberingTypeRebar.SCHEDULE_REBAR_GROUP_LEVEL.ToString());
            return hasParam ? rebar.LookupParameter(OptionNumberingTypeRebar.SCHEDULE_REBAR_GROUP_LEVEL.ToString()).AsValueString() : string.Empty;
        }
        private string GetPrefix(Rebar rebar)
        {
            var hasParam = ElementUtils.HasParameter(rebar, OptionNumberingTypeRebar.Prefix.ToString());
            return hasParam ? rebar.LookupParameter(OptionNumberingTypeRebar.Prefix.ToString()).AsValueString() : string.Empty;
        }
        private string GetZone(Rebar rebar)
        {
            var hasParam = ElementUtils.HasParameter(rebar, OptionNumberingTypeRebar.SCHEDULE_REBAR_ZONE.ToString());
            return hasParam ? rebar.LookupParameter(OptionNumberingTypeRebar.SCHEDULE_REBAR_ZONE.ToString()).AsValueString() : string.Empty;
        }
        public static void Numbering(List<NumberingRevitRebar> numberingRevitRebars, List<OptionNumberingTypeRebar> optionNumberingTypeRebars, SchemaInfo schemaRebarNumberingInfo)
        {
            var rebarsBase = numberingRevitRebars
                .Select(x => (new ElementId(int.Parse(x.ElementId.ToString()))).ToElement(AC.Document) as Rebar)
                .ToList();
            var rebarsBaseGr = rebarsBase
                .GroupBy(x => x.LookupParameter(OptionNumberingTypeRebar.Prefix.ToString()))
                .Select(x => x.ToList())
                .ToList();
            var results = new List<List<NumberingRevitRebar>>();
            foreach (var rebarsPrefixGr in rebarsBaseGr)
            {
                var rebarsOptionWrap = new List<List<Rebar>>() { rebarsPrefixGr };
                try
                {
                    foreach (var optionNumberingTypeRebar in optionNumberingTypeRebars)
                    {
                        var compareRebar = GetCompareRebar(optionNumberingTypeRebar);
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
                        var result = new List<NumberingRevitRebar>();
                        foreach (var rebar in rebars)
                        {
                            var rebarNumbering = new NumberingRevitRebar(rebar);
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

        public static CompareRebar GetCompareRebar(OptionNumberingTypeRebar optionNumberingTypeRebar)
        {
            var result = new CompareRebar(OptionNumberingTypeRebar.Prefix.ToString());
            switch (optionNumberingTypeRebar)
            {
                case OptionNumberingTypeRebar.Prefix:
                    result = new CompareRebar(OptionNumberingTypeRebar.Prefix.ToString());
                    break;
                case OptionNumberingTypeRebar.Length:
                    result = new CompareRebar(BuiltInParameter.REBAR_ELEM_LENGTH);
                    break;
                case OptionNumberingTypeRebar.RebarShape:
                    result = new CompareRebar(BuiltInParameter.REBAR_SHAPE);
                    break;
                case OptionNumberingTypeRebar.Diameter:
#if REVIT2021
                    result = new CompareRebar(BuiltInParameter.REBAR_BAR_DIAMETER);
#else
                    result = new CompareRebar(BuiltInParameter.REBAR_MODEL_BAR_DIAMETER);
#endif
                    break;
                case OptionNumberingTypeRebar.SCHEDULE_REBAR_ZONE:
                    result = new CompareRebar(OptionNumberingTypeRebar.SCHEDULE_REBAR_ZONE.ToString());
                    break;
                case OptionNumberingTypeRebar.SCHEDULE_REBAR_GROUP_LEVEL:
                    result = new CompareRebar(OptionNumberingTypeRebar.SCHEDULE_REBAR_GROUP_LEVEL.ToString());
                    break;
                case OptionNumberingTypeRebar.StartThread:
                    result = new CompareRebar(OptionNumberingTypeRebar.StartThread.ToString());
                    break;
                case OptionNumberingTypeRebar.EndThread:
                    result = new CompareRebar(OptionNumberingTypeRebar.EndThread.ToString());
                    break;
                case OptionNumberingTypeRebar.Comments:
                    result = new CompareRebar(BuiltInParameter.ALL_MODEL_INSTANCE_COMMENTS);
                    break;
                case OptionNumberingTypeRebar.工区:
                    result = new CompareRebar("工区");
                    break;
            }
            return result;
        }
    }
    public enum OptionNumberingTypeRebar
    {
        Prefix = 1,
        Length = 2,
        RebarShape = 3,
        Diameter = 4,
        SCHEDULE_REBAR_ZONE = 5,
        SCHEDULE_REBAR_GROUP_LEVEL = 6,
        StartThread = 7,
        EndThread = 8,
        Comments = 9,
        工区 = 10,//zone,
        GroupElevation = 11,
        Group = 11,
        Zone = 12,
    }
}
