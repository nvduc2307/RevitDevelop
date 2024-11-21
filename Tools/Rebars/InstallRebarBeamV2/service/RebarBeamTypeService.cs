using Newtonsoft.Json;
using RevitDevelop.Tools.Rebars.InstallRebarBeamV2.iservices;
using RevitDevelop.Tools.Rebars.InstallRebarBeamV2.models;
using System.IO;
using Utils.Messages;

namespace RevitDevelop.Tools.Rebars.InstallRebarBeamV2.service
{
    public class RebarBeamTypeService : IRebarBeamTypeService
    {
        public void Apply(ElementInstances elementInstances)
        {
            try
            {
                if (elementInstances.RebarBeamTypeSelected.RebarBeamSectionStart == null) throw new Exception("Khong co du lieu, vui long setting va luu du lieu truoc khi apply");
                if (elementInstances.RebarBeamTypeSelected.RebarBeamSectionMid == null) throw new Exception("Khong co du lieu, vui long setting va luu du lieu truoc khi apply");
                if (elementInstances.RebarBeamTypeSelected.RebarBeamSectionEnd == null) throw new Exception("Khong co du lieu, vui long setting va luu du lieu truoc khi apply");
            }
            catch (Exception ex)
            {
                IO.ShowWarning(ex.Message);
            }
        }

        public List<RebarBeam> Delete(List<RebarBeam> rebarBeamTypes, string nameType, string pathSave)
        {
            try
            {
                if (rebarBeamTypes.Count == 0) throw new Exception("Khong co type nao");
                var isRebarBeamTypeExist = rebarBeamTypes.FirstOrDefault(x => x.NameType == nameType);
                if (isRebarBeamTypeExist != null)
                {
                    rebarBeamTypes.Remove(isRebarBeamTypeExist);
                    List<RebarBeam> rebarBeamTypesN = [.. rebarBeamTypes];
                    foreach (var item in rebarBeamTypesN)
                    {
                        RebarBeam.ResetActionChange(item);
                    }
                    var content = JsonConvert.SerializeObject(rebarBeamTypesN);
                    File.WriteAllText(pathSave, content);
                }
            }
            catch (Exception ex)
            {
                IO.ShowWarning(ex.Message);
            }
            return rebarBeamTypes.ToList();
        }

        public List<RebarBeam> Save(List<RebarBeam> rebarBeamTypes, RebarBeam rebarBeamSave, string pathSave)
        {
            try
            {

            }
            catch (Exception)
            {
            }
            return rebarBeamTypes;
        }

        public List<RebarBeam> SaveAs(List<RebarBeam> rebarBeamTypes, string nameType, string pathSave)
        {
            List<RebarBeam> results = [.. rebarBeamTypes];
            try
            {
                if (string.IsNullOrEmpty(nameType)) throw new Exception("nameType is not empty");
                var isRebarBeamTypeExist = rebarBeamTypes.Any(x => x.NameType == nameType);
                if (isRebarBeamTypeExist) throw new Exception("Type is existed");
                var rebarBeamType = new RebarBeam
                {
                    NameType = nameType
                };
                results.Add(rebarBeamType);
                foreach (var item in results)
                {
                    RebarBeam.ResetActionChange(item);
                }
                var content = JsonConvert.SerializeObject(results);
                File.WriteAllText(pathSave, content);
            }
            catch (Exception ex)
            {
                IO.ShowWarning(ex.Message);
                results = rebarBeamTypes;
            }
            return results;
        }
    }
}
