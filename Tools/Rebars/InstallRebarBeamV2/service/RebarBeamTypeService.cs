using Newtonsoft.Json;
using RevitDevelop.Tools.Rebars.InstallRebarBeamV2.iservices;
using RevitDevelop.Tools.Rebars.InstallRebarBeamV2.models;
using RevitDevelop.Tools.Rebars.InstallRebarBeamV2.viewModels;
using System.IO;
using Utils.Messages;

namespace RevitDevelop.Tools.Rebars.InstallRebarBeamV2.service
{
    public class RebarBeamTypeService : IRebarBeamTypeService
    {
        private IDrawRebarBeamInCanvasSerice _drawRebarBeamInCanvasSerice;
        public RebarBeamTypeService(
            IDrawRebarBeamInCanvasSerice drawRebarBeamInCanvasSerice)
        {
            _drawRebarBeamInCanvasSerice = drawRebarBeamInCanvasSerice;
        }
        public void Apply(InstallRebarBeamV2ViewModel installRebarBeamV2ViewModel)
        {
            try
            {
                var elementInstances = installRebarBeamV2ViewModel.ElementInstances;
                if (elementInstances.RebarBeamTypeSelected.RebarBeamSectionStart == null) throw new Exception("Khong co du lieu, vui long setting va luu du lieu truoc khi apply");
                if (elementInstances.RebarBeamTypeSelected.RebarBeamSectionMid == null) throw new Exception("Khong co du lieu, vui long setting va luu du lieu truoc khi apply");
                if (elementInstances.RebarBeamTypeSelected.RebarBeamSectionEnd == null) throw new Exception("Khong co du lieu, vui long setting va luu du lieu truoc khi apply");
                elementInstances.RebarBeams = elementInstances.Beam.ElementSubs?.Select(x => new RebarBeam(x)).ToList();
                elementInstances.InitDataRebarBeam();
                elementInstances.RebarBeamActive = elementInstances.RebarBeams.FirstOrDefault();
                elementInstances.MainRebarTopUIElement = _drawRebarBeamInCanvasSerice.DrawSectionBeamMainBarTop(elementInstances.RebarBeamActive, installRebarBeamV2ViewModel);
                elementInstances.MainRebarBotUIElement = _drawRebarBeamInCanvasSerice.DrawSectionBeamMainBarBot(elementInstances.RebarBeamActive, installRebarBeamV2ViewModel);
                elementInstances.SideBarUIElement = _drawRebarBeamInCanvasSerice.DrawSectionBeamSideBar(elementInstances.RebarBeamActive, installRebarBeamV2ViewModel);
            }
            catch (Exception ex)
            {
                IO.ShowWarning(ex.Message);
            }
        }

        public void Delete(List<RebarBeam> rebarBeamTypes, string nameType, string pathSave)
        {
            try
            {
                List<RebarBeam> rebarBeams = [.. rebarBeamTypes];
                if (rebarBeams.Count == 0) throw new Exception("Khong co type nao");
                var isRebarBeamTypeExist = rebarBeams.FirstOrDefault(x => x.NameType == nameType);
                if (isRebarBeamTypeExist != null) {
                    rebarBeams.Remove(isRebarBeamTypeExist);
                    var content = JsonConvert.SerializeObject(rebarBeams);
                    File.WriteAllText(pathSave, content);
                }
            }
            catch (Exception ex)
            {
                IO.ShowWarning(ex.Message);
            }
        }

        public void Save(List<RebarBeam> rebarBeamTypes, RebarBeam rebarBeamSave, string pathSave)
        {
            try
            {
                List<RebarBeam> rebarBeams = [.. rebarBeamTypes];
                var rebarBeam = rebarBeams.FirstOrDefault(x=>x.NameType == rebarBeamSave.NameType);
                var indexOf = rebarBeams.IndexOf(rebarBeam);
                rebarBeams.Insert(indexOf, rebarBeamSave);
                rebarBeams.RemoveAt(indexOf + 1);
                var content = JsonConvert.SerializeObject(rebarBeams);
                File.WriteAllText(pathSave, content);
            }
            catch (Exception)
            {
            }
        }

        public void SaveAs(List<RebarBeam> rebarBeamTypes, string nameType, string pathSave)
        {
            try
            {
                List<RebarBeam> rebarBeams = [..rebarBeamTypes];
                if (string.IsNullOrEmpty(nameType)) throw new Exception("nameType is not empty");
                var isRebarBeamTypeExist = rebarBeams.Any(x=>x.NameType == nameType);
                if (isRebarBeamTypeExist) throw new Exception("Type is existed");
                var rebarBeamType = new RebarBeam
                {
                    NameType = nameType
                };
                rebarBeams.Add(rebarBeamType);
                foreach (var item in rebarBeams)
                {
                    RebarBeam.ResetActionChange(item);
                }
                var content = JsonConvert.SerializeObject(rebarBeams);
                File.WriteAllText(pathSave, content);
            }
            catch (Exception ex)
            {
                IO.ShowWarning(ex.Message);
            }
        }
    }
}
