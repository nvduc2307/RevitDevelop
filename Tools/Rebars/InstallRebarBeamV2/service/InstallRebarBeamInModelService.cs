using Autodesk.Revit.DB.Structure;
using RevitDevelop.Tools.Rebars.InstallRebarBeamV2.exceptions;
using RevitDevelop.Tools.Rebars.InstallRebarBeamV2.iservices;
using RevitDevelop.Tools.Rebars.InstallRebarBeamV2.middleWares;
using RevitDevelop.Tools.Rebars.InstallRebarBeamV2.models;
using RevitDevelop.Tools.Rebars.InstallRebarBeamV2.viewModels;
using Utils.Messages;

namespace RevitDevelop.Tools.Rebars.InstallRebarBeamV2.service
{
    public class InstallRebarBeamInModelService : IInstallRebarBeamInModelService
    {
        private InstallRebarBeamV2MiddleWare _installRebarBeamV2MiddleWare;
        public InstallRebarBeamInModelService(InstallRebarBeamV2MiddleWare installRebarBeamV2MiddleWare)
        {
            _installRebarBeamV2MiddleWare = installRebarBeamV2MiddleWare;
        }
        public List<Rebar> InstallRebarBot1(InstallRebarBeamV2ViewModel installRebarBeamV2ViewModel)
        {
            var result = new List<Rebar>();
            var isQTYMatch = _installRebarBeamV2MiddleWare.RebarQuantityNotMatch(
                installRebarBeamV2ViewModel,
                RebarBeamMainBarLevelType.RebarTop,
                RebarBeamMainBarGroupType.GroupLevel1);
            if (!isQTYMatch) throw new Exception_Rebar_Top_1_Quantity_Not_Match();
            try
            {
            }
            catch (Exception ex)
            {
                IO.ShowWarning(ex.Message);
            }
            return result;
        }

        public List<Rebar> InstallRebarBot2(InstallRebarBeamV2ViewModel installRebarBeamV2ViewModel)
        {
            throw new NotImplementedException();
        }

        public List<Rebar> InstallRebarBot3(InstallRebarBeamV2ViewModel installRebarBeamV2ViewModel)
        {
            throw new NotImplementedException();
        }

        public List<Rebar> InstallRebarSide(InstallRebarBeamV2ViewModel installRebarBeamV2ViewModel)
        {
            throw new NotImplementedException();
        }

        public List<Rebar> InstallRebarStirrup(InstallRebarBeamV2ViewModel installRebarBeamV2ViewModel)
        {
            throw new NotImplementedException();
        }

        public List<Rebar> InstallRebarTop1(InstallRebarBeamV2ViewModel installRebarBeamV2ViewModel)
        {
            throw new NotImplementedException();
        }

        public List<Rebar> InstallRebarTop2(InstallRebarBeamV2ViewModel installRebarBeamV2ViewModel)
        {
            throw new NotImplementedException();
        }

        public List<Rebar> InstallRebarTop3(InstallRebarBeamV2ViewModel installRebarBeamV2ViewModel)
        {
            throw new NotImplementedException();
        }
    }
}
