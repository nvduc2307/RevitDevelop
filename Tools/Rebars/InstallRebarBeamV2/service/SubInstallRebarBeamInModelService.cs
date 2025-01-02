using RevitDevelop.Tools.Rebars.InstallRebarBeamV2.iservices;
using RevitDevelop.Tools.Rebars.InstallRebarBeamV2.models;
using RevitDevelop.Tools.Rebars.InstallRebarBeamV2.viewModels;

namespace RevitDevelop.Tools.Rebars.InstallRebarBeamV2.service
{
    public class SubInstallRebarBeamInModelService : ISubInstallRebarBeamInModelService
    {
        public List<RebarBeamSectionStart> GetRebarBeamSectionStart(
            InstallRebarBeamV2ViewModel installRebarBeamV2ViewModel,
            RebarBeamSectionType sectionType)
        {
            var result = new List<RebarBeamSectionStart>();
            try
            {
                switch (sectionType)
                {
                    case RebarBeamSectionType.SectionStart:
                        result = installRebarBeamV2ViewModel
                            .ElementInstances
                            .RebarBeams
                            .Select(x => x.RebarBeamSectionStart)
                            .ToList();
                        break;
                    case RebarBeamSectionType.SectionMid:
                        result = null;
                        break;
                    case RebarBeamSectionType.SectionEnd:
                        result = null;
                        break;
                }
            }
            catch (Exception)
            {
            }
            return result;
        }
        public List<RebarBeamSectionMid> GetRebarBeamSectionMid(InstallRebarBeamV2ViewModel installRebarBeamV2ViewModel, RebarBeamSectionType sectionType)
        {
            var result = new List<RebarBeamSectionMid>();
            try
            {
                switch (sectionType)
                {
                    case RebarBeamSectionType.SectionStart:
                        result = null;
                        break;
                    case RebarBeamSectionType.SectionMid:
                        result = installRebarBeamV2ViewModel
                            .ElementInstances
                            .RebarBeams
                            .Select(x => x.RebarBeamSectionMid)
                            .ToList();
                        break;
                    case RebarBeamSectionType.SectionEnd:
                        result = null;
                        break;
                }
            }
            catch (Exception)
            {
            }
            return result;
        }

        public List<RebarBeamSectionEnd> GetRebarBeamSectionEnd(InstallRebarBeamV2ViewModel installRebarBeamV2ViewModel, RebarBeamSectionType sectionType)
        {
            var result = new List<RebarBeamSectionEnd>();
            try
            {
                switch (sectionType)
                {
                    case RebarBeamSectionType.SectionStart:
                        result = null;
                        break;
                    case RebarBeamSectionType.SectionMid:
                        result = null;
                        break;
                    case RebarBeamSectionType.SectionEnd:
                        result = installRebarBeamV2ViewModel
                            .ElementInstances
                            .RebarBeams
                            .Select(x => x.RebarBeamSectionEnd)
                            .ToList();
                        break;
                }
            }
            catch (Exception)
            {
            }
            return result;
        }

        public List<RebarBeamMainBar> GetRebarBeamGroupInfo(
            InstallRebarBeamV2ViewModel installRebarBeamV2ViewModel,
            RebarBeamSectionType sectionType,
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
                                switch (sectionType)
                                {
                                    case RebarBeamSectionType.SectionStart:
                                        var rebarBeamSectionsStart = GetRebarBeamSectionStart(installRebarBeamV2ViewModel, sectionType);
                                        result = rebarBeamSectionsStart.Select(x => x.RebarBeamTop.RebarBeamTopLevel1).ToList();
                                        break;
                                    case RebarBeamSectionType.SectionMid:
                                        var rebarBeamSectionsMid = GetRebarBeamSectionMid(installRebarBeamV2ViewModel, sectionType);
                                        result = rebarBeamSectionsMid.Select(x => x.RebarBeamTop.RebarBeamTopLevel1).ToList();
                                        break;
                                    case RebarBeamSectionType.SectionEnd:
                                        var rebarBeamSectionsEnd = GetRebarBeamSectionEnd(installRebarBeamV2ViewModel, sectionType);
                                        result = rebarBeamSectionsEnd.Select(x => x.RebarBeamTop.RebarBeamTopLevel1).ToList();
                                        break;
                                }
                                break;
                            case RebarBeamMainBarGroupType.GroupLevel2:
                                switch (sectionType)
                                {
                                    case RebarBeamSectionType.SectionStart:
                                        var rebarBeamSectionsStart = GetRebarBeamSectionStart(installRebarBeamV2ViewModel, sectionType);
                                        result = rebarBeamSectionsStart.Select(x => x.RebarBeamTop.RebarBeamTopLevel2).ToList();
                                        break;
                                    case RebarBeamSectionType.SectionMid:
                                        var rebarBeamSectionsMid = GetRebarBeamSectionMid(installRebarBeamV2ViewModel, sectionType);
                                        result = rebarBeamSectionsMid.Select(x => x.RebarBeamTop.RebarBeamTopLevel2).ToList();
                                        break;
                                    case RebarBeamSectionType.SectionEnd:
                                        var rebarBeamSectionsEnd = GetRebarBeamSectionEnd(installRebarBeamV2ViewModel, sectionType);
                                        result = rebarBeamSectionsEnd.Select(x => x.RebarBeamTop.RebarBeamTopLevel2).ToList();
                                        break;
                                }
                                break;
                            case RebarBeamMainBarGroupType.GroupLevel3:
                                switch (sectionType)
                                {
                                    case RebarBeamSectionType.SectionStart:
                                        var rebarBeamSectionsStart = GetRebarBeamSectionStart(installRebarBeamV2ViewModel, sectionType);
                                        result = rebarBeamSectionsStart.Select(x => x.RebarBeamTop.RebarBeamTopLevel3).ToList();
                                        break;
                                    case RebarBeamSectionType.SectionMid:
                                        var rebarBeamSectionsMid = GetRebarBeamSectionMid(installRebarBeamV2ViewModel, sectionType);
                                        result = rebarBeamSectionsMid.Select(x => x.RebarBeamTop.RebarBeamTopLevel3).ToList();
                                        break;
                                    case RebarBeamSectionType.SectionEnd:
                                        var rebarBeamSectionsEnd = GetRebarBeamSectionEnd(installRebarBeamV2ViewModel, sectionType);
                                        result = rebarBeamSectionsEnd.Select(x => x.RebarBeamTop.RebarBeamTopLevel3).ToList();
                                        break;
                                }
                                break;
                        }
                        break;
                    case RebarBeamMainBarLevelType.RebarBot:
                        switch (rebarBeamMainBarGroupType)
                        {
                            case RebarBeamMainBarGroupType.GroupLevel1:
                                switch (sectionType)
                                {
                                    case RebarBeamSectionType.SectionStart:
                                        var rebarBeamSectionsStart = GetRebarBeamSectionStart(installRebarBeamV2ViewModel, sectionType);
                                        result = rebarBeamSectionsStart.Select(x => x.RebarBeamBot.RebarBeamBotLevel1).ToList();
                                        break;
                                    case RebarBeamSectionType.SectionMid:
                                        var rebarBeamSectionsMid = GetRebarBeamSectionMid(installRebarBeamV2ViewModel, sectionType);
                                        result = rebarBeamSectionsMid.Select(x => x.RebarBeamBot.RebarBeamBotLevel1).ToList();
                                        break;
                                    case RebarBeamSectionType.SectionEnd:
                                        var rebarBeamSectionsEnd = GetRebarBeamSectionEnd(installRebarBeamV2ViewModel, sectionType);
                                        result = rebarBeamSectionsEnd.Select(x => x.RebarBeamBot.RebarBeamBotLevel1).ToList();
                                        break;
                                }
                                break;
                            case RebarBeamMainBarGroupType.GroupLevel2:
                                switch (sectionType)
                                {
                                    case RebarBeamSectionType.SectionStart:
                                        var rebarBeamSectionsStart = GetRebarBeamSectionStart(installRebarBeamV2ViewModel, sectionType);
                                        result = rebarBeamSectionsStart.Select(x => x.RebarBeamBot.RebarBeamBotLevel2).ToList();
                                        break;
                                    case RebarBeamSectionType.SectionMid:
                                        var rebarBeamSectionsMid = GetRebarBeamSectionMid(installRebarBeamV2ViewModel, sectionType);
                                        result = rebarBeamSectionsMid.Select(x => x.RebarBeamBot.RebarBeamBotLevel2).ToList();
                                        break;
                                    case RebarBeamSectionType.SectionEnd:
                                        var rebarBeamSectionsEnd = GetRebarBeamSectionEnd(installRebarBeamV2ViewModel, sectionType);
                                        result = rebarBeamSectionsEnd.Select(x => x.RebarBeamBot.RebarBeamBotLevel2).ToList();
                                        break;
                                }
                                break;
                            case RebarBeamMainBarGroupType.GroupLevel3:
                                switch (sectionType)
                                {
                                    case RebarBeamSectionType.SectionStart:
                                        var rebarBeamSectionsStart = GetRebarBeamSectionStart(installRebarBeamV2ViewModel, sectionType);
                                        result = rebarBeamSectionsStart.Select(x => x.RebarBeamBot.RebarBeamBotLevel3).ToList();
                                        break;
                                    case RebarBeamSectionType.SectionMid:
                                        var rebarBeamSectionsMid = GetRebarBeamSectionMid(installRebarBeamV2ViewModel, sectionType);
                                        result = rebarBeamSectionsMid.Select(x => x.RebarBeamBot.RebarBeamBotLevel3).ToList();
                                        break;
                                    case RebarBeamSectionType.SectionEnd:
                                        var rebarBeamSectionsEnd = GetRebarBeamSectionEnd(installRebarBeamV2ViewModel, sectionType);
                                        result = rebarBeamSectionsEnd.Select(x => x.RebarBeamBot.RebarBeamBotLevel3).ToList();
                                        break;
                                }
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

        public List<RebarBeamMainBar> GetRebarBeamGroupLevelInfo(
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

        public List<RebarBeamStirrup> GetStirrupGroupInfo(
            InstallRebarBeamV2ViewModel installRebarBeamV2ViewModel,
            RebarBeamSectionType sectionType)
        {
            var result = new List<RebarBeamStirrup>();
            try
            {
                switch (sectionType)
                {
                    case RebarBeamSectionType.SectionStart:
                        var rebarBeamSectionsStart = GetRebarBeamSectionStart(installRebarBeamV2ViewModel, sectionType);
                        result = rebarBeamSectionsStart.Select(x => x.RebarBeamStirrup).ToList();
                        break;
                    case RebarBeamSectionType.SectionMid:
                        var rebarBeamSectionsMid = GetRebarBeamSectionMid(installRebarBeamV2ViewModel, sectionType);
                        result = rebarBeamSectionsMid.Select(x => x.RebarBeamStirrup).ToList();
                        break;
                    case RebarBeamSectionType.SectionEnd:
                        var rebarBeamSectionsEnd = GetRebarBeamSectionEnd(installRebarBeamV2ViewModel, sectionType);
                        result = rebarBeamSectionsEnd.Select(x => x.RebarBeamStirrup).ToList();
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
