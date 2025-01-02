using RevitDevelop.Tools.Rebars.InstallRebarBeamV2.models;
using RevitDevelop.Tools.Rebars.InstallRebarBeamV2.viewModels;

namespace RevitDevelop.Tools.Rebars.InstallRebarBeamV2.iservices
{
    public interface ISubInstallRebarBeamInModelService
    {
        public List<RebarBeamSectionStart> GetRebarBeamSectionStart(
            InstallRebarBeamV2ViewModel installRebarBeamV2ViewModel,
            RebarBeamSectionType sectionType);
        public List<RebarBeamSectionMid> GetRebarBeamSectionMid(
            InstallRebarBeamV2ViewModel installRebarBeamV2ViewModel,
            RebarBeamSectionType sectionType);
        public List<RebarBeamSectionEnd> GetRebarBeamSectionEnd(
            InstallRebarBeamV2ViewModel installRebarBeamV2ViewModel,
            RebarBeamSectionType sectionType);
        public List<RebarBeamMainBar> GetRebarBeamGroupLevelInfo(
            InstallRebarBeamV2ViewModel installRebarBeamV2ViewModel,
            RebarBeamMainBarLevelType rebarBeamMainBarLevelType,
            RebarBeamMainBarGroupType rebarBeamMainBarGroupType);
        public List<RebarBeamMainBar> GetRebarBeamGroupInfo(
            InstallRebarBeamV2ViewModel installRebarBeamV2ViewModel,
            RebarBeamSectionType sectionType,
            RebarBeamMainBarLevelType rebarBeamMainBarLevelType,
            RebarBeamMainBarGroupType rebarBeamMainBarGroupType);
        public List<RebarBeamStirrup> GetStirrupGroupInfo(
            InstallRebarBeamV2ViewModel installRebarBeamV2ViewModel,
            RebarBeamSectionType sectionType);
    }
}
