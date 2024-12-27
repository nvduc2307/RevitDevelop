using Autodesk.Revit.DB.Structure;
using HcBimUtils;
using Newtonsoft.Json;
using RevitDevelop.AutoCutRebarBeamV2.models;
using RevitDevelop.BeamRebar.ViewModel;
using RevitDevelop.Tools.Modifies.AutoCutRebarBeamV2.iservices;
using RevitDevelop.Utils.RevElements.RevRebars;
using RevitDevelop.Utils.Window2Ds;
using Utils.canvass;
using Utils.Entities;
using Utils.GraphicInViews;
using Utils.RevSolids;

namespace RevitDevelop.AutoCutRebarBeamV2.services
{
    public class AutoCutRebarBeamV2Service : IAutoCutRebarBeamV2Service
    {
        private ElementInstance _elementInstance;
        public AutoCutRebarBeamV2Service(ElementInstance elementInstance)
        {
            _elementInstance = elementInstance;
        }

        public List<Rebar> Cut(RebarBeamGroupInfo rebarBeamGroupInfo)
        {
            var results = new List<Rebar>();
            var vtCanvasX = new System.Windows.Point(1, 0);
            var vtCanvasY = new System.Windows.Point(0, 1);
            var typeLap = (RebarBeamCutType)_elementInstance.RebarBeamCutTypeInfoSelected.Id;

            var rebarBeamCutInfo = RebarBeamCutInfo.InitRebarBeamCutInfo(rebarBeamGroupInfo);
            var contentRebarBeamCut = JsonConvert.SerializeObject(rebarBeamCutInfo);

            foreach (var gr in rebarBeamGroupInfo.Groups)
            {
                try
                {
                    var lap_symbol_width = 50;
                    var lapType = (RebarBeamCutType)_elementInstance.RebarBeamCutTypeInfoSelected.Id;
                    var diameter = rebarBeamGroupInfo.Rebars.FirstOrDefault().GetBarDiameter().FootToMm();
                    var a = lapType == RebarBeamCutType.LapLength
                        ? _elementInstance.A
                        : _elementInstance.A;
                    XYZ vtx = null;
                    XYZ vty = null;
                    foreach (var cur in gr.RevRebarCurves)
                    {
                        try
                        {
                            vtx = cur.VTX;
                            vty = cur.VTY;
                            var rebar = cur.Rebar;
                            var rebarInfo = SchemaInfo.ReadAll(
                                _elementInstance.RebarBeamSchemal.SchemaBase,
                                _elementInstance.RebarBeamSchemal.SchemaField,
                                rebar);
                            var document = rebar.Document;
                            var curvesOrgin = cur.LinesOrigin;
                            var curveCuts = cur.RevRebarCurveCutUICanvasAfterCut;
                            var cCurveCuts = curveCuts.Count;
                            var curveBase = curvesOrgin.FirstOrDefault(x => x.Direction().IsParallel(vtx));
                            var curveLastBase = curvesOrgin.LastOrDefault(x => x.Direction().IsParallel(vtx));
                            if (curveBase == null) continue;
                            var pBase = curveBase.GetEndPoint(0);
                            var pLastBase = curveLastBase.GetEndPoint(1);
                            var rebarCuts = new List<CurveLoop>();
                            var c = 0;
                            foreach (var curveCut in curveCuts)
                            {
                                try
                                {
                                    var rebarItems = curveCut.UIElements;
                                    var rCurveLoop = new CurveLoop();
                                    foreach (var rebarItem in rebarItems)
                                    {
                                        var countRebarItems = rebarItems.Count;
                                        var indexRebarItem = rebarItems.IndexOf(rebarItem);
                                        if (cCurveCuts != 1)
                                        {
                                            if (rebarItem.Element is System.Windows.Shapes.Line l)
                                            {
                                                if (l.Direction().DotProduct(vtCanvasX).IsAlmostEqual(0))
                                                {
                                                    //is hook
                                                    if (c == cCurveCuts - 1)
                                                    {
                                                        var dir = (RebarBeamLevel)rebarItem.LevelGroup == RebarBeamLevel.Top ? -XYZ.BasisZ : XYZ.BasisZ;
                                                        var p1 = pLastBase + dir * rebarItem.LengthMm.MmToFoot();
                                                        if (indexRebarItem == 0) rCurveLoop.Append(Line.CreateBound(p1, pLastBase));
                                                        else rCurveLoop.Append(Line.CreateBound(pLastBase, p1));
                                                    }
                                                    else
                                                    {
                                                        var dir = (RebarBeamLevel)rebarItem.LevelGroup == RebarBeamLevel.Top ? -XYZ.BasisZ : XYZ.BasisZ;
                                                        var p1 = pBase + dir * rebarItem.LengthMm.MmToFoot();
                                                        if (indexRebarItem == 0) rCurveLoop.Append(Line.CreateBound(p1, pBase));
                                                        else rCurveLoop.Append(Line.CreateBound(pBase, p1));
                                                    }
                                                }
                                                else
                                                {
                                                    //is main curve
                                                    if (c == cCurveCuts - 1) rCurveLoop.Append(Line.CreateBound(pBase, pLastBase));
                                                    else
                                                    {
                                                        var p1 = pBase + vtx * rebarItem.LengthMm.MmToFoot();
                                                        rCurveLoop.Append(Line.CreateBound(pBase, p1));
                                                        pBase = typeLap == RebarBeamCutType.LapLength
                                                            ? p1 - vtx * a.MmToFoot()
                                                            : p1 + vtx * a.MmToFoot();
                                                    }
                                                }
                                            }
                                        }
                                        else
                                        {
                                            if (rebarItem.Element is System.Windows.Shapes.Line l)
                                            {
                                                if (l.Direction().DotProduct(vtCanvasX).IsAlmostEqual(0))
                                                {
                                                    //is hook
                                                    var dir = (RebarBeamLevel)rebarItem.LevelGroup == RebarBeamLevel.Top ? -XYZ.BasisZ : XYZ.BasisZ;
                                                    var p1 = indexRebarItem == 0
                                                        ? pBase + dir * rebarItem.LengthMm.MmToFoot()
                                                        : pLastBase + dir * rebarItem.LengthMm.MmToFoot();
                                                    if (indexRebarItem == 0) rCurveLoop.Append(Line.CreateBound(p1, pBase));
                                                    else rCurveLoop.Append(Line.CreateBound(pLastBase, p1));
                                                }
                                                else rCurveLoop.Append(Line.CreateBound(pBase, pLastBase));
                                            }
                                        }
                                    }
                                    if (rCurveLoop.Any())
                                    {
                                        //create rebar cut
                                        rebarCuts.Add(rCurveLoop);
                                        var rb = rebar.CreateRebarBaseOldRebar(rCurveLoop.ToList());
                                        results.Add(rb);
                                        if (!string.IsNullOrEmpty(rebarInfo.Value))
                                        {
                                            _elementInstance.RebarBeamSchemal.SchemaField.Value = rebarInfo.Value;
                                            SchemaInfo.Write(_elementInstance.RebarBeamSchemal.SchemaBase, rb, _elementInstance.RebarBeamSchemal.SchemaField);
                                        }
                                        //create lap
                                        if (cCurveCuts != 1 && typeLap != RebarBeamCutType.LapLength)
                                        {
                                            var sl = (pBase - vtx * a.MmToFoot() / 2).CreateSolid(vtx, lap_symbol_width, lap_symbol_width);
                                            var lap = sl.CreateDirectShape(document);
                                            var color = _elementInstance.RebarBeamCutTypeInfoSelected.Id == 0
                                                ? StyleColorInCanvas.Revit_Color_Weld
                                                : StyleColorInCanvas.Revit_Color_Coupler;
                                            document.SetTransparenColorElement(lap, color, 0, document.ActiveView);
                                            var lapInfo = new LapElementInfo(int.Parse(rb.Id.ToString()), _elementInstance.RebarBeamCutTypeInfoSelected.Id);
                                            _elementInstance.LapSchemal.SchemaField.Value = JsonConvert.SerializeObject(lapInfo);
                                            SchemaInfo.Write(_elementInstance.LapSchemal.SchemaBase, lap, _elementInstance.LapSchemal.SchemaField);
                                        }
                                        //rebar beam cut info
                                        _elementInstance.RebarBeamCutSchemal.SchemaField.Value = contentRebarBeamCut;
                                        SchemaInfo.Write(_elementInstance.RebarBeamCutSchemal.SchemaBase, rb, _elementInstance.RebarBeamCutSchemal.SchemaField);
                                    }
                                }
                                catch (Exception)
                                {
                                }
                                c++;
                            }
                        }
                        catch (Exception)
                        {
                        }
                    }
                }
                catch (Exception)
                {
                }
            }
            return results;
        }
    }
}
