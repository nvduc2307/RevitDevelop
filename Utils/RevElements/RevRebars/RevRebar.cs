using Autodesk.Revit.DB.Structure;
using HcBimUtils;
using HcBimUtils.RebarUtils;
using RevitDevelop.Utils.RevElementNumberings;
using Utils.RevElementNumberings;
using Utils.Units;

namespace RevitDevelop.Utils.RevElements.RevRebars
{
    public class RevRebar : RevElementNumbering
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
        public RevRebar(Rebar rebar)
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
            LengthOrder = LengthPerOne;
            TotalLength = LengthPerOne * Quantity;
            WeightPerOne = GetWeightPerOne();
            TotalWeight = WeightPerOne * Quantity;
            StartThread = GetStartThread(rebar);
            EndThread = GetEndThread(rebar);
            CouplerCount = GetCouplerCount();
            Unit = (int)UnitEnum.cm;
        }
        private int GetCouplerCount()
        {
            var result = 0;
            if (StartThread) result++;
            if (EndThread) result++;
            return result;
        }
        private bool GetStartThread(Rebar rebar)
        {
            var hasParam = ElementUtils.HasParameter(rebar, OptionRebarNumbering.StartThread.ToString());
            return hasParam ? rebar.LookupParameter(OptionRebarNumbering.StartThread.ToString()).AsInteger() == 1 : false;
        }
        private bool GetEndThread(Rebar rebar)
        {
            var hasParam = ElementUtils.HasParameter(rebar, OptionRebarNumbering.EndThread.ToString());
            return hasParam ? rebar.LookupParameter(OptionRebarNumbering.EndThread.ToString()).AsInteger() == 1 : false;
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
            var hasParam = ElementUtils.HasParameter(rebar, OptionRebarNumbering.SCHEDULE_REBAR_GROUP_LEVEL.ToString());
            return hasParam ? rebar.LookupParameter(OptionRebarNumbering.SCHEDULE_REBAR_GROUP_LEVEL.ToString()).AsValueString() : string.Empty;
        }
        private string GetPrefix(Rebar rebar)
        {
            var hasParam = ElementUtils.HasParameter(rebar, OptionRebarNumbering.Prefix.ToString());
            return hasParam ? rebar.LookupParameter(OptionRebarNumbering.Prefix.ToString()).AsValueString() : string.Empty;
        }
        private string GetZone(Rebar rebar)
        {
            var hasParam = ElementUtils.HasParameter(rebar, OptionRebarNumbering.SCHEDULE_REBAR_ZONE.ToString());
            return hasParam ? rebar.LookupParameter(OptionRebarNumbering.SCHEDULE_REBAR_ZONE.ToString()).AsValueString() : string.Empty;
        }
    }
}
