using Autodesk.Revit.DB.Structure;

namespace RevitDevelop.Utils.RevElements.RevRebars
{
    public class RebarBarTypeCustom
    {
        public RebarBarType RebarBarType { get; set; }
        public string Name { get; set; }
        public string NameStyle { get; set; }
        public double ModelBarDiameter { get; set; }
        public double BarDiameter { get; set; }
        public double BarDiameterReal { get; set; }
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
        private void GetRebarBarTypeProperties()
        {
            NameStyle = RebarBarType.get_Parameter(BuiltInParameter.ALL_MODEL_TYPE_NAME).AsValueString();
#if R21
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
