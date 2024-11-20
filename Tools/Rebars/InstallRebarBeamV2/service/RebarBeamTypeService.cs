using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.UI;
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
                elementInstances.RebarBeams = elementInstances.Beam.ElementSubs?.Select(x => new RebarBeam(x)).ToList();
                elementInstances.InitDataRebarBeam();
                elementInstances.RebarBeamActive = elementInstances.RebarBeams.FirstOrDefault();
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
                if (isRebarBeamTypeExist != null) {
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
                var rebarBeam = rebarBeamTypes.FirstOrDefault(x=>x.NameType == rebarBeamSave.NameType);
                var indexOf = rebarBeamTypes.IndexOf(rebarBeam);
                rebarBeamTypes.Insert(indexOf, rebarBeamSave);
                rebarBeamTypes.RemoveAt(indexOf + 1);
                var content = JsonConvert.SerializeObject(rebarBeamTypes);
                File.WriteAllText(pathSave, content);
            }
            catch (Exception)
            {
            }
            return [.. rebarBeamTypes];
        }

        public RebarBeam SaveAs(List<RebarBeam> rebarBeamTypes, string nameType, string pathSave)
        {
            var result = rebarBeamTypes.FirstOrDefault();
            try
            {
                if (string.IsNullOrEmpty(nameType)) throw new Exception("nameType is not empty");
                var isRebarBeamTypeExist = rebarBeamTypes.Any(x=>x.NameType == nameType);
                if (isRebarBeamTypeExist) throw new Exception("Type is existed");
                var rebarBeamType = new RebarBeam
                {
                    NameType = nameType
                };
                rebarBeamTypes.Add(rebarBeamType);
                List<RebarBeam> rebarBeamTypesN = [.. rebarBeamTypes];
                foreach (var item in rebarBeamTypesN)
                {
                    RebarBeam.ResetActionChange(item);
                }
                var content = JsonConvert.SerializeObject(rebarBeamTypesN);
                File.WriteAllText(pathSave, content);
                result = rebarBeamType;
            }
            catch (Exception ex)
            {
                IO.ShowWarning(ex.Message);
                result = rebarBeamTypes.FirstOrDefault();
            }
            return result;
        }
    }
}
