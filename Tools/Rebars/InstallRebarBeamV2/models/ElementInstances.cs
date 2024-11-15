using Autodesk.Revit.DB.Structure;
using HcBimUtils.DocumentUtils;
using RevitDevelop.Utils.RevElements;
using RevitDevelop.Utils.RevElements.RevRebars;
using Utils.FilterElements;
using Utils.SelectionFilterInRevit;

namespace RevitDevelop.Tools.Rebars.InstallRebarBeamV2.models
{
    public partial class ElementInstances : ObservableObject
    {
        public RevElement Beam { get; set; }
        public List<RebarBarTypeCustom> RebarBarTypeCustoms { get; set; }
        public List<string> RebarDiameters { get; set; }
        public List<int> MainBarGroupTypes
        {
            get => new List<int> { 1, 2, 3, };
        }
        public List<RebarBeam> RebarBeams { get; set; }
        [ObservableProperty]
        private RebarBeam _rebarBeamActive;
        public ElementInstances()
        {
            var obj = AC.UiDoc.Selection.PickObject(
                Autodesk.Revit.UI.Selection.ObjectType.Element,
                new GenericSelectionFilterFromCategory(BuiltInCategory.OST_StructuralFraming)).ToElement();
            Beam = new RevElement(obj);
            RebarBarTypeCustoms = AC.Document.GetElementsFromClass<RebarBarType>()
                .Select(x => new RebarBarTypeCustom(x))
                .ToList();
            RebarDiameters = RebarBarTypeCustoms.Select(x => x.NameStyle).ToList();
            RebarBeams = Beam.ElementSubs?.Select(x => new RebarBeam(x)).ToList();
            InitDataRebarBeam();
            RebarBeamActive = RebarBeams.FirstOrDefault();
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
        private void InitDataRebarBeamSection(RebarBeam rebarBeam, RebarBeamSection rebarBeamSection)
        {
            rebarBeamSection.RebarBeamTop = new RebarBeamTop();
            rebarBeamSection.RebarBeamTop.RebarGroupTypeActive = (int)RebarBeamMainBarGroupType.GroupLevel1;
            rebarBeamSection.RebarBeamTop.RebarGroupTypeChange = () =>
            {
                RebarBeamTop.TopRebarGroupTypeChangeFunc(rebarBeamSection.RebarBeamTop);
            };
            rebarBeamSection.RebarBeamTop.RebarBeamTopLevel1 = new RebarBeamMainBar();
            rebarBeamSection.RebarBeamTop.RebarBeamTopLevel1.HostId = rebarBeam.BeamId;
            rebarBeamSection.RebarBeamTop.RebarBeamTopLevel1.Diameter = RebarDiameters.FirstOrDefault();
            rebarBeamSection.RebarBeamTop.RebarBeamTopLevel1.Quantity = 3;
            rebarBeamSection.RebarBeamTop.RebarBeamTopLevel1.RebarBeamType = 3;
            rebarBeamSection.RebarBeamTop.RebarBeamTopLevel1.RebarLevelType = (int)RebarBeamMainBarLevelType.RebarTop;
            rebarBeamSection.RebarBeamTop.RebarBeamTopLevel1.RebarGroupType = (int)RebarBeamMainBarGroupType.GroupLevel1;
            rebarBeamSection.RebarBeamTop.RebarBeamTopLevelActive = rebarBeamSection.RebarBeamTop.RebarBeamTopLevel1;

            rebarBeamSection.RebarBeamTop.RebarBeamTopLevel2 = new RebarBeamMainBar();
            rebarBeamSection.RebarBeamTop.RebarBeamTopLevel2.HostId = rebarBeam.BeamId;
            rebarBeamSection.RebarBeamTop.RebarBeamTopLevel2.Diameter = RebarDiameters.FirstOrDefault();
            rebarBeamSection.RebarBeamTop.RebarBeamTopLevel2.Quantity = 3;
            rebarBeamSection.RebarBeamTop.RebarBeamTopLevel2.RebarBeamType = 3;
            rebarBeamSection.RebarBeamTop.RebarBeamTopLevel2.RebarLevelType = (int)RebarBeamMainBarLevelType.RebarTop;
            rebarBeamSection.RebarBeamTop.RebarBeamTopLevel2.RebarGroupType = (int)RebarBeamMainBarGroupType.GroupLevel2;

            rebarBeamSection.RebarBeamTop.RebarBeamTopLevel3 = new RebarBeamMainBar();
            rebarBeamSection.RebarBeamTop.RebarBeamTopLevel3.HostId = rebarBeam.BeamId;
            rebarBeamSection.RebarBeamTop.RebarBeamTopLevel3.Diameter = RebarDiameters.FirstOrDefault();
            rebarBeamSection.RebarBeamTop.RebarBeamTopLevel3.Quantity = 3;
            rebarBeamSection.RebarBeamTop.RebarBeamTopLevel3.RebarBeamType = 3;
            rebarBeamSection.RebarBeamTop.RebarBeamTopLevel3.RebarLevelType = (int)RebarBeamMainBarLevelType.RebarTop;
            rebarBeamSection.RebarBeamTop.RebarBeamTopLevel3.RebarGroupType = (int)RebarBeamMainBarGroupType.GroupLevel2;

            rebarBeamSection.RebarBeamBot = new RebarBeamBot();
            rebarBeamSection.RebarBeamBot.RebarGroupTypeActive = (int)RebarBeamMainBarGroupType.GroupLevel1;
            rebarBeamSection.RebarBeamBot.RebarGroupTypeChange = () =>
            {
                RebarBeamBot.BotRebarGroupTypeChangeFunc(rebarBeamSection.RebarBeamBot);
            };
            rebarBeamSection.RebarBeamBot.RebarBeamBotLevel1 = new RebarBeamMainBar();
            rebarBeamSection.RebarBeamBot.RebarBeamBotLevel1.HostId = rebarBeam.BeamId;
            rebarBeamSection.RebarBeamBot.RebarBeamBotLevel1.Diameter = RebarDiameters.FirstOrDefault();
            rebarBeamSection.RebarBeamBot.RebarBeamBotLevel1.Quantity = 3;
            rebarBeamSection.RebarBeamBot.RebarBeamBotLevel1.RebarBeamType = 3;
            rebarBeamSection.RebarBeamBot.RebarBeamBotLevel1.RebarLevelType = (int)RebarBeamMainBarLevelType.RebarBot;
            rebarBeamSection.RebarBeamBot.RebarBeamBotLevel1.RebarGroupType = (int)RebarBeamMainBarGroupType.GroupLevel1;
            rebarBeamSection.RebarBeamBot.RebarBeamBotLevelActive = rebarBeamSection.RebarBeamBot.RebarBeamBotLevel1;

            rebarBeamSection.RebarBeamBot.RebarBeamBotLevel2 = new RebarBeamMainBar();
            rebarBeamSection.RebarBeamBot.RebarBeamBotLevel2.HostId = rebarBeam.BeamId;
            rebarBeamSection.RebarBeamBot.RebarBeamBotLevel2.Diameter = RebarDiameters.FirstOrDefault();
            rebarBeamSection.RebarBeamBot.RebarBeamBotLevel2.Quantity = 3;
            rebarBeamSection.RebarBeamBot.RebarBeamBotLevel2.RebarBeamType = 3;
            rebarBeamSection.RebarBeamBot.RebarBeamBotLevel2.RebarLevelType = (int)RebarBeamMainBarLevelType.RebarBot;
            rebarBeamSection.RebarBeamBot.RebarBeamBotLevel2.RebarGroupType = (int)RebarBeamMainBarGroupType.GroupLevel2;

            rebarBeamSection.RebarBeamBot.RebarBeamBotLevel3 = new RebarBeamMainBar();
            rebarBeamSection.RebarBeamBot.RebarBeamBotLevel3.HostId = rebarBeam.BeamId;
            rebarBeamSection.RebarBeamBot.RebarBeamBotLevel3.Diameter = RebarDiameters.FirstOrDefault();
            rebarBeamSection.RebarBeamBot.RebarBeamBotLevel3.Quantity = 3;
            rebarBeamSection.RebarBeamBot.RebarBeamBotLevel3.RebarBeamType = 3;
            rebarBeamSection.RebarBeamBot.RebarBeamBotLevel3.RebarLevelType = (int)RebarBeamMainBarLevelType.RebarBot;
            rebarBeamSection.RebarBeamBot.RebarBeamBotLevel3.RebarGroupType = (int)RebarBeamMainBarGroupType.GroupLevel2;
        }
    }
}
