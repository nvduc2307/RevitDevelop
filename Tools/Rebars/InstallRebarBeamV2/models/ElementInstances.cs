using Autodesk.Revit.DB.Structure;
using HcBimUtils.DocumentUtils;
using Newtonsoft.Json;
using RevitDevelop.Utils.RevElements;
using RevitDevelop.Utils.RevElements.RevRebars;
using System.IO;
using System.Windows;
using Utils.Directionaries;
using Utils.FilterElements;
using Utils.PathInWindows;
using Utils.SelectionFilterInRevit;

namespace RevitDevelop.Tools.Rebars.InstallRebarBeamV2.models
{
    public partial class ElementInstances : ObservableObject
    {
        public static string DIR_TOOL = $"{PathInWindow.AppDataRimT}\\CreateRebarBeam";
        public string PathRebarBeamType { get; set; }
        public double DistanceRebarToRebarMm { get; set; }
        public RevElement Beam { get; set; }
        public List<RebarBarTypeCustom> RebarBarTypeCustoms { get; set; }
        public List<string> RebarDiameters { get; set; }
        public List<int> MainBarGroupTypes
        {
            get => new List<int> { 1, 2, 3, };
        }
        [ObservableProperty]
        private List<RebarBeam> _rebarBeams;
        public List<UIElement> MainRebarTopUIElement { get; set; }
        public List<UIElement> MainRebarBotUIElement { get; set; }
        public List<UIElement> SideBarUIElement { get; set; }
        [ObservableProperty]
        private List<RebarBeam> _rebarBeamTypes;
        [ObservableProperty]
        private RebarBeam _rebarBeamTypeSelected;
        [ObservableProperty]
        private string _rebarBeamTypeName;
        public double CoverMm { get; set; }
        [ObservableProperty]
        private RebarBeam _rebarBeamActive;
        public ElementInstances()
        {
            RebarBeamTypeName = "";
            PathRebarBeamType = $"{DIR_TOOL}\\{AC.Document.ProjectInformation.UniqueId}\\RebarBeamTypes.json";
            DirectionaryExt.CreateDirectory(PathRebarBeamType);
            RebarBeamTypes = JsonConvert.DeserializeObject<List<RebarBeam>>(File.ReadAllText(PathRebarBeamType));
            RebarBeamTypeSelected = RebarBeamTypes.FirstOrDefault();
            var obj = AC.UiDoc.Selection.PickObject(
                Autodesk.Revit.UI.Selection.ObjectType.Element,
                new GenericSelectionFilterFromCategory(BuiltInCategory.OST_StructuralFraming)).ToElement();
            Beam = new RevElement(obj);
            RebarBarTypeCustoms = AC.Document.GetElementsFromClass<RebarBarType>()
                .Select(x => new RebarBarTypeCustom(x))
                .ToList();
            RebarDiameters = RebarBarTypeCustoms.Select(x => x.NameStyle).OrderBy(x => x).ToList();
            RebarBeams = Beam.ElementSubs?.Select(x => new RebarBeam(x)).ToList();
            InitDataRebarBeam();
            RebarBeamActive = RebarBeams.FirstOrDefault();
            DistanceRebarToRebarMm = 100;
            CoverMm = 30;
            MainRebarTopUIElement = new List<UIElement>();
            MainRebarBotUIElement = new List<UIElement>();
            SideBarUIElement = new List<UIElement>();
        }
        public void InitDataRebarBeam()
        {
            foreach (var rebarBeam in RebarBeams)
            {
                rebarBeam.RebarBeamSectionStart = new RebarBeamSectionStart();
                rebarBeam.RebarBeamSectionMid = new RebarBeamSectionMid();
                rebarBeam.RebarBeamSectionEnd = new RebarBeamSectionEnd();
                InitDataRebarBeamSection(rebarBeam, rebarBeam.RebarBeamSectionStart);
                InitDataRebarBeamSection(rebarBeam, rebarBeam.RebarBeamSectionMid);
                InitDataRebarBeamSection(rebarBeam, rebarBeam.RebarBeamSectionEnd);
            }
        }
        private void InitDataRebarBeamSection(RebarBeam rebarBeam, RebarBeamSection rebarBeamSection, RebarBeamSection rebarBeamSectionActive)
        {
            RebarBeamSection sectionActive = null;
            switch (rebarBeamSection.RebarBeamSectionType)
            {
                case (int)RebarBeamSectionType.SectionStart:
                    sectionActive = RebarBeamTypeSelected?.RebarBeamSectionStart;
                    break;
                case (int)RebarBeamSectionType.SectionMid:
                    sectionActive = RebarBeamTypeSelected?.RebarBeamSectionMid;
                    break;
                case (int)RebarBeamSectionType.SectionEnd:
                    sectionActive = RebarBeamTypeSelected?.RebarBeamSectionEnd;
                    break;
            }

            rebarBeamSection.RebarBeamStirrup = new RebarBeamStirrup();
            rebarBeamSection.RebarBeamStirrup.HostId = rebarBeam.BeamId;
            rebarBeamSection.RebarBeamStirrup.Diameter = sectionActive == null ? RebarDiameters.FirstOrDefault() : sectionActive.RebarBeamStirrup.Diameter;
            rebarBeamSection.RebarBeamStirrup.Spacing = sectionActive == null ? 100 : sectionActive.RebarBeamStirrup.Spacing;
            rebarBeamSection.RebarBeamStirrup.RebarBeamType = (int)RebarBeamType.RebarBeamStirrup;

            rebarBeamSection.RebarBeamSideBar = new RebarBeamSideBar();
            rebarBeamSection.RebarBeamSideBar.HostId = rebarBeam.BeamId;
            rebarBeamSection.RebarBeamSideBar.Diameter = sectionActive == null ? RebarDiameters.FirstOrDefault() : sectionActive.RebarBeamSideBar.Diameter;
            rebarBeamSection.RebarBeamSideBar.QuantitySide = sectionActive == null ? 1 : sectionActive.RebarBeamSideBar.QuantitySide;
            rebarBeamSection.RebarBeamSideBar.RebarBeamType = (int)RebarBeamType.RebarBeamSideBar;

            rebarBeamSection.RebarBeamTop = new RebarBeamTop();
            rebarBeamSection.RebarBeamTop.RebarBeamTopLevel1 = new RebarBeamMainBar();
            rebarBeamSection.RebarBeamTop.RebarBeamTopLevel1.HostId = rebarBeam.BeamId;
            rebarBeamSection.RebarBeamTop.RebarBeamTopLevel1.Diameter = sectionActive == null ? RebarDiameters.FirstOrDefault() : sectionActive.RebarBeamTop.RebarBeamTopLevel1.Diameter;
            rebarBeamSection.RebarBeamTop.RebarBeamTopLevel1.Quantity = sectionActive == null ? 3 : sectionActive.RebarBeamTop.RebarBeamTopLevel1.Quantity;
            rebarBeamSection.RebarBeamTop.RebarBeamTopLevel1.RebarBeamType = sectionActive == null ? 0 : sectionActive.RebarBeamTop.RebarBeamTopLevel1.RebarBeamType;
            rebarBeamSection.RebarBeamTop.RebarBeamTopLevel1.RebarLevelType = sectionActive == null ? (int)RebarBeamMainBarLevelType.RebarTop : sectionActive.RebarBeamTop.RebarBeamTopLevel1.RebarLevelType;
            rebarBeamSection.RebarBeamTop.RebarBeamTopLevel1.RebarGroupType = sectionActive == null ? (int)RebarBeamMainBarGroupType.GroupLevel1 : sectionActive.RebarBeamTop.RebarBeamTopLevel1.RebarGroupType;
            rebarBeamSection.RebarBeamTop.RebarBeamTopLevel1.RebarBeamType = (int)RebarBeamType.RebarBeamMainBar;

            rebarBeamSection.RebarBeamTop.RebarBeamTopLevel2 = new RebarBeamMainBar();
            rebarBeamSection.RebarBeamTop.RebarBeamTopLevel2.HostId = rebarBeam.BeamId;
            rebarBeamSection.RebarBeamTop.RebarBeamTopLevel2.Diameter = sectionActive == null ? RebarDiameters.FirstOrDefault() : sectionActive.RebarBeamTop.RebarBeamTopLevel2.Diameter;
            rebarBeamSection.RebarBeamTop.RebarBeamTopLevel2.Quantity = sectionActive == null ? 3 : sectionActive.RebarBeamTop.RebarBeamTopLevel2.Quantity;
            rebarBeamSection.RebarBeamTop.RebarBeamTopLevel2.RebarBeamType = sectionActive == null ? 0 : sectionActive.RebarBeamTop.RebarBeamTopLevel2.RebarBeamType;
            rebarBeamSection.RebarBeamTop.RebarBeamTopLevel2.RebarLevelType = sectionActive == null ? (int)RebarBeamMainBarLevelType.RebarTop : sectionActive.RebarBeamTop.RebarBeamTopLevel2.RebarLevelType;
            rebarBeamSection.RebarBeamTop.RebarBeamTopLevel2.RebarGroupType = sectionActive == null ? (int)RebarBeamMainBarGroupType.GroupLevel2 : sectionActive.RebarBeamTop.RebarBeamTopLevel2.RebarGroupType;
            rebarBeamSection.RebarBeamTop.RebarBeamTopLevel2.RebarBeamType = (int)RebarBeamType.RebarBeamMainBar;

            rebarBeamSection.RebarBeamTop.RebarBeamTopLevel3 = new RebarBeamMainBar();
            rebarBeamSection.RebarBeamTop.RebarBeamTopLevel3.HostId = rebarBeam.BeamId;
            rebarBeamSection.RebarBeamTop.RebarBeamTopLevel3.Diameter = sectionActive == null ? RebarDiameters.FirstOrDefault() : sectionActive.RebarBeamTop.RebarBeamTopLevel3.Diameter;
            rebarBeamSection.RebarBeamTop.RebarBeamTopLevel3.Quantity = sectionActive == null ? 3 : sectionActive.RebarBeamTop.RebarBeamTopLevel3.Quantity;
            rebarBeamSection.RebarBeamTop.RebarBeamTopLevel3.RebarBeamType = sectionActive == null ? 0 : sectionActive.RebarBeamTop.RebarBeamTopLevel3.RebarBeamType;
            rebarBeamSection.RebarBeamTop.RebarBeamTopLevel3.RebarLevelType = sectionActive == null ? (int)RebarBeamMainBarLevelType.RebarTop : sectionActive.RebarBeamTop.RebarBeamTopLevel3.RebarLevelType;
            rebarBeamSection.RebarBeamTop.RebarBeamTopLevel3.RebarGroupType = sectionActive == null ? (int)RebarBeamMainBarGroupType.GroupLevel3 : sectionActive.RebarBeamTop.RebarBeamTopLevel3.RebarGroupType;
            rebarBeamSection.RebarBeamTop.RebarBeamTopLevel3.RebarBeamType = (int)RebarBeamType.RebarBeamMainBar;

            rebarBeamSection.RebarBeamTop.RebarGroupTypeActive = sectionActive == null ? (int)RebarBeamMainBarGroupType.GroupLevel1 : sectionActive.RebarBeamTop.RebarGroupTypeActive;
            switch (rebarBeamSection.RebarBeamTop.RebarGroupTypeActive)
            {
                case (int)RebarBeamMainBarGroupType.GroupLevel1:
                    rebarBeamSection.RebarBeamTop.RebarBeamTopLevelActive = rebarBeamSection.RebarBeamTop.RebarBeamTopLevel1;
                    break;
                case (int)RebarBeamMainBarGroupType.GroupLevel2:
                    rebarBeamSection.RebarBeamTop.RebarBeamTopLevelActive = rebarBeamSection.RebarBeamTop.RebarBeamTopLevel2;
                    break;
                case (int)RebarBeamMainBarGroupType.GroupLevel3:
                    rebarBeamSection.RebarBeamTop.RebarBeamTopLevelActive = rebarBeamSection.RebarBeamTop.RebarBeamTopLevel3;
                    break;
            }
            rebarBeamSection.RebarBeamTop.RebarGroupTypeChange = () =>
            {
                RebarBeamTop.TopRebarLeveTypeChangeFunc(rebarBeamSection.RebarBeamTop);
            };

            rebarBeamSection.RebarBeamBot = new RebarBeamBot();
            rebarBeamSection.RebarBeamBot.RebarBeamBotLevel1 = new RebarBeamMainBar();
            rebarBeamSection.RebarBeamBot.RebarBeamBotLevel1.HostId = rebarBeam.BeamId;
            rebarBeamSection.RebarBeamBot.RebarBeamBotLevel1.Diameter = sectionActive == null ? RebarDiameters.FirstOrDefault() : sectionActive.RebarBeamBot.RebarBeamBotLevel1.Diameter;
            rebarBeamSection.RebarBeamBot.RebarBeamBotLevel1.Quantity = sectionActive == null ? 3 : sectionActive.RebarBeamBot.RebarBeamBotLevel1.Quantity;
            rebarBeamSection.RebarBeamBot.RebarBeamBotLevel1.RebarBeamType = sectionActive == null ? 0 : sectionActive.RebarBeamBot.RebarBeamBotLevel1.RebarBeamType;
            rebarBeamSection.RebarBeamBot.RebarBeamBotLevel1.RebarLevelType = sectionActive == null ? (int)RebarBeamMainBarLevelType.RebarBot : sectionActive.RebarBeamBot.RebarBeamBotLevel1.RebarLevelType;
            rebarBeamSection.RebarBeamBot.RebarBeamBotLevel1.RebarGroupType = sectionActive == null ? (int)RebarBeamMainBarGroupType.GroupLevel1 : sectionActive.RebarBeamBot.RebarBeamBotLevel1.RebarGroupType;
            rebarBeamSection.RebarBeamBot.RebarBeamBotLevel1.RebarBeamType = (int)RebarBeamType.RebarBeamMainBar;

            rebarBeamSection.RebarBeamBot.RebarBeamBotLevel2 = new RebarBeamMainBar();
            rebarBeamSection.RebarBeamBot.RebarBeamBotLevel2.HostId = rebarBeam.BeamId;
            rebarBeamSection.RebarBeamBot.RebarBeamBotLevel2.Diameter = sectionActive == null ? RebarDiameters.FirstOrDefault() : sectionActive.RebarBeamBot.RebarBeamBotLevel2.Diameter;
            rebarBeamSection.RebarBeamBot.RebarBeamBotLevel2.Quantity = sectionActive == null ? 3 : sectionActive.RebarBeamBot.RebarBeamBotLevel2.Quantity;
            rebarBeamSection.RebarBeamBot.RebarBeamBotLevel2.RebarBeamType = sectionActive == null ? 0 : sectionActive.RebarBeamBot.RebarBeamBotLevel2.RebarBeamType;
            rebarBeamSection.RebarBeamBot.RebarBeamBotLevel2.RebarLevelType = sectionActive == null ? (int)RebarBeamMainBarLevelType.RebarBot : sectionActive.RebarBeamBot.RebarBeamBotLevel2.RebarLevelType;
            rebarBeamSection.RebarBeamBot.RebarBeamBotLevel2.RebarGroupType = sectionActive == null ? (int)RebarBeamMainBarGroupType.GroupLevel2 : sectionActive.RebarBeamBot.RebarBeamBotLevel2.RebarGroupType;
            rebarBeamSection.RebarBeamBot.RebarBeamBotLevel2.RebarBeamType = (int)RebarBeamType.RebarBeamMainBar;

            rebarBeamSection.RebarBeamBot.RebarBeamBotLevel3 = new RebarBeamMainBar();
            rebarBeamSection.RebarBeamBot.RebarBeamBotLevel3.HostId = rebarBeam.BeamId;
            rebarBeamSection.RebarBeamBot.RebarBeamBotLevel3.Diameter = sectionActive == null ? RebarDiameters.FirstOrDefault() : sectionActive.RebarBeamBot.RebarBeamBotLevel3.Diameter;
            rebarBeamSection.RebarBeamBot.RebarBeamBotLevel3.Quantity = sectionActive == null ? 3 : sectionActive.RebarBeamBot.RebarBeamBotLevel3.Quantity;
            rebarBeamSection.RebarBeamBot.RebarBeamBotLevel3.RebarBeamType = sectionActive == null ? 0 : sectionActive.RebarBeamBot.RebarBeamBotLevel3.RebarBeamType;
            rebarBeamSection.RebarBeamBot.RebarBeamBotLevel3.RebarLevelType = sectionActive == null ? (int)RebarBeamMainBarLevelType.RebarBot : sectionActive.RebarBeamBot.RebarBeamBotLevel3.RebarLevelType;
            rebarBeamSection.RebarBeamBot.RebarBeamBotLevel3.RebarGroupType = sectionActive == null ? (int)RebarBeamMainBarGroupType.GroupLevel2 : sectionActive.RebarBeamBot.RebarBeamBotLevel3.RebarGroupType;
            rebarBeamSection.RebarBeamBot.RebarBeamBotLevel3.RebarBeamType = (int)RebarBeamType.RebarBeamMainBar;

            rebarBeamSection.RebarBeamBot.RebarGroupTypeActive = sectionActive == null ? (int)RebarBeamMainBarGroupType.GroupLevel1 : sectionActive.RebarBeamBot.RebarGroupTypeActive;
            switch (rebarBeamSection.RebarBeamBot.RebarGroupTypeActive)
            {
                case (int)RebarBeamMainBarGroupType.GroupLevel1:
                    rebarBeamSection.RebarBeamBot.RebarBeamBotLevelActive = rebarBeamSection.RebarBeamBot.RebarBeamBotLevel1;
                    break;
                case (int)RebarBeamMainBarGroupType.GroupLevel2:
                    rebarBeamSection.RebarBeamBot.RebarBeamBotLevelActive = rebarBeamSection.RebarBeamBot.RebarBeamBotLevel2;
                    break;
                case (int)RebarBeamMainBarGroupType.GroupLevel3:
                    rebarBeamSection.RebarBeamBot.RebarBeamBotLevelActive = rebarBeamSection.RebarBeamBot.RebarBeamBotLevel3;
                    break;
            }
            rebarBeamSection.RebarBeamBot.RebarGroupTypeChange = () =>
            {
                RebarBeamBot.BotRebarGroupTypeChangeFunc(rebarBeamSection.RebarBeamBot);
            };
        }
    }
}
