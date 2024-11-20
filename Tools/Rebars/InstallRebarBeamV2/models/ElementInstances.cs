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
        public string PathRebarBeamType {  get; set; }
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
                rebarBeam.RebarBeamSectionStart = RebarBeamTypeSelected?.RebarBeamSectionStart ?? new RebarBeamSectionStart();
                rebarBeam.RebarBeamSectionMid = RebarBeamTypeSelected?.RebarBeamSectionMid ?? new RebarBeamSectionMid();
                rebarBeam.RebarBeamSectionEnd = RebarBeamTypeSelected?.RebarBeamSectionEnd ?? new RebarBeamSectionEnd();
                InitDataRebarBeamSection(rebarBeam, rebarBeam.RebarBeamSectionStart);
                InitDataRebarBeamSection(rebarBeam, rebarBeam.RebarBeamSectionMid);
                InitDataRebarBeamSection(rebarBeam, rebarBeam.RebarBeamSectionEnd);
            }
        }
        private void InitDataRebarBeamSection(RebarBeam rebarBeam, RebarBeamSection rebarBeamSection)
        {
            if (rebarBeamSection.RebarBeamStirrup == null)
            {
                rebarBeamSection.RebarBeamStirrup = new RebarBeamStirrup();
                rebarBeamSection.RebarBeamStirrup.Diameter = RebarDiameters.FirstOrDefault();
                rebarBeamSection.RebarBeamStirrup.Spacing = 100;
                rebarBeamSection.RebarBeamStirrup.RebarBeamType = 2;
            }
            rebarBeamSection.RebarBeamStirrup.HostId = rebarBeam.BeamId;

            if (rebarBeamSection.RebarBeamSideBar == null)
            {
                rebarBeamSection.RebarBeamSideBar = new RebarBeamSideBar();
                rebarBeamSection.RebarBeamSideBar.Diameter = RebarDiameters.FirstOrDefault();
                rebarBeamSection.RebarBeamSideBar.QuantitySide = 1;
            }
            rebarBeamSection.RebarBeamSideBar.HostId = rebarBeam.BeamId;

            if (rebarBeamSection.RebarBeamTop == null)
            {
                rebarBeamSection.RebarBeamTop = new RebarBeamTop();
                if (rebarBeamSection.RebarBeamTop == null)
                {
                    rebarBeamSection.RebarBeamTop.RebarBeamTopLevel1 = new RebarBeamMainBar();
                    rebarBeamSection.RebarBeamTop.RebarBeamTopLevel1.Diameter = RebarDiameters.FirstOrDefault();
                    rebarBeamSection.RebarBeamTop.RebarBeamTopLevel1.Quantity = 3;
                    rebarBeamSection.RebarBeamTop.RebarBeamTopLevel1.RebarBeamType = 0;
                    rebarBeamSection.RebarBeamTop.RebarBeamTopLevel1.RebarLevelType = (int)RebarBeamMainBarLevelType.RebarTop;
                    rebarBeamSection.RebarBeamTop.RebarBeamTopLevel1.RebarGroupType = (int)RebarBeamMainBarGroupType.GroupLevel1;
                }
                if (rebarBeamSection.RebarBeamTop.RebarBeamTopLevel2 == null)
                {
                    rebarBeamSection.RebarBeamTop.RebarBeamTopLevel2 = new RebarBeamMainBar();
                    rebarBeamSection.RebarBeamTop.RebarBeamTopLevel2.Diameter = RebarDiameters.FirstOrDefault();
                    rebarBeamSection.RebarBeamTop.RebarBeamTopLevel2.Quantity = 3;
                    rebarBeamSection.RebarBeamTop.RebarBeamTopLevel2.RebarBeamType = 0;
                    rebarBeamSection.RebarBeamTop.RebarBeamTopLevel2.RebarLevelType = (int)RebarBeamMainBarLevelType.RebarTop;
                    rebarBeamSection.RebarBeamTop.RebarBeamTopLevel2.RebarGroupType = (int)RebarBeamMainBarGroupType.GroupLevel2;
                }

                if(rebarBeamSection.RebarBeamTop.RebarBeamTopLevel3 == null)
                {
                    rebarBeamSection.RebarBeamTop.RebarBeamTopLevel3 = new RebarBeamMainBar();
                    rebarBeamSection.RebarBeamTop.RebarBeamTopLevel3.Diameter = RebarDiameters.FirstOrDefault();
                    rebarBeamSection.RebarBeamTop.RebarBeamTopLevel3.Quantity = 3;
                    rebarBeamSection.RebarBeamTop.RebarBeamTopLevel3.RebarBeamType = 0;
                    rebarBeamSection.RebarBeamTop.RebarBeamTopLevel3.RebarLevelType = (int)RebarBeamMainBarLevelType.RebarTop;
                    rebarBeamSection.RebarBeamTop.RebarBeamTopLevel3.RebarGroupType = (int)RebarBeamMainBarGroupType.GroupLevel2;
                }

            }
            rebarBeamSection.RebarBeamTop.RebarBeamTopLevel1.HostId = rebarBeam.BeamId;
            rebarBeamSection.RebarBeamTop.RebarBeamTopLevel2.HostId = rebarBeam.BeamId;
            rebarBeamSection.RebarBeamTop.RebarBeamTopLevel3.HostId = rebarBeam.BeamId;
            rebarBeamSection.RebarBeamTop.RebarGroupTypeActive = (int)RebarBeamMainBarGroupType.GroupLevel1;
            rebarBeamSection.RebarBeamTop.RebarBeamTopLevelActive = rebarBeamSection.RebarBeamTop.RebarBeamTopLevel1;
            rebarBeamSection.RebarBeamTop.RebarGroupTypeChange = () =>
            {
                RebarBeamTop.TopRebarGroupTypeChangeFunc(rebarBeamSection.RebarBeamTop);
            };

            if(rebarBeamSection.RebarBeamBot == null)
            {
                rebarBeamSection.RebarBeamBot = new RebarBeamBot();
                if (rebarBeamSection.RebarBeamBot.RebarBeamBotLevel1 == null)
                {
                    rebarBeamSection.RebarBeamBot.RebarBeamBotLevel1 = new RebarBeamMainBar();
                    rebarBeamSection.RebarBeamBot.RebarBeamBotLevel1.Diameter = RebarDiameters.FirstOrDefault();
                    rebarBeamSection.RebarBeamBot.RebarBeamBotLevel1.Quantity = 3;
                    rebarBeamSection.RebarBeamBot.RebarBeamBotLevel1.RebarBeamType = 0;
                    rebarBeamSection.RebarBeamBot.RebarBeamBotLevel1.RebarLevelType = (int)RebarBeamMainBarLevelType.RebarBot;
                    rebarBeamSection.RebarBeamBot.RebarBeamBotLevel1.RebarGroupType = (int)RebarBeamMainBarGroupType.GroupLevel1;
                }
                if(rebarBeamSection.RebarBeamBot.RebarBeamBotLevel2 == null)
                {
                    rebarBeamSection.RebarBeamBot.RebarBeamBotLevel2 = new RebarBeamMainBar();
                    rebarBeamSection.RebarBeamBot.RebarBeamBotLevel2.Diameter = RebarDiameters.FirstOrDefault();
                    rebarBeamSection.RebarBeamBot.RebarBeamBotLevel2.Quantity = 3;
                    rebarBeamSection.RebarBeamBot.RebarBeamBotLevel2.RebarBeamType = 0;
                    rebarBeamSection.RebarBeamBot.RebarBeamBotLevel2.RebarLevelType = (int)RebarBeamMainBarLevelType.RebarBot;
                    rebarBeamSection.RebarBeamBot.RebarBeamBotLevel2.RebarGroupType = (int)RebarBeamMainBarGroupType.GroupLevel2;
                }

                if (rebarBeamSection.RebarBeamBot.RebarBeamBotLevel3 == null)
                {
                    rebarBeamSection.RebarBeamBot.RebarBeamBotLevel3 = new RebarBeamMainBar();
                    rebarBeamSection.RebarBeamBot.RebarBeamBotLevel3.Diameter = RebarDiameters.FirstOrDefault();
                    rebarBeamSection.RebarBeamBot.RebarBeamBotLevel3.Quantity = 3;
                    rebarBeamSection.RebarBeamBot.RebarBeamBotLevel3.RebarBeamType = 0;
                    rebarBeamSection.RebarBeamBot.RebarBeamBotLevel3.RebarLevelType = (int)RebarBeamMainBarLevelType.RebarBot;
                    rebarBeamSection.RebarBeamBot.RebarBeamBotLevel3.RebarGroupType = (int)RebarBeamMainBarGroupType.GroupLevel2;
                }
            }
            rebarBeamSection.RebarBeamBot.RebarBeamBotLevel1.HostId = rebarBeam.BeamId;
            rebarBeamSection.RebarBeamBot.RebarBeamBotLevel2.HostId = rebarBeam.BeamId;
            rebarBeamSection.RebarBeamBot.RebarBeamBotLevel3.HostId = rebarBeam.BeamId;
            rebarBeamSection.RebarBeamBot.RebarGroupTypeActive = (int)RebarBeamMainBarGroupType.GroupLevel1;
            rebarBeamSection.RebarBeamBot.RebarBeamBotLevelActive = rebarBeamSection.RebarBeamBot.RebarBeamBotLevel1;
            rebarBeamSection.RebarBeamBot.RebarGroupTypeChange = () =>
            {
                RebarBeamBot.BotRebarGroupTypeChangeFunc(rebarBeamSection.RebarBeamBot);
            };
        }
    }
}
