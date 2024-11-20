using RevitDevelop.Tools.Rebars.InstallRebarBeamV2.models;
using RevitDevelop.Tools.Rebars.InstallRebarBeamV2.viewModels;
using System.Windows;

namespace RevitDevelop.Tools.Rebars.InstallRebarBeamV2.iservices
{
    public interface IDrawRebarBeamInCanvas
    {
        public void DrawSectionBeamConcrete(RebarBeam rebarBeam, InstallRebarBeamV2ViewModel installRebarBeamV2ViewModel);
        public void DrawSectionBeamStirrup(RebarBeam rebarBeam, InstallRebarBeamV2ViewModel installRebarBeamV2ViewModel);
        public List<UIElement> DrawSectionBeammMainBarTop(RebarBeam rebarBeam, InstallRebarBeamV2ViewModel installRebarBeamV2ViewModel);
        public List<UIElement> DrawSectionBeammMainBarBot(RebarBeam rebarBeam, InstallRebarBeamV2ViewModel installRebarBeamV2ViewModel);
    }
}
