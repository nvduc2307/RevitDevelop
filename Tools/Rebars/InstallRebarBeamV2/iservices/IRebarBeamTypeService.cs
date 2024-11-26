using RevitDevelop.Tools.Rebars.InstallRebarBeamV2.models;

namespace RevitDevelop.Tools.Rebars.InstallRebarBeamV2.iservices
{
    public interface IRebarBeamTypeService
    {
        public void SaveAs(List<RebarBeam> rebarBeamTypes, string nameType, string pathSave);
        public void Apply(ElementInstances elementInstances);
        public void Delete(List<RebarBeam> rebarBeamTypes, string nameType, string pathSave);
        public void Save(List<RebarBeam> rebarBeamTypes, RebarBeam rebarBeamSave, string pathSave);
    }
}
