using RevitDevelop.Tools.Rebars.InstallRebarBeamV2.models;

namespace RevitDevelop.Tools.Rebars.InstallRebarBeamV2.iservices
{
    public interface IRebarBeamTypeService
    {
        public List<RebarBeam> SaveAs(List<RebarBeam> rebarBeamTypes, string nameType, string pathSave);
        public void Apply(ElementInstances elementInstances);
        public List<RebarBeam> Delete(List<RebarBeam> rebarBeamTypes, string nameType, string pathSave);
        public List<RebarBeam> Save(List<RebarBeam> rebarBeamTypes, RebarBeam rebarBeamSave, string pathSave);
    }
}
