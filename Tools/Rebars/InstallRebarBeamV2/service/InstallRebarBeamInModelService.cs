using Autodesk.Revit.DB.Structure;
using HcBimUtils;
using RevitDevelop.Tools.Rebars.InstallRebarBeamV2.iservices;
using RevitDevelop.Tools.Rebars.InstallRebarBeamV2.models;
using RevitDevelop.Tools.Rebars.InstallRebarBeamV2.viewModels;
using RevitDevelop.Utils.RevCurves;
using RevitDevelop.Utils.RevElements;
using RevitDevelop.Utils.RevElements.RevRebars;
using Utils.Messages;
using Utils.RevPoints;

namespace RevitDevelop.Tools.Rebars.InstallRebarBeamV2.service
{
    public class InstallRebarBeamInModelService : IInstallRebarBeamInModelService
    {
        private InstallRebarBeamV2Cmd _cmd;
        private ISubInstallRebarBeamInModelService _subInstallRebarBeamInModelService;
        public InstallRebarBeamInModelService(
            InstallRebarBeamV2Cmd cmd,
            ISubInstallRebarBeamInModelService subInstallRebarBeamInModelService)
        {
            _cmd = cmd;
            _subInstallRebarBeamInModelService = subInstallRebarBeamInModelService;
        }
        public List<Rebar> InstallRebarStirrup(InstallRebarBeamV2ViewModel installRebarBeamV2ViewModel)
        {
            var result = new List<Rebar>();
            try
            {
                var host = _cmd.Document.CreateHost(BuiltInCategory.OST_StructuralFraming);
                var coverBase = installRebarBeamV2ViewModel.ElementInstances.CoverMm.MmToFoot();
                var rebarGroupInfosStart = _subInstallRebarBeamInModelService.GetStirrupGroupInfo(
                    installRebarBeamV2ViewModel,
                    RebarBeamSectionType.SectionStart);
                var rebarGroupInfosMid = _subInstallRebarBeamInModelService.GetStirrupGroupInfo(
                    installRebarBeamV2ViewModel,
                    RebarBeamSectionType.SectionMid);
                var rebarGroupInfosEnd = _subInstallRebarBeamInModelService.GetStirrupGroupInfo(
                    installRebarBeamV2ViewModel,
                    RebarBeamSectionType.SectionEnd);
                var vtx = installRebarBeamV2ViewModel.ElementInstances.Beam.BoxElement.VTX;
                var vty = installRebarBeamV2ViewModel.ElementInstances.Beam.BoxElement.VTY;
                var vtz = installRebarBeamV2ViewModel.ElementInstances.Beam.BoxElement.VTZ;

                var rebarBeams = installRebarBeamV2ViewModel.ElementInstances.RebarBeams;
                var subBeams = installRebarBeamV2ViewModel.ElementInstances.Beam.ElementSubs;
                var cb = 0;
                foreach (var subBeam in subBeams)
                {
                    var rebarBeam = rebarBeams[cb];
                    var beamStressRule = rebarBeam.BeamStressRule;
                    var qbeamStressRule = beamStressRule.Stress.Count;
                    var boxPs = subBeam.BoxElementPoint;
                    var beamLength = boxPs.P1.DistanceTo(boxPs.P4);
                    RebarBarTypeCustom diameter = null;
                    var cover = coverBase;
                    var spacing = 0.0;
                    for (int i = 0; i < qbeamStressRule; i++)
                    {
                        var spacingSturrup = i == 0
                            ? rebarGroupInfosStart[cb].Spacing
                            : i == qbeamStressRule - 1
                            ? rebarGroupInfosEnd[cb].Spacing : rebarGroupInfosMid[cb].Spacing;
                        diameter = i == 0
                            ? installRebarBeamV2ViewModel.ElementInstances.RebarBarTypeCustoms
                                .FirstOrDefault(x => x.NameStyle == rebarGroupInfosStart[cb].Diameter)
                            : i == qbeamStressRule - 1
                                ? installRebarBeamV2ViewModel.ElementInstances.RebarBarTypeCustoms
                                                            .FirstOrDefault(x => x.NameStyle == rebarGroupInfosEnd[cb].Diameter)
                                : installRebarBeamV2ViewModel.ElementInstances.RebarBarTypeCustoms
                                                            .FirstOrDefault(x => x.NameStyle == rebarGroupInfosMid[cb].Diameter);
                        if (diameter == null) continue;
                        cover = coverBase + diameter.ModelBarDiameter / 2;

                        spacing += i == 0
                            ? 0
                            : beamStressRule.Stress[i - 1] * beamLength;
                        var psNew = new List<XYZ>()
                        {
                            boxPs.P1 + vtx * spacing,
                            boxPs.P2 + vtx * spacing,
                            boxPs.P6 + vtx * spacing,
                            boxPs.P5 + vtx * spacing,
                        };

                        var curs = psNew.PointsToCurves(true);
                        var curveLoop = CurveLoop.CreateViaOffset(curs.ToCurveLoop(), cover, -vtx);
                        var rebar = Rebar.CreateFromCurves(
                            _cmd.Document,
                            RebarStyle.StirrupTie,
                            diameter.RebarBarType,
                            null,
                            null,
                            host,
                            vtx,
                            curveLoop.ToList(),
                            RebarHookOrientation.Left,
                            RebarHookOrientation.Left,
                            true,
                            true);
                        rebar.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).Set((int)RebarLayoutRule.MaximumSpacing);
                        rebar.get_Parameter(BuiltInParameter.REBAR_ELEM_BAR_SPACING).Set(spacingSturrup.MmToFoot());
                        rebar.GetShapeDrivenAccessor().ArrayLength = beamStressRule.Stress[i] * beamLength;
                        RevRebarUtils.SetSolidRebar3DView(rebar, _cmd.Document.ActiveView);
                        result.Add(rebar);
                    }
                    cb++;
                }
            }
            catch (Exception ex)
            {
                IO.ShowWarning(ex.Message);
            }
            return result;
        }

        public List<Rebar> InstallRebarTop1(InstallRebarBeamV2ViewModel installRebarBeamV2ViewModel)
        {
            var result = new List<Rebar>();
            try
            {
                var host = _cmd.Document.CreateHost(BuiltInCategory.OST_StructuralFraming);
                var rebarStirrupInfos = _subInstallRebarBeamInModelService.GetStirrupGroupInfo(
                    installRebarBeamV2ViewModel,
                    RebarBeamSectionType.SectionStart)
                    .LastOrDefault();
                var diameterStirrup = installRebarBeamV2ViewModel.ElementInstances.RebarBarTypeCustoms
                    .FirstOrDefault(x => x.NameStyle == rebarStirrupInfos.Diameter);
                var rebarGroupInfos = _subInstallRebarBeamInModelService.GetRebarBeamGroupInfo(
                    installRebarBeamV2ViewModel,
                    RebarBeamSectionType.SectionStart,
                    RebarBeamMainBarLevelType.RebarTop,
                    RebarBeamMainBarGroupType.GroupLevel1);
                var rebarGroupInfo = rebarGroupInfos.FirstOrDefault();
                var diameter = installRebarBeamV2ViewModel.ElementInstances.RebarBarTypeCustoms
                    .FirstOrDefault(x => x.NameStyle == rebarGroupInfo.Diameter);
                var vtx = installRebarBeamV2ViewModel.ElementInstances.Beam.BoxElement.VTX;
                var vty = installRebarBeamV2ViewModel.ElementInstances.Beam.BoxElement.VTY;
                var vtz = installRebarBeamV2ViewModel.ElementInstances.Beam.BoxElement.VTZ;
                var cover = installRebarBeamV2ViewModel.ElementInstances.CoverMm.MmToFoot()
                    + diameterStirrup.ModelBarDiameter
                    + diameter.ModelBarDiameter / 2
                    + diameterStirrup.StandardBendDiameter / 4;
                var rebarBeams = installRebarBeamV2ViewModel.ElementInstances.RebarBeams;
                var subBeams = installRebarBeamV2ViewModel.ElementInstances.Beam.ElementSubs;
                var qRebarBeams = rebarBeams.Count;
                var boxElementPoint = installRebarBeamV2ViewModel.ElementInstances.Beam.BoxElement.BoxElementPoint;
                var p1 = boxElementPoint.P5;
                var p2 = boxElementPoint.P6;
                var p3 = boxElementPoint.P7;
                var p4 = boxElementPoint.P8;
                var ps = new List<XYZ>()
                {
                    p1.EditZ(p1.Z - cover + diameterStirrup.StandardBendDiameter / 4),
                    p2.EditZ(p2.Z - cover + diameterStirrup.StandardBendDiameter / 4),
                    p3.EditZ(p2.Z - cover + diameterStirrup.StandardBendDiameter / 4),
                    p4.EditZ(p2.Z - cover + diameterStirrup.StandardBendDiameter / 4) };
                var curs = ps.PointsToCurves(true);
                var curves = CurveLoop
                    .CreateViaOffset(curs.ToCurveLoop(), cover, vtz)
                    .ToList().OrderBy(x => x.Length);
                var mainCurve = Line.CreateBound(
                    curves.LastOrDefault().GetEndPoint(0) - vtx * cover,
                    curves.LastOrDefault().GetEndPoint(1) + vtx * cover);
                var lengthArr = curves.FirstOrDefault().Length;

                var rebar = Rebar.CreateFromCurves(
                            _cmd.Document,
                            RebarStyle.Standard,
                            diameter.RebarBarType,
                            null,
                            null,
                            host,
                            -vty,
                            new List<Curve>() { mainCurve },
                            RebarHookOrientation.Left,
                            RebarHookOrientation.Left,
                            true,
                            true);
                rebar.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).Set((int)RebarLayoutRule.NumberWithSpacing);
                rebar.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set(rebarGroupInfo.Quantity);
                rebar.get_Parameter(BuiltInParameter.REBAR_ELEM_BAR_SPACING).Set(lengthArr / (rebarGroupInfo.Quantity - 1));
                rebar.GetShapeDrivenAccessor().ArrayLength = lengthArr;
                RevRebarUtils.SetSolidRebar3DView(rebar, _cmd.Document.ActiveView);
                result.Add(rebar);
            }
            catch (Exception ex)
            {
                IO.ShowWarning(ex.Message);
            }
            return result;
        }

        public List<Rebar> InstallRebarTop2(InstallRebarBeamV2ViewModel installRebarBeamV2ViewModel)
        {
            throw new NotImplementedException();
        }

        public List<Rebar> InstallRebarTop3(InstallRebarBeamV2ViewModel installRebarBeamV2ViewModel)
        {
            throw new NotImplementedException();
        }

        public List<Rebar> InstallRebarBot1(InstallRebarBeamV2ViewModel installRebarBeamV2ViewModel)
        {
            var result = new List<Rebar>();
            try
            {
                var host = _cmd.Document.CreateHost(BuiltInCategory.OST_StructuralFraming);
                var rebarStirrupInfos = _subInstallRebarBeamInModelService.GetStirrupGroupInfo(
                    installRebarBeamV2ViewModel,
                    RebarBeamSectionType.SectionStart)
                    .LastOrDefault();
                var diameterStirrup = installRebarBeamV2ViewModel.ElementInstances.RebarBarTypeCustoms
                    .FirstOrDefault(x => x.NameStyle == rebarStirrupInfos.Diameter);
                var rebarGroupInfos = _subInstallRebarBeamInModelService.GetRebarBeamGroupInfo(
                    installRebarBeamV2ViewModel,
                    RebarBeamSectionType.SectionStart,
                    RebarBeamMainBarLevelType.RebarBot,
                    RebarBeamMainBarGroupType.GroupLevel1);
                var rebarGroupInfo = rebarGroupInfos.FirstOrDefault();
                var diameter = installRebarBeamV2ViewModel.ElementInstances.RebarBarTypeCustoms
                    .FirstOrDefault(x => x.NameStyle == rebarGroupInfo.Diameter);
                var vtx = installRebarBeamV2ViewModel.ElementInstances.Beam.BoxElement.VTX;
                var vty = installRebarBeamV2ViewModel.ElementInstances.Beam.BoxElement.VTY;
                var vtz = installRebarBeamV2ViewModel.ElementInstances.Beam.BoxElement.VTZ;
                var cover = installRebarBeamV2ViewModel.ElementInstances.CoverMm.MmToFoot()
                    + diameterStirrup.ModelBarDiameter
                    + diameter.ModelBarDiameter / 2
                    + diameterStirrup.StandardBendDiameter / 4;
                var rebarBeams = installRebarBeamV2ViewModel.ElementInstances.RebarBeams;
                var subBeams = installRebarBeamV2ViewModel.ElementInstances.Beam.ElementSubs;
                var qRebarBeams = rebarBeams.Count;
                var boxElementPoint = installRebarBeamV2ViewModel.ElementInstances.Beam.BoxElement.BoxElementPoint;
                var p1 = boxElementPoint.P1;
                var p2 = boxElementPoint.P2;
                var p3 = boxElementPoint.P3;
                var p4 = boxElementPoint.P4;
                var ps = new List<XYZ>()
                {
                    p1.EditZ(p1.Z + cover - diameterStirrup.StandardBendDiameter / 4),
                    p2.EditZ(p2.Z + cover - diameterStirrup.StandardBendDiameter / 4),
                    p3.EditZ(p2.Z + cover - diameterStirrup.StandardBendDiameter / 4),
                    p4.EditZ(p2.Z + cover - diameterStirrup.StandardBendDiameter / 4) };
                var curs = ps.PointsToCurves(true);
                var curves = CurveLoop
                    .CreateViaOffset(curs.ToCurveLoop(), cover, vtz)
                    .ToList().OrderBy(x => x.Length);
                var mainCurve = Line.CreateBound(
                    curves.LastOrDefault().GetEndPoint(0) - vtx * cover,
                    curves.LastOrDefault().GetEndPoint(1) + vtx * cover);
                var lengthArr = curves.FirstOrDefault().Length;

                var rebar = Rebar.CreateFromCurves(
                            _cmd.Document,
                            RebarStyle.Standard,
                            diameter.RebarBarType,
                            null,
                            null,
                            host,
                            -vty,
                            new List<Curve>() { mainCurve },
                            RebarHookOrientation.Left,
                            RebarHookOrientation.Left,
                            true,
                            true);
                rebar.get_Parameter(BuiltInParameter.REBAR_ELEM_LAYOUT_RULE).Set((int)RebarLayoutRule.NumberWithSpacing);
                rebar.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS).Set(rebarGroupInfo.Quantity);
                rebar.get_Parameter(BuiltInParameter.REBAR_ELEM_BAR_SPACING).Set(lengthArr / (rebarGroupInfo.Quantity - 1));
                rebar.GetShapeDrivenAccessor().ArrayLength = lengthArr;
                RevRebarUtils.SetSolidRebar3DView(rebar, _cmd.Document.ActiveView);
                result.Add(rebar);
            }
            catch (Exception ex)
            {
                IO.ShowWarning(ex.Message);
            }
            return result;
        }

        public List<Rebar> InstallRebarBot2(InstallRebarBeamV2ViewModel installRebarBeamV2ViewModel)
        {
            throw new NotImplementedException();
        }

        public List<Rebar> InstallRebarBot3(InstallRebarBeamV2ViewModel installRebarBeamV2ViewModel)
        {
            throw new NotImplementedException();
        }

        public List<Rebar> InstallRebarSide(InstallRebarBeamV2ViewModel installRebarBeamV2ViewModel)
        {
            throw new NotImplementedException();
        }
    }
}
