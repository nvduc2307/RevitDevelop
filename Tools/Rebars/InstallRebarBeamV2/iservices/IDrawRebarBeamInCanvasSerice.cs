using Autodesk.Revit.DB.Structure;
using RevitDevelop.Tools.Rebars.InstallRebarBeamV2.models;
using RevitDevelop.Tools.Rebars.InstallRebarBeamV2.viewModels;
using System.Windows;

namespace RevitDevelop.Tools.Rebars.InstallRebarBeamV2.iservices
{
    public interface IDrawRebarBeamInCanvasSerice
    {
        public void DrawSectionBeamConcrete(RebarBeam rebarBeam, InstallRebarBeamV2ViewModel installRebarBeamV2ViewModel);
        public void DrawSectionBeamStirrup(RebarBeam rebarBeam, InstallRebarBeamV2ViewModel installRebarBeamV2ViewModel);
        public List<UIElement> DrawSectionBeamMainBarTop(RebarBeam rebarBeam, InstallRebarBeamV2ViewModel installRebarBeamV2ViewModel);
        public List<UIElement> DrawSectionBeamMainBarBot(RebarBeam rebarBeam, InstallRebarBeamV2ViewModel installRebarBeamV2ViewModel);
        public List<UIElement> DrawSectionBeamSideBar(RebarBeam rebarBeam, InstallRebarBeamV2ViewModel installRebarBeamV2ViewModel);
    }
}
