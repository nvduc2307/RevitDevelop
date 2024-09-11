using HcBimUtils;
using HcBimUtils.GeometryUtils.Geometry;
using Utils.canvass;
using Utils.CompareElement;
using Utils.Geometries;

namespace Tools.InstallRebarBeam.model
{
    public class BeamInfo : ObservableObject
    {
        private int _beamSectionSelected;
        private BeamSection _beamSectionActive;
        public FamilyInstance Parent { get; set; }
        public Solid SolidOriginal { get; set; }
        public double HeightMm { get; set; }
        public double WidthMm { get; set; }
        public double LengthMm { get; set; }
        public XYZ VtX { get; set; }
        public XYZ VtY { get; set; }
        public XYZ VtZ { get; set; }
        public XYZ MidBeam { get; set; }
        public int BeamSectionSelected
        {
            get => _beamSectionSelected;
            set
            {
                _beamSectionSelected = value;
                OnPropertyChanged();
                UpdateBeamSectionActive();
            }
        }
        public BeamSection BeamSectionActive
        {
            get => _beamSectionActive;
            set
            {
                _beamSectionActive = value;
                OnPropertyChanged();
                if (UIElement.CanvasBaseSection != null)
                {
                    DrawRebarMainInCanvas(UIElement.CanvasBaseSection, this);
                }
            }
        }
        public StartBeamSection StartBeamSection { get; set; }
        public MiddleBeamSection MiddleBeamSection { get; set; }
        public EndBeamSection EndBeamSection { get; set; }
        public BeamSetting BeamSetting { get; set; }
        public UIElement UIElement { get; set; }
        public List<System.Windows.Point> SectionBeamInCanvas { get; set; }

        public BeamInfo(FamilyInstance beam)
        {
            Parent = beam;
            SolidOriginal = beam.GetSolidOriginalFromFamilyInstance();
            VtX = GetVtX(out double length);
            VtY = VtX.CrossProduct(XYZ.BasisZ);
            VtZ = VtX.CrossProduct(VtY);
            LengthMm = Math.Round(length.FootToMm(), 0);
            GetWidthAndHeight(out double height, out double width, out XYZ midBeamResult);
            HeightMm = Math.Round(height.FootToMm(), 0);
            WidthMm = Math.Round(width.FootToMm(), 0);
            MidBeam = midBeamResult;
            SectionBeamInCanvas = new List<System.Windows.Point>();
        }
        public BeamInfo(FamilyInstance beam, UIElement uIElement)
        {
            UIElement = uIElement;
            Parent = beam;
            SolidOriginal = beam.GetSolidOriginalFromFamilyInstance();
            VtX = GetVtX(out double length);
            VtY = VtX.CrossProduct(XYZ.BasisZ);
            VtZ = VtX.CrossProduct(VtY);
            LengthMm = Math.Round(length.FootToMm(), 0);
            GetWidthAndHeight(out double height, out double width, out XYZ midBeamResult);
            HeightMm = Math.Round(height.FootToMm(), 0);
            WidthMm = Math.Round(width.FootToMm(), 0);
            MidBeam = midBeamResult;
            SectionBeamInCanvas = new List<System.Windows.Point>();
        }
        public BeamInfo()
        {

        }
        private XYZ GetVtX(out double length)
        {
            length = 0;
            XYZ result = null;
            try
            {
                var locationType = Parent.Location as LocationCurve;
                var location = locationType.Curve;
                var midBeam = location.Midpoint();
                var locationDir = location.Direction();
                var locationNor = locationDir.CrossProduct(XYZ.BasisZ);
                var faces = SolidOriginal.GetFacesFromSolid();
                var points = faces.Select(f => f.GetPoints())
                    .Aggregate((a, b) => a.Concat(b).ToList())
                    .Where(x => x != null)
                    .ToList();
                var pointsGrNormal = points.GroupBy(x => x.DotProduct(locationNor)).Select(x => x.ToList()).FirstOrDefault();
                var pointsGrNormalGr = pointsGrNormal.GroupBy(x => (midBeam - x).DotProduct(locationDir))
                    .Select(x => x.OrderBy(x => x.Z).ToList())
                    .OrderBy(x => (midBeam - x.FirstOrDefault()).DotProduct(locationDir))
                    .ToList();
                var p1 = pointsGrNormalGr.FirstOrDefault().FirstOrDefault();
                var p2 = pointsGrNormalGr.FirstOrDefault().LastOrDefault();
                var p3 = pointsGrNormalGr.LastOrDefault().LastOrDefault();
                var p4 = pointsGrNormalGr.LastOrDefault().FirstOrDefault();

                var vt1 = (p4 - p1).Normalize();
                var vt2 = (p3 - p2).Normalize();

                result = vt1.IsParallel(vt2) && vt1.DotProduct(vt2).IsGreater(0)
                    ? vt1.DotProduct(locationDir).IsGreater(0) ? vt1 : -vt1
                    : locationDir;
                length = p1.DistanceTo(p4);
            }
            catch (Exception)
            {
            }
            return result;
        }
        private void GetWidthAndHeight(out double height, out double width, out XYZ midBeamResult)
        {
            midBeamResult = null;
            height = 0;
            width = 0;
            try
            {
                var locationType = Parent.Location as LocationCurve;
                var location = locationType.Curve;
                var midBeam = location.Midpoint();
                var planeSection = new FaceCustom(VtX, midBeam);
                var faces = SolidOriginal.GetFacesFromSolid();
                var points = faces.Select(f => f.GetPoints())
                    .Aggregate((a, b) => a.Concat(b).ToList())
                    .Where(x => x != null)
                    .Select(x => x.RayPointToFace(VtX, planeSection))
                    .Distinct(new ComparePoint())
                    .ToList();
                var pointsGrRight = points.Where(x => (midBeam - x).DotProduct(VtY) > 0).OrderBy(x => x.Z).ToList();
                var pointsGrLeft = points.Where(x => (midBeam - x).DotProduct(VtY) < 0).OrderBy(x => x.Z).ToList();

                var p1 = pointsGrRight.FirstOrDefault();
                var p2 = pointsGrRight.LastOrDefault();
                var p3 = pointsGrLeft.LastOrDefault();
                var p4 = pointsGrLeft.FirstOrDefault();
                height = p1.DistanceTo(p2);
                width = p1.DistanceTo(p4);
                midBeamResult = p1.MidPoint(p3);
            }
            catch (Exception)
            {
            }
        }
        private void UpdateBeamSectionActive()
        {
            BeamSectionActive = BeamSectionSelected == 0 ? StartBeamSection : BeamSectionSelected == 1 ? MiddleBeamSection : EndBeamSection;
            OnPropertyChanged(nameof(BeamSectionActive));
        }
        public static void DrawBeamInCanvas(CanvasPageBase canvasBase, BeamInfo beamInfo)
        {
            canvasBase.Scale = Math.Min(canvasBase.Height, canvasBase.Width) * 0.45 / Math.Max(beamInfo.WidthMm, beamInfo.HeightMm);
            var options = new OptionStyleInstanceInCanvas(
                StyleThicknessInCanvas.Thickness_2,
                StyleDashInCanvas.Style_Solid,
                StyleColorInCanvas.Color_Concrete,
                null);
            var p1 = canvasBase.Center
                - canvasBase.VTY * beamInfo.HeightMm * canvasBase.Scale
                - canvasBase.VTX * beamInfo.WidthMm * canvasBase.Scale;
            var p2 = canvasBase.Center
                - canvasBase.VTY * beamInfo.HeightMm * canvasBase.Scale
                + canvasBase.VTX * beamInfo.WidthMm * canvasBase.Scale;
            var p3 = canvasBase.Center
                + canvasBase.VTY * beamInfo.HeightMm * canvasBase.Scale
                + canvasBase.VTX * beamInfo.WidthMm * canvasBase.Scale;
            var p4 = canvasBase.Center
                + canvasBase.VTY * beamInfo.HeightMm * canvasBase.Scale
                - canvasBase.VTX * beamInfo.WidthMm * canvasBase.Scale;
            beamInfo.SectionBeamInCanvas.Add(p1);
            beamInfo.SectionBeamInCanvas.Add(p2);
            beamInfo.SectionBeamInCanvas.Add(p3);
            beamInfo.SectionBeamInCanvas.Add(p4);
            var beam = new InstanceInCanvasPolygon(canvasBase, options, beamInfo.SectionBeamInCanvas);
            beam.DrawInCanvas();
        }
        public static void DrawRebarStirrupInCanvas(CanvasPageBase canvasBase, BeamInfo beamInfo)
        {
            foreach (var item in beamInfo.UIElement.RebarStirrupMains)
            {
                canvasBase.Parent.Children.Remove(item);
            }
            var ratio = 2;
            var cover_top = beamInfo.BeamSetting.CoverTop * canvasBase.Scale * ratio;
            var cover_right = beamInfo.BeamSetting.CoverRight * canvasBase.Scale * ratio;
            var cover_bot = beamInfo.BeamSetting.CoverBot * canvasBase.Scale * ratio;
            var cover_left = beamInfo.BeamSetting.CoverLeft * canvasBase.Scale * ratio;

            var options = new OptionStyleInstanceInCanvas(
                StyleThicknessInCanvas.Thickness_1,
                StyleDashInCanvas.Style_Solid,
                StyleColorInCanvas.Color4,
                null);

            var p1 = beamInfo.SectionBeamInCanvas[0]
                + canvasBase.VTX * cover_left
                + canvasBase.VTY * cover_top;
            var p2 = beamInfo.SectionBeamInCanvas[1]
                - canvasBase.VTX * cover_right
                + canvasBase.VTY * cover_top;
            var p3 = beamInfo.SectionBeamInCanvas[2]
                - canvasBase.VTX * cover_right
                - canvasBase.VTY * cover_bot;
            var p4 = beamInfo.SectionBeamInCanvas[3]
                + canvasBase.VTX * cover_left
                - canvasBase.VTY * cover_bot;
            var ps = new List<System.Windows.Point>() { p1, p2, p3, p4 };
            var stirrupMain = new InstanceInCanvasPolygon(canvasBase, options, ps);
            stirrupMain.DrawInCanvas();
            beamInfo.UIElement.RebarStirrupMains.Add(stirrupMain.UIElement);
        }
        public static void DrawRebarMainInCanvas(CanvasPageBase canvasBase, BeamInfo beamInfo)
        {
            var diameter = 5;
            var ratio = 2;
            var cover_top = beamInfo.BeamSetting.CoverTop * canvasBase.Scale * ratio;
            var cover_right = beamInfo.BeamSetting.CoverRight * canvasBase.Scale * ratio;
            var cover_bot = beamInfo.BeamSetting.CoverBot * canvasBase.Scale * ratio;
            var cover_left = beamInfo.BeamSetting.CoverLeft * canvasBase.Scale * ratio;

            var options = new OptionStyleInstanceInCanvas(
                StyleThicknessInCanvas.Thickness_1,
                StyleDashInCanvas.Style_Solid,
                StyleColorInCanvas.Color4,
                StyleColorInCanvas.Color4);

            DrawRebarMain_Top_1(canvasBase, beamInfo);
            DrawRebarMain_Top_2(canvasBase, beamInfo);
            DrawRebarMain_Top_3(canvasBase, beamInfo);

            DrawRebarMain_Bot_1(canvasBase, beamInfo);
            DrawRebarMain_Bot_2(canvasBase, beamInfo);
            DrawRebarMain_Bot_3(canvasBase, beamInfo);

            DrawRebarSideBarInCanvas(canvasBase, beamInfo);

            void DrawRebarMain_Top(CanvasPageBase canvasBase, BeamInfo beamInfo, int level)
            {
                var grap = 10 * (level - 1);
                List<System.Windows.UIElement> uiElements = null;
                RebarClass rebarClass = null;
                switch (level)
                {
                    case 1:
                        uiElements = beamInfo.UIElement.RebarMainTopClass1;
                        rebarClass = beamInfo.BeamSectionActive.TopRebar.RebarClass1;
                        break;
                    case 2:
                        uiElements = beamInfo.UIElement.RebarMainTopClass2;
                        rebarClass = beamInfo.BeamSectionActive.TopRebar.RebarClass2;
                        break;
                    case 3:
                        uiElements = beamInfo.UIElement.RebarMainTopClass3;
                        rebarClass = beamInfo.BeamSectionActive.TopRebar.RebarClass3;
                        break;
                }
                foreach (var item in uiElements)
                {
                    canvasBase.Parent.Children.Remove(item);
                }
                var p_first = beamInfo.SectionBeamInCanvas[0]
                    + canvasBase.VTX * (cover_left + diameter)
                    + canvasBase.VTY * (cover_top + diameter + grap);
                var p_last = beamInfo.SectionBeamInCanvas[1]
                    - canvasBase.VTX * (cover_left + diameter)
                    + canvasBase.VTY * (cover_top + diameter + grap);
                var distance = p_first.DistanceTo(p_last);
                var spacing = distance / (rebarClass.Quantity - 1);

                for (int i = 0; i < rebarClass.Quantity; i++)
                {
                    var p_install = p_first + canvasBase.VTX * i * spacing;
                    var rebar = new InstanceInCanvasCircel(
                        canvasBase,
                        options,
                        canvasBase.Center,
                        diameter,
                        p_install
                        ,
                        new System.Windows.Point(-1, -1), "");
                    rebar.DrawInCanvas();
                    uiElements.Add(rebar.UIElement);
                }
            }
            void DrawRebarMain_Top_1(CanvasPageBase canvasBase, BeamInfo beamInfo)
            {
                DrawRebarMain_Top(canvasBase, beamInfo, 1);
            }
            void DrawRebarMain_Top_2(CanvasPageBase canvasBase, BeamInfo beamInfo)
            {
                DrawRebarMain_Top(canvasBase, beamInfo, 2);
            }
            void DrawRebarMain_Top_3(CanvasPageBase canvasBase, BeamInfo beamInfo)
            {
                DrawRebarMain_Top(canvasBase, beamInfo, 3);
            }

            void DrawRebarMain_Bot(CanvasPageBase canvasBase, BeamInfo beamInfo, int level)
            {
                var grap = 10 * (level - 1);
                List<System.Windows.UIElement> uiElements = null;
                RebarClass rebarClass = null;
                switch (level)
                {
                    case 1:
                        uiElements = beamInfo.UIElement.RebarMainBotClass1;
                        rebarClass = beamInfo.BeamSectionActive.BottomRebar.RebarClass1;
                        break;
                    case 2:
                        uiElements = beamInfo.UIElement.RebarMainBotClass2;
                        rebarClass = beamInfo.BeamSectionActive.BottomRebar.RebarClass2;
                        break;
                    case 3:
                        uiElements = beamInfo.UIElement.RebarMainBotClass3;
                        rebarClass = beamInfo.BeamSectionActive.BottomRebar.RebarClass3;
                        break;
                }
                foreach (var item in uiElements)
                {
                    canvasBase.Parent.Children.Remove(item);
                }
                var p_first = beamInfo.SectionBeamInCanvas[3]
                    + canvasBase.VTX * (cover_left + diameter)
                    - canvasBase.VTY * (cover_top + diameter + grap);
                var p_last = beamInfo.SectionBeamInCanvas[2]
                    - canvasBase.VTX * (cover_left + diameter)
                    - canvasBase.VTY * (cover_top + diameter + grap);
                var distance = p_first.DistanceTo(p_last);
                var spacing = distance / (rebarClass.Quantity - 1);

                for (int i = 0; i < rebarClass.Quantity; i++)
                {
                    var p_install = p_first + canvasBase.VTX * i * spacing;
                    var rebar = new InstanceInCanvasCircel(
                        canvasBase,
                        options,
                        canvasBase.Center,
                        diameter,
                        p_install
                        ,
                        new System.Windows.Point(-1, -1), "");
                    rebar.DrawInCanvas();
                    uiElements.Add(rebar.UIElement);
                }
            }
            void DrawRebarMain_Bot_1(CanvasPageBase canvasBase, BeamInfo beamInfo)
            {
                DrawRebarMain_Bot(canvasBase, beamInfo, 1);
            }
            void DrawRebarMain_Bot_2(CanvasPageBase canvasBase, BeamInfo beamInfo)
            {
                DrawRebarMain_Bot(canvasBase, beamInfo, 2);
            }
            void DrawRebarMain_Bot_3(CanvasPageBase canvasBase, BeamInfo beamInfo)
            {
                DrawRebarMain_Bot(canvasBase, beamInfo, 3);
            }
        }
        public static void DrawRebarSideBarInCanvas(CanvasPageBase canvasBase, BeamInfo beamInfo)
        {
            var diameter = 5;
            var ratio = 2;
            var cover_top = beamInfo.BeamSetting.CoverTop * canvasBase.Scale * ratio;
            var cover_right = beamInfo.BeamSetting.CoverRight * canvasBase.Scale * ratio;
            var cover_bot = beamInfo.BeamSetting.CoverBot * canvasBase.Scale * ratio;
            var cover_left = beamInfo.BeamSetting.CoverLeft * canvasBase.Scale * ratio;

            var options = new OptionStyleInstanceInCanvas(
                StyleThicknessInCanvas.Thickness_1,
                StyleDashInCanvas.Style_Solid,
                StyleColorInCanvas.Color4,
                StyleColorInCanvas.Color4);

            foreach (var item in beamInfo.UIElement.RebarSideBar)
            {
                canvasBase.Parent.Children.Remove(item);
            }
            var p_start_top_left = beamInfo.SectionBeamInCanvas[0]
            + canvasBase.VTX * (cover_left + diameter)
            + canvasBase.VTY * (cover_bot + diameter + 10);

            var p_start_bot_left = beamInfo.SectionBeamInCanvas[3]
            + canvasBase.VTX * (cover_left + diameter)
            - canvasBase.VTY * (cover_bot + diameter + 10);

            var p_start_bot_right = beamInfo.SectionBeamInCanvas[2]
            - canvasBase.VTX * (cover_right + diameter)
            - canvasBase.VTY * (cover_bot + diameter + 10);


            var distance = p_start_bot_left.DistanceTo(p_start_top_left);
            var spacing = distance / (beamInfo.BeamSectionActive.SideRebar.Quantity + 1);

            for (int i = 0; i < beamInfo.BeamSectionActive.SideRebar.Quantity; i++)
            {
                var p_install_1 = p_start_bot_left - canvasBase.VTY * (i + 1) * spacing;
                var p_install_2 = p_start_bot_right - canvasBase.VTY * (i + 1) * spacing;
                var rebar1 = new InstanceInCanvasCircel(
                    canvasBase,
                    options,
                    canvasBase.Center,
                    diameter,
                    p_install_1,
                    new System.Windows.Point(-1, -1), "");
                var rebar2 = new InstanceInCanvasCircel(
                    canvasBase,
                    options,
                    canvasBase.Center,
                    diameter,
                    p_install_2,
                    new System.Windows.Point(-1, -1), "");
                rebar1.DrawInCanvas();
                rebar2.DrawInCanvas();
                beamInfo.UIElement.RebarSideBar.Add(rebar1.UIElement);
                beamInfo.UIElement.RebarSideBar.Add(rebar2.UIElement);
            }
        }
    }
}
