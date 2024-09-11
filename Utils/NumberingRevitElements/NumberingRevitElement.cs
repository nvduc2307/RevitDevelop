using Autodesk.Revit.DB.Structure;
using HcBimUtils;

namespace Utils.NumberingRevitElements
{
    public class NumberingRevitElement
    {
        public const string ShareParam_Prefix = "Prefix";
        public const string ShareParam_Zone = "Zone";
        public const string ShareParam_GroupElevation = "GroupElevation";
        public static List<List<Rebar>> NumberingRebars(Document document, List<Rebar> rebarsBase, List<OptionNumberingTypeRebar> optionNumberingTypeRebars)
        {
            var rebarsOptionWrap = new List<List<Rebar>>();
            try
            {
                var optionFirst = optionNumberingTypeRebars.First();
                switch (optionFirst)
                {
                    case OptionNumberingTypeRebar.Prefix:
                        rebarsOptionWrap = rebarsBase
                            .GroupBy(x => x, new CompareRebar(ShareParam_Prefix))
                            .Select(x => x.ToList())
                            .ToList();
                        break;
                    case OptionNumberingTypeRebar.Length:
                        rebarsOptionWrap = rebarsBase
                            .GroupBy(x => x, new CompareRebar(BuiltInParameter.REBAR_ELEM_LENGTH))
                            .Select(x => x.ToList())
                            .ToList();
                        break;
                    case OptionNumberingTypeRebar.RebarShape:
                        rebarsOptionWrap = rebarsBase
                            .GroupBy(x => x, new CompareRebar(BuiltInParameter.REBAR_SHAPE))
                            .Select(x => x.ToList())
                            .ToList();
                        break;
                    case OptionNumberingTypeRebar.Diameter:
                        rebarsOptionWrap = rebarsBase
                            .GroupBy(x => x, new CompareRebar(BuiltInParameter.REBAR_BAR_DIAMETER))
                            .Select(x => x.ToList())
                            .ToList();
                        break;
                    case OptionNumberingTypeRebar.Zone:
                        rebarsOptionWrap = rebarsBase
                            .GroupBy(x => x, new CompareRebar(ShareParam_Zone))
                            .Select(x => x.ToList())
                            .ToList();
                        break;
                    case OptionNumberingTypeRebar.GroupElevation:
                        rebarsOptionWrap = rebarsBase
                            .GroupBy(x => x, new CompareRebar(ShareParam_GroupElevation))
                            .Select(x => x.ToList())
                            .ToList();
                        break;
                }
                var c = 0;
                foreach (var optionNumberingTypeRebar in optionNumberingTypeRebars)
                {
                    if (c != 0)
                    {
                        var dm = new List<List<Rebar>>();
                        foreach (var rebars in rebarsOptionWrap)
                        {
                            var rebarsOption = new List<List<Rebar>>();
                            switch (optionFirst)
                            {
                                case OptionNumberingTypeRebar.Prefix:
                                    rebarsOption = rebars
                                        .GroupBy(x => x, new CompareRebar(ShareParam_Prefix))
                                        .Select(x => x.ToList())
                                        .ToList();
                                    break;
                                case OptionNumberingTypeRebar.Length:
                                    rebarsOption = rebars
                                        .GroupBy(x => x, new CompareRebar(BuiltInParameter.REBAR_ELEM_LENGTH))
                                        .Select(x => x.ToList())
                                        .ToList();
                                    break;
                                case OptionNumberingTypeRebar.RebarShape:
                                    rebarsOption = rebars
                                        .GroupBy(x => x, new CompareRebar(BuiltInParameter.REBAR_SHAPE))
                                        .Select(x => x.ToList())
                                        .ToList();
                                    break;
                                case OptionNumberingTypeRebar.Diameter:
                                    rebarsOption = rebars
                                        .GroupBy(x => x, new CompareRebar(BuiltInParameter.REBAR_BAR_DIAMETER))
                                        .Select(x => x.ToList())
                                        .ToList();
                                    break;
                                case OptionNumberingTypeRebar.Zone:
                                    rebarsOption = rebars
                                        .GroupBy(x => x, new CompareRebar(ShareParam_Zone))
                                        .Select(x => x.ToList())
                                        .ToList();
                                    break;
                                case OptionNumberingTypeRebar.GroupElevation:
                                    rebarsOption = rebars
                                        .GroupBy(x => x, new CompareRebar(ShareParam_GroupElevation))
                                        .Select(x => x.ToList())
                                        .ToList();
                                    break;
                            }
                            dm.AddRange(rebarsOption);
                        }
                        rebarsOptionWrap = dm;
                    }
                    c++;
                }
            }
            catch (System.Exception)
            {
                rebarsOptionWrap.Add(rebarsBase);
            }
            return rebarsOptionWrap;
        }
    }
    public enum OptionNumberingTypeRebar
    {
        Prefix = 1,
        Length = 2,
        RebarShape = 3,
        Diameter = 4,
        Zone = 5,
        GroupElevation = 6,
    }
    public class CompareRebar : IEqualityComparer<Rebar>
    {
        private BuiltInParameter _builtInParameter = BuiltInParameter.INVALID;
        private string _builtInParameterName;
        public CompareRebar(BuiltInParameter builtInParameter)
        {
            _builtInParameter = builtInParameter;
        }
        public CompareRebar(string builtInParameterName)
        {
            _builtInParameterName = builtInParameterName;
        }
        public bool Equals(Rebar x, Rebar y)
        {
            try
            {
                var paraX = _builtInParameter != BuiltInParameter.INVALID
                    ? x.get_Parameter(_builtInParameter)
                    : x.LookupParameter(_builtInParameterName);
                var paraY = _builtInParameter != BuiltInParameter.INVALID
                    ? y.get_Parameter(_builtInParameter)
                    : y.LookupParameter(_builtInParameterName);
                var paraType = paraX.StorageType;
                var result = false;
                switch (paraType)
                {
                    case StorageType.None:
                        result = false;
                        break;
                    case StorageType.Integer:
                        result = paraX.AsInteger().Equals(paraY.AsInteger());
                        break;
                    case StorageType.Double:
                        result = paraX.AsDouble().IsAlmostEqual(paraY.AsDouble());
                        break;
                    case StorageType.String:
                        result = paraX.AsValueString().IsEqual(paraY.AsValueString());
                        break;
                    case StorageType.ElementId:
                        result = paraX.AsElementId().ToString().IsEqual(paraY.AsElementId().ToString());
                        break;
                }
                return result;
            }
            catch (System.Exception)
            {
                return false;
            }
        }

        public int GetHashCode(Rebar obj)
        {
            return obj.GetHashCode();
        }
    }
}
