using Autodesk.Revit.DB.Structure;
using HcBimUtils;
using RevitDevelop.AutoCutRebarBeamV2.models;
using RevitDevelop.BeamRebar.ViewModel;
using RevitDevelop.Utils.Window2Ds.Compares;
using RevitDevelop.Utils.Window2Ds;
using Utils.canvass;
using Utils.Geometries;
using Utils.RevPoints;
using RevitDevelop.Utils.RevElements.RevRebars;
using Utils.NumberUtils;

namespace RevitDevelop.Tools.Modifies.AutoCutRebarBeamV2.models
{
    public partial class RevRebarCurve : ObservableObject
    {
        public int Id { get; set; }
        public RebarBeamGroupSubInfo Parent { get; set; }
        public int Index { get; set; }
        public int LevelGroup { get; set; }
        public Rebar Rebar { get; set; }
        public double DiameterMm { get; set; }
        public double TotalLengthMm { get; set; }
        public XYZ VTX { get; set; }
        public XYZ VTY { get; set; }
        public XYZ VTZ { get; set; }
        public XYZ Origin { get; set; }
        public XYZ OriginFake { get; set; }
        public double Height { get; set; }
        public double Width { get; set; }
        public List<Curve> CurvesOrgin { get; set; } //doi voi cac thanh thep khep kin thi cac line co the khac mat phang
        public List<Curve> LinesOrigin { get; set; } //Line of rebar
        public List<Curve> CurvesGenerate { get; set; } // xoay truc z, xoay truc x, move ve goc toa do
        public Line Axis { get; set; }
        public CanvasPageBase CanvasPage { get; set; }
        public RevRebarCurveCutUICanvas RevRebarCurveCutUICanvas { get; set; }
        public List<RevRebarCurveCutUICanvas> RevRebarCurveCutUICanvasAfterCut { get; set; }
        [ObservableProperty]
        private bool _isSelected;
        public Action IsSelectedAction { get; set; }
        public Action IsSelectedActionBeforeChanged { get; set; }
        public XYZ CenterGroup { get; set; }
        public RevRebarCurve(RebarBeamGroupSubInfo parent, int index, Rebar rebar, int levelGroup, XYZ centerGr)
        {
            Parent = parent;
            Index = index;
            Rebar = rebar;
#if REVIT2021

            DiameterMm = rebar.get_Parameter(BuiltInParameter.REBAR_BAR_DIAMETER).AsDouble().FootToMm();
#else
            DiameterMm = rebar.get_Parameter(BuiltInParameter.REBAR_MODEL_BAR_DIAMETER).AsDouble().FootToMm();
#endif
            LevelGroup = levelGroup;
            CenterGroup = centerGr;
            TotalLengthMm = Math.Round(rebar.get_Parameter(BuiltInParameter.REBAR_ELEM_LENGTH).AsDouble().FootToMm(), 0);
            Id = int.Parse(rebar.Id.ToString());
            VTY = Rebar.GetNormal();
            VTX = GetVectorX();
            VTZ = GetVectorZ();
            Origin = GetOrigin(out double width, out double height);
            Width = width;
            Height = height;
            CurvesOrgin = GetCurvesOrgin();
            LinesOrigin = GetLinesOrigin(rebar);
            CurvesGenerate = GetCurvesGenerate();
            RevRebarCurveCutUICanvasAfterCut = new List<RevRebarCurveCutUICanvas>();
        }
        private List<Curve> GetCurvesGenerate(XYZ pCoordinate = null)
        {
            var rebarBeamLevel = (RebarBeamLevel)LevelGroup;
            var vtOffset = rebarBeamLevel == RebarBeamLevel.Top ? -XYZ.BasisZ : XYZ.BasisZ;
            var vtCheckY = rebarBeamLevel == RebarBeamLevel.Top ? -VTX : VTX;
            var results = new List<Curve>();
            try
            {
                var resultsTG = new List<Curve>();
                var fCustom = new FaceCustom(VTY, Origin);
                var plane = new FaceCustom(XYZ.BasisZ, Origin);
                var angle = fCustom.Normal.AngleTo(plane.Normal);
                var axisRotate = plane.FaceIntersectFace(fCustom);
                Axis = axisRotate.LineBase;

                foreach (var curve in LinesOrigin)
                {
                    try
                    {
                        if (curve is Line l)
                        {
                            var p1 = l.GetEndPoint(0).RayPointToFace(VTY, fCustom);
                            var p2 = l.GetEndPoint(1).RayPointToFace(VTY, fCustom);
                            //transform to plane
                            var p1New = p1.Rotate(fCustom, plane);
                            var p2New = p2.Rotate(fCustom, plane);
                            var lNew = Line.CreateBound(p1New, p2New);
                            resultsTG.Add(lNew);
                        }
                    }
                    catch (Exception)
                    {
                    }
                }
                //ReCalcular coordinate
                VTX = resultsTG.OrderBy(x => x.Length).LastOrDefault().Direction();
                VTZ = XYZ.BasisZ;
                VTY = VTX.CrossProduct(VTZ);
                //translate OX and translate global coordinate
                var angleX = VTX.Y > 0
                    ? -VTX.AngleTo(XYZ.BasisX)
                    : VTX.AngleTo(XYZ.BasisX);

                var curveMain = resultsTG
                .Where(x => x.Direction()
                .IsParallel(VTX)).FirstOrDefault(x =>
                {
                    var f = new FaceCustom(VTY, x.Midpoint());
                    var p = Origin.RayPointToFace(VTY, f);
                    return (x.GetEndPoint(0) - p).DotProduct((x.GetEndPoint(1) - p)) < 0;
                });

                OriginFake =
                curveMain == null
                ? Origin
                : Origin.RayPointToFace(VTY, new FaceCustom(VTY, curveMain.Midpoint()));

                var centerRotate = VTX.IsParallel(XYZ.BasisX)
                    ? OriginFake.RayPointToFace(XYZ.BasisY, new FaceCustom(XYZ.BasisY, new XYZ()))
                    : (new XYZ()).RayPointToFace(XYZ.BasisX, new FaceCustom(VTY, OriginFake));
                pCoordinate = VTX.IsParallel(XYZ.BasisX)
                    ? OriginFake.RayPointToFace(XYZ.BasisY, new FaceCustom(XYZ.BasisY, new XYZ()))
                    : OriginFake.Rotate(centerRotate, angleX);
                var vtMove = pCoordinate - OriginFake;
                foreach (var curve in resultsTG)
                {
                    try
                    {
                        if (curve is Line l)
                        {
                            //transform to plane
                            var p1 = l.GetEndPoint(0).Rotate(OriginFake, angleX) + vtMove;
                            var p2 = l.GetEndPoint(1).Rotate(OriginFake, angleX) + vtMove;

                            var lNew = Line.CreateBound(p1, p2);
                            var lAdd = lNew.CreateOffset(DiameterMm.MmToFoot() / 2, vtOffset);
                            results.Add(lAdd);
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
            var resultsCount = results.Count;
            var ps = new List<XYZ>() { results.FirstOrDefault().GetEndPoint(0) };
            if (resultsCount == 1) ps.Add(results.FirstOrDefault().GetEndPoint(1));
            else
            {
                for (int i = 0; i < resultsCount - 1; i++)
                {
                    var l1 = results[i];
                    var l2 = results[i + 1];
                    var f = new FaceCustom(l1.Direction().CrossProduct(VTZ), l1.Midpoint());
                    var p = l2.Midpoint().RayPointToFace(l2.Direction(), f);
                    ps.Add(p);
                    if (i == resultsCount - 2) ps.Add(results[i + 1].GetEndPoint(1));
                }
            }
            results = ps.PointsToCurves();
            return results;
        }
        private List<Curve> GetLinesOrigin(Rebar rb)
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
        private List<Curve> GetCurvesOrgin()
        {
            var results = new List<Curve>();
            try
            {
                results = Rebar.GetCenterlineCurves(true, false, false, MultiplanarOption.IncludeOnlyPlanarCurves, 0).ToList();
            }
            catch (Exception)
            {
            }
            return results;
        }
        private XYZ GetOrigin(out double width, out double height)
        {
            width = 0;
            height = 0;
            XYZ result = null;
            try
            {
                var ps = Rebar.GetRebarPoints();
                var psX = ps.OrderBy(x => x.DotProduct(VTX));
                var psY = ps.OrderBy(x => x.DotProduct(VTZ));

                var minX = psX.FirstOrDefault();
                var maxX = psX.LastOrDefault();
                var minY = psY.FirstOrDefault();
                var maxY = psY.LastOrDefault();
                if (minX.IsSame(maxX))
                {
                    width = 0;
                    height = minY.Distance(maxY);
                    return minY.MidPoint(maxY);
                }
                if (minY.IsSame(maxY))
                {
                    height = 0;
                    width = minX.Distance(maxX);
                    return minX.MidPoint(maxX);
                }

                var fx1 = new FaceCustom(VTX, minX);
                var fx2 = new FaceCustom(VTX, maxX);
                var fy1 = new FaceCustom(VTZ, minY);
                var fy2 = new FaceCustom(VTZ, maxY);

                var l1 = fx1.FaceIntersectFace(fy1);
                var l2 = fx1.FaceIntersectFace(fy2);
                var l3 = fx2.FaceIntersectFace(fy1);
                var l4 = fx2.FaceIntersectFace(fy2);

                height = l1.BasePoint.Distance(l2.BasePoint);
                width = l1.BasePoint.Distance(l3.BasePoint);
                result = l1.BasePoint.MidPoint(l4.BasePoint);
            }
            catch (Exception)
            {
                result = null;
            }
            return result;
        }
        private XYZ GetVectorZ()
        {
            XYZ result = null;
            try
            {
                result = VTX.CrossProduct(VTY);
            }
            catch (Exception)
            {
            }
            return result;
        }
        private XYZ GetVectorX()
        {
            XYZ result = null;
            try
            {
                result = Rebar.GetCenterlineCurves(true, true, false, MultiplanarOption.IncludeOnlyPlanarCurves, 0)
                    .Where(x => x is Line)
                    .OrderBy(x => x.Length)
                    .LastOrDefault()
                    .Direction();
            }
            catch (Exception)
            {
            }
            return result;
        }

        public List<RevRebarCurveCutUICanvas> GetRevRebarCurveCutUICanvasAfterCut(
            int typeLap,
            double rebarLengthCutMm,
            System.Windows.Point pOriginCanvas)
        {
            var results = new List<RevRebarCurveCutUICanvas>();
            try
            {
                var lLastUI = CurvesGenerate.LastOrDefault();
                var rbLength = CurvesGenerate.Sum(x => x.Length.FootToMm());
                var lLast = CurvesGenerate.LastOrDefault().Length.FootToMm();
                var curvesGenerateCount = CurvesGenerate.Count;
                var qtyCut = rbLength.DivInterger(rebarLengthCutMm, out double lDu);
                var checkLength = rbLength - lLast - (qtyCut - 2) * rebarLengthCutMm;
                var check = checkLength < rebarLengthCutMm && lLastUI.Direction().IsParallel(XYZ.BasisY);
                qtyCut = check ? qtyCut - 1 : qtyCut;
                if (qtyCut - 1 <= 0) results.Add(GetRevRebarCurveCutUICanvas(pOriginCanvas));
                //dm1
                var ps = new List<System.Windows.Point>();
                for (int i = 0; i < qtyCut - 1; i++)
                {
                    var lRMm = (i + 1) * rebarLengthCutMm;
                    var length = 0.0;
                    var c = 0;
                    foreach (var curve in CurvesGenerate)
                    {
                        var p1 = curve.GetEndPoint(0).ConvertPointRToC(OriginFake, CanvasPage, pOriginCanvas);
                        var p2 = curve.GetEndPoint(1).ConvertPointRToC(OriginFake, CanvasPage, pOriginCanvas);
                        length += Math.Round(curve.Length.FootToMm(), 0);
                        if (length > lRMm)
                        {
                            var lengthDu = lRMm + curve.Length.FootToMm() - length;
                            var p = (curve.GetEndPoint(0) + curve.Direction() * lengthDu.MmToFoot())
                                .ConvertPointRToC(OriginFake, CanvasPage, pOriginCanvas);
                            ps.Add(p1);
                            ps.Add(p);
                            if (i == qtyCut - 2)
                            {
                                ps.Add(p);
                                ps.Add(p2);
                                try
                                {
                                    for (int j = c + 1; j < curvesGenerateCount; j++)
                                    {
                                        var p11 = CurvesGenerate[j].GetEndPoint(0).ConvertPointRToC(OriginFake, CanvasPage, pOriginCanvas);
                                        var p22 = CurvesGenerate[j].GetEndPoint(1).ConvertPointRToC(OriginFake, CanvasPage, pOriginCanvas);
                                        if (j == curvesGenerateCount - 1)
                                        {
                                            ps.Add(p11);
                                            ps.Add(p22);
                                        }
                                        else ps.Add(p11);
                                    }
                                }
                                catch (Exception)
                                {
                                }
                            }
                            break;
                        }
                        else ps.Add(p1);
                        c++;
                    }
                }
                ps = ps.Distinct(new PointCompare()).ToList();
                var psCount = ps.Count;
                if (psCount > 1)
                {
                    for (int j = 0; j < psCount - 1; j++)
                    {
                        var r = new RevRebarCurveCutUICanvas(this);
                        r.Id = Id;
                        r.CanvasPage = CanvasPage;
                        r.UIElements = new List<RebarBeamUIElement>();
                        var l = ps[j].CreateLine(ps[j + 1], StyleColorInCanvas.Color_Rebar, 2);
                        r.UIElements.Add(new RebarBeamUIElement(j, r, l, CanvasPage, LevelGroup, OriginFake));
                        if (j == 0)
                        {
                            if (l.Direction().DotProduct(new System.Windows.Point(1, 0)).IsAlmostEqual(0))
                            {
                                try
                                {
                                    var l1 = ps[j + 1].CreateLine(ps[j + 2], StyleColorInCanvas.Color_Rebar, 2);
                                    r.UIElements.Add(new RebarBeamUIElement(j, r, l1, CanvasPage, LevelGroup, OriginFake));
                                    j += 1;
                                }
                                catch (Exception)
                                {
                                }
                            }
                        }
                        else if (j == psCount - 3)
                        {
                            try
                            {
                                var l2 = ps[j + 1].CreateLine(ps[j + 2], StyleColorInCanvas.Color_Rebar, 2);
                                if (l2.Direction().DotProduct(new System.Windows.Point(1, 0)).IsAlmostEqual(0))
                                {
                                    r.UIElements.Add(new RebarBeamUIElement(j, r, l2, CanvasPage, LevelGroup, OriginFake));
                                    j += 2;
                                }
                            }
                            catch (Exception)
                            {
                            }
                        }
                        results.Add(r);
                    }
                }
            }
            catch (Exception)
            {
            }
            for (int i = 0; i < results.Count; i++)
            {
                var color = i % 2 == 0 ? StyleColorInCanvas.Color_Rebar : StyleColorInCanvas.Color_Selected1;
                foreach (var item in results[i].UIElements)
                {
                    if (item.Element is System.Windows.Shapes.Line l)
                    {
                        l.Stroke = color;
                        l.MouseLeftButtonUp += L_MouseLeftButtonUp;
                    }
                }
            }
            return results;
        }
        public RevRebarCurveCutUICanvas GetRevRebarCurveCutUICanvas(System.Windows.Point pOriginCanvas)
        {
            RevRebarCurveCutUICanvas result = null;
            var options = OptionStyleInstanceInCanvas.OPTION_REBAR;
            try
            {
                var r = new RevRebarCurveCutUICanvas(this);
                r.Id = Id;
                r.CanvasPage = CanvasPage;
                r.UIElements = new List<RebarBeamUIElement>();
                var count = 0;
                foreach (var c in CurvesGenerate)
                {
                    var p1 = c.GetEndPoint(0).ConvertPointRToC(OriginFake, CanvasPage, pOriginCanvas);
                    var p2 = c.GetEndPoint(1).ConvertPointRToC(OriginFake, CanvasPage, pOriginCanvas);
                    var l = new System.Windows.Shapes.Line();
                    l.X1 = p1.X;
                    l.X2 = p2.X;
                    l.Y1 = p1.Y;
                    l.Y2 = p2.Y;
                    l.Stroke = StyleColorInCanvas.Color_Rebar;
                    l.StrokeThickness = 2;
                    r.UIElements.Add(new RebarBeamUIElement(count, r, l, CanvasPage, LevelGroup, OriginFake));
                    count++;
                }
                result = r;
            }
            catch (Exception)
            {
            }
            return result;
        }
        private void L_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            IsSelectedActionBeforeChanged?.Invoke();
            IsSelected = !IsSelected;
            IsSelectedAction?.Invoke();
        }
        public static void ResetStatus(RebarBeamGroupInfo rebarBeamGroupInfo, RevRebarCurve revRebarCurve = null)
        {
            foreach (var item in rebarBeamGroupInfo.Groups)
            {
                foreach (var item1 in item.RevRebarCurves)
                {
                    if (revRebarCurve != null)
                    {
                        if (item1.Id != revRebarCurve.Id) item1.IsSelected = false;
                    }
                    else
                    {
                        item1.IsSelected = false;
                    }
                }
            }
        }
    }
    public class RevRebarCurveCutUICanvas
    {
        public int Id { get; set; }
        public RevRebarCurve Parent { get; set; }
        public CanvasPageBase CanvasPage { get; set; }
        public List<RebarBeamUIElement> UIElements { get; set; }
        public List<double> Lengths { get; set; }
        public RevRebarCurveCutUICanvas(RevRebarCurve parent)
        {
            Parent = parent;
        }
    }
}
