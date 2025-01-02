using RevitDevelop.Tools.Rebars.InstallRebarBeamV2.iservices;
using RevitDevelop.Tools.Rebars.InstallRebarBeamV2.models;
using RevitDevelop.Tools.Rebars.InstallRebarBeamV2.viewModels;

namespace RevitDevelop.Tools.Rebars.InstallRebarBeamV2.middleWares
{
    public class InstallRebarBeamV2MiddleWare
    {
        private ISubInstallRebarBeamInModelService _subInstallRebarBeamInModelService;
        public InstallRebarBeamV2MiddleWare(ISubInstallRebarBeamInModelService subInstallRebarBeamInModelService)
        {
            _subInstallRebarBeamInModelService = subInstallRebarBeamInModelService;
        }
        public bool RebarQuantityNotMatch(
            InstallRebarBeamV2ViewModel installRebarBeamV2ViewModel,
            RebarBeamMainBarLevelType rebarBeamMainBarLevelType,
            RebarBeamMainBarGroupType rebarBeamMainBarGroupType)
        {
            try
            {
                var rebarGroups = _subInstallRebarBeamInModelService.GetRebarBeamGroupLevelInfo(
                    installRebarBeamV2ViewModel,
                    rebarBeamMainBarLevelType,
                    rebarBeamMainBarGroupType);
                return rebarGroups.GroupBy(x => x.Quantity).Count() == 1;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
