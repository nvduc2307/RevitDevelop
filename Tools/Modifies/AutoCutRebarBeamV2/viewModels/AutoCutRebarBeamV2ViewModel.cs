using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.UI;
using HcBimUtils;
using HcBimUtils.MoreLinq;
using Newtonsoft.Json;
using RevitDevelop.AutoCutRebarBeamV2.middleWares;
using RevitDevelop.AutoCutRebarBeamV2.models;
using RevitDevelop.BeamRebar.ViewModel;
using RevitDevelop.Tools.Modifies.AutoCutRebarBeamV2.iservices;
using RevitDevelop.Tools.Modifies.AutoCutRebarBeamV2.models;
using RevitDevelop.Tools.Modifies.AutoCutRebarBeamV2.views;
using RevitDevelop.Utils.CanvasUtils.LapElements;
using RevitDevelop.Utils.RevElements;
using RevitDevelop.Utils.RevElements.RevRebars;
using RevitDevelop.Utils.Units;
using RevitDevelop.Utils.Window2Ds;
using System.Windows.Controls;
using Utils.canvass;
using Utils.Entities;
using Utils.FilterElements;
using Utils.RevPoints;
using Utils.SkipWarnings;

namespace RevitDevelop.AutoCutRebarBeamV2.viewModels
{
    public partial class AutoCutRebarBeamV2ViewModel : ObservableObject
    {
        private const double _canvasRatio = 800;
        private const double _canvasRatioY = 3;
        private const double _canvasRatioY1 = 1.44;
        private AutoCutRebarBeamV2Cmd _cmd;
        private IRebarBeamAssemblyAnalysis _rebarBeamAssemblyAnalysis;
        private IAutoCutRebarBeamV2Service _autoCutRebarBeamV2Service;
        public ElementInstance ElementInstance { get; set; }
        public RevElement Beam { get; set; }
        public ElementId RebarBeamAssemblyId { get; set; }
        public List<Rebar> RebarsInBeamAssembly { get; set; }
        public List<Rebar> RebarTop1 { get; set; }
        public List<Rebar> RebarTop2 { get; set; }
        public List<Rebar> RebarTop3 { get; set; }
        public List<Rebar> RebarBot1 { get; set; }
        public List<Rebar> RebarBot2 { get; set; }
        public List<Rebar> RebarBot3 { get; set; }

        public RebarBeamGroupInfo RebarBeamGroupInfoRebarTop1 { get; set; }
        public RebarBeamGroupInfo RebarBeamGroupInfoRebarTop2 { get; set; }
        public RebarBeamGroupInfo RebarBeamGroupInfoRebarTop3 { get; set; }
        public RebarBeamGroupInfo RebarBeamGroupInfoRebarBot1 { get; set; }
        public RebarBeamGroupInfo RebarBeamGroupInfoRebarBot2 { get; set; }
        public RebarBeamGroupInfo RebarBeamGroupInfoRebarBot3 { get; set; }

        public AutoCutRebarBeamV2View MainView { get; set; }
        public CanvasPageBase CanvasPage { get; set; }
        public CanvasPageBase CanvasPageSub { get; set; }
        public AutoCutRebarBeamV2ViewModel(
            AutoCutRebarBeamV2Cmd cmd,
            IRebarBeamAssemblyAnalysis rebarBeamAssemblyAnalysis,
            IAutoCutRebarBeamV2Service autoCutRebarBeamV2Service,
            ElementInstance elementInstance)
        {
            _cmd = cmd;
            _rebarBeamAssemblyAnalysis = rebarBeamAssemblyAnalysis;
            _autoCutRebarBeamV2Service = autoCutRebarBeamV2Service;
            ElementInstance = elementInstance;
            Beam = _rebarBeamAssemblyAnalysis.SelecteBeamAssembly();
            if (Beam == null) throw new Exception("Beam is Null");
            RebarBeamAssemblyId = _rebarBeamAssemblyAnalysis.GetRebarBeamAssembly(Beam);
            RebarsInBeamAssembly = _rebarBeamAssemblyAnalysis.GetRebarsInBeamAssembly(RebarBeamAssemblyId);
            RebarTop1 = _rebarBeamAssemblyAnalysis.GetRebarsTopClass1(RebarsInBeamAssembly);
            RebarTop2 = _rebarBeamAssemblyAnalysis.GetRebarsTopClass2(RebarsInBeamAssembly);
            RebarTop3 = _rebarBeamAssemblyAnalysis.GetRebarsTopClass3(RebarsInBeamAssembly);
            RebarBot1 = _rebarBeamAssemblyAnalysis.GetRebarsBotClass1(RebarsInBeamAssembly);
            RebarBot2 = _rebarBeamAssemblyAnalysis.GetRebarsBotClass2(RebarsInBeamAssembly);
            RebarBot3 = _rebarBeamAssemblyAnalysis.GetRebarsBotClass3(RebarsInBeamAssembly);

            RebarBeamGroupInfoRebarTop1 = new RebarBeamGroupInfo(
                RebarTop1,
                Beam.BoxElement.VTX,
                Beam.BoxElement.VTY,
                Beam.BoxElement.VTZ,
                (int)RebarBeamLevel.Top,
                (int)RebarBeamGroup.Level1);
            RebarBeamGroupInfoRebarTop2 = new RebarBeamGroupInfo(
                RebarTop2,
                Beam.BoxElement.VTX,
                Beam.BoxElement.VTY,
                Beam.BoxElement.VTZ,
                (int)RebarBeamLevel.Top,
                (int)RebarBeamGroup.Level2);
            RebarBeamGroupInfoRebarTop3 = new RebarBeamGroupInfo(
                RebarTop3,
                Beam.BoxElement.VTX,
                Beam.BoxElement.VTY,
                Beam.BoxElement.VTZ,
                (int)RebarBeamLevel.Top,
                (int)RebarBeamGroup.Level3);

            RebarBeamGroupInfoRebarBot1 = new RebarBeamGroupInfo(
                RebarBot1,
                Beam.BoxElement.VTX,
                Beam.BoxElement.VTY,
                Beam.BoxElement.VTZ,
                (int)RebarBeamLevel.Bottom,
                (int)RebarBeamGroup.Level1);
            RebarBeamGroupInfoRebarBot2 = new RebarBeamGroupInfo(
                RebarBot2,
                Beam.BoxElement.VTX,
                Beam.BoxElement.VTY,
                Beam.BoxElement.VTZ,
                (int)RebarBeamLevel.Bottom,
                (int)RebarBeamGroup.Level2);
            RebarBeamGroupInfoRebarBot3 = new RebarBeamGroupInfo(
                RebarBot3,
                Beam.BoxElement.VTX,
                Beam.BoxElement.VTY,
                Beam.BoxElement.VTZ,
                (int)RebarBeamLevel.Bottom,
                (int)RebarBeamGroup.Level3);

            MainView = new AutoCutRebarBeamV2View() { DataContext = this };
            MainView.Loaded += MainView_Loaded;
            ActionInit();
            _cmd.UiDocument.Selection.SetElementIds(RebarTop1.Select(x => x.Id).ToList());
            //_cmd.UiDocument.Selection.SetElementIds(Beam.ElementSubs.Select(x => x.Element.Id).ToList().ToList());
        }

        [RelayCommand]
        private void Reset()
        {
            try
            {
                var laps = _cmd.Document.GetElementsFromClass<DirectShape>()
                .Where(x =>
                {
                    var info = SchemaInfo.ReadAll(ElementInstance.LapSchemal.SchemaBase, ElementInstance.LapSchemal.SchemaField, x);
                    if (info == null) return false;
                    if (string.IsNullOrEmpty(info.Value)) return false;

                    var lap = JsonConvert.DeserializeObject<LapElementInfo>(info.Value);
                    return lap == null ? false : RebarsInBeamAssembly.Any(x => int.Parse(x.Id.ToString()) == lap.Hostid);
                })
                .ToList();
                using (var ts = new Transaction(_cmd.Document, "name transaction"))
                {
                    ts.SkipAllWarnings();
                    ts.Start();
                    //--------
                    var rbs1 = ReSetRebarGroup(RebarTop1);
                    var rbs2 = ReSetRebarGroup(RebarTop2);
                    var rbs3 = ReSetRebarGroup(RebarTop3);
                    var rbs4 = ReSetRebarGroup(RebarBot1);
                    var rbs5 = ReSetRebarGroup(RebarBot2);
                    var rbs6 = ReSetRebarGroup(RebarBot3);

                    var rbs = rbs1
                        .Concat(rbs2)
                        .Concat(rbs3)
                        .Concat(rbs4)
                        .Concat(rbs5)
                        .Concat(rbs6);
                    var ass = _cmd.Document.GetElement(RebarBeamAssemblyId) as AssemblyInstance;
                    if (rbs.Any())
                    {
                        ass.AddMemberIds(rbs.Select(x => x.Id).ToList());
                        if (RebarsInBeamAssembly.Any())
                            _cmd.Document.Delete(RebarsInBeamAssembly.Select(x => x.Id).ToList());
                        if (laps.Any())
                            _cmd.Document.Delete(laps.Select(x => x.Id).ToList());
                        MainView.Close();
                    }
                    //--------
                    ts.Commit();
                }
            }
            catch (Exception)
            {
            }
        }

        [RelayCommand]
        private void Ok()
        {
            try
            {
                GenerateValueRebarCut(ElementInstance, RebarBeamGroupInfoRebarTop1, CanvasPage);
                GenerateValueRebarCut(ElementInstance, RebarBeamGroupInfoRebarTop2, CanvasPage);
                GenerateValueRebarCut(ElementInstance, RebarBeamGroupInfoRebarTop3, CanvasPage);
                GenerateValueRebarCut(ElementInstance, RebarBeamGroupInfoRebarBot1, CanvasPage);
                GenerateValueRebarCut(ElementInstance, RebarBeamGroupInfoRebarBot2, CanvasPage);
                GenerateValueRebarCut(ElementInstance, RebarBeamGroupInfoRebarBot3, CanvasPage);

                var rebarAss = _cmd.Document.GetElement(RebarBeamAssemblyId) as AssemblyInstance;
                using (var ts = new Transaction(_cmd.Document, "name transaction"))
                {
                    ts.SkipAllWarnings();
                    ts.Start();
                    //--------
                    var rbs_top_1 = _autoCutRebarBeamV2Service.Cut(RebarBeamGroupInfoRebarTop1);
                    var rbs_top_2 = _autoCutRebarBeamV2Service.Cut(RebarBeamGroupInfoRebarTop2);
                    var rbs_top_3 = _autoCutRebarBeamV2Service.Cut(RebarBeamGroupInfoRebarTop3);
                    var rbs_bot_1 = _autoCutRebarBeamV2Service.Cut(RebarBeamGroupInfoRebarBot1);
                    var rbs_bot_2 = _autoCutRebarBeamV2Service.Cut(RebarBeamGroupInfoRebarBot2);
                    var rbs_bot_3 = _autoCutRebarBeamV2Service.Cut(RebarBeamGroupInfoRebarBot3);

                    if (rbs_top_1.Any()) rebarAss.AddMemberIds(rbs_top_1.Select(x => x.Id).ToList());
                    if (rbs_top_2.Any()) rebarAss.AddMemberIds(rbs_top_2.Select(x => x.Id).ToList());
                    if (rbs_top_3.Any()) rebarAss.AddMemberIds(rbs_top_3.Select(x => x.Id).ToList());
                    if (rbs_bot_1.Any()) rebarAss.AddMemberIds(rbs_bot_1.Select(x => x.Id).ToList());
                    if (rbs_bot_2.Any()) rebarAss.AddMemberIds(rbs_bot_2.Select(x => x.Id).ToList());
                    if (rbs_bot_3.Any()) rebarAss.AddMemberIds(rbs_bot_3.Select(x => x.Id).ToList());

                    _cmd.Document.Delete(RebarTop1.Select(x => x.Id).ToList());
                    _cmd.Document.Delete(RebarTop2.Select(x => x.Id).ToList());
                    _cmd.Document.Delete(RebarTop3.Select(x => x.Id).ToList());
                    _cmd.Document.Delete(RebarBot1.Select(x => x.Id).ToList());
                    _cmd.Document.Delete(RebarBot2.Select(x => x.Id).ToList());
                    _cmd.Document.Delete(RebarBot3.Select(x => x.Id).ToList());

                    //--------
                    ts.Commit();
                }
                MainView.Close();
            }
            catch (Exception)
            {
            }
        }

        [RelayCommand]
        private void Cancel()
        {
            MainView.Close();
        }

        [RelayCommand]
        private void AddCut()
        {
            AddCutAction(1);
        }
        [RelayCommand]
        private void RemoveCut()
        {
            AddCutAction(-1);
        }

        private List<Rebar> ReSetRebarGroup(List<Rebar> rebars)
        {
            var results = new List<Rebar>();
            try
            {
                var rebar = rebars.FirstOrDefault();
                var rebarBeamInfo = rebars
                    .Select(x => SchemaInfo.ReadAll(ElementInstance.RebarBeamSchemal.SchemaBase, ElementInstance.RebarBeamSchemal.SchemaField, x))
                    .FirstOrDefault(x => !string.IsNullOrEmpty(x.Value));
                var rebarBeamCutInfo = rebars
                    .Select(x => SchemaInfo.ReadAll(ElementInstance.RebarBeamCutSchemal.SchemaBase, ElementInstance.RebarBeamCutSchemal.SchemaField, x))
                    .FirstOrDefault(x => !string.IsNullOrEmpty(x.Value));
                if (rebarBeamInfo == null) throw new Exception();
                if (rebarBeamCutInfo == null) throw new Exception();

                var rebarCutInfo = JsonConvert.DeserializeObject<RebarBeamCutInfo>(rebarBeamCutInfo.Value);
                if (rebarCutInfo == null) throw new Exception();

                foreach (var shape in rebarCutInfo.ShapesOrigin)
                {
                    var plg = shape.Shape.Select(x => new XYZ(x.X, x.Y, x.Z)).ToList().PointsToCurves();
                    var rb = rebar.CreateRebarBaseOldRebar(plg);
                    if (rb != null)
                    {
                        results.Add(rb);
                        ElementInstance.RebarBeamSchemal.SchemaField.Value = rebarBeamInfo.Value;
                        SchemaInfo.Write(ElementInstance.RebarBeamSchemal.SchemaBase, rb, ElementInstance.RebarBeamSchemal.SchemaField);
                    }
                }
            }
            catch (Exception)
            {
            }
            return results;
        }

        private void AddCutAction(int numAdd)
        {
            try
            {
                RebarBeamGroupInfo rebarBeamGroupInfoTarget = null;
                var groupCutTarget = new List<RebarBeamGroupSubInfo>();
                switch (ElementInstance.RebarBeamLevelInfoSelected.Id)
                {
                    case 0:
                        switch (ElementInstance.RebarBeamGroupInfoSelected.Id)
                        {
                            case 0:
                                rebarBeamGroupInfoTarget = RebarBeamGroupInfoRebarTop1;
                                groupCutTarget = RebarBeamGroupInfoRebarTop1.GroupCuts
                                    .FirstOrDefault(x => x.Any(y => y.RevRebarCurves.Any(z => z.IsSelected)));
                                break;
                            case 1:
                                rebarBeamGroupInfoTarget = RebarBeamGroupInfoRebarTop2;
                                groupCutTarget = RebarBeamGroupInfoRebarTop2.GroupCuts
                                    .FirstOrDefault(x => x.Any(y => y.RevRebarCurves.Any(z => z.IsSelected)));
                                break;
                            case 2:
                                rebarBeamGroupInfoTarget = RebarBeamGroupInfoRebarTop3;
                                groupCutTarget = RebarBeamGroupInfoRebarTop3.GroupCuts
                                    .FirstOrDefault(x => x.Any(y => y.RevRebarCurves.Any(z => z.IsSelected)));
                                break;
                        }
                        break;
                    case 1:
                        switch (ElementInstance.RebarBeamGroupInfoSelected.Id)
                        {
                            case 0:
                                rebarBeamGroupInfoTarget = RebarBeamGroupInfoRebarBot1;
                                groupCutTarget = RebarBeamGroupInfoRebarBot1.GroupCuts
                                    .FirstOrDefault(x => x.Any(y => y.RevRebarCurves.Any(z => z.IsSelected)));
                                break;
                            case 1:
                                rebarBeamGroupInfoTarget = RebarBeamGroupInfoRebarBot2;
                                groupCutTarget = RebarBeamGroupInfoRebarBot2.GroupCuts
                                    .FirstOrDefault(x => x.Any(y => y.RevRebarCurves.Any(z => z.IsSelected)));
                                break;
                            case 2:
                                rebarBeamGroupInfoTarget = RebarBeamGroupInfoRebarBot3;
                                groupCutTarget = RebarBeamGroupInfoRebarBot3.GroupCuts
                                    .FirstOrDefault(x => x.Any(y => y.RevRebarCurves.Any(z => z.IsSelected)));
                                break;
                        }
                        break;
                }
                if (groupCutTarget == null) throw new Exception();
                var target = groupCutTarget
                    .FirstOrDefault()
                    .RevRebarCurves
                    .FirstOrDefault(x => x.IsSelected);
                if (target == null) throw new Exception();
                var indexOfTarget = groupCutTarget
                    .FirstOrDefault()
                    .RevRebarCurves.IndexOf(target);

                foreach (var gr in groupCutTarget)
                {
                    var cur = gr.RevRebarCurves[indexOfTarget];
                    var lenthTotal = cur.TotalLengthMm;
                    var targetCount = cur.RevRebarCurveCutUICanvasAfterCut.Count;
                    var cCut = targetCount + numAdd == 0 ? 1 : targetCount + numAdd;
                    var lCut = lenthTotal / cCut;
                    if (lCut < ElementInstance.REBAR_LENGTH_MIN && cCut != 1) throw new Exception();
                    cur.RevRebarCurveCutUICanvasAfterCut = cCut >= 1
                    ? cur.GetRevRebarCurveCutUICanvasAfterCut(
                        0,
                        lCut,
                        cur.OriginFake.ConvertPointRToC(rebarBeamGroupInfoTarget.CenterGroup, CanvasPage))
                    : new List<RevRebarCurveCutUICanvas>() { cur.RevRebarCurveCutUICanvas };
                }
                DrawRebarInCanvas(ElementInstance, rebarBeamGroupInfoTarget, CanvasPage);
            }
            catch (Exception)
            {
            }
        }

        private void MainView_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            var canvas = MainView.FindName("MainCanvas") as Canvas;
            var canvasSub = MainView.FindName("SubCanvas") as Canvas;
            CanvasPage = new CanvasPageBase(canvas);
            CanvasPageSub = new CanvasPageBase(canvasSub);
            this.ElementInstance.RebarGroupCuts = ElementInstance.GetRebarGroupCuts(RebarBeamGroupInfoRebarTop1.GroupCuts.Count);
            this.ElementInstance.RebarGroupCutSelected = this.ElementInstance.RebarGroupCuts.FirstOrDefault();

            RevRebarCurveCutUICanvasInitData(ElementInstance, RebarBeamGroupInfoRebarTop1, CanvasPage);
            RevRebarCurveCutUICanvasInitData(ElementInstance, RebarBeamGroupInfoRebarTop2, CanvasPage);
            RevRebarCurveCutUICanvasInitData(ElementInstance, RebarBeamGroupInfoRebarTop3, CanvasPage);
            RevRebarCurveCutUICanvasInitData(ElementInstance, RebarBeamGroupInfoRebarBot1, CanvasPage);
            RevRebarCurveCutUICanvasInitData(ElementInstance, RebarBeamGroupInfoRebarBot2, CanvasPage);
            RevRebarCurveCutUICanvasInitData(ElementInstance, RebarBeamGroupInfoRebarBot3, CanvasPage);

            DrawRebarInCanvas(ElementInstance, RebarBeamGroupInfoRebarTop1, CanvasPage);
        }

        private void Item1_EventElementValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (sender is RebarBeamUIElement curveTarget)
                {
                    //check value has text
                    curveTarget.CheckValueHasText(Item1_EventElementValueChanged);
                    curveTarget.CheckValueLimit(Item1_EventElementValueChanged);

                    var l = curveTarget.Element as System.Windows.Shapes.Line;
                    var parentCurveTarget = curveTarget.Parent; //[các curve của thanh thép sau khi cắt]
                    var rebarTarget = parentCurveTarget.Parent; //[các curve của thanh thép trước khi cắt]
                    var groupTarget = rebarTarget.Parent; //[group thép đang thao tác]

                    var lineTarget = curveTarget.Element as System.Windows.Shapes.Line;
                    var dirTarget = lineTarget.Direction();

                    var countCurveTargets = parentCurveTarget.UIElements.Count; //[số lượng đoạn thẳng trong thanh thép sau khi thực hiện cắt]
                    var countParentCurveTarget = rebarTarget.RevRebarCurveCutUICanvasAfterCut.Count; //[số lượng thanh thép sau khi thực hiện cắt]

                    var indexCurveTarget = parentCurveTarget.UIElements.IndexOf(curveTarget); //[chỉ số của curve đang thao tác - thanh thép sau khi cắt]
                    var indexParentCurveTarget = rebarTarget.RevRebarCurveCutUICanvasAfterCut.IndexOf(parentCurveTarget); //[chỉ số của thanh thép đang tao tác - thanh thép sau khi cắt]
                    var indexOfParentCurveNext = indexParentCurveTarget == countParentCurveTarget - 1
                        ? indexParentCurveTarget - 1
                    : indexParentCurveTarget + 1;

                    var parentCurveNext = indexOfParentCurveNext == -1
                        ? null :
                        rebarTarget.RevRebarCurveCutUICanvasAfterCut[indexOfParentCurveNext];

                    var rebarGroupLevel = (RebarBeamLevel)rebarTarget.LevelGroup == RebarBeamLevel.Top ? 1 : -1;
                    var canvasPage = rebarTarget.CanvasPage;

                    if (countParentCurveTarget == 1)
                    {
                        if (!l.Direction().DotProduct(new System.Windows.Point(1, 0)).IsAlmostEqual(0))
                        {
                            curveTarget.EventElementValueChanged -= Item1_EventElementValueChanged;
                            curveTarget.ElementValue.Text = curveTarget.LengthMm.ToString();
                            curveTarget.EventElementValueChanged += Item1_EventElementValueChanged;
                        }
                        else
                        {
                            var lCurrent =
                                    (double.Parse(curveTarget.ElementValue.Text).MmToPixel() * canvasPage.ScaleY / _canvasRatio) / _canvasRatioY1;
                            if (indexCurveTarget == 0)
                            {
                                //hook start
                                lineTarget.X1 = lineTarget.X2;
                                lineTarget.Y1 = lineTarget.Y2 + lCurrent * rebarGroupLevel;
                            }
                            else
                            {
                                //hook end
                                lineTarget.X2 = lineTarget.X1;
                                lineTarget.Y2 = lineTarget.Y1 + lCurrent * rebarGroupLevel;
                            }
                            RebarBeamUIElement.AlignElementValue(curveTarget);
                        }
                    }
                    else
                    {
                        if (indexParentCurveTarget == 0)
                        {
                            //Thanh dau tien
                            if (indexCurveTarget == 0)
                            {
                                // curve dau tien
                                if (dirTarget.DotProduct(new System.Windows.Point(1, 0)).IsAlmostEqual(0))
                                {
                                    var lCurrent =
                                    (double.Parse(curveTarget.ElementValue.Text).MmToPixel() * canvasPage.ScaleY / _canvasRatio) / _canvasRatioY1;
                                    lineTarget.X1 = lineTarget.X2;
                                    lineTarget.Y1 = lineTarget.Y2 + lCurrent * rebarGroupLevel;
                                }
                                else
                                {
                                    if (countParentCurveTarget > 1)
                                    {
                                        var curveNext = parentCurveNext.UIElements[0].Element as System.Windows.Shapes.Line;
                                        var lCurrent =
                                        (double.Parse(curveTarget.ElementValue.Text).MmToPixel() * canvasPage.ScaleX / _canvasRatio) / _canvasRatioY1;
                                        lineTarget.X2 = lineTarget.X1 + lCurrent;
                                        lineTarget.Y2 = lineTarget.Y1;
                                        curveNext.X1 = lineTarget.X2;
                                        curveNext.Y1 = lineTarget.Y2;
                                        var lcurveNext = Math.Round(curveNext.Distance().PixelToMm() * _canvasRatio * _canvasRatioY1 / canvasPage.ScaleX, 0);
                                        parentCurveNext.UIElements[0].EventElementValueChanged -= Item1_EventElementValueChanged;
                                        parentCurveNext.UIElements[0].ElementValue.Text = lcurveNext.ToString();
                                        parentCurveNext.UIElements[0].EventElementValueChanged += Item1_EventElementValueChanged;
                                        RebarBeamUIElement.AlignElementValue(parentCurveNext.UIElements[0]);
                                    }
                                }
                            }
                            else if (indexCurveTarget == countCurveTargets - 1)
                            {
                                // curve cuoi cung
                                if (dirTarget.DotProduct(new System.Windows.Point(1, 0)).IsAlmostEqual(0))
                                {
                                    var lCurrent =
                                    (double.Parse(curveTarget.ElementValue.Text).MmToPixel() * canvasPage.ScaleY / _canvasRatio) / _canvasRatioY1;
                                    lineTarget.X2 = lineTarget.X1;
                                    lineTarget.Y2 = lineTarget.Y1 + lCurrent * rebarGroupLevel;
                                }
                                else
                                {
                                    if (countParentCurveTarget > 1)
                                    {
                                        var curveNext = parentCurveNext.UIElements[parentCurveNext.UIElements.Count - 1].Element as System.Windows.Shapes.Line;
                                        var lCurrent =
                                        (double.Parse(curveTarget.ElementValue.Text).MmToPixel() * canvasPage.ScaleX / _canvasRatio) / _canvasRatioY1;
                                        lineTarget.X2 = lineTarget.X1 + lCurrent;
                                        lineTarget.Y2 = lineTarget.Y1;
                                        curveNext.X1 = lineTarget.X2;
                                        curveNext.Y1 = lineTarget.Y2;
                                        var lcurveNext = Math.Round(curveNext.Distance().PixelToMm() * _canvasRatio * _canvasRatioY1 / canvasPage.ScaleX, 0);
                                        parentCurveNext.UIElements[parentCurveNext.UIElements.Count - 1].EventElementValueChanged -= Item1_EventElementValueChanged;
                                        parentCurveNext.UIElements[parentCurveNext.UIElements.Count - 1].ElementValue.Text = lcurveNext.ToString();
                                        parentCurveNext.UIElements[parentCurveNext.UIElements.Count - 1].EventElementValueChanged += Item1_EventElementValueChanged;
                                        RebarBeamUIElement.AlignElementValue(parentCurveNext.UIElements[parentCurveNext.UIElements.Count - 1]);
                                    }
                                }
                            }
                            else
                            {
                                // curve o giua
                            }
                        }
                        else if (indexParentCurveTarget == countParentCurveTarget - 1)
                        {
                            //Thanh cuoi cung
                            if (indexCurveTarget == 0)
                            {
                                // curve dau tien
                                if (countParentCurveTarget > 1)
                                {
                                    var curveNext = parentCurveNext.UIElements[parentCurveNext.UIElements.Count - 1].Element as System.Windows.Shapes.Line;
                                    var lCurrent =
                                    (double.Parse(curveTarget.ElementValue.Text).MmToPixel() * canvasPage.ScaleX / _canvasRatio) / _canvasRatioY1;
                                    lineTarget.X1 = lineTarget.X2 - lCurrent;
                                    lineTarget.Y1 = lineTarget.Y2;
                                    curveNext.X2 = lineTarget.X1;
                                    curveNext.Y2 = lineTarget.Y1;
                                    var lcurveNext = Math.Round(curveNext.Distance().PixelToMm() * _canvasRatio * _canvasRatioY1 / canvasPage.ScaleX, 0);
                                    parentCurveNext.UIElements[parentCurveNext.UIElements.Count - 1].EventElementValueChanged -= Item1_EventElementValueChanged;
                                    parentCurveNext.UIElements[parentCurveNext.UIElements.Count - 1].ElementValue.Text = lcurveNext.ToString();
                                    parentCurveNext.UIElements[parentCurveNext.UIElements.Count - 1].EventElementValueChanged += Item1_EventElementValueChanged;
                                    RebarBeamUIElement.AlignElementValue(parentCurveNext.UIElements[parentCurveNext.UIElements.Count - 1]);
                                }
                            }
                            else if (indexCurveTarget == countCurveTargets - 1)
                            {
                                // curve cuoi cung
                                if (dirTarget.DotProduct(new System.Windows.Point(1, 0)).IsAlmostEqual(0))
                                {
                                    var lCurrent =
                                    (double.Parse(curveTarget.ElementValue.Text).MmToPixel() * canvasPage.ScaleY / _canvasRatio) / _canvasRatioY1;
                                    lineTarget.X2 = lineTarget.X1;
                                    lineTarget.Y2 = lineTarget.Y1 + lCurrent * rebarGroupLevel;
                                }
                                else
                                {
                                    if (countParentCurveTarget > 1)
                                    {
                                        var curveNext = parentCurveNext.UIElements[parentCurveNext.UIElements.Count - 1].Element as System.Windows.Shapes.Line;
                                        var lCurrent =
                                        (double.Parse(curveTarget.ElementValue.Text).MmToPixel() * canvasPage.ScaleX / _canvasRatio) / _canvasRatioY1;
                                        lineTarget.X1 = lineTarget.X2 - lCurrent;
                                        lineTarget.Y1 = lineTarget.Y2;
                                        curveNext.X2 = lineTarget.X1;
                                        curveNext.Y2 = lineTarget.Y1;
                                        var lcurveNext = Math.Round(curveNext.Distance().PixelToMm() * _canvasRatio * _canvasRatioY1 / canvasPage.ScaleX, 0);
                                        parentCurveNext.UIElements[parentCurveNext.UIElements.Count - 1].EventElementValueChanged -= Item1_EventElementValueChanged;
                                        parentCurveNext.UIElements[parentCurveNext.UIElements.Count - 1].ElementValue.Text = lcurveNext.ToString();
                                        parentCurveNext.UIElements[parentCurveNext.UIElements.Count - 1].EventElementValueChanged += Item1_EventElementValueChanged;
                                        RebarBeamUIElement.AlignElementValue(parentCurveNext.UIElements[parentCurveNext.UIElements.Count - 1]);
                                    }
                                }
                            }
                            else
                            {
                                // curve o giua
                            }
                        }
                        else
                        {
                            //Thanh o giua
                            if (indexCurveTarget == 0)
                            {
                                // curve dau tien
                                if (countParentCurveTarget > 1)
                                {
                                    var curveNext = parentCurveNext.UIElements[0].Element as System.Windows.Shapes.Line;
                                    var lCurrent =
                                    (double.Parse(curveTarget.ElementValue.Text).MmToPixel() * canvasPage.ScaleX / _canvasRatio) / _canvasRatioY1;
                                    lineTarget.X2 = lineTarget.X1 + lCurrent;
                                    lineTarget.Y2 = lineTarget.Y1;
                                    curveNext.X1 = lineTarget.X2;
                                    curveNext.Y1 = lineTarget.Y2;
                                    var lcurveNext = Math.Round(curveNext.Distance().PixelToMm() * _canvasRatio * _canvasRatioY1 / canvasPage.ScaleX, 0);
                                    parentCurveNext.UIElements[0].EventElementValueChanged -= Item1_EventElementValueChanged;
                                    parentCurveNext.UIElements[0].ElementValue.Text = lcurveNext.ToString();
                                    parentCurveNext.UIElements[0].EventElementValueChanged += Item1_EventElementValueChanged;
                                    RebarBeamUIElement.AlignElementValue(parentCurveNext.UIElements[0]);
                                }
                            }
                            else if (indexCurveTarget == countCurveTargets - 1)
                            {
                                // curve cuoi cung
                            }
                            else
                            {
                                // curve o giua
                            }
                        }
                        RebarBeamUIElement.AlignElementValue(curveTarget);
                    }
                    var rebarBeamGroupInfoTarget = GetRebarBeamGroupInfoTarget(ElementInstance.RebarBeamLevelInfoSelected.Id, ElementInstance.RebarBeamGroupInfoSelected.Id);
                    GenerateValueRebarCut(ElementInstance, rebarBeamGroupInfoTarget, CanvasPage);
                    DrawRebarOnSubCanvan(ElementInstance, rebarBeamGroupInfoTarget, CanvasPageSub);
                }
            }
            catch (Exception)
            {
            }
        }

        private RebarBeamGroupInfo GetRebarBeamGroupInfoTarget(int rebarBeamLevelInfoSelected, int rebarBeamGroupInfoSelected)
        {
            RebarBeamGroupInfo result = null;
            try
            {
                switch (rebarBeamLevelInfoSelected)
                {
                    case 0:
                        switch (rebarBeamGroupInfoSelected)
                        {
                            case 0:
                                result = RebarBeamGroupInfoRebarTop1;
                                break;
                            case 1:
                                result = RebarBeamGroupInfoRebarTop2;
                                break;
                            case 2:
                                result = RebarBeamGroupInfoRebarTop3;
                                break;
                        }
                        break;
                    case 1:
                        switch (rebarBeamGroupInfoSelected)
                        {
                            case 0:
                                result = RebarBeamGroupInfoRebarBot1;
                                break;
                            case 1:
                                result = RebarBeamGroupInfoRebarBot2;
                                break;
                            case 2:
                                result = RebarBeamGroupInfoRebarBot3;
                                break;
                        }
                        break;
                }
            }
            catch (Exception)
            {
            }
            return result;
        }

        private void ActionInit()
        {
            //BChanged
            ElementInstance.BChanged = () =>
            {
                var rebarBeamGroupInfoTarget = GetRebarBeamGroupInfoTarget(ElementInstance.RebarBeamLevelInfoSelected.Id, ElementInstance.RebarBeamGroupInfoSelected.Id);
                DrawRebarInCanvas(ElementInstance, rebarBeamGroupInfoTarget, CanvasPage);
            };

            //RebarLengthCutChanged
            ElementInstance.RebarLengthCutChanged = () =>
            {
                RevRebarCurveCutUICanvasInitData(ElementInstance, RebarBeamGroupInfoRebarTop1, CanvasPage);
                RevRebarCurveCutUICanvasInitData(ElementInstance, RebarBeamGroupInfoRebarTop2, CanvasPage);
                RevRebarCurveCutUICanvasInitData(ElementInstance, RebarBeamGroupInfoRebarTop3, CanvasPage);
                RevRebarCurveCutUICanvasInitData(ElementInstance, RebarBeamGroupInfoRebarBot1, CanvasPage);
                RevRebarCurveCutUICanvasInitData(ElementInstance, RebarBeamGroupInfoRebarBot2, CanvasPage);
                RevRebarCurveCutUICanvasInitData(ElementInstance, RebarBeamGroupInfoRebarBot3, CanvasPage);

                //Draw
                var rebarBeamGroupInfoTarget = GetRebarBeamGroupInfoTarget(ElementInstance.RebarBeamLevelInfoSelected.Id, ElementInstance.RebarBeamGroupInfoSelected.Id);
                DrawRebarInCanvas(ElementInstance, rebarBeamGroupInfoTarget, CanvasPage);
            };
            //RebarBeamCutTypeInfoSelectedChange
            ElementInstance.RebarBeamCutTypeInfoSelectedChange = () =>
            {
                ElementInstance.InitDefaultAAndB(ElementInstance.RebarBeamCutTypeInfoSelected, out double a, out double b, out string aTitle);
                ElementInstance.A = a;
                ElementInstance.B = b;
                ElementInstance.ATitle = aTitle;
            };

            //RebarBeamLevelInfoSelectedChange
            ElementInstance.RebarBeamLevelInfoSelectedChange = () =>
            {
                RevRebarCurve.ResetStatus(RebarBeamGroupInfoRebarTop1);
                RevRebarCurve.ResetStatus(RebarBeamGroupInfoRebarTop2);
                RevRebarCurve.ResetStatus(RebarBeamGroupInfoRebarTop3);
                RevRebarCurve.ResetStatus(RebarBeamGroupInfoRebarBot1);
                RevRebarCurve.ResetStatus(RebarBeamGroupInfoRebarBot2);
                RevRebarCurve.ResetStatus(RebarBeamGroupInfoRebarBot3);

                ElementInstance.RebarBeamGroupInfoSelected = ElementInstance.RebarBeamGroupInfos.FirstOrDefault();
                switch (ElementInstance.RebarBeamLevelInfoSelected.Id)
                {
                    case 0:
                        _cmd.UiDocument.Selection.SetElementIds(RebarTop1.Select(x => x.Id).ToList());
                        ElementInstance.RebarGroupCuts = ElementInstance.GetRebarGroupCuts(RebarBeamGroupInfoRebarTop1.GroupCuts.Count);
                        break;
                    case 1:
                        _cmd.UiDocument.Selection.SetElementIds(RebarBot1.Select(x => x.Id).ToList());
                        ElementInstance.RebarGroupCuts = ElementInstance.GetRebarGroupCuts(RebarBeamGroupInfoRebarBot1.GroupCuts.Count);
                        break;
                }
                ElementInstance.RebarGroupCutSelected = ElementInstance.RebarGroupCuts.FirstOrDefault();
                ElementInstance.IsSelectedRebarGroup = false;
            };

            //RebarBeamGroupInfoSelectedChange
            ElementInstance.RebarBeamGroupInfoSelectedChange = () =>
            {
                RevRebarCurve.ResetStatus(RebarBeamGroupInfoRebarTop1);
                RevRebarCurve.ResetStatus(RebarBeamGroupInfoRebarTop2);
                RevRebarCurve.ResetStatus(RebarBeamGroupInfoRebarTop3);
                RevRebarCurve.ResetStatus(RebarBeamGroupInfoRebarBot1);
                RevRebarCurve.ResetStatus(RebarBeamGroupInfoRebarBot2);
                RevRebarCurve.ResetStatus(RebarBeamGroupInfoRebarBot3);

                var rebarBeamGroupInfoTarget = GetRebarBeamGroupInfoTarget(ElementInstance.RebarBeamLevelInfoSelected.Id, ElementInstance.RebarBeamGroupInfoSelected.Id);
                _cmd.UiDocument.Selection.SetElementIds(rebarBeamGroupInfoTarget.Rebars.Select(x => x.Id).ToList());
                ElementInstance.RebarGroupCuts = ElementInstance.GetRebarGroupCuts(rebarBeamGroupInfoTarget.GroupCuts.Count);
                ElementInstance.RebarGroupCutSelected = ElementInstance.RebarGroupCuts.FirstOrDefault();
                ElementInstance.IsSelectedRebarGroup = false;
            };

            //RebarGroupCutSelectedChanged
            ElementInstance.RebarGroupCutSelectedChanged = () =>
            {
                var rebarBeamGroupInfoTarget = GetRebarBeamGroupInfoTarget(ElementInstance.RebarBeamLevelInfoSelected.Id, ElementInstance.RebarBeamGroupInfoSelected.Id);
                DrawRebarInCanvas(ElementInstance, rebarBeamGroupInfoTarget, CanvasPage);
                ElementInstance.IsSelectedRebarGroup = false;
            };

        }

        private void IsSelectedActionInit(
            RevRebarCurve cur,
            UIDocument uidoc,
            ElementInstance instance,
            RebarBeamGroupInfo rebarBeamGroupInfo,
            CanvasPageBase canvasPage)
        {
            if (cur.IsSelected)
            {
                var rebarSelected = rebarBeamGroupInfo.GroupCuts[instance.RebarGroupCutSelected - 1]
                .Select(x => x.Rebars[cur.Index].Id)
                .ToList();
                _cmd.UiDocument.Selection.SetElementIds(rebarSelected);
            }
            else _cmd.UiDocument.Selection.SetElementIds(new List<ElementId>());

            instance.IsSelectedRebarGroup = rebarBeamGroupInfo
                .GroupCuts[instance.RebarGroupCutSelected - 1]
                .FirstOrDefault()
                .RevRebarCurves
                .Any(x => x.IsSelected);
        }

        private void GenerateValueRebarCut(
            ElementInstance instance,
            RebarBeamGroupInfo rebarBeamGroupInfo,
            CanvasPageBase canvasPage)
        {
            try
            {
                var diameter = rebarBeamGroupInfo.Rebars.FirstOrDefault().GetBarDiameter().FootToMm();
                var b = instance.B;
                var dLapToLap = (b.MmToPixel() * canvasPage.ScaleY / _canvasRatio) / _canvasRatioY1;
                foreach (var gr in rebarBeamGroupInfo.GroupCuts)
                {
                    var c = 0;
                    var f = gr.FirstOrDefault();
                    if (f == null) continue;
                    foreach (var item in gr)
                    {
                        try
                        {
                            if (c == 0) throw new Exception();
                            foreach (var item1 in item.RevRebarCurves)
                            {
                                var cRevRebarCurveCutUICanvasAfterCut = item1.RevRebarCurveCutUICanvasAfterCut.Count;
                                var cAfterCut = 0;
                                foreach (var item2 in item1.RevRebarCurveCutUICanvasAfterCut)
                                {
                                    var cUIElements = item2.UIElements.Count;
                                    var cUi = 0;
                                    foreach (var item3 in item2.UIElements)
                                    {
                                        try
                                        {
                                            var revRebarCurvesFTarget = f.RevRebarCurves[item.RevRebarCurves.IndexOf(item1)];
                                            var revRebarCurveCutUICanvasAfterCutFTarget =
                                                revRebarCurvesFTarget.RevRebarCurveCutUICanvasAfterCut[item1.RevRebarCurveCutUICanvasAfterCut.IndexOf(item2)];
                                            var uIElementsFTarget = revRebarCurveCutUICanvasAfterCutFTarget.UIElements[item2.UIElements.IndexOf(item3)];
                                            var lFTarget = uIElementsFTarget.Element as System.Windows.Shapes.Line;
                                            var lTarget = item3.Element as System.Windows.Shapes.Line;

                                            if (cRevRebarCurveCutUICanvasAfterCut == 1)
                                            {
                                                lTarget.X1 = lFTarget.X1;
                                                lTarget.Y1 = lFTarget.Y1;
                                                lTarget.X2 = lFTarget.X2;
                                                lTarget.Y2 = lFTarget.Y2;
                                                item3.LengthMm = uIElementsFTarget.LengthMm;
                                            }
                                            else
                                            {
                                                if (c % 2 == 0)
                                                {
                                                    lTarget.X1 = lFTarget.X1;
                                                    lTarget.Y1 = lFTarget.Y1;
                                                    lTarget.X2 = lFTarget.X2;
                                                    lTarget.Y2 = lFTarget.Y2;
                                                    item3.LengthMm = uIElementsFTarget.LengthMm;
                                                }
                                                else
                                                {
                                                    if (cAfterCut == 0)
                                                    {
                                                        if (lTarget.Direction().DotProduct(new System.Windows.Point(0, 1)).IsAlmostEqual(0))
                                                        {
                                                            lTarget.X1 = lFTarget.X1;
                                                            lTarget.Y1 = lFTarget.Y1;
                                                            lTarget.X2 = lFTarget.X2 + dLapToLap;
                                                            lTarget.Y2 = lFTarget.Y2;
                                                            item3.LengthMm = uIElementsFTarget.LengthMm + b;
                                                        }
                                                        else
                                                        {
                                                            lTarget.X1 = lFTarget.X1;
                                                            lTarget.Y1 = lFTarget.Y1;
                                                            lTarget.X2 = lFTarget.X2;
                                                            lTarget.Y2 = lFTarget.Y2;
                                                            item3.LengthMm = uIElementsFTarget.LengthMm;
                                                        }
                                                    }
                                                    else if (cAfterCut == cRevRebarCurveCutUICanvasAfterCut - 1)
                                                    {
                                                        if (lTarget.Direction().DotProduct(new System.Windows.Point(0, 1)).IsAlmostEqual(0))
                                                        {
                                                            lTarget.X1 = lFTarget.X1 + dLapToLap;
                                                            lTarget.Y1 = lFTarget.Y1;
                                                            lTarget.X2 = lFTarget.X2;
                                                            lTarget.Y2 = lFTarget.Y2;
                                                            item3.LengthMm = uIElementsFTarget.LengthMm - b;
                                                        }
                                                        else
                                                        {
                                                            lTarget.X1 = lFTarget.X1;
                                                            lTarget.Y1 = lFTarget.Y1;
                                                            lTarget.X2 = lFTarget.X2;
                                                            lTarget.Y2 = lFTarget.Y2;
                                                            item3.LengthMm = uIElementsFTarget.LengthMm;
                                                        }
                                                    }
                                                    else
                                                    {
                                                        if (lTarget.Direction().DotProduct(new System.Windows.Point(0, 1)).IsAlmostEqual(0))
                                                        {
                                                            lTarget.X1 = lFTarget.X1 + dLapToLap;
                                                            lTarget.Y1 = lFTarget.Y1;
                                                            lTarget.X2 = lFTarget.X2 + dLapToLap;
                                                            lTarget.Y2 = lFTarget.Y2;
                                                            item3.LengthMm = uIElementsFTarget.LengthMm;
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                        catch (Exception)
                                        {
                                        }
                                        cUi++;
                                    }
                                    cAfterCut++;
                                }
                            }
                        }
                        catch (Exception)
                        {
                        }
                        c++;
                    }
                }
            }
            catch (Exception)
            {
            }
        }

        private void DrawRebarOnSubCanvan(
            ElementInstance instance,
            RebarBeamGroupInfo rebarBeamGroupInfo,
            CanvasPageBase canvasPage)
        {
            try
            {
                var lapType = (RebarBeamCutType)instance.RebarBeamCutTypeInfoSelected.Id;
                canvasPage.RatioScale = _canvasRatio;
                canvasPage.Parent.Children.Clear();
                var crossRev = Beam.BoxElement.LineBox.Length.FootToMm();
                var scale = canvasPage.DistanceCrossScreen.PixelToMm() * canvasPage.RatioScale / crossRev;
                canvasPage.Scale = scale;
                canvasPage.ScaleX = scale;
                canvasPage.ScaleY = _canvasRatioY * scale;
                var extent = 20; //[pixel]

                var groupActive = rebarBeamGroupInfo.GroupCuts[instance.RebarGroupCutSelected - 1];
                var revRebarCurvesActive = groupActive.Select(x => x.RevRebarCurves).Aggregate((a, b) => a.Concat(b).ToList());
                var revRebarCurveCutUICanvasAfterCutActive = revRebarCurvesActive
                    .Select(x => x.RevRebarCurveCutUICanvasAfterCut)
                    .Aggregate((a, b) => a.Concat(b).ToList());
                var uIElementsActive = revRebarCurveCutUICanvasAfterCutActive
                    .Select(x => x.UIElements)
                    .Aggregate((a, b) => a.Concat(b).ToList());

                var cGroup = rebarBeamGroupInfo.Groups.Count;
                var hGroup = cGroup == 0 ? 0 : (cGroup - 1) * extent;
                var yCenterGroup = (rebarBeamGroupInfo
                    .Groups.FirstOrDefault()
                    .RevRebarCurves.FirstOrDefault()
                    .RevRebarCurveCutUICanvasAfterCut.FirstOrDefault()
                    .UIElements.FirstOrDefault(x =>
                    {
                        var l = x.Element as System.Windows.Shapes.Line;
                        return l.Direction().DotProduct(new System.Windows.Point(0, 1)).IsAlmostEqual(0);
                    }).Element as System.Windows.Shapes.Line).Y1 + hGroup / 2;
                var deltaY = Math.Abs(canvasPage.Center.Y - yCenterGroup);

                var laps = new List<LapElement>();
                var c = 0;
                foreach (var gr in rebarBeamGroupInfo.Groups)
                {
                    foreach (var cur in gr.RevRebarCurves)
                    {
                        var qRebarCut = cur.RevRebarCurveCutUICanvasAfterCut.Count;
                        var cRebarCut = 0;
                        foreach (var item in cur.RevRebarCurveCutUICanvasAfterCut)
                        {
                            var qUIElements = item.UIElements.Count;
                            var cUIElements = 0;
                            foreach (var item1 in item.UIElements)
                            {
                                var l = item1.Element as System.Windows.Shapes.Line;
                                if (l.Direction().DotProduct(new System.Windows.Point(0, 1)).IsAlmostEqual(0))
                                {
                                    var lnew = new System.Windows.Shapes.Line();
                                    lnew.X1 = l.X1;
                                    lnew.Y1 = l.Y1 + c * extent - deltaY;
                                    lnew.X2 = l.X2;
                                    lnew.Y2 = l.Y2 + c * extent - deltaY;
                                    lnew.StrokeThickness = l.StrokeThickness;
                                    lnew.Stroke = StyleColorInCanvas.Color_Enable;
                                    if (revRebarCurvesActive.Any(x => x.Id == cur.Id))
                                        lnew.Stroke = l.Stroke;
                                    canvasPage.Parent.Children.Add(lnew);
                                    //create lap
                                    if (qRebarCut > 1)
                                    {
                                        if (cRebarCut != qRebarCut - 1)
                                        {
                                            if (qUIElements - 1 == cUIElements)
                                            {
                                                var p = new System.Windows.Point(lnew.X2, lnew.Y2);
                                                var lap = new LapElement(lapType, p, canvasPage);
                                                laps.Add(lap);
                                            }
                                        }
                                    }
                                }
                                cUIElements++;
                            }
                            cRebarCut++;
                        }
                    }
                    c++;
                }
                foreach (var item in laps)
                {
                    item.Create();
                }
            }
            catch (Exception)
            {
            }
        }

        private void DrawRebarInCanvas(
            ElementInstance instance,
            RebarBeamGroupInfo rebarBeamGroupInfo,
            CanvasPageBase canvasPage)
        {
            try
            {
                canvasPage.RatioScale = _canvasRatio;
                canvasPage.Parent.Children.Clear();
                var crossRev = Beam.BoxElement.LineBox.Length.FootToMm();
                var scale = canvasPage.DistanceCrossScreen.PixelToMm() * canvasPage.RatioScale / crossRev;
                canvasPage.Scale = scale;
                canvasPage.ScaleX = scale;
                canvasPage.ScaleY = _canvasRatioY * scale;
                var c = 0;
                foreach (var gr in rebarBeamGroupInfo.GroupCuts[instance.RebarGroupCutSelected - 1])
                {
                    if (c == 0)
                    {
                        foreach (var cur in gr.RevRebarCurves)
                        {
                            //cur.RevRebarCurveCutUICanvas.CanvasPage = CanvasPage;
                            foreach (var item in cur.RevRebarCurveCutUICanvasAfterCut)
                            {
                                foreach (var item1 in item.UIElements)
                                {
                                    var l = item1.Element as System.Windows.Shapes.Line;
                                    CanvasPage.Parent.Children.Add(item1.Element);
                                    CanvasPage.Parent.Children.Add(item1.ElementValue);
                                    item1.EventElementValueChanged += Item1_EventElementValueChanged;
                                }
                            }
                        }
                    }
                    c++;
                }

                GenerateValueRebarCut(instance, rebarBeamGroupInfo, CanvasPage);
                DrawRebarOnSubCanvan(instance, rebarBeamGroupInfo, CanvasPageSub);
            }
            catch (Exception)
            {
            }
        }

        private void RevRebarCurveCutUICanvasInitData(
            ElementInstance instance,
            RebarBeamGroupInfo rebarBeamGroupInfo,
            CanvasPageBase canvasPage)
        {
            try
            {
                canvasPage.RatioScale = _canvasRatio;
                canvasPage.Parent.Children.Clear();
                var crossRev = Beam.BoxElement.LineBox.Length.FootToMm();
                var scale = canvasPage.DistanceCrossScreen.PixelToMm() * canvasPage.RatioScale / crossRev;
                canvasPage.Scale = scale;
                canvasPage.ScaleX = scale;
                canvasPage.ScaleY = _canvasRatioY * scale;
                foreach (var items in rebarBeamGroupInfo.GroupCuts)
                {
                    foreach (var gr in items)
                    {
                        foreach (var cur in gr.RevRebarCurves)
                        {
                            cur.IsSelectedActionBeforeChanged = () =>
                            {
                                RevRebarCurve.ResetStatus(RebarBeamGroupInfoRebarTop1, cur);
                                RevRebarCurve.ResetStatus(RebarBeamGroupInfoRebarTop2, cur);
                                RevRebarCurve.ResetStatus(RebarBeamGroupInfoRebarTop3, cur);
                                RevRebarCurve.ResetStatus(RebarBeamGroupInfoRebarBot1, cur);
                                RevRebarCurve.ResetStatus(RebarBeamGroupInfoRebarBot2, cur);
                                RevRebarCurve.ResetStatus(RebarBeamGroupInfoRebarBot3, cur);
                            };
                            cur.IsSelectedAction = () =>
                            {
                                IsSelectedActionInit(cur, _cmd.UiDocument, instance, rebarBeamGroupInfo, canvasPage);
                            };
                            cur.CanvasPage = CanvasPage;
                            //cur.RevRebarCurveCutUICanvas =
                            //        cur.GetRevRebarCurveCutUICanvas(cur.OriginFake.ConvertPointRToC(rebarBeamGroupInfo.CenterGroup, CanvasPage));
                            cur.RevRebarCurveCutUICanvasAfterCut =
                                cur.GetRevRebarCurveCutUICanvasAfterCut(
                                    0,
                                    ElementInstance.RebarLengthCut,
                                    cur.OriginFake.ConvertPointRToC(rebarBeamGroupInfo.CenterGroup, CanvasPage));
                        }
                    }
                }
            }
            catch (Exception)
            {
            }
        }
    }
}
