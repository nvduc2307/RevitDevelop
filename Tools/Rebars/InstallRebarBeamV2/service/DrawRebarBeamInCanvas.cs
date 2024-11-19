using RevitDevelop.Tools.Rebars.InstallRebarBeamV2.iservices;
using RevitDevelop.Tools.Rebars.InstallRebarBeamV2.models;
using RevitDevelop.Tools.Rebars.InstallRebarBeamV2.viewModels;
using Utils.canvass;

namespace RevitDevelop.Tools.Rebars.InstallRebarBeamV2.service
{
    public class DrawRebarBeamInCanvas : IDrawRebarBeamInCanvas
    {
        private InstallRebarBeamV2ViewModel _installRebarBeamV2ViewModel;
        public DrawRebarBeamInCanvas(InstallRebarBeamV2ViewModel installRebarBeamV2ViewModel)
        {
            _installRebarBeamV2ViewModel = installRebarBeamV2ViewModel;
        }
        public void DrawSectionBeamConcrete(RebarBeam rebarBeam)
        {
            var canvasStart = _installRebarBeamV2ViewModel.CanvasPageSectionStart;
            var canvasMid = _installRebarBeamV2ViewModel.CanvasPageSectionMid;
            var canvasEnd = _installRebarBeamV2ViewModel.CanvasPageSectionEnd;
            _drawSectionBeamConcrete(rebarBeam, canvasStart);
            _drawSectionBeamConcrete(rebarBeam, canvasMid);
            _drawSectionBeamConcrete(rebarBeam, canvasEnd);

        }
        private void _drawSectionBeamConcrete(RebarBeam rebarBeam, CanvasPageBase canvasPageBase)
        {
            var centerCanvas = canvasPageBase.Center;
            var option = OptionStyleInstanceInCanvas.OPTION_CONCRETE_STRUCTURE;
            var ps = new List<System.Windows.Point>() {
                centerCanvas,
                new System.Windows.Point(),
                new System.Windows.Point(100, 200),
            };
            var sectionBeam = new InstanceInCanvasPolygon(canvasPageBase, option, ps);
            sectionBeam.DrawInCanvas();
        }
    }
}
