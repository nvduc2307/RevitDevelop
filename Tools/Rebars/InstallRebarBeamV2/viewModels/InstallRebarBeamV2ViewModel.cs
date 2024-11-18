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
            CanvasPageSectionStart = new CanvasPageBase(MainView.FindName("CanvasSectionStart") as Canvas);
            CanvasPageSectionMid = new CanvasPageBase(MainView.FindName("CanvasSectionMid") as Canvas);
            CanvasPageSectionEnd = new CanvasPageBase(MainView.FindName("CanvasSectionEnd") as Canvas);
            _drawRebarBeamInCanvas.DrawSectionBeamConcrete(ElementInstances.RebarBeamActive);
        }
        private void InitAction()
        {

        }
    }
}
