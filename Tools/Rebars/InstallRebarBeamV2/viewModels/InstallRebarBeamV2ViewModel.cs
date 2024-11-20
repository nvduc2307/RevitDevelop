using RevitDevelop.Tools.Rebars.InstallRebarBeamV2.iservices;
using RevitDevelop.Tools.Rebars.InstallRebarBeamV2.models;
using RevitDevelop.Tools.Rebars.InstallRebarBeamV2.views;
using System.Windows.Controls;
using Utils.canvass;

namespace RevitDevelop.Tools.Rebars.InstallRebarBeamV2.viewModels
{
    public class InstallRebarBeamV2ViewModel : ObservableObject
    {
        public ElementInstances ElementInstances { get; set; }
        public InstallRebarBeamView MainView { get; set; }
        public SettingRebarSection SettingRebarSection { get; set; }
        public CanvasPageBase CanvasPageSectionStart { get; set; }
        public CanvasPageBase CanvasPageSectionMid { get; set; }
        public CanvasPageBase CanvasPageSectionEnd { get; set; }
        private IDrawRebarBeamInCanvas _drawRebarBeamInCanvas;
        public InstallRebarBeamV2ViewModel(IDrawRebarBeamInCanvas drawRebarBeamInCanvas)
        {
            ElementInstances = new ElementInstances();
            MainView = new InstallRebarBeamView() { DataContext = this };
            MainView.Loaded += MainView_Loaded;
            InitAction();
            _drawRebarBeamInCanvas = drawRebarBeamInCanvas;
        }

        private void MainView_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            var settingRebarSection = MainView.FindName("SettingRebarSection") as SettingRebarSection;
            CanvasPageSectionStart = new CanvasPageBase(settingRebarSection.FindName("CanvasSectionStart") as Canvas);
            CanvasPageSectionMid = new CanvasPageBase(settingRebarSection.FindName("CanvasSectionMid") as Canvas);
            CanvasPageSectionEnd = new CanvasPageBase(settingRebarSection.FindName("CanvasSectionEnd") as Canvas);
            _drawRebarBeamInCanvas.DrawSectionBeamConcrete(ElementInstances.RebarBeamActive, this);
            _drawRebarBeamInCanvas.DrawSectionBeamStirrup(ElementInstances.RebarBeamActive, this);
            ElementInstances.MainRebarSectionTop = _drawRebarBeamInCanvas.DrawSectionBeammMainBarTop(ElementInstances.RebarBeamActive, this);
            ElementInstances.MainRebarSectionBot = _drawRebarBeamInCanvas.DrawSectionBeammMainBarBot(ElementInstances.RebarBeamActive, this);
        }
        private void InitAction()
        {
            foreach (var rebarBeam in ElementInstances.RebarBeams)
            {
                rebarBeam.RebarBeamSectionStart.RebarBeamTop.RebarBeamTopLevel1.QuantityChange = () =>
                {
                    ElementInstances.MainRebarSectionTop = _drawRebarBeamInCanvas.DrawSectionBeammMainBarTop(ElementInstances.RebarBeamActive, this);
                };
                rebarBeam.RebarBeamSectionMid.RebarBeamTop.RebarBeamTopLevel1.QuantityChange = () =>
                {
                    ElementInstances.MainRebarSectionTop = _drawRebarBeamInCanvas.DrawSectionBeammMainBarTop(ElementInstances.RebarBeamActive, this);
                };
                rebarBeam.RebarBeamSectionEnd.RebarBeamTop.RebarBeamTopLevel1.QuantityChange = () =>
                {
                    ElementInstances.MainRebarSectionTop = _drawRebarBeamInCanvas.DrawSectionBeammMainBarTop(ElementInstances.RebarBeamActive, this);
                };

                rebarBeam.RebarBeamSectionStart.RebarBeamTop.RebarBeamTopLevel2.QuantityChange = () =>
                {
                    ElementInstances.MainRebarSectionTop = _drawRebarBeamInCanvas.DrawSectionBeammMainBarTop(ElementInstances.RebarBeamActive, this);
                };
                rebarBeam.RebarBeamSectionMid.RebarBeamTop.RebarBeamTopLevel2.QuantityChange = () =>
                {
                    ElementInstances.MainRebarSectionTop = _drawRebarBeamInCanvas.DrawSectionBeammMainBarTop(ElementInstances.RebarBeamActive, this);
                };
                rebarBeam.RebarBeamSectionEnd.RebarBeamTop.RebarBeamTopLevel2.QuantityChange = () =>
                {
                    ElementInstances.MainRebarSectionTop = _drawRebarBeamInCanvas.DrawSectionBeammMainBarTop(ElementInstances.RebarBeamActive, this);
                };

                rebarBeam.RebarBeamSectionStart.RebarBeamTop.RebarBeamTopLevel3.QuantityChange = () =>
                {
                    ElementInstances.MainRebarSectionTop = _drawRebarBeamInCanvas.DrawSectionBeammMainBarTop(ElementInstances.RebarBeamActive, this);
                };
                rebarBeam.RebarBeamSectionMid.RebarBeamTop.RebarBeamTopLevel3.QuantityChange = () =>
                {
                    ElementInstances.MainRebarSectionTop = _drawRebarBeamInCanvas.DrawSectionBeammMainBarTop(ElementInstances.RebarBeamActive, this);
                };
                rebarBeam.RebarBeamSectionEnd.RebarBeamTop.RebarBeamTopLevel3.QuantityChange = () =>
                {
                    ElementInstances.MainRebarSectionTop = _drawRebarBeamInCanvas.DrawSectionBeammMainBarTop(ElementInstances.RebarBeamActive, this);
                };

                rebarBeam.RebarBeamSectionStart.RebarBeamBot.RebarBeamBotLevel1.QuantityChange = () =>
                {
                    ElementInstances.MainRebarSectionBot = _drawRebarBeamInCanvas.DrawSectionBeammMainBarBot(ElementInstances.RebarBeamActive, this);
                };
                rebarBeam.RebarBeamSectionMid.RebarBeamBot.RebarBeamBotLevel1.QuantityChange = () =>
                {
                    ElementInstances.MainRebarSectionBot = _drawRebarBeamInCanvas.DrawSectionBeammMainBarBot(ElementInstances.RebarBeamActive, this);
                };
                rebarBeam.RebarBeamSectionEnd.RebarBeamBot.RebarBeamBotLevel1.QuantityChange = () =>
                {
                    ElementInstances.MainRebarSectionBot = _drawRebarBeamInCanvas.DrawSectionBeammMainBarBot(ElementInstances.RebarBeamActive, this);
                };

                rebarBeam.RebarBeamSectionStart.RebarBeamBot.RebarBeamBotLevel2.QuantityChange = () =>
                {
                    ElementInstances.MainRebarSectionBot = _drawRebarBeamInCanvas.DrawSectionBeammMainBarBot(ElementInstances.RebarBeamActive, this);
                };
                rebarBeam.RebarBeamSectionMid.RebarBeamBot.RebarBeamBotLevel2.QuantityChange = () =>
                {
                    ElementInstances.MainRebarSectionBot = _drawRebarBeamInCanvas.DrawSectionBeammMainBarBot(ElementInstances.RebarBeamActive, this);
                };
                rebarBeam.RebarBeamSectionEnd.RebarBeamBot.RebarBeamBotLevel2.QuantityChange = () =>
                {
                    ElementInstances.MainRebarSectionBot = _drawRebarBeamInCanvas.DrawSectionBeammMainBarBot(ElementInstances.RebarBeamActive, this);
                };

                rebarBeam.RebarBeamSectionStart.RebarBeamBot.RebarBeamBotLevel3.QuantityChange = () =>
                {
                    ElementInstances.MainRebarSectionBot = _drawRebarBeamInCanvas.DrawSectionBeammMainBarBot(ElementInstances.RebarBeamActive, this);
                };
                rebarBeam.RebarBeamSectionMid.RebarBeamBot.RebarBeamBotLevel3.QuantityChange = () =>
                {
                    ElementInstances.MainRebarSectionBot = _drawRebarBeamInCanvas.DrawSectionBeammMainBarBot(ElementInstances.RebarBeamActive, this);
                };
                rebarBeam.RebarBeamSectionEnd.RebarBeamBot.RebarBeamBotLevel3.QuantityChange = () =>
                {
                    ElementInstances.MainRebarSectionBot = _drawRebarBeamInCanvas.DrawSectionBeammMainBarBot(ElementInstances.RebarBeamActive, this);
                };
            }
        }
    }
}
