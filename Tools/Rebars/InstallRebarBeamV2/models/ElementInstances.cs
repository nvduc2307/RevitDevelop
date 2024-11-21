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
                InitDataRebarBeamSection(rebarBeam, rebarBeam.RebarBeamSectionStart, RebarBeamTypeSelected?.RebarBeamSectionStart);
                InitDataRebarBeamSection(rebarBeam, rebarBeam.RebarBeamSectionMid, RebarBeamTypeSelected?.RebarBeamSectionMid);
                InitDataRebarBeamSection(rebarBeam, rebarBeam.RebarBeamSectionEnd, RebarBeamTypeSelected?.RebarBeamSectionEnd);
            }
        }
        private void InitDataRebarBeamSection(RebarBeam rebarBeam, RebarBeamSection rebarBeamSection, RebarBeamSection rebarBeamSectionActive)
        {
            rebarBeamSection.RebarBeamStirrup = new RebarBeamStirrup();
            rebarBeamSection.RebarBeamStirrup.HostId = rebarBeam.BeamId;
            rebarBeamSection.RebarBeamStirrup.Diameter = rebarBeamSectionActive == null ? RebarDiameters.FirstOrDefault() : rebarBeamSectionActive.RebarBeamStirrup.Diameter;
            rebarBeamSection.RebarBeamStirrup.Spacing = rebarBeamSectionActive == null ? 100 : rebarBeamSection.RebarBeamStirrup.Spacing;
            rebarBeamSection.RebarBeamStirrup.RebarBeamType = rebarBeamSectionActive == null ? 2 : rebarBeamSectionActive.RebarBeamStirrup.RebarBeamType;

            rebarBeamSection.RebarBeamSideBar = new RebarBeamSideBar();
            rebarBeamSection.RebarBeamSideBar.HostId = rebarBeam.BeamId;
            rebarBeamSection.RebarBeamSideBar.Diameter = rebarBeamSectionActive == null ? RebarDiameters.FirstOrDefault() : rebarBeamSectionActive.RebarBeamSideBar.Diameter;
            rebarBeamSection.RebarBeamSideBar.QuantitySide = rebarBeamSectionActive == null ? 1 : rebarBeamSectionActive.RebarBeamSideBar.QuantitySide;

            rebarBeamSection.RebarBeamTop = new RebarBeamTop();
            rebarBeamSection.RebarBeamTop.RebarBeamTopLevel1 = new RebarBeamMainBar();
            rebarBeamSection.RebarBeamTop.RebarBeamTopLevel1.Diameter = rebarBeamSectionActive == null ? RebarDiameters.FirstOrDefault() : rebarBeamSectionActive.RebarBeamTop.RebarBeamTopLevel1.Diameter;
            rebarBeamSection.RebarBeamTop.RebarBeamTopLevel1.Quantity = rebarBeamSectionActive == null ? 3 : rebarBeamSectionActive.RebarBeamTop.RebarBeamTopLevel1.Quantity;
            rebarBeamSection.RebarBeamTop.RebarBeamTopLevel1.RebarBeamType = rebarBeamSectionActive == null ? 0 : rebarBeamSectionActive.RebarBeamTop.RebarBeamTopLevel1.RebarBeamType;
            rebarBeamSection.RebarBeamTop.RebarBeamTopLevel1.RebarLevelType = rebarBeamSectionActive == null ? (int)RebarBeamMainBarLevelType.RebarTop : rebarBeamSectionActive.RebarBeamTop.RebarBeamTopLevel1.RebarLevelType;
            rebarBeamSection.RebarBeamTop.RebarBeamTopLevel1.RebarGroupType = rebarBeamSectionActive == null ? (int)RebarBeamMainBarGroupType.GroupLevel1 : rebarBeamSectionActive.RebarBeamTop.RebarBeamTopLevel1.RebarGroupType;
            rebarBeamSection.RebarBeamTop.RebarBeamTopLevel1.HostId = rebarBeam.BeamId;

            rebarBeamSection.RebarBeamTop.RebarBeamTopLevel2 = new RebarBeamMainBar();
            rebarBeamSection.RebarBeamTop.RebarBeamTopLevel2.Diameter = rebarBeamSectionActive == null ? RebarDiameters.FirstOrDefault() : rebarBeamSectionActive.RebarBeamTop.RebarBeamTopLevel2.Diameter;
            rebarBeamSection.RebarBeamTop.RebarBeamTopLevel2.Quantity = rebarBeamSectionActive == null ? 3 : rebarBeamSectionActive.RebarBeamTop.RebarBeamTopLevel2.Quantity;
            rebarBeamSection.RebarBeamTop.RebarBeamTopLevel2.RebarBeamType = rebarBeamSectionActive == null ? 0 : rebarBeamSectionActive.RebarBeamTop.RebarBeamTopLevel2.RebarBeamType;
            rebarBeamSection.RebarBeamTop.RebarBeamTopLevel2.RebarLevelType = rebarBeamSectionActive == null ? (int)RebarBeamMainBarLevelType.RebarTop : rebarBeamSectionActive.RebarBeamTop.RebarBeamTopLevel2.RebarLevelType;
            rebarBeamSection.RebarBeamTop.RebarBeamTopLevel2.RebarGroupType = rebarBeamSectionActive == null ? (int)RebarBeamMainBarGroupType.GroupLevel2 : rebarBeamSectionActive.RebarBeamTop.RebarBeamTopLevel2.RebarGroupType;
            rebarBeamSection.RebarBeamTop.RebarBeamTopLevel2.HostId = rebarBeam.BeamId;

            rebarBeamSection.RebarBeamTop.RebarBeamTopLevel3 = new RebarBeamMainBar();
            rebarBeamSection.RebarBeamTop.RebarBeamTopLevel3.Diameter = rebarBeamSectionActive == null ? RebarDiameters.FirstOrDefault() : rebarBeamSectionActive.RebarBeamTop.RebarBeamTopLevel3.Diameter;
            rebarBeamSection.RebarBeamTop.RebarBeamTopLevel3.Quantity = rebarBeamSectionActive == null ? 3 : rebarBeamSectionActive.RebarBeamTop.RebarBeamTopLevel3.Quantity;
            rebarBeamSection.RebarBeamTop.RebarBeamTopLevel3.RebarBeamType = rebarBeamSectionActive == null ? 0 : rebarBeamSectionActive.RebarBeamTop.RebarBeamTopLevel3.RebarBeamType;
            rebarBeamSection.RebarBeamTop.RebarBeamTopLevel3.RebarLevelType = rebarBeamSectionActive == null ? (int)RebarBeamMainBarLevelType.RebarTop : rebarBeamSectionActive.RebarBeamTop.RebarBeamTopLevel3.RebarLevelType;
            rebarBeamSection.RebarBeamTop.RebarBeamTopLevel3.RebarGroupType = rebarBeamSectionActive == null ? (int)RebarBeamMainBarGroupType.GroupLevel2 : rebarBeamSectionActive.RebarBeamTop.RebarBeamTopLevel3.RebarGroupType;
            rebarBeamSection.RebarBeamTop.RebarBeamTopLevel3.HostId = rebarBeam.BeamId;

            rebarBeamSection.RebarBeamTop.RebarGroupTypeActive = rebarBeamSectionActive == null ? (int)RebarBeamMainBarGroupType.GroupLevel1 : rebarBeamSectionActive.RebarBeamTop.RebarGroupTypeActive;
            rebarBeamSection.RebarBeamTop.RebarBeamTopLevelActive = rebarBeamSectionActive == null ? rebarBeamSection.RebarBeamTop.RebarBeamTopLevel1 : rebarBeamSectionActive.RebarBeamTop.RebarBeamTopLevelActive;
            rebarBeamSection.RebarBeamTop.RebarGroupTypeChange = () =>
            {
                RebarBeamTop.TopRebarGroupTypeChangeFunc(rebarBeamSection.RebarBeamTop);
            };

            rebarBeamSection.RebarBeamBot = new RebarBeamBot();
            rebarBeamSection.RebarBeamBot.RebarBeamBotLevel1 = new RebarBeamMainBar();
            rebarBeamSection.RebarBeamBot.RebarBeamBotLevel1.Diameter = rebarBeamSectionActive == null ? RebarDiameters.FirstOrDefault() : rebarBeamSectionActive.RebarBeamBot.RebarBeamBotLevel1.Diameter;
            rebarBeamSection.RebarBeamBot.RebarBeamBotLevel1.Quantity = rebarBeamSectionActive == null ? 3 : rebarBeamSectionActive.RebarBeamBot.RebarBeamBotLevel1.Quantity;
            rebarBeamSection.RebarBeamBot.RebarBeamBotLevel1.RebarBeamType = rebarBeamSectionActive == null ? 0 : rebarBeamSectionActive.RebarBeamBot.RebarBeamBotLevel1.RebarBeamType;
            rebarBeamSection.RebarBeamBot.RebarBeamBotLevel1.RebarLevelType = rebarBeamSectionActive == null ? (int)RebarBeamMainBarLevelType.RebarBot : rebarBeamSectionActive.RebarBeamBot.RebarBeamBotLevel1.RebarLevelType;
            rebarBeamSection.RebarBeamBot.RebarBeamBotLevel1.RebarGroupType = rebarBeamSectionActive == null ? (int)RebarBeamMainBarGroupType.GroupLevel1 : rebarBeamSectionActive.RebarBeamBot.RebarBeamBotLevel1.RebarGroupType;
            rebarBeamSection.RebarBeamBot.RebarBeamBotLevel1.HostId = rebarBeam.BeamId;

            rebarBeamSection.RebarBeamBot.RebarBeamBotLevel2 = new RebarBeamMainBar();
            rebarBeamSection.RebarBeamBot.RebarBeamBotLevel2.Diameter = rebarBeamSectionActive == null ? RebarDiameters.FirstOrDefault() : rebarBeamSectionActive.RebarBeamBot.RebarBeamBotLevel2.Diameter;
            rebarBeamSection.RebarBeamBot.RebarBeamBotLevel2.Quantity = rebarBeamSectionActive == null ? 3 : rebarBeamSectionActive.RebarBeamBot.RebarBeamBotLevel2.Quantity;
            rebarBeamSection.RebarBeamBot.RebarBeamBotLevel2.RebarBeamType = rebarBeamSectionActive == null ? 0 : rebarBeamSectionActive.RebarBeamBot.RebarBeamBotLevel2.RebarBeamType;
            rebarBeamSection.RebarBeamBot.RebarBeamBotLevel2.RebarLevelType = rebarBeamSectionActive == null ? (int)RebarBeamMainBarLevelType.RebarBot : rebarBeamSectionActive.RebarBeamBot.RebarBeamBotLevel2.RebarLevelType;
            rebarBeamSection.RebarBeamBot.RebarBeamBotLevel2.RebarGroupType = rebarBeamSectionActive == null ? (int)RebarBeamMainBarGroupType.GroupLevel2 : rebarBeamSectionActive.RebarBeamBot.RebarBeamBotLevel2.RebarGroupType;
            rebarBeamSection.RebarBeamBot.RebarBeamBotLevel2.HostId = rebarBeam.BeamId;

            rebarBeamSection.RebarBeamBot.RebarBeamBotLevel3 = new RebarBeamMainBar();
            rebarBeamSection.RebarBeamBot.RebarBeamBotLevel3.Diameter = rebarBeamSectionActive == null ? RebarDiameters.FirstOrDefault() : rebarBeamSectionActive.RebarBeamBot.RebarBeamBotLevel3.Diameter;
            rebarBeamSection.RebarBeamBot.RebarBeamBotLevel3.Quantity = rebarBeamSectionActive == null ? 3 : rebarBeamSectionActive.RebarBeamBot.RebarBeamBotLevel3.Quantity;
            rebarBeamSection.RebarBeamBot.RebarBeamBotLevel3.RebarBeamType = rebarBeamSectionActive == null ? 0 : rebarBeamSectionActive.RebarBeamBot.RebarBeamBotLevel3.RebarBeamType;
            rebarBeamSection.RebarBeamBot.RebarBeamBotLevel3.RebarLevelType = rebarBeamSectionActive == null ? (int)RebarBeamMainBarLevelType.RebarBot : rebarBeamSectionActive.RebarBeamBot.RebarBeamBotLevel3.RebarLevelType;
            rebarBeamSection.RebarBeamBot.RebarBeamBotLevel3.RebarGroupType = rebarBeamSectionActive == null ? (int)RebarBeamMainBarGroupType.GroupLevel2 : rebarBeamSectionActive.RebarBeamBot.RebarBeamBotLevel3.RebarGroupType;
            rebarBeamSection.RebarBeamBot.RebarBeamBotLevel3.HostId = rebarBeam.BeamId;

            rebarBeamSection.RebarBeamBot.RebarGroupTypeActive = rebarBeamSectionActive == null ? (int)RebarBeamMainBarGroupType.GroupLevel1 : rebarBeamSectionActive.RebarBeamBot.RebarGroupTypeActive;
            rebarBeamSection.RebarBeamBot.RebarBeamBotLevelActive = rebarBeamSectionActive == null ? rebarBeamSection.RebarBeamBot.RebarBeamBotLevel1 : rebarBeamSectionActive.RebarBeamBot.RebarBeamBotLevelActive;
            rebarBeamSection.RebarBeamBot.RebarGroupTypeChange = () =>
            {
                RebarBeamBot.BotRebarGroupTypeChangeFunc(rebarBeamSection.RebarBeamBot);
            };
        }
    }
}
