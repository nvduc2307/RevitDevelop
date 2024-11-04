using Autodesk.Revit.DB.Structure;
using HcBimUtils;
using Newtonsoft.Json;
using System.Diagnostics;
using System.IO;
using Utils.FilterElements;

namespace Utils.RebarInRevits.Models
{
    public class RebarBarTypeCustom
    {
        public RebarBarType RebarBarType { get; set; }
        public string Name { get; set; }
        public string NameStyle { get; set; }
        public double ModelBarDiameter { get; set; }
        public double BarDiameter { get; set; }
        public double StandardBendDiameter { get; set; }
        public double StandardHookBendDiameter { get; set; }
        public double StirrupOrTieBendDiameter { get; set; }
        public double MaximumBendRadius { get; set; }
        public RebarBarTypeCustom(RebarBarType rebarBarType)
        {
            RebarBarType = rebarBarType;
            GetRebarBarTypeProperties();
        }
        public RebarBarTypeCustom()
        {

        }
        public static void LoadDataRebarBarDiameter(Document document)
        {
            var pathData = $"{PathInWindows.PathInWindow.PathData}RebarbarTypeData.json";
            if (File.Exists(pathData))
            {
                var contents = JsonConvert.DeserializeObject<List<RebarBarTypeCustom>>(File.ReadAllText(pathData));
                var rebarbarTypes = document.GetElementsFromClass<RebarBarType>();
                var typeBase = rebarbarTypes.FirstOrDefault();
                if (typeBase != null)
                {
                    foreach (var item in contents)
                    {
                        try
                        {
                            var isExited = rebarbarTypes.Find(x => x.Name == item.NameStyle);
                            var typeNew = isExited == null
                                ? typeBase.Duplicate(item.NameStyle) as RebarBarType
                                : isExited;
#if REVIT2021
                            typeNew.get_Parameter(BuiltInParameter.REBAR_BAR_DIAMETER).Set(item.BarDiameter.MmToFoot());
#else 
                            typeNew.get_Parameter(BuiltInParameter.REBAR_BAR_DIAMETER).Set(item.BarDiameter.MmToFoot());
                            typeNew.get_Parameter(BuiltInParameter.REBAR_MODEL_BAR_DIAMETER).Set(item.ModelBarDiameter.MmToFoot());
#endif
                            typeNew.get_Parameter(BuiltInParameter.REBAR_STANDARD_BEND_DIAMETER).Set(item.StandardBendDiameter.MmToFoot());
                            typeNew.get_Parameter(BuiltInParameter.REBAR_STANDARD_HOOK_BEND_DIAMETER).Set(item.StandardHookBendDiameter.MmToFoot());
                            typeNew.get_Parameter(BuiltInParameter.REBAR_BAR_STIRRUP_BEND_DIAMETER).Set(item.StirrupOrTieBendDiameter.MmToFoot());

                            Debug.WriteLine(item.BarDiameter);
                        }
                        catch (Exception)
                        {
                        }
                    }
                }
            }
        }
        private void GetRebarBarTypeProperties()
        {
            NameStyle = RebarBarType.get_Parameter(BuiltInParameter.ALL_MODEL_TYPE_NAME).AsValueString();
#if REVIT2021
            BarDiameter = RebarBarType.get_Parameter(BuiltInParameter.REBAR_BAR_DIAMETER).AsDouble();
            ModelBarDiameter = RebarBarType.get_Parameter(BuiltInParameter.REBAR_BAR_DIAMETER).AsDouble();
#else
            BarDiameter = RebarBarType.get_Parameter(BuiltInParameter.REBAR_BAR_DIAMETER).AsDouble();
            ModelBarDiameter = RebarBarType.get_Parameter(BuiltInParameter.REBAR_MODEL_BAR_DIAMETER).AsDouble();
#endif
            StandardBendDiameter = RebarBarType.get_Parameter(BuiltInParameter.REBAR_STANDARD_BEND_DIAMETER).AsDouble();
            StandardHookBendDiameter = RebarBarType.get_Parameter(BuiltInParameter.REBAR_STANDARD_HOOK_BEND_DIAMETER).AsDouble();
            StirrupOrTieBendDiameter = RebarBarType.get_Parameter(BuiltInParameter.REBAR_BAR_STIRRUP_BEND_DIAMETER).AsDouble();
            MaximumBendRadius = RebarBarType.get_Parameter(BuiltInParameter.REBAR_BAR_MAXIMUM_BEND_RADIUS).AsDouble();
        }
    }
}
