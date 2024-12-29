using RevitDevelop.Tools.Rebars.InstallRebarBeamV2.iservices;
using RevitDevelop.Tools.Rebars.InstallRebarBeamV2.models;
using RevitDevelop.Tools.Rebars.InstallRebarBeamV2.viewModels;

namespace RevitDevelop.Tools.Rebars.InstallRebarBeamV2.middleWares
{
    public class InstallRebarBeamV2MiddleWare
    {
        public bool RebarQuantityNotMatch(
            InstallRebarBeamV2ViewModel installRebarBeamV2ViewModel,
            RebarBeamMainBarLevelType rebarBeamMainBarLevelType,
            RebarBeamMainBarGroupType rebarBeamMainBarGroupType)
        {
            try
            {
                var rebarGroups = GetRebarBeamGroupLevel(
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
        private List<RebarBeamMainBar> GetRebarBeamGroupLevel(
            InstallRebarBeamV2ViewModel installRebarBeamV2ViewModel,
            RebarBeamMainBarLevelType rebarBeamMainBarLevelType,
            RebarBeamMainBarGroupType rebarBeamMainBarGroupType)
        {
            var result = new List<RebarBeamMainBar>();
            try
            {
                switch (rebarBeamMainBarLevelType)
                {
                    case RebarBeamMainBarLevelType.RebarTop:
                        switch (rebarBeamMainBarGroupType)
                        {
                            case RebarBeamMainBarGroupType.GroupLevel1:
                                result.AddRange(
                                    installRebarBeamV2ViewModel
                                    .ElementInstances.RebarBeams
                                    .Select(x => x.RebarBeamSectionStart.RebarBeamTop.RebarBeamTopLevel1)
                                    .ToList());
                                result.AddRange(
                                    installRebarBeamV2ViewModel
                                    .ElementInstances.RebarBeams
                                    .Select(x => x.RebarBeamSectionMid.RebarBeamTop.RebarBeamTopLevel1)
                                    .ToList());
                                result.AddRange(
                                    installRebarBeamV2ViewModel
                                    .ElementInstances.RebarBeams
                                    .Select(x => x.RebarBeamSectionEnd.RebarBeamTop.RebarBeamTopLevel1)
                                    .ToList());
                                break;
                            case RebarBeamMainBarGroupType.GroupLevel2:
                                result.AddRange(
                                    installRebarBeamV2ViewModel
                                    .ElementInstances.RebarBeams
                                    .Select(x => x.RebarBeamSectionStart.RebarBeamTop.RebarBeamTopLevel2)
                                    .ToList());
                                result.AddRange(
                                    installRebarBeamV2ViewModel
                                    .ElementInstances.RebarBeams
                                    .Select(x => x.RebarBeamSectionMid.RebarBeamTop.RebarBeamTopLevel2)
                                    .ToList());
                                result.AddRange(
                                    installRebarBeamV2ViewModel
                                    .ElementInstances.RebarBeams
                                    .Select(x => x.RebarBeamSectionEnd.RebarBeamTop.RebarBeamTopLevel2)
                                    .ToList());
                                break;
                            case RebarBeamMainBarGroupType.GroupLevel3:
                                result.AddRange(
                                    installRebarBeamV2ViewModel
                                    .ElementInstances.RebarBeams
                                    .Select(x => x.RebarBeamSectionStart.RebarBeamTop.RebarBeamTopLevel3)
                                    .ToList());
                                result.AddRange(
                                    installRebarBeamV2ViewModel
                                    .ElementInstances.RebarBeams
                                    .Select(x => x.RebarBeamSectionMid.RebarBeamTop.RebarBeamTopLevel3)
                                    .ToList());
                                result.AddRange(
                                    installRebarBeamV2ViewModel
                                    .ElementInstances.RebarBeams
                                    .Select(x => x.RebarBeamSectionEnd.RebarBeamTop.RebarBeamTopLevel3)
                                    .ToList());
                                break;
                        }
                        break;
                    case RebarBeamMainBarLevelType.RebarBot:
                        switch (rebarBeamMainBarGroupType)
                        {
                            case RebarBeamMainBarGroupType.GroupLevel1:
                                result.AddRange(
                                    installRebarBeamV2ViewModel
                                    .ElementInstances.RebarBeams
                                    .Select(x => x.RebarBeamSectionStart.RebarBeamBot.RebarBeamBotLevel1)
                                    .ToList());
                                result.AddRange(
                                    installRebarBeamV2ViewModel
                                    .ElementInstances.RebarBeams
                                    .Select(x => x.RebarBeamSectionMid.RebarBeamBot.RebarBeamBotLevel1)
                                    .ToList());
                                result.AddRange(
                                    installRebarBeamV2ViewModel
                                    .ElementInstances.RebarBeams
                                    .Select(x => x.RebarBeamSectionEnd.RebarBeamBot.RebarBeamBotLevel1)
                                    .ToList());
                                break;
                            case RebarBeamMainBarGroupType.GroupLevel2:
                                result.AddRange(
                                    installRebarBeamV2ViewModel
                                    .ElementInstances.RebarBeams
                                    .Select(x => x.RebarBeamSectionStart.RebarBeamBot.RebarBeamBotLevel2)
                                    .ToList());
                                result.AddRange(
                                    installRebarBeamV2ViewModel
                                    .ElementInstances.RebarBeams
                                    .Select(x => x.RebarBeamSectionMid.RebarBeamBot.RebarBeamBotLevel2)
                                    .ToList());
                                result.AddRange(
                                    installRebarBeamV2ViewModel
                                    .ElementInstances.RebarBeams
                                    .Select(x => x.RebarBeamSectionEnd.RebarBeamBot.RebarBeamBotLevel2)
                                    .ToList());
                                break;
                            case RebarBeamMainBarGroupType.GroupLevel3:
                                result.AddRange(
                                    installRebarBeamV2ViewModel
                                    .ElementInstances.RebarBeams
                                    .Select(x => x.RebarBeamSectionStart.RebarBeamBot.RebarBeamBotLevel3)
                                    .ToList());
                                result.AddRange(
                                    installRebarBeamV2ViewModel
                                    .ElementInstances.RebarBeams
                                    .Select(x => x.RebarBeamSectionMid.RebarBeamBot.RebarBeamBotLevel3)
                                    .ToList());
                                result.AddRange(
                                    installRebarBeamV2ViewModel
                                    .ElementInstances.RebarBeams
                                    .Select(x => x.RebarBeamSectionEnd.RebarBeamBot.RebarBeamBotLevel3)
                                    .ToList());
                                break;
                        }
                        break;
                }
            }
            catch (Exception)
            {
            }
            return result;
        }
    }
}
