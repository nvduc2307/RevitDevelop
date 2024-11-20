using HcBimUtils;
using RevitDevelop.Tools.Rebars.InstallRebarBeamV2.iservices;
using RevitDevelop.Tools.Rebars.InstallRebarBeamV2.models;
using RevitDevelop.Tools.Rebars.InstallRebarBeamV2.viewModels;
using RevitDevelop.Utils.RevElements.RevRebars;
using System.Windows;
using Utils.canvass;

namespace RevitDevelop.Tools.Rebars.InstallRebarBeamV2.service
{
    public class DrawRebarBeamInCanvasSerice : IDrawRebarBeamInCanvasSerice
    {
        public DrawRebarBeamInCanvasSerice()
        {
        }
        public void DrawSectionBeamConcrete(RebarBeam rebarBeam, InstallRebarBeamV2ViewModel installRebarBeamV2ViewModel)
        {
            var canvasStart = installRebarBeamV2ViewModel.CanvasPageSectionStart;
            var canvasMid = installRebarBeamV2ViewModel.CanvasPageSectionMid;
            var canvasEnd = installRebarBeamV2ViewModel.CanvasPageSectionEnd;
            _drawSectionBeamConcrete(rebarBeam, canvasStart);
            _drawSectionBeamConcrete(rebarBeam, canvasMid);
            _drawSectionBeamConcrete(rebarBeam, canvasEnd);

        }

        public void DrawSectionBeamStirrup(RebarBeam rebarBeam, InstallRebarBeamV2ViewModel installRebarBeamV2ViewModel)
        {
            var canvasStart = installRebarBeamV2ViewModel.CanvasPageSectionStart;
            var canvasMid = installRebarBeamV2ViewModel.CanvasPageSectionMid;
            var canvasEnd = installRebarBeamV2ViewModel.CanvasPageSectionEnd;
            _drawSectionBeamStirrup(rebarBeam, installRebarBeamV2ViewModel.ElementInstances.CoverMm, canvasStart);
            _drawSectionBeamStirrup(rebarBeam, installRebarBeamV2ViewModel.ElementInstances.CoverMm, canvasMid);
            _drawSectionBeamStirrup(rebarBeam, installRebarBeamV2ViewModel.ElementInstances.CoverMm, canvasEnd);
        }

        public List<UIElement> DrawSectionBeamMainBarTop(RebarBeam rebarBeam, InstallRebarBeamV2ViewModel installRebarBeamV2ViewModel)
        {

            var results = new List<UIElement>();
            var canvasStart = installRebarBeamV2ViewModel.CanvasPageSectionStart;
            var canvasMid = installRebarBeamV2ViewModel.CanvasPageSectionMid;
            var canvasEnd = installRebarBeamV2ViewModel.CanvasPageSectionEnd;

            foreach (var item in installRebarBeamV2ViewModel.ElementInstances.MainRebarTopUIElement)
            {
                try
                {
                    canvasStart.Parent.Children.Remove(item);
                }
                catch (Exception)
                {
                }
                try
                {
                    canvasMid.Parent.Children.Remove(item);
                }
                catch (Exception)
                {
                }
                try
                {
                    canvasEnd.Parent.Children.Remove(item);
                }
                catch (Exception)
                {
                }
            }

            var uiElement1 = _drawSectionBeamMainBar(
                rebarBeam,
                installRebarBeamV2ViewModel,
                installRebarBeamV2ViewModel.ElementInstances.CoverMm,
                canvasStart,
                RebarBeamMainBarLevelType.RebarTop,
                RebarBeamSectionType.SectionStart,
                RebarBeamMainBarGroupType.GroupLevel1);
            var uiElement2 = _drawSectionBeamMainBar(
               rebarBeam,
               installRebarBeamV2ViewModel,
               installRebarBeamV2ViewModel.ElementInstances.CoverMm,
               canvasStart,
               RebarBeamMainBarLevelType.RebarTop,
               RebarBeamSectionType.SectionStart,
               RebarBeamMainBarGroupType.GroupLevel2);
            var uiElement3 = _drawSectionBeamMainBar(
               rebarBeam,
               installRebarBeamV2ViewModel,
               installRebarBeamV2ViewModel.ElementInstances.CoverMm,
               canvasStart,
               RebarBeamMainBarLevelType.RebarTop,
               RebarBeamSectionType.SectionStart,
               RebarBeamMainBarGroupType.GroupLevel3);

            var uiElement4 = _drawSectionBeamMainBar(
                rebarBeam,
                installRebarBeamV2ViewModel,
                installRebarBeamV2ViewModel.ElementInstances.CoverMm,
                canvasMid,
                RebarBeamMainBarLevelType.RebarTop,
                RebarBeamSectionType.SectionMid,
                RebarBeamMainBarGroupType.GroupLevel1);
            var uiElement5 = _drawSectionBeamMainBar(
               rebarBeam,
               installRebarBeamV2ViewModel,
               installRebarBeamV2ViewModel.ElementInstances.CoverMm,
               canvasMid,
               RebarBeamMainBarLevelType.RebarTop,
               RebarBeamSectionType.SectionMid,
               RebarBeamMainBarGroupType.GroupLevel2);
            var uiElement6 = _drawSectionBeamMainBar(
               rebarBeam,
               installRebarBeamV2ViewModel,
               installRebarBeamV2ViewModel.ElementInstances.CoverMm,
               canvasMid,
               RebarBeamMainBarLevelType.RebarTop,
               RebarBeamSectionType.SectionMid,
               RebarBeamMainBarGroupType.GroupLevel3);

            var uiElement7 = _drawSectionBeamMainBar(
                rebarBeam,
                installRebarBeamV2ViewModel,
                installRebarBeamV2ViewModel.ElementInstances.CoverMm,
                canvasEnd,
                RebarBeamMainBarLevelType.RebarTop,
                RebarBeamSectionType.SectionEnd,
                RebarBeamMainBarGroupType.GroupLevel1);
            var uiElement8 = _drawSectionBeamMainBar(
               rebarBeam,
               installRebarBeamV2ViewModel,
               installRebarBeamV2ViewModel.ElementInstances.CoverMm,
               canvasEnd,
               RebarBeamMainBarLevelType.RebarTop,
               RebarBeamSectionType.SectionEnd,
               RebarBeamMainBarGroupType.GroupLevel2);
            var uiElement9 = _drawSectionBeamMainBar(
               rebarBeam,
               installRebarBeamV2ViewModel,
               installRebarBeamV2ViewModel.ElementInstances.CoverMm,
               canvasEnd,
               RebarBeamMainBarLevelType.RebarTop,
               RebarBeamSectionType.SectionEnd,
               RebarBeamMainBarGroupType.GroupLevel3);
            results.AddRange(uiElement1);
            results.AddRange(uiElement2);
            results.AddRange(uiElement3);
            results.AddRange(uiElement4);
            results.AddRange(uiElement5);
            results.AddRange(uiElement6);
            results.AddRange(uiElement7);
            results.AddRange(uiElement8);
            results.AddRange(uiElement9);
            return results;
        }

        public List<UIElement> DrawSectionBeamMainBarBot(RebarBeam rebarBeam, InstallRebarBeamV2ViewModel installRebarBeamV2ViewModel)
        {
            var results = new List<UIElement>();
            var canvasStart = installRebarBeamV2ViewModel.CanvasPageSectionStart;
            var canvasMid = installRebarBeamV2ViewModel.CanvasPageSectionMid;
            var canvasEnd = installRebarBeamV2ViewModel.CanvasPageSectionEnd;

            foreach (var item in installRebarBeamV2ViewModel.ElementInstances.MainRebarBotUIElement)
            {
                try
                {
                    canvasStart.Parent.Children.Remove(item);
                }
                catch (Exception)
                {
                }
                try
                {
                    canvasMid.Parent.Children.Remove(item);
                }
                catch (Exception)
                {
                }
                try
                {
                    canvasEnd.Parent.Children.Remove(item);
                }
                catch (Exception)
                {
                }
            }

            var uiElement1 = _drawSectionBeamMainBar(
                rebarBeam,
                installRebarBeamV2ViewModel,
                installRebarBeamV2ViewModel.ElementInstances.CoverMm,
                canvasStart,
                RebarBeamMainBarLevelType.RebarBot,
                RebarBeamSectionType.SectionStart,
                RebarBeamMainBarGroupType.GroupLevel1);
            var uiElement2 = _drawSectionBeamMainBar(
               rebarBeam,
               installRebarBeamV2ViewModel,
               installRebarBeamV2ViewModel.ElementInstances.CoverMm,
               canvasStart,
               RebarBeamMainBarLevelType.RebarBot,
               RebarBeamSectionType.SectionStart,
               RebarBeamMainBarGroupType.GroupLevel2);
            var uiElement3 = _drawSectionBeamMainBar(
               rebarBeam,
               installRebarBeamV2ViewModel,
               installRebarBeamV2ViewModel.ElementInstances.CoverMm,
               canvasStart,
               RebarBeamMainBarLevelType.RebarBot,
               RebarBeamSectionType.SectionStart,
               RebarBeamMainBarGroupType.GroupLevel3);

            var uiElement4 = _drawSectionBeamMainBar(
                rebarBeam,
                installRebarBeamV2ViewModel,
                installRebarBeamV2ViewModel.ElementInstances.CoverMm,
                canvasMid,
                RebarBeamMainBarLevelType.RebarBot,
                RebarBeamSectionType.SectionMid,
                RebarBeamMainBarGroupType.GroupLevel1);
            var uiElement5 = _drawSectionBeamMainBar(
               rebarBeam,
               installRebarBeamV2ViewModel,
               installRebarBeamV2ViewModel.ElementInstances.CoverMm,
               canvasMid,
               RebarBeamMainBarLevelType.RebarBot,
               RebarBeamSectionType.SectionMid,
               RebarBeamMainBarGroupType.GroupLevel2);
            var uiElement6 = _drawSectionBeamMainBar(
               rebarBeam,
               installRebarBeamV2ViewModel,
               installRebarBeamV2ViewModel.ElementInstances.CoverMm,
               canvasMid,
               RebarBeamMainBarLevelType.RebarBot,
               RebarBeamSectionType.SectionMid,
               RebarBeamMainBarGroupType.GroupLevel3);

            var uiElement7 = _drawSectionBeamMainBar(
                rebarBeam,
                installRebarBeamV2ViewModel,
                installRebarBeamV2ViewModel.ElementInstances.CoverMm,
                canvasEnd,
                RebarBeamMainBarLevelType.RebarBot,
                RebarBeamSectionType.SectionEnd,
                RebarBeamMainBarGroupType.GroupLevel1);
            var uiElement8 = _drawSectionBeamMainBar(
               rebarBeam,
               installRebarBeamV2ViewModel,
               installRebarBeamV2ViewModel.ElementInstances.CoverMm,
               canvasEnd,
               RebarBeamMainBarLevelType.RebarBot,
               RebarBeamSectionType.SectionEnd,
               RebarBeamMainBarGroupType.GroupLevel2);
            var uiElement9 = _drawSectionBeamMainBar(
               rebarBeam,
               installRebarBeamV2ViewModel,
               installRebarBeamV2ViewModel.ElementInstances.CoverMm,
               canvasEnd,
               RebarBeamMainBarLevelType.RebarBot,
               RebarBeamSectionType.SectionEnd,
               RebarBeamMainBarGroupType.GroupLevel3);
            results.AddRange(uiElement1);
            results.AddRange(uiElement2);
            results.AddRange(uiElement3);
            results.AddRange(uiElement4);
            results.AddRange(uiElement5);
            results.AddRange(uiElement6);
            results.AddRange(uiElement7);
            results.AddRange(uiElement8);
            results.AddRange(uiElement9);
            return results;
        }

        public List<UIElement> DrawSectionBeamSideBar(RebarBeam rebarBeam, InstallRebarBeamV2ViewModel installRebarBeamV2ViewModel)
        {
            var results = new List<UIElement>();
            try
            {
                var canvasStart = installRebarBeamV2ViewModel.CanvasPageSectionStart;
                var canvasMid = installRebarBeamV2ViewModel.CanvasPageSectionMid;
                var canvasEnd = installRebarBeamV2ViewModel.CanvasPageSectionEnd;

                foreach (var item in installRebarBeamV2ViewModel.ElementInstances.SideBarUIElement)
                {
                    try
                    {
                        canvasStart.Parent.Children.Remove(item);
                    }
                    catch (Exception)
                    {
                    }
                    try
                    {
                        canvasMid.Parent.Children.Remove(item);
                    }
                    catch (Exception)
                    {
                    }
                    try
                    {
                        canvasEnd.Parent.Children.Remove(item);
                    }
                    catch (Exception)
                    {
                    }
                }

                var uiElement1 = _drawSectionBeamSideBar(
                    rebarBeam,
                    installRebarBeamV2ViewModel,
                    installRebarBeamV2ViewModel.ElementInstances.CoverMm,
                    canvasStart,
                    RebarBeamMainBarLevelType.RebarBot,
                    RebarBeamSectionType.SectionStart,
                    RebarBeamMainBarGroupType.GroupLevel1);
                var uiElement2 = _drawSectionBeamSideBar(
                    rebarBeam,
                    installRebarBeamV2ViewModel,
                    installRebarBeamV2ViewModel.ElementInstances.CoverMm,
                    canvasMid,
                    RebarBeamMainBarLevelType.RebarBot,
                    RebarBeamSectionType.SectionMid,
                    RebarBeamMainBarGroupType.GroupLevel1);
                var uiElement3 = _drawSectionBeamSideBar(
                    rebarBeam,
                    installRebarBeamV2ViewModel,
                    installRebarBeamV2ViewModel.ElementInstances.CoverMm,
                    canvasEnd,
                    RebarBeamMainBarLevelType.RebarBot,
                    RebarBeamSectionType.SectionEnd,
                    RebarBeamMainBarGroupType.GroupLevel1);
                results.AddRange(uiElement1);
                results.AddRange(uiElement2);
                results.AddRange(uiElement3);
            }
            catch (Exception)
            {
            }
            return results;
        }

        private List<UIElement> _drawSectionBeamSideBar(
            RebarBeam rebarBeam,
            InstallRebarBeamV2ViewModel installRebarBeamV2ViewModel,
            double coverMm,
            CanvasPageBase canvasPageBase,
            RebarBeamMainBarLevelType rebarBeamMainBarLevelType,
            RebarBeamSectionType sectionType,
            RebarBeamMainBarGroupType rebarBeamMainBarGroupType)
        {
            var results = new List<UIElement>();
            try
            {
                var centerCanvas = canvasPageBase.Center;
                var scale = canvasPageBase.RatioScale * canvasPageBase.DistanceCrossScreen / Math.Sqrt(rebarBeam.BeamWidthMm * rebarBeam.BeamWidthMm + rebarBeam.BeamHeightMm * rebarBeam.BeamHeightMm);
                var option = OptionStyleInstanceInCanvas.OPTION_REBAR;
                var rebarBarTypeCustoms = installRebarBeamV2ViewModel.ElementInstances.RebarBarTypeCustoms;
                var distanceRToRMm = installRebarBeamV2ViewModel.ElementInstances.DistanceRebarToRebarMm;
                var coverUpMm = coverMm;
                var coverSideMm = coverMm;
                _getCover(
                    rebarBeam,
                    installRebarBeamV2ViewModel,
                    coverMm,
                    canvasPageBase,
                    sectionType,
                    rebarBeamMainBarLevelType,
                    rebarBeamMainBarGroupType,
                    out double _coverUpMm,
                    out double _coverSideMm);
                coverUpMm = _coverUpMm + 40;
                coverSideMm = _coverSideMm + 40;
                var p1 = new System.Windows.Point(centerCanvas.X - scale * (rebarBeam.BeamWidthMm / 2 - coverSideMm), centerCanvas.Y - scale * (rebarBeam.BeamHeightMm / 2 - coverUpMm));
                var p2 = new System.Windows.Point(centerCanvas.X + scale * (rebarBeam.BeamWidthMm / 2 - coverSideMm), centerCanvas.Y - scale * (rebarBeam.BeamHeightMm / 2 - coverUpMm));
                var p3 = new System.Windows.Point(centerCanvas.X + scale * (rebarBeam.BeamWidthMm / 2 - coverSideMm), centerCanvas.Y + scale * (rebarBeam.BeamHeightMm / 2 - coverUpMm));
                var p4 = new System.Windows.Point(centerCanvas.X - scale * (rebarBeam.BeamWidthMm / 2 - coverSideMm), centerCanvas.Y + scale * (rebarBeam.BeamHeightMm / 2 - coverUpMm));
                var ps = new List<System.Windows.Point>() {
                    p1, p2, p3, p4
                };
                var diameterMm = 7;
                InstanceInCanvasCircel circleL = null;
                InstanceInCanvasCircel circleR = null;
                int qty = 0;
                int qtyHaft = 0;
                RebarBeamSection section = null;
                double distance = p1.DistanceTo(p4) - 50 * 6 * scale;
                double spacing = 0;
                var midL = new System.Windows.Point((p1.X + p4.X) * 0.5, (p1.Y + p4.Y) * 0.5);
                var midR = new System.Windows.Point((p2.X + p3.X) * 0.5, (p2.Y + p3.Y) * 0.5);
                var pL = midL;
                var pR = midR;
                switch (sectionType)
                {
                    case RebarBeamSectionType.SectionStart:
                        section = installRebarBeamV2ViewModel.ElementInstances.RebarBeamActive.RebarBeamSectionStart;
                        qty = section.RebarBeamSideBar.QuantitySide;
                        qtyHaft = (qty - 1) / 2;
                        spacing = distance / (qty + 1);
                        pL = qty % 2 == 0
                            ? new System.Windows.Point(midL.X, midL.Y - spacing / 2 - qtyHaft * spacing)
                            : new System.Windows.Point(midL.X, midL.Y - qtyHaft * spacing);
                        pR = qty % 2 == 0
                            ? new System.Windows.Point(midR.X, midR.Y - spacing / 2 - qtyHaft * spacing)
                            : new System.Windows.Point(midR.X, midR.Y - qtyHaft * spacing);
                        for (int i = 0; i < qty; i++)
                        {
                            var ppL = pL.Translate(new System.Windows.Point(0, spacing * i));
                            var ppR = pR.Translate(new System.Windows.Point(0, spacing * i));
                            circleL = new InstanceInCanvasCircel(canvasPageBase, option, centerCanvas, diameterMm, ppL, new System.Windows.Point(0, 0), "");
                            circleR = new InstanceInCanvasCircel(canvasPageBase, option, centerCanvas, diameterMm, ppR, new System.Windows.Point(0, 0), "");
                            circleL.DrawInCanvas();
                            circleR.DrawInCanvas();
                            results.Add(circleL.UIElement);
                            results.Add(circleR.UIElement);
                        }
                        break;
                    case RebarBeamSectionType.SectionMid:
                        section = installRebarBeamV2ViewModel.ElementInstances.RebarBeamActive.RebarBeamSectionMid;
                        qty = section.RebarBeamSideBar.QuantitySide;
                        qtyHaft = (qty - 1) / 2;
                        spacing = distance / (qty + 1);
                        pL = qty % 2 == 0
                            ? new System.Windows.Point(midL.X, midL.Y - spacing / 2 - qtyHaft * spacing)
                            : new System.Windows.Point(midL.X, midL.Y - qtyHaft * spacing);
                        pR = qty % 2 == 0
                            ? new System.Windows.Point(midR.X, midR.Y - spacing / 2 - qtyHaft * spacing)
                            : new System.Windows.Point(midR.X, midR.Y - qtyHaft * spacing);
                        for (int i = 0; i < qty; i++)
                        {
                            var ppL = pL.Translate(new System.Windows.Point(0, spacing * i));
                            var ppR = pR.Translate(new System.Windows.Point(0, spacing * i));
                            circleL = new InstanceInCanvasCircel(canvasPageBase, option, centerCanvas, diameterMm, ppL, new System.Windows.Point(0, 0), "");
                            circleR = new InstanceInCanvasCircel(canvasPageBase, option, centerCanvas, diameterMm, ppR, new System.Windows.Point(0, 0), "");
                            circleL.DrawInCanvas();
                            circleR.DrawInCanvas();
                            results.Add(circleL.UIElement);
                            results.Add(circleR.UIElement);
                        }
                        break;
                    case RebarBeamSectionType.SectionEnd:
                        section = installRebarBeamV2ViewModel.ElementInstances.RebarBeamActive.RebarBeamSectionEnd;
                        qty = section.RebarBeamSideBar.QuantitySide;
                        spacing = distance / (qty + 1);
                        qtyHaft = (qty - 1) / 2;
                        pL = qty % 2 == 0
                            ? new System.Windows.Point(midL.X, midL.Y - spacing / 2 - qtyHaft * spacing)
                            : new System.Windows.Point(midL.X, midL.Y - qtyHaft * spacing);
                        pR = qty % 2 == 0
                            ? new System.Windows.Point(midR.X, midR.Y - spacing / 2 - qtyHaft * spacing)
                            : new System.Windows.Point(midR.X, midR.Y - qtyHaft * spacing);
                        for (int i = 0; i < qty; i++)
                        {
                            var ppL = pL.Translate(new System.Windows.Point(0, spacing * i));
                            var ppR = pR.Translate(new System.Windows.Point(0, spacing * i));
                            circleL = new InstanceInCanvasCircel(canvasPageBase, option, centerCanvas, diameterMm, ppL, new System.Windows.Point(0, 0), "");
                            circleR = new InstanceInCanvasCircel(canvasPageBase, option, centerCanvas, diameterMm, ppR, new System.Windows.Point(0, 0), "");
                            circleL.DrawInCanvas();
                            circleR.DrawInCanvas();
                            results.Add(circleL.UIElement);
                            results.Add(circleR.UIElement);
                        }

                        break;
                }
            }
            catch (Exception)
            {
            }
            return results;
        }
        private List<UIElement> _drawSectionBeamMainBar(
            RebarBeam rebarBeam,
            InstallRebarBeamV2ViewModel installRebarBeamV2ViewModel,
            double coverMm,
            CanvasPageBase canvasPageBase,
            RebarBeamMainBarLevelType rebarBeamMainBarLevelType,
            RebarBeamSectionType sectionType,
            RebarBeamMainBarGroupType rebarBeamMainBarGroupType)
        {
            var results = new List<UIElement>();
            try
            {
                var centerCanvas = canvasPageBase.Center;
                var scale = canvasPageBase.RatioScale * canvasPageBase.DistanceCrossScreen / Math.Sqrt(rebarBeam.BeamWidthMm * rebarBeam.BeamWidthMm + rebarBeam.BeamHeightMm * rebarBeam.BeamHeightMm);
                var option = OptionStyleInstanceInCanvas.OPTION_REBAR;
                var rebarBarTypeCustoms = installRebarBeamV2ViewModel.ElementInstances.RebarBarTypeCustoms;
                var distanceRToRMm = installRebarBeamV2ViewModel.ElementInstances.DistanceRebarToRebarMm;
                var coverUpMm = coverMm;
                var coverSideMm = coverMm;
                _getCover(
                    rebarBeam,
                    installRebarBeamV2ViewModel,
                    coverMm,
                    canvasPageBase,
                    sectionType,
                    rebarBeamMainBarLevelType,
                    rebarBeamMainBarGroupType,
                    out double _coverUpMm,
                    out double _coverSideMm);
                coverUpMm = _coverUpMm + 40;
                coverSideMm = _coverSideMm + 40;
                var p1 = new System.Windows.Point(centerCanvas.X - scale * (rebarBeam.BeamWidthMm / 2 - coverSideMm), centerCanvas.Y - scale * (rebarBeam.BeamHeightMm / 2 - coverUpMm));
                var p2 = new System.Windows.Point(centerCanvas.X + scale * (rebarBeam.BeamWidthMm / 2 - coverSideMm), centerCanvas.Y - scale * (rebarBeam.BeamHeightMm / 2 - coverUpMm));
                var p3 = new System.Windows.Point(centerCanvas.X + scale * (rebarBeam.BeamWidthMm / 2 - coverSideMm), centerCanvas.Y + scale * (rebarBeam.BeamHeightMm / 2 - coverUpMm));
                var p4 = new System.Windows.Point(centerCanvas.X - scale * (rebarBeam.BeamWidthMm / 2 - coverSideMm), centerCanvas.Y + scale * (rebarBeam.BeamHeightMm / 2 - coverUpMm));
                var ps = new List<System.Windows.Point>() {
                    p1, p2, p3, p4
                };
                var diameterMm = 7;
                InstanceInCanvasCircel circle = null;
                int qty = 0;
                RebarBeamSection section = null;
                double distance = 0;
                double spacing = 0;
                switch (sectionType)
                {
                    case RebarBeamSectionType.SectionStart:
                        section = installRebarBeamV2ViewModel.ElementInstances.RebarBeamActive.RebarBeamSectionStart;
                        switch (rebarBeamMainBarLevelType)
                        {
                            case RebarBeamMainBarLevelType.RebarTop:
                                switch (rebarBeamMainBarGroupType)
                                {
                                    case RebarBeamMainBarGroupType.GroupLevel1:
                                        qty = section.RebarBeamTop.RebarBeamTopLevel1.Quantity;
                                        distance = p1.DistanceTo(p2);
                                        spacing = distance / (qty - 1);
                                        for (int i = 0; i < qty; i++)
                                        {
                                            var pp = p1.Translate(new System.Windows.Point(spacing * i, 0));
                                            circle = new InstanceInCanvasCircel(canvasPageBase, option, centerCanvas, diameterMm, pp, new System.Windows.Point(0, 0), "");
                                            circle.DrawInCanvas();
                                            results.Add(circle.UIElement);
                                        }
                                        break;
                                    case RebarBeamMainBarGroupType.GroupLevel2:
                                        qty = section.RebarBeamTop.RebarBeamTopLevel2.Quantity;
                                        distance = p1.DistanceTo(p2);
                                        spacing = distance / (qty - 1);
                                        for (int i = 0; i < qty; i++)
                                        {
                                            var pp = p1.Translate(new System.Windows.Point(spacing * i, 0));
                                            circle = new InstanceInCanvasCircel(canvasPageBase, option, centerCanvas, diameterMm, pp, new System.Windows.Point(0, 0), "");
                                            circle.DrawInCanvas();
                                            results.Add(circle.UIElement);
                                        }
                                        break;
                                    case RebarBeamMainBarGroupType.GroupLevel3:
                                        qty = section.RebarBeamTop.RebarBeamTopLevel3.Quantity;
                                        distance = p1.DistanceTo(p2);
                                        spacing = distance / (qty - 1);
                                        for (int i = 0; i < qty; i++)
                                        {
                                            var pp = p1.Translate(new System.Windows.Point(spacing * i, 0));
                                            circle = new InstanceInCanvasCircel(canvasPageBase, option, centerCanvas, diameterMm, pp, new System.Windows.Point(0, 0), "");
                                            circle.DrawInCanvas();
                                            results.Add(circle.UIElement);
                                        }
                                        break;
                                }
                                break;
                            case RebarBeamMainBarLevelType.RebarBot:
                                switch (rebarBeamMainBarGroupType)
                                {
                                    case RebarBeamMainBarGroupType.GroupLevel1:
                                        qty = section.RebarBeamBot.RebarBeamBotLevel1.Quantity;
                                        distance = p4.DistanceTo(p3);
                                        spacing = distance / (qty - 1);
                                        for (int i = 0; i < qty; i++)
                                        {
                                            var pp = p4.Translate(new System.Windows.Point(spacing * i, 0));
                                            circle = new InstanceInCanvasCircel(canvasPageBase, option, centerCanvas, diameterMm, pp, new System.Windows.Point(0, 0), "");
                                            circle.DrawInCanvas();
                                            results.Add(circle.UIElement);
                                        }
                                        break;
                                    case RebarBeamMainBarGroupType.GroupLevel2:
                                        qty = section.RebarBeamBot.RebarBeamBotLevel2.Quantity;
                                        distance = p4.DistanceTo(p3);
                                        spacing = distance / (qty - 1);
                                        for (int i = 0; i < qty; i++)
                                        {
                                            var pp = p4.Translate(new System.Windows.Point(spacing * i, 0));
                                            circle = new InstanceInCanvasCircel(canvasPageBase, option, centerCanvas, diameterMm, pp, new System.Windows.Point(0, 0), "");
                                            circle.DrawInCanvas();
                                            results.Add(circle.UIElement);
                                        }
                                        break;
                                    case RebarBeamMainBarGroupType.GroupLevel3:
                                        qty = section.RebarBeamBot.RebarBeamBotLevel3.Quantity;
                                        distance = p4.DistanceTo(p3);
                                        spacing = distance / (qty - 1);
                                        for (int i = 0; i < qty; i++)
                                        {
                                            var pp = p4.Translate(new System.Windows.Point(spacing * i, 0));
                                            circle = new InstanceInCanvasCircel(canvasPageBase, option, centerCanvas, diameterMm, pp, new System.Windows.Point(0, 0), "");
                                            circle.DrawInCanvas();
                                            results.Add(circle.UIElement);
                                        }
                                        break;
                                }
                                break;
                        }
                        break;
                    case RebarBeamSectionType.SectionMid:
                        section = installRebarBeamV2ViewModel.ElementInstances.RebarBeamActive.RebarBeamSectionMid;
                        switch (rebarBeamMainBarLevelType)
                        {
                            case RebarBeamMainBarLevelType.RebarTop:
                                switch (rebarBeamMainBarGroupType)
                                {
                                    case RebarBeamMainBarGroupType.GroupLevel1:
                                        qty = section.RebarBeamTop.RebarBeamTopLevel1.Quantity;
                                        distance = p1.DistanceTo(p2);
                                        spacing = distance / (qty - 1);
                                        for (int i = 0; i < qty; i++)
                                        {
                                            var pp = p1.Translate(new System.Windows.Point(spacing * i, 0));
                                            circle = new InstanceInCanvasCircel(canvasPageBase, option, centerCanvas, diameterMm, pp, new System.Windows.Point(0, 0), "");
                                            circle.DrawInCanvas();
                                            results.Add(circle.UIElement);
                                        }
                                        break;
                                    case RebarBeamMainBarGroupType.GroupLevel2:
                                        qty = section.RebarBeamTop.RebarBeamTopLevel2.Quantity;
                                        distance = p1.DistanceTo(p2);
                                        spacing = distance / (qty - 1);
                                        for (int i = 0; i < qty; i++)
                                        {
                                            var pp = p1.Translate(new System.Windows.Point(spacing * i, 0));
                                            circle = new InstanceInCanvasCircel(canvasPageBase, option, centerCanvas, diameterMm, pp, new System.Windows.Point(0, 0), "");
                                            circle.DrawInCanvas();
                                            results.Add(circle.UIElement);
                                        }
                                        break;
                                    case RebarBeamMainBarGroupType.GroupLevel3:
                                        qty = section.RebarBeamTop.RebarBeamTopLevel3.Quantity;
                                        distance = p1.DistanceTo(p2);
                                        spacing = distance / (qty - 1);
                                        for (int i = 0; i < qty; i++)
                                        {
                                            var pp = p1.Translate(new System.Windows.Point(spacing * i, 0));
                                            circle = new InstanceInCanvasCircel(canvasPageBase, option, centerCanvas, diameterMm, pp, new System.Windows.Point(0, 0), "");
                                            circle.DrawInCanvas();
                                            results.Add(circle.UIElement);
                                        }
                                        break;
                                }
                                break;
                            case RebarBeamMainBarLevelType.RebarBot:
                                switch (rebarBeamMainBarGroupType)
                                {
                                    case RebarBeamMainBarGroupType.GroupLevel1:
                                        qty = section.RebarBeamBot.RebarBeamBotLevel1.Quantity;
                                        distance = p4.DistanceTo(p3);
                                        spacing = distance / (qty - 1);
                                        for (int i = 0; i < qty; i++)
                                        {
                                            var pp = p4.Translate(new System.Windows.Point(spacing * i, 0));
                                            circle = new InstanceInCanvasCircel(canvasPageBase, option, centerCanvas, diameterMm, pp, new System.Windows.Point(0, 0), "");
                                            circle.DrawInCanvas();
                                            results.Add(circle.UIElement);
                                        }
                                        break;
                                    case RebarBeamMainBarGroupType.GroupLevel2:
                                        qty = section.RebarBeamBot.RebarBeamBotLevel2.Quantity;
                                        distance = p4.DistanceTo(p3);
                                        spacing = distance / (qty - 1);
                                        for (int i = 0; i < qty; i++)
                                        {
                                            var pp = p4.Translate(new System.Windows.Point(spacing * i, 0));
                                            circle = new InstanceInCanvasCircel(canvasPageBase, option, centerCanvas, diameterMm, pp, new System.Windows.Point(0, 0), "");
                                            circle.DrawInCanvas();
                                            results.Add(circle.UIElement);
                                        }
                                        break;
                                    case RebarBeamMainBarGroupType.GroupLevel3:
                                        qty = section.RebarBeamBot.RebarBeamBotLevel3.Quantity;
                                        distance = p4.DistanceTo(p3);
                                        spacing = distance / (qty - 1);
                                        for (int i = 0; i < qty; i++)
                                        {
                                            var pp = p4.Translate(new System.Windows.Point(spacing * i, 0));
                                            circle = new InstanceInCanvasCircel(canvasPageBase, option, centerCanvas, diameterMm, pp, new System.Windows.Point(0, 0), "");
                                            circle.DrawInCanvas();
                                            results.Add(circle.UIElement);
                                        }
                                        break;
                                }
                                break;
                        }
                        break;
                    case RebarBeamSectionType.SectionEnd:
                        section = installRebarBeamV2ViewModel.ElementInstances.RebarBeamActive.RebarBeamSectionEnd;
                        switch (rebarBeamMainBarLevelType)
                        {
                            case RebarBeamMainBarLevelType.RebarTop:
                                switch (rebarBeamMainBarGroupType)
                                {
                                    case RebarBeamMainBarGroupType.GroupLevel1:
                                        qty = section.RebarBeamTop.RebarBeamTopLevel1.Quantity;
                                        distance = p1.DistanceTo(p2);
                                        spacing = distance / (qty - 1);
                                        for (int i = 0; i < qty; i++)
                                        {
                                            var pp = p1.Translate(new System.Windows.Point(spacing * i, 0));
                                            circle = new InstanceInCanvasCircel(canvasPageBase, option, centerCanvas, diameterMm, pp, new System.Windows.Point(0, 0), "");
                                            circle.DrawInCanvas();
                                            results.Add(circle.UIElement);
                                        }
                                        break;
                                    case RebarBeamMainBarGroupType.GroupLevel2:
                                        qty = section.RebarBeamTop.RebarBeamTopLevel2.Quantity;
                                        distance = p1.DistanceTo(p2);
                                        spacing = distance / (qty - 1);
                                        for (int i = 0; i < qty; i++)
                                        {
                                            var pp = p1.Translate(new System.Windows.Point(spacing * i, 0));
                                            circle = new InstanceInCanvasCircel(canvasPageBase, option, centerCanvas, diameterMm, pp, new System.Windows.Point(0, 0), "");
                                            circle.DrawInCanvas();
                                            results.Add(circle.UIElement);
                                        }
                                        break;
                                    case RebarBeamMainBarGroupType.GroupLevel3:
                                        qty = section.RebarBeamTop.RebarBeamTopLevel3.Quantity;
                                        distance = p1.DistanceTo(p2);
                                        spacing = distance / (qty - 1);
                                        for (int i = 0; i < qty; i++)
                                        {
                                            var pp = p1.Translate(new System.Windows.Point(spacing * i, 0));
                                            circle = new InstanceInCanvasCircel(canvasPageBase, option, centerCanvas, diameterMm, pp, new System.Windows.Point(0, 0), "");
                                            circle.DrawInCanvas();
                                            results.Add(circle.UIElement);
                                        }
                                        break;
                                }
                                break;
                            case RebarBeamMainBarLevelType.RebarBot:
                                switch (rebarBeamMainBarGroupType)
                                {
                                    case RebarBeamMainBarGroupType.GroupLevel1:
                                        qty = section.RebarBeamBot.RebarBeamBotLevel1.Quantity;
                                        distance = p4.DistanceTo(p3);
                                        spacing = distance / (qty - 1);
                                        for (int i = 0; i < qty; i++)
                                        {
                                            var pp = p4.Translate(new System.Windows.Point(spacing * i, 0));
                                            circle = new InstanceInCanvasCircel(canvasPageBase, option, centerCanvas, diameterMm, pp, new System.Windows.Point(0, 0), "");
                                            circle.DrawInCanvas();
                                            results.Add(circle.UIElement);
                                        }
                                        break;
                                    case RebarBeamMainBarGroupType.GroupLevel2:
                                        qty = section.RebarBeamBot.RebarBeamBotLevel2.Quantity;
                                        distance = p4.DistanceTo(p3);
                                        spacing = distance / (qty - 1);
                                        for (int i = 0; i < qty; i++)
                                        {
                                            var pp = p4.Translate(new System.Windows.Point(spacing * i, 0));
                                            circle = new InstanceInCanvasCircel(canvasPageBase, option, centerCanvas, diameterMm, pp, new System.Windows.Point(0, 0), "");
                                            circle.DrawInCanvas();
                                            results.Add(circle.UIElement);
                                        }
                                        break;
                                    case RebarBeamMainBarGroupType.GroupLevel3:
                                        qty = section.RebarBeamBot.RebarBeamBotLevel3.Quantity;
                                        distance = p4.DistanceTo(p3);
                                        spacing = distance / (qty - 1);
                                        for (int i = 0; i < qty; i++)
                                        {
                                            var pp = p4.Translate(new System.Windows.Point(spacing * i, 0));
                                            circle = new InstanceInCanvasCircel(canvasPageBase, option, centerCanvas, diameterMm, pp, new System.Windows.Point(0, 0), "");
                                            circle.DrawInCanvas();
                                            results.Add(circle.UIElement);
                                        }
                                        break;
                                }
                                break;
                        }
                        break;
                }
            }
            catch (Exception)
            {
            }
            return results;
        }
        private void _getCover(
            RebarBeam rebarBeam,
            InstallRebarBeamV2ViewModel installRebarBeamV2ViewModel,
            double coverMm,
            CanvasPageBase canvasPageBase,
            RebarBeamSectionType sectionType,
            RebarBeamMainBarLevelType rebarBeamMainBarLevelType,
            RebarBeamMainBarGroupType rebarBeamMainBarGroupType,
            out double coverUpMm,
            out double coverSideMm)
        {
            coverUpMm = coverMm;
            coverSideMm = coverMm;
            var rebarBarTypeCustoms = installRebarBeamV2ViewModel.ElementInstances.RebarBarTypeCustoms;
            var distanceRToRMm = installRebarBeamV2ViewModel.ElementInstances.DistanceRebarToRebarMm;
            switch (sectionType)
            {
                case RebarBeamSectionType.SectionStart:
                    _getCoverSub(
                        rebarBeam,
                        installRebarBeamV2ViewModel,
                        coverMm,
                        canvasPageBase,
                        installRebarBeamV2ViewModel.ElementInstances.RebarBeamActive.RebarBeamSectionStart,
                        rebarBeamMainBarLevelType,
                        rebarBeamMainBarGroupType,
                        out double start_coverUpMm,
                        out double start_coverSideMm);
                    coverUpMm = start_coverUpMm;
                    coverSideMm = start_coverSideMm;
                    break;
                case RebarBeamSectionType.SectionMid:
                    _getCoverSub(
                        rebarBeam,
                        installRebarBeamV2ViewModel,
                        coverMm,
                        canvasPageBase,
                        installRebarBeamV2ViewModel.ElementInstances.RebarBeamActive.RebarBeamSectionMid,
                        rebarBeamMainBarLevelType,
                        rebarBeamMainBarGroupType,
                        out double mid_coverUpMm,
                        out double mid_coverSideMm);
                    coverUpMm = mid_coverUpMm;
                    coverSideMm = mid_coverSideMm;
                    break;
                case RebarBeamSectionType.SectionEnd:
                    _getCoverSub(
                        rebarBeam,
                        installRebarBeamV2ViewModel,
                        coverMm,
                        canvasPageBase,
                        installRebarBeamV2ViewModel.ElementInstances.RebarBeamActive.RebarBeamSectionMid,
                        rebarBeamMainBarLevelType,
                        rebarBeamMainBarGroupType,
                        out double end_coverUpMm,
                        out double end_coverSideMm);
                    coverUpMm = end_coverUpMm;
                    coverSideMm = end_coverSideMm;
                    break;
            }
        }
        private void _getCoverSub(
            RebarBeam rebarBeam,
            InstallRebarBeamV2ViewModel installRebarBeamV2ViewModel,
            double coverMm,
            CanvasPageBase canvasPageBase,
            RebarBeamSection rebarBeamSection,
            RebarBeamMainBarLevelType rebarBeamMainBarLevelType,
            RebarBeamMainBarGroupType rebarBeamMainBarGroupType,
            out double coverUpMm,
            out double coverSideMm)
        {
            coverUpMm = coverMm;
            coverSideMm = coverMm;
            var rebarBarTypeCustoms = installRebarBeamV2ViewModel.ElementInstances.RebarBarTypeCustoms;
            var distanceRToRMm = installRebarBeamV2ViewModel.ElementInstances.DistanceRebarToRebarMm;
            RebarBarTypeCustom rebarbarTypeStp = null;
            RebarBarTypeCustom rebarbarTypeMainBar1 = null;
            RebarBarTypeCustom rebarbarTypeMainBar2 = null;
            RebarBarTypeCustom rebarbarTypeMainBar3 = null;

            switch (rebarBeamMainBarLevelType)
            {
                case RebarBeamMainBarLevelType.RebarTop:
                    switch (rebarBeamMainBarGroupType)
                    {
                        case RebarBeamMainBarGroupType.GroupLevel1:
                            rebarbarTypeStp = RebarBarTypeCustomUtils
                                .GetRebarBarTypeCustom(rebarBeamSection.RebarBeamStirrup.Diameter, rebarBarTypeCustoms);
                            rebarbarTypeMainBar1 = RebarBarTypeCustomUtils
                                .GetRebarBarTypeCustom(rebarBeamSection.RebarBeamTop.RebarBeamTopLevel1.Diameter, rebarBarTypeCustoms);
                            coverUpMm += rebarbarTypeStp == null ? 0 : rebarbarTypeStp.ModelBarDiameter.FootToMm();
                            coverUpMm += rebarbarTypeMainBar1 == null ? 0 : rebarbarTypeMainBar1.ModelBarDiameter.FootToMm() / 2;
                            coverSideMm += rebarbarTypeStp == null ? 0 : rebarbarTypeStp.ModelBarDiameter.FootToMm();
                            coverSideMm += rebarbarTypeMainBar1 == null ? 0 : rebarbarTypeMainBar1.ModelBarDiameter.FootToMm() / 2;
                            break;
                        case RebarBeamMainBarGroupType.GroupLevel2:
                            rebarbarTypeStp = RebarBarTypeCustomUtils
                                .GetRebarBarTypeCustom(rebarBeamSection.RebarBeamStirrup.Diameter, rebarBarTypeCustoms);
                            rebarbarTypeMainBar1 = RebarBarTypeCustomUtils
                                .GetRebarBarTypeCustom(rebarBeamSection.RebarBeamTop.RebarBeamTopLevel1.Diameter, rebarBarTypeCustoms);
                            rebarbarTypeMainBar2 = RebarBarTypeCustomUtils
                                .GetRebarBarTypeCustom(rebarBeamSection.RebarBeamTop.RebarBeamTopLevel2.Diameter, rebarBarTypeCustoms);
                            coverUpMm += rebarbarTypeStp == null ? 0 : rebarbarTypeStp.ModelBarDiameter.FootToMm();
                            coverUpMm += rebarbarTypeMainBar1 == null ? 0 : rebarbarTypeMainBar1.ModelBarDiameter.FootToMm() + distanceRToRMm;
                            coverUpMm += rebarbarTypeMainBar2 == null ? 0 : rebarbarTypeMainBar2.ModelBarDiameter.FootToMm() / 2;

                            coverSideMm += rebarbarTypeStp == null ? 0 : rebarbarTypeStp.ModelBarDiameter.FootToMm();
                            coverSideMm += rebarbarTypeMainBar1 == null ? 0 : rebarbarTypeMainBar1.ModelBarDiameter.FootToMm() / 2;
                            break;
                        case RebarBeamMainBarGroupType.GroupLevel3:
                            rebarbarTypeStp = RebarBarTypeCustomUtils
                                .GetRebarBarTypeCustom(rebarBeamSection.RebarBeamStirrup.Diameter, rebarBarTypeCustoms);
                            rebarbarTypeMainBar1 = RebarBarTypeCustomUtils
                                .GetRebarBarTypeCustom(rebarBeamSection.RebarBeamTop.RebarBeamTopLevel1.Diameter, rebarBarTypeCustoms);
                            rebarbarTypeMainBar2 = RebarBarTypeCustomUtils
                                .GetRebarBarTypeCustom(rebarBeamSection.RebarBeamTop.RebarBeamTopLevel2.Diameter, rebarBarTypeCustoms);
                            rebarbarTypeMainBar3 = RebarBarTypeCustomUtils
                                .GetRebarBarTypeCustom(rebarBeamSection.RebarBeamTop.RebarBeamTopLevel3.Diameter, rebarBarTypeCustoms);
                            coverUpMm += rebarbarTypeStp == null ? 0 : rebarbarTypeStp.ModelBarDiameter.FootToMm();
                            coverUpMm += rebarbarTypeMainBar1 == null ? 0 : rebarbarTypeMainBar1.ModelBarDiameter.FootToMm() + distanceRToRMm;
                            coverUpMm += rebarbarTypeMainBar2 == null ? 0 : rebarbarTypeMainBar2.ModelBarDiameter.FootToMm() + distanceRToRMm;
                            coverUpMm += rebarbarTypeMainBar3 == null ? 0 : rebarbarTypeMainBar3.ModelBarDiameter.FootToMm() / 2;

                            coverSideMm += rebarbarTypeStp == null ? 0 : rebarbarTypeStp.ModelBarDiameter.FootToMm();
                            coverSideMm += rebarbarTypeMainBar1 == null ? 0 : rebarbarTypeMainBar1.ModelBarDiameter.FootToMm() / 2;
                            break;
                    }
                    break;
                case RebarBeamMainBarLevelType.RebarBot:
                    switch (rebarBeamMainBarGroupType)
                    {
                        case RebarBeamMainBarGroupType.GroupLevel1:
                            rebarbarTypeStp = RebarBarTypeCustomUtils
                                .GetRebarBarTypeCustom(rebarBeamSection.RebarBeamStirrup.Diameter, rebarBarTypeCustoms);
                            rebarbarTypeMainBar1 = RebarBarTypeCustomUtils
                                .GetRebarBarTypeCustom(rebarBeamSection.RebarBeamBot.RebarBeamBotLevel1.Diameter, rebarBarTypeCustoms);
                            coverUpMm += rebarbarTypeStp == null ? 0 : rebarbarTypeStp.ModelBarDiameter.FootToMm();
                            coverUpMm += rebarbarTypeMainBar1 == null ? 0 : rebarbarTypeMainBar1.ModelBarDiameter.FootToMm() / 2;
                            coverSideMm += rebarbarTypeStp == null ? 0 : rebarbarTypeStp.ModelBarDiameter.FootToMm();
                            coverSideMm += rebarbarTypeMainBar1 == null ? 0 : rebarbarTypeMainBar1.ModelBarDiameter.FootToMm() / 2;
                            break;
                        case RebarBeamMainBarGroupType.GroupLevel2:
                            rebarbarTypeStp = RebarBarTypeCustomUtils
                                .GetRebarBarTypeCustom(rebarBeamSection.RebarBeamStirrup.Diameter, rebarBarTypeCustoms);
                            rebarbarTypeMainBar1 = RebarBarTypeCustomUtils
                                .GetRebarBarTypeCustom(rebarBeamSection.RebarBeamBot.RebarBeamBotLevel1.Diameter, rebarBarTypeCustoms);
                            rebarbarTypeMainBar2 = RebarBarTypeCustomUtils
                                .GetRebarBarTypeCustom(rebarBeamSection.RebarBeamBot.RebarBeamBotLevel2.Diameter, rebarBarTypeCustoms);
                            coverUpMm += rebarbarTypeStp == null ? 0 : rebarbarTypeStp.ModelBarDiameter.FootToMm();
                            coverUpMm += rebarbarTypeMainBar1 == null ? 0 : rebarbarTypeMainBar1.ModelBarDiameter.FootToMm() + distanceRToRMm;
                            coverUpMm += rebarbarTypeMainBar2 == null ? 0 : rebarbarTypeMainBar2.ModelBarDiameter.FootToMm() / 2;

                            coverSideMm += rebarbarTypeStp == null ? 0 : rebarbarTypeStp.ModelBarDiameter.FootToMm();
                            coverSideMm += rebarbarTypeMainBar1 == null ? 0 : rebarbarTypeMainBar1.ModelBarDiameter.FootToMm() / 2;
                            break;
                        case RebarBeamMainBarGroupType.GroupLevel3:
                            rebarbarTypeStp = RebarBarTypeCustomUtils
                                .GetRebarBarTypeCustom(rebarBeamSection.RebarBeamStirrup.Diameter, rebarBarTypeCustoms);
                            rebarbarTypeMainBar1 = RebarBarTypeCustomUtils
                                .GetRebarBarTypeCustom(rebarBeamSection.RebarBeamBot.RebarBeamBotLevel1.Diameter, rebarBarTypeCustoms);
                            rebarbarTypeMainBar2 = RebarBarTypeCustomUtils
                                .GetRebarBarTypeCustom(rebarBeamSection.RebarBeamBot.RebarBeamBotLevel2.Diameter, rebarBarTypeCustoms);
                            rebarbarTypeMainBar3 = RebarBarTypeCustomUtils
                                .GetRebarBarTypeCustom(rebarBeamSection.RebarBeamBot.RebarBeamBotLevel2.Diameter, rebarBarTypeCustoms);
                            coverUpMm += rebarbarTypeStp == null ? 0 : rebarbarTypeStp.ModelBarDiameter.FootToMm();
                            coverUpMm += rebarbarTypeMainBar1 == null ? 0 : rebarbarTypeMainBar1.ModelBarDiameter.FootToMm() + distanceRToRMm;
                            coverUpMm += rebarbarTypeMainBar2 == null ? 0 : rebarbarTypeMainBar2.ModelBarDiameter.FootToMm() + distanceRToRMm;
                            coverUpMm += rebarbarTypeMainBar3 == null ? 0 : rebarbarTypeMainBar3.ModelBarDiameter.FootToMm() / 2;

                            coverSideMm += rebarbarTypeStp == null ? 0 : rebarbarTypeStp.ModelBarDiameter.FootToMm();
                            coverSideMm += rebarbarTypeMainBar1 == null ? 0 : rebarbarTypeMainBar1.ModelBarDiameter.FootToMm() / 2;
                            break;
                    }
                    break;
            }
        }
        private void _getDiameter(
            RebarBeam rebarBeam,
            InstallRebarBeamV2ViewModel installRebarBeamV2ViewModel,
            double coverMm,
            CanvasPageBase canvasPageBase,
            RebarBeamSectionType sectionType,
            RebarBeamMainBarLevelType rebarBeamMainBarLevelType,
            RebarBeamMainBarGroupType rebarBeamMainBarGroupType,
            out double diameterMm)
        {
            diameterMm = 0;
            var rebarBarTypeCustoms = installRebarBeamV2ViewModel.ElementInstances.RebarBarTypeCustoms;
            RebarBarTypeCustom rebarBarTypeCustom = null;
            switch (sectionType)
            {
                case RebarBeamSectionType.SectionStart:
                    switch (rebarBeamMainBarLevelType)
                    {
                        case RebarBeamMainBarLevelType.RebarTop:
                            switch (rebarBeamMainBarGroupType)
                            {
                                case RebarBeamMainBarGroupType.GroupLevel1:
                                    rebarBarTypeCustom = RebarBarTypeCustomUtils
                                        .GetRebarBarTypeCustom(
                                        installRebarBeamV2ViewModel.ElementInstances.RebarBeamActive.RebarBeamSectionStart.RebarBeamTop.RebarBeamTopLevel1.Diameter, rebarBarTypeCustoms);
                                    diameterMm = rebarBarTypeCustom.ModelBarDiameter.FootToMm();
                                    break;
                                case RebarBeamMainBarGroupType.GroupLevel2:
                                    rebarBarTypeCustom = RebarBarTypeCustomUtils
                                        .GetRebarBarTypeCustom(
                                        installRebarBeamV2ViewModel.ElementInstances.RebarBeamActive.RebarBeamSectionStart.RebarBeamTop.RebarBeamTopLevel2.Diameter, rebarBarTypeCustoms);
                                    diameterMm = rebarBarTypeCustom.ModelBarDiameter.FootToMm();
                                    break;
                                case RebarBeamMainBarGroupType.GroupLevel3:
                                    rebarBarTypeCustom = RebarBarTypeCustomUtils
                                        .GetRebarBarTypeCustom(
                                        installRebarBeamV2ViewModel.ElementInstances.RebarBeamActive.RebarBeamSectionStart.RebarBeamTop.RebarBeamTopLevel3.Diameter, rebarBarTypeCustoms);
                                    diameterMm = rebarBarTypeCustom.ModelBarDiameter.FootToMm();
                                    break;
                            }
                            break;
                        case RebarBeamMainBarLevelType.RebarBot:
                            switch (rebarBeamMainBarGroupType)
                            {
                                case RebarBeamMainBarGroupType.GroupLevel1:
                                    rebarBarTypeCustom = RebarBarTypeCustomUtils
                                        .GetRebarBarTypeCustom(
                                        installRebarBeamV2ViewModel.ElementInstances.RebarBeamActive.RebarBeamSectionStart.RebarBeamBot.RebarBeamBotLevel1.Diameter, rebarBarTypeCustoms);
                                    diameterMm = rebarBarTypeCustom.ModelBarDiameter.FootToMm();
                                    break;
                                case RebarBeamMainBarGroupType.GroupLevel2:
                                    rebarBarTypeCustom = RebarBarTypeCustomUtils
                                        .GetRebarBarTypeCustom(
                                        installRebarBeamV2ViewModel.ElementInstances.RebarBeamActive.RebarBeamSectionStart.RebarBeamBot.RebarBeamBotLevel2.Diameter, rebarBarTypeCustoms);
                                    diameterMm = rebarBarTypeCustom.ModelBarDiameter.FootToMm();
                                    break;
                                case RebarBeamMainBarGroupType.GroupLevel3:
                                    rebarBarTypeCustom = RebarBarTypeCustomUtils
                                        .GetRebarBarTypeCustom(
                                        installRebarBeamV2ViewModel.ElementInstances.RebarBeamActive.RebarBeamSectionStart.RebarBeamBot.RebarBeamBotLevel3.Diameter, rebarBarTypeCustoms);
                                    diameterMm = rebarBarTypeCustom.ModelBarDiameter.FootToMm();
                                    break;
                            }
                            break;
                    }
                    break;
                case RebarBeamSectionType.SectionMid:
                    switch (rebarBeamMainBarLevelType)
                    {
                        case RebarBeamMainBarLevelType.RebarTop:
                            switch (rebarBeamMainBarGroupType)
                            {
                                case RebarBeamMainBarGroupType.GroupLevel1:
                                    rebarBarTypeCustom = RebarBarTypeCustomUtils
                                        .GetRebarBarTypeCustom(
                                        installRebarBeamV2ViewModel.ElementInstances.RebarBeamActive.RebarBeamSectionMid.RebarBeamTop.RebarBeamTopLevel1.Diameter, rebarBarTypeCustoms);
                                    diameterMm = rebarBarTypeCustom.ModelBarDiameter.FootToMm();
                                    break;
                                case RebarBeamMainBarGroupType.GroupLevel2:
                                    rebarBarTypeCustom = RebarBarTypeCustomUtils
                                        .GetRebarBarTypeCustom(
                                        installRebarBeamV2ViewModel.ElementInstances.RebarBeamActive.RebarBeamSectionMid.RebarBeamTop.RebarBeamTopLevel2.Diameter, rebarBarTypeCustoms);
                                    diameterMm = rebarBarTypeCustom.ModelBarDiameter.FootToMm();
                                    break;
                                case RebarBeamMainBarGroupType.GroupLevel3:
                                    rebarBarTypeCustom = RebarBarTypeCustomUtils
                                        .GetRebarBarTypeCustom(
                                        installRebarBeamV2ViewModel.ElementInstances.RebarBeamActive.RebarBeamSectionMid.RebarBeamTop.RebarBeamTopLevel3.Diameter, rebarBarTypeCustoms);
                                    diameterMm = rebarBarTypeCustom.ModelBarDiameter.FootToMm();
                                    break;
                            }
                            break;
                        case RebarBeamMainBarLevelType.RebarBot:
                            switch (rebarBeamMainBarGroupType)
                            {
                                case RebarBeamMainBarGroupType.GroupLevel1:
                                    rebarBarTypeCustom = RebarBarTypeCustomUtils
                                        .GetRebarBarTypeCustom(
                                        installRebarBeamV2ViewModel.ElementInstances.RebarBeamActive.RebarBeamSectionMid.RebarBeamBot.RebarBeamBotLevel1.Diameter, rebarBarTypeCustoms);
                                    diameterMm = rebarBarTypeCustom.ModelBarDiameter.FootToMm();
                                    break;
                                case RebarBeamMainBarGroupType.GroupLevel2:
                                    rebarBarTypeCustom = RebarBarTypeCustomUtils
                                        .GetRebarBarTypeCustom(
                                        installRebarBeamV2ViewModel.ElementInstances.RebarBeamActive.RebarBeamSectionMid.RebarBeamBot.RebarBeamBotLevel2.Diameter, rebarBarTypeCustoms);
                                    diameterMm = rebarBarTypeCustom.ModelBarDiameter.FootToMm();
                                    break;
                                case RebarBeamMainBarGroupType.GroupLevel3:
                                    rebarBarTypeCustom = RebarBarTypeCustomUtils
                                        .GetRebarBarTypeCustom(
                                        installRebarBeamV2ViewModel.ElementInstances.RebarBeamActive.RebarBeamSectionMid.RebarBeamBot.RebarBeamBotLevel3.Diameter, rebarBarTypeCustoms);
                                    diameterMm = rebarBarTypeCustom.ModelBarDiameter.FootToMm();
                                    break;
                            }
                            break;
                    }
                    break;
                case RebarBeamSectionType.SectionEnd:
                    switch (rebarBeamMainBarLevelType)
                    {
                        case RebarBeamMainBarLevelType.RebarTop:
                            switch (rebarBeamMainBarGroupType)
                            {
                                case RebarBeamMainBarGroupType.GroupLevel1:
                                    rebarBarTypeCustom = RebarBarTypeCustomUtils
                                        .GetRebarBarTypeCustom(
                                        installRebarBeamV2ViewModel.ElementInstances.RebarBeamActive.RebarBeamSectionEnd.RebarBeamTop.RebarBeamTopLevel1.Diameter, rebarBarTypeCustoms);
                                    diameterMm = rebarBarTypeCustom.ModelBarDiameter.FootToMm();
                                    break;
                                case RebarBeamMainBarGroupType.GroupLevel2:
                                    rebarBarTypeCustom = RebarBarTypeCustomUtils
                                        .GetRebarBarTypeCustom(
                                        installRebarBeamV2ViewModel.ElementInstances.RebarBeamActive.RebarBeamSectionEnd.RebarBeamTop.RebarBeamTopLevel2.Diameter, rebarBarTypeCustoms);
                                    diameterMm = rebarBarTypeCustom.ModelBarDiameter.FootToMm();
                                    break;
                                case RebarBeamMainBarGroupType.GroupLevel3:
                                    rebarBarTypeCustom = RebarBarTypeCustomUtils
                                        .GetRebarBarTypeCustom(
                                        installRebarBeamV2ViewModel.ElementInstances.RebarBeamActive.RebarBeamSectionEnd.RebarBeamTop.RebarBeamTopLevel3.Diameter, rebarBarTypeCustoms);
                                    diameterMm = rebarBarTypeCustom.ModelBarDiameter.FootToMm();
                                    break;
                            }
                            break;
                        case RebarBeamMainBarLevelType.RebarBot:
                            switch (rebarBeamMainBarGroupType)
                            {
                                case RebarBeamMainBarGroupType.GroupLevel1:
                                    rebarBarTypeCustom = RebarBarTypeCustomUtils
                                        .GetRebarBarTypeCustom(
                                        installRebarBeamV2ViewModel.ElementInstances.RebarBeamActive.RebarBeamSectionEnd.RebarBeamBot.RebarBeamBotLevel1.Diameter, rebarBarTypeCustoms);
                                    diameterMm = rebarBarTypeCustom.ModelBarDiameter.FootToMm();
                                    break;
                                case RebarBeamMainBarGroupType.GroupLevel2:
                                    rebarBarTypeCustom = RebarBarTypeCustomUtils
                                        .GetRebarBarTypeCustom(
                                        installRebarBeamV2ViewModel.ElementInstances.RebarBeamActive.RebarBeamSectionEnd.RebarBeamBot.RebarBeamBotLevel2.Diameter, rebarBarTypeCustoms);
                                    diameterMm = rebarBarTypeCustom.ModelBarDiameter.FootToMm();
                                    break;
                                case RebarBeamMainBarGroupType.GroupLevel3:
                                    rebarBarTypeCustom = RebarBarTypeCustomUtils
                                        .GetRebarBarTypeCustom(
                                        installRebarBeamV2ViewModel.ElementInstances.RebarBeamActive.RebarBeamSectionEnd.RebarBeamBot.RebarBeamBotLevel3.Diameter, rebarBarTypeCustoms);
                                    diameterMm = rebarBarTypeCustom.ModelBarDiameter.FootToMm();
                                    break;
                            }
                            break;
                    }
                    break;
            }
        }
        private void _drawSectionBeamStirrup(RebarBeam rebarBeam, double coverMm, CanvasPageBase canvasPageBase)
        {
            var centerCanvas = canvasPageBase.Center;
            var scale = canvasPageBase.RatioScale * canvasPageBase.DistanceCrossScreen / Math.Sqrt(rebarBeam.BeamWidthMm * rebarBeam.BeamWidthMm + rebarBeam.BeamHeightMm * rebarBeam.BeamHeightMm);
            var option = OptionStyleInstanceInCanvas.OPTION_REBAR_LINE;
            var p1 = new System.Windows.Point(centerCanvas.X - scale * (rebarBeam.BeamWidthMm / 2 - coverMm), centerCanvas.Y - scale * (rebarBeam.BeamHeightMm / 2 - coverMm));
            var p2 = new System.Windows.Point(centerCanvas.X + scale * (rebarBeam.BeamWidthMm / 2 - coverMm), centerCanvas.Y - scale * (rebarBeam.BeamHeightMm / 2 - coverMm));
            var p3 = new System.Windows.Point(centerCanvas.X + scale * (rebarBeam.BeamWidthMm / 2 - coverMm), centerCanvas.Y + scale * (rebarBeam.BeamHeightMm / 2 - coverMm));
            var p4 = new System.Windows.Point(centerCanvas.X - scale * (rebarBeam.BeamWidthMm / 2 - coverMm), centerCanvas.Y + scale * (rebarBeam.BeamHeightMm / 2 - coverMm));
            var ps = new List<System.Windows.Point>() {
                p1,p2, p3,p4
            };
            var stirrup = new InstanceInCanvasPolygon(canvasPageBase, option, ps);
            stirrup.DrawInCanvas();
        }
        private void _drawSectionBeamConcrete(RebarBeam rebarBeam, CanvasPageBase canvasPageBase)
        {
            var centerCanvas = canvasPageBase.Center;
            var scale = canvasPageBase.RatioScale * canvasPageBase.DistanceCrossScreen / Math.Sqrt(rebarBeam.BeamWidthMm * rebarBeam.BeamWidthMm + rebarBeam.BeamHeightMm * rebarBeam.BeamHeightMm);
            var option = OptionStyleInstanceInCanvas.OPTION_CONCRETE_STRUCTURE;

            var p1 = new System.Windows.Point(centerCanvas.X - scale * rebarBeam.BeamWidthMm / 2, centerCanvas.Y - scale * rebarBeam.BeamHeightMm / 2);
            var p2 = new System.Windows.Point(centerCanvas.X + scale * rebarBeam.BeamWidthMm / 2, centerCanvas.Y - scale * rebarBeam.BeamHeightMm / 2);
            var p3 = new System.Windows.Point(centerCanvas.X + scale * rebarBeam.BeamWidthMm / 2, centerCanvas.Y + scale * rebarBeam.BeamHeightMm / 2);
            var p4 = new System.Windows.Point(centerCanvas.X - scale * rebarBeam.BeamWidthMm / 2, centerCanvas.Y + scale * rebarBeam.BeamHeightMm / 2);
            var ps = new List<System.Windows.Point>() {
                p1,p2, p3,p4
            };
            var sectionBeam = new InstanceInCanvasPolygon(canvasPageBase, option, ps);
            sectionBeam.DrawInCanvas();
        }
    }
}
