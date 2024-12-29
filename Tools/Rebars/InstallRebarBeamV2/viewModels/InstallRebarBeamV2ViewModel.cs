using Newtonsoft.Json;
using RevitDevelop.Tools.Rebars.InstallRebarBeamV2.iservices;
using RevitDevelop.Tools.Rebars.InstallRebarBeamV2.models;
using RevitDevelop.Tools.Rebars.InstallRebarBeamV2.service;
using RevitDevelop.Tools.Rebars.InstallRebarBeamV2.views;
using RevitDevelop.Utils.RevCurves;
using System.IO;
using System.Windows.Controls;
using Utils.canvass;
using Utils.Messages;

namespace RevitDevelop.Tools.Rebars.InstallRebarBeamV2.viewModels
{
    public partial class InstallRebarBeamV2ViewModel : ObservableObject
    {
        private InstallRebarBeamV2Cmd _cmd;
        private IRebarBeamTypeService _rebarBeamTypeService;
        private IDrawRebarBeamInCanvasSerice _drawRebarBeamInCanvasSerice;
        private IInstallRebarBeamInModelService _installRebarBeamInModelService;
        public ElementInstances ElementInstances { get; set; }
        public InstallRebarBeamView MainView { get; set; }
        public SettingRebarSection SettingRebarSection { get; set; }
        public CanvasPageBase CanvasPageSectionStart { get; set; }
        public CanvasPageBase CanvasPageSectionMid { get; set; }
        public CanvasPageBase CanvasPageSectionEnd { get; set; }
        public InstallRebarBeamV2ViewModel(
            InstallRebarBeamV2Cmd cmd,
            IDrawRebarBeamInCanvasSerice drawRebarBeamInCanvas,
            IRebarBeamTypeService rebarBeamTypeService,
            IInstallRebarBeamInModelService installRebarBeamInModelService)
        {
            _cmd = cmd;
            _rebarBeamTypeService = rebarBeamTypeService;
            _drawRebarBeamInCanvasSerice = drawRebarBeamInCanvas;
            _installRebarBeamInModelService = installRebarBeamInModelService;
            ElementInstances = new ElementInstances();
            MainView = new InstallRebarBeamView() { DataContext = this };
            MainView.Loaded += MainView_Loaded;
            InitAction();
        }
        [RelayCommand]
        private void Apply()
        {
            _rebarBeamTypeService.Apply(this);
            InitAction();
        }
        [RelayCommand]
        private void Save()
        {
            RebarBeam.ResetActionChange(ElementInstances.RebarBeamActive);
            ElementInstances.RebarBeamActive.NameType = ElementInstances.RebarBeamTypeSelected.NameType;
            _rebarBeamTypeService.Save(ElementInstances.RebarBeamTypes, ElementInstances.RebarBeamActive, ElementInstances.PathRebarBeamType);
            InitAction();
            ElementInstances.RebarBeamTypes = JsonConvert.DeserializeObject<List<RebarBeam>>(File.ReadAllText(ElementInstances.PathRebarBeamType));
            ElementInstances.RebarBeamTypeSelected = ElementInstances.RebarBeamTypes.FirstOrDefault(x => x.NameType == ElementInstances.RebarBeamActive.NameType)
                ?? ElementInstances.RebarBeamTypes.FirstOrDefault();
        }
        [RelayCommand]
        private void Delete()
        {
            try
            {
                _rebarBeamTypeService.Delete(
                    ElementInstances.RebarBeamTypes,
                    ElementInstances.RebarBeamTypeSelected.NameType,
                    ElementInstances.PathRebarBeamType);
                ElementInstances.RebarBeamTypes = JsonConvert.DeserializeObject<List<RebarBeam>>(File.ReadAllText(ElementInstances.PathRebarBeamType));
                ElementInstances.RebarBeamTypeSelected = ElementInstances.RebarBeamTypes.FirstOrDefault();
            }
            catch (Exception)
            {

            }
        }
        [RelayCommand]
        private void SaveAs()
        {
            try
            {
                _rebarBeamTypeService.SaveAs(
                    ElementInstances.RebarBeamTypes,
                    ElementInstances.RebarBeamTypeName,
                    ElementInstances.PathRebarBeamType);
                ElementInstances.RebarBeamTypeName = string.Empty;

                ElementInstances.RebarBeamTypes = JsonConvert.DeserializeObject<List<RebarBeam>>(File.ReadAllText(ElementInstances.PathRebarBeamType));
                ElementInstances.RebarBeamTypeSelected = ElementInstances.RebarBeamTypes.LastOrDefault();
            }
            catch (Exception)
            {
            }
        }
        [RelayCommand]
        private void OK()
        {
            try
            {
                using (var ts = new Transaction(_cmd.Document, "name transaction"))
                {
                    ts.Start();
                    //--------
                    _installRebarBeamInModelService.InstallRebarBot1(this);
                    //--------
                    ts.Commit();
                }
                MainView.Close();
            }
            catch (Exception ex)
            {
                IO.ShowWarning(ex.Message);
            }
        }
        private void MainView_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            var settingRebarSection = MainView.FindName("SettingRebarSection") as SettingRebarSection;
            CanvasPageSectionStart = new CanvasPageBase(settingRebarSection.FindName("CanvasSectionStart") as Canvas);
            CanvasPageSectionMid = new CanvasPageBase(settingRebarSection.FindName("CanvasSectionMid") as Canvas);
            CanvasPageSectionEnd = new CanvasPageBase(settingRebarSection.FindName("CanvasSectionEnd") as Canvas);
            _drawRebarBeamInCanvasSerice.DrawSectionBeamConcrete(ElementInstances.RebarBeamActive, this);
            _drawRebarBeamInCanvasSerice.DrawSectionBeamStirrup(ElementInstances.RebarBeamActive, this);
            ElementInstances.MainRebarTopUIElement = _drawRebarBeamInCanvasSerice.DrawSectionBeamMainBarTop(ElementInstances.RebarBeamActive, this);
            ElementInstances.MainRebarBotUIElement = _drawRebarBeamInCanvasSerice.DrawSectionBeamMainBarBot(ElementInstances.RebarBeamActive, this);
            ElementInstances.SideBarUIElement = _drawRebarBeamInCanvasSerice.DrawSectionBeamSideBar(ElementInstances.RebarBeamActive, this);
        }
        private void InitAction()
        {
            foreach (var rebarBeam in ElementInstances.RebarBeams)
            {
                rebarBeam.RebarBeamSectionStart.RebarBeamSideBar.QuantitySideChange = () =>
                {
                    ElementInstances.SideBarUIElement = _drawRebarBeamInCanvasSerice.DrawSectionBeamSideBar(ElementInstances.RebarBeamActive, this);
                };
                rebarBeam.RebarBeamSectionMid.RebarBeamSideBar.QuantitySideChange = () =>
                {
                    ElementInstances.SideBarUIElement = _drawRebarBeamInCanvasSerice.DrawSectionBeamSideBar(ElementInstances.RebarBeamActive, this);
                };
                rebarBeam.RebarBeamSectionEnd.RebarBeamSideBar.QuantitySideChange = () =>
                {
                    ElementInstances.SideBarUIElement = _drawRebarBeamInCanvasSerice.DrawSectionBeamSideBar(ElementInstances.RebarBeamActive, this);
                };

                rebarBeam.RebarBeamSectionStart.RebarBeamTop.RebarBeamTopLevel1.QuantityChange = () =>
                {
                    ElementInstances.MainRebarTopUIElement = _drawRebarBeamInCanvasSerice.DrawSectionBeamMainBarTop(ElementInstances.RebarBeamActive, this);
                };
                rebarBeam.RebarBeamSectionMid.RebarBeamTop.RebarBeamTopLevel1.QuantityChange = () =>
                {
                    ElementInstances.MainRebarTopUIElement = _drawRebarBeamInCanvasSerice.DrawSectionBeamMainBarTop(ElementInstances.RebarBeamActive, this);
                };
                rebarBeam.RebarBeamSectionEnd.RebarBeamTop.RebarBeamTopLevel1.QuantityChange = () =>
                {
                    ElementInstances.MainRebarTopUIElement = _drawRebarBeamInCanvasSerice.DrawSectionBeamMainBarTop(ElementInstances.RebarBeamActive, this);
                };

                rebarBeam.RebarBeamSectionStart.RebarBeamTop.RebarBeamTopLevel2.QuantityChange = () =>
                {
                    ElementInstances.MainRebarTopUIElement = _drawRebarBeamInCanvasSerice.DrawSectionBeamMainBarTop(ElementInstances.RebarBeamActive, this);
                };
                rebarBeam.RebarBeamSectionMid.RebarBeamTop.RebarBeamTopLevel2.QuantityChange = () =>
                {
                    ElementInstances.MainRebarTopUIElement = _drawRebarBeamInCanvasSerice.DrawSectionBeamMainBarTop(ElementInstances.RebarBeamActive, this);
                };
                rebarBeam.RebarBeamSectionEnd.RebarBeamTop.RebarBeamTopLevel2.QuantityChange = () =>
                {
                    ElementInstances.MainRebarTopUIElement = _drawRebarBeamInCanvasSerice.DrawSectionBeamMainBarTop(ElementInstances.RebarBeamActive, this);
                };

                rebarBeam.RebarBeamSectionStart.RebarBeamTop.RebarBeamTopLevel3.QuantityChange = () =>
                {
                    ElementInstances.MainRebarTopUIElement = _drawRebarBeamInCanvasSerice.DrawSectionBeamMainBarTop(ElementInstances.RebarBeamActive, this);
                };
                rebarBeam.RebarBeamSectionMid.RebarBeamTop.RebarBeamTopLevel3.QuantityChange = () =>
                {
                    ElementInstances.MainRebarTopUIElement = _drawRebarBeamInCanvasSerice.DrawSectionBeamMainBarTop(ElementInstances.RebarBeamActive, this);
                };
                rebarBeam.RebarBeamSectionEnd.RebarBeamTop.RebarBeamTopLevel3.QuantityChange = () =>
                {
                    ElementInstances.MainRebarTopUIElement = _drawRebarBeamInCanvasSerice.DrawSectionBeamMainBarTop(ElementInstances.RebarBeamActive, this);
                };

                rebarBeam.RebarBeamSectionStart.RebarBeamBot.RebarBeamBotLevel1.QuantityChange = () =>
                {
                    ElementInstances.MainRebarBotUIElement = _drawRebarBeamInCanvasSerice.DrawSectionBeamMainBarBot(ElementInstances.RebarBeamActive, this);
                };
                rebarBeam.RebarBeamSectionMid.RebarBeamBot.RebarBeamBotLevel1.QuantityChange = () =>
                {
                    ElementInstances.MainRebarBotUIElement = _drawRebarBeamInCanvasSerice.DrawSectionBeamMainBarBot(ElementInstances.RebarBeamActive, this);
                };
                rebarBeam.RebarBeamSectionEnd.RebarBeamBot.RebarBeamBotLevel1.QuantityChange = () =>
                {
                    ElementInstances.MainRebarBotUIElement = _drawRebarBeamInCanvasSerice.DrawSectionBeamMainBarBot(ElementInstances.RebarBeamActive, this);
                };

                rebarBeam.RebarBeamSectionStart.RebarBeamBot.RebarBeamBotLevel2.QuantityChange = () =>
                {
                    ElementInstances.MainRebarBotUIElement = _drawRebarBeamInCanvasSerice.DrawSectionBeamMainBarBot(ElementInstances.RebarBeamActive, this);
                };
                rebarBeam.RebarBeamSectionMid.RebarBeamBot.RebarBeamBotLevel2.QuantityChange = () =>
                {
                    ElementInstances.MainRebarBotUIElement = _drawRebarBeamInCanvasSerice.DrawSectionBeamMainBarBot(ElementInstances.RebarBeamActive, this);
                };
                rebarBeam.RebarBeamSectionEnd.RebarBeamBot.RebarBeamBotLevel2.QuantityChange = () =>
                {
                    ElementInstances.MainRebarBotUIElement = _drawRebarBeamInCanvasSerice.DrawSectionBeamMainBarBot(ElementInstances.RebarBeamActive, this);
                };

                rebarBeam.RebarBeamSectionStart.RebarBeamBot.RebarBeamBotLevel3.QuantityChange = () =>
                {
                    ElementInstances.MainRebarBotUIElement = _drawRebarBeamInCanvasSerice.DrawSectionBeamMainBarBot(ElementInstances.RebarBeamActive, this);
                };
                rebarBeam.RebarBeamSectionMid.RebarBeamBot.RebarBeamBotLevel3.QuantityChange = () =>
                {
                    ElementInstances.MainRebarBotUIElement = _drawRebarBeamInCanvasSerice.DrawSectionBeamMainBarBot(ElementInstances.RebarBeamActive, this);
                };
                rebarBeam.RebarBeamSectionEnd.RebarBeamBot.RebarBeamBotLevel3.QuantityChange = () =>
                {
                    ElementInstances.MainRebarBotUIElement = _drawRebarBeamInCanvasSerice.DrawSectionBeamMainBarBot(ElementInstances.RebarBeamActive, this);
                };
            }
        }
    }
}
