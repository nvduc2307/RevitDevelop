﻿using Autodesk.Revit.DB.Structure;
using HcBimUtils;
using HcBimUtils.RebarUtils;
using Newtonsoft.Json;
using Utils.CompareElement;
using Utils.Entities;
using Utils.NumberingRevitElements;

namespace RevitDevelop.Utils.NumberingRevitElements
{
    public class NumberingRevitRebar : NumberingRevitElement
    {
        public string GroupElevation { get; set; }
        public double Diameter { get; set; } //[mm]
        public double LengthPerOne { get; set; }// [mm]
        public double TotalLength { get; set; }// [mm]
        public double WeightPerOne { get; set; }// [kg]
        public double TotalWeight { get; set; }// [kg]
        public long Quantity { get; set; }
        public bool StartThread { get; set; } // Ren
        public bool EndThread { get; set; } // Ren
        public long CouplerCount { get; set; }
        public NumberingRevitRebar(Rebar rebar)
        {
            ElementId = long.Parse(rebar.Id.ToString());
            Diameter = rebar.get_Parameter(BuiltInParameter.REBAR_MODEL_BAR_DIAMETER).AsDouble().FootToMm();
            Name = rebar.GetRebarBarType().Name;
            GroupElevation = GetGroupElevation(rebar);
            Prefix = GetPrefix(rebar);
            Zone = GetZone(rebar);
            Quantity = rebar.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).AsInteger();
            LengthPerOne = Math.Round(rebar.get_Parameter(BuiltInParameter.REBAR_ELEM_LENGTH).AsDouble().FootToMm(), 0);
            TotalLength = LengthPerOne * Quantity;
            WeightPerOne = GetWeightPerOne();
            TotalWeight = WeightPerOne * Quantity;
            StartThread = GetStartThread(rebar);
            EndThread = GetEndThread(rebar);
            CouplerCount = GetCouplerCount();
        }
        public NumberingRevitRebar() { }
        private long GetCouplerCount()
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
            var hasParam = ElementUtils.HasParameter(rebar, OptionNumberingTypeRebar.GroupElevation.ToString());
            return hasParam ? rebar.LookupParameter(OptionNumberingTypeRebar.GroupElevation.ToString()).AsValueString() : string.Empty;
        }
        private string GetPrefix(Rebar rebar)
        {
            var hasParam = ElementUtils.HasParameter(rebar, OptionNumberingTypeRebar.Prefix.ToString());
            return hasParam ? rebar.LookupParameter(OptionNumberingTypeRebar.Prefix.ToString()).AsValueString() : string.Empty;
        }
        private string GetZone(Rebar rebar)
        {
            var hasParam = ElementUtils.HasParameter(rebar, OptionNumberingTypeRebar.Zone.ToString());
            return hasParam ? rebar.LookupParameter(OptionNumberingTypeRebar.Zone.ToString()).AsValueString() : string.Empty;
        }
        public static List<List<NumberingRevitRebar>> Numbering(List<Rebar> rebarsBase, List<OptionNumberingTypeRebar> optionNumberingTypeRebars, SchemaInfo schemaRebarNumberingInfo)
        {
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
            return results;
        }
        private static CompareRebar GetCompareRebar(OptionNumberingTypeRebar optionNumberingTypeRebar)
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
                    result = new CompareRebar(BuiltInParameter.REBAR_MODEL_BAR_DIAMETER);
                    break;
                case OptionNumberingTypeRebar.Zone:
                    result = new CompareRebar(OptionNumberingTypeRebar.Zone.ToString());
                    break;
                case OptionNumberingTypeRebar.GroupElevation:
                    result = new CompareRebar(OptionNumberingTypeRebar.GroupElevation.ToString());
                    break;
                case OptionNumberingTypeRebar.StartThread:
                    result = new CompareRebar(OptionNumberingTypeRebar.StartThread.ToString());
                    break;
                case OptionNumberingTypeRebar.EndThread:
                    result = new CompareRebar(OptionNumberingTypeRebar.EndThread.ToString());
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
        Zone = 5,
        GroupElevation = 6,
        StartThread = 7,
        EndThread = 8,
    }
}
