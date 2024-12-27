using Autodesk.Revit.DB.Structure;
using HcBimUtils;
using Newtonsoft.Json;
using RevitDevelop.AutoCutRebarBeamV2.models;
using RevitDevelop.BeamRebar.ViewModel;
using RevitDevelop.Tools.Modifies.AutoCutRebarBeamV2.iservices;
using RevitDevelop.Utils.RevElements;
using Utils.Entities;
using Utils.RevBoundingBoxs;
using Utils.RevSolids;
using Utils.SelectionFilterInRevit;

namespace RevitDevelop.AutoCutRebarBeamV2.services
{
    public class RebarBeamAssemblyAnalysis : IRebarBeamAssemblyAnalysis
    {
        private AutoCutRebarBeamV2Cmd _cmd;
        private ElementInstance _elementInstance;
        public RebarBeamAssemblyAnalysis(AutoCutRebarBeamV2Cmd cmd, ElementInstance elementInstance)
        {
            _cmd = cmd;
            _elementInstance = elementInstance;
        }

        public List<Rebar> GetRebarsInBeamAssembly(ElementId rebarBeamAssembly)
        {
            var results = new List<Rebar>();
            try
            {
                var rebarAss = _cmd.Document.GetElement(rebarBeamAssembly) as AssemblyInstance;
                results = rebarAss.GetMemberIds()
                    .Select(x => _cmd.Document.GetElement(x))
                    .Where(x => x is Rebar)
                    .Where(x => x != null)
                    .Cast<Rebar>()
                    .Where(x => x.get_Parameter(BuiltInParameter.REBAR_ELEM_HOOK_STYLE).AsInteger() != (int)RebarStyle.StirrupTie)
                    .ToList();
            }
            catch (Exception)
            {
                results = new List<Rebar>();
            }
            return results;
        }

        public ElementId GetRebarBeamAssembly(RevElement beam)
        {
            ElementId result = null;
            try
            {
                var pbs = new List<XYZ>() {
                    beam.BoxElement.BoxElementPoint.P1,
                    beam .BoxElement.BoxElementPoint.P2,
                    beam .BoxElement.BoxElementPoint.P3,
                    beam .BoxElement.BoxElementPoint.P4
                };
                var hBox = beam.BoxElement.BoxElementPoint.P1.DistanceTo(beam.BoxElement.BoxElementPoint.P5);
                var sl = pbs.CreateSolid(beam.BoxElement.VTZ, hBox.FootToMm());
                result = sl.GetElementIntersectBoundingBox<Rebar>(_cmd.Document, BuiltInCategory.OST_Rebar, 100)
                    .Where(x => x.AssemblyInstanceId.ToString() != "-1")
                    .Select(x => x.AssemblyInstanceId)
                    .GroupBy(x => x.ToString())
                    .Select(x => x.ToList())
                    .OrderBy(x => x.Count())
                    .LastOrDefault()
                    .FirstOrDefault();
            }
            catch (Exception)
            {
                result = null;
            }
            return result;
        }

        public RevElement SelecteBeamAssembly()
        {
            RevElement result = null;
            try
            {
                var b = _cmd.Document.GetElement(_cmd.UiDocument.Selection.PickObject(
                    Autodesk.Revit.UI.Selection.ObjectType.Element,
                    new GenericSelectionFilterFromCategory(Autodesk.Revit.DB.BuiltInCategory.OST_StructuralFraming)));
                result = new RevElement(b);
            }
            catch (Autodesk.Revit.Exceptions.OperationCanceledException ex)
            {
                throw ex;
            }
            catch (Exception)
            {
                result = null;
            }
            return result;
        }

        public List<Rebar> GetRebarsTopClass1(List<Rebar> rebarsInBeamAssembly)
        {
            var results = new List<Rebar>();
            try
            {
                results = rebarsInBeamAssembly.Where(x =>
                {
                    var info = SchemaInfo.ReadAll(
                        _elementInstance.RebarBeamSchemal.SchemaBase,
                        _elementInstance.RebarBeamSchemal.SchemaField,
                        x);
                    if (info == null) return false;
                    var rebarBeamInfo = JsonConvert.DeserializeObject<BeamRebarInfo>(info.Value);
                    return rebarBeamInfo.RebarBeamLevel == (int)RebarBeamLevel.Top && rebarBeamInfo.RebarBeamGroup == (int)RebarBeamGroup.Level1;
                }).ToList();
            }
            catch (Exception)
            {
                results = new List<Rebar>();
            }
            return results;
        }

        public List<Rebar> GetRebarsTopClass2(List<Rebar> rebarsInBeamAssembly)
        {
            var results = new List<Rebar>();
            try
            {
                results = rebarsInBeamAssembly.Where(x =>
                {
                    var info = SchemaInfo.ReadAll(
                        _elementInstance.RebarBeamSchemal.SchemaBase,
                        _elementInstance.RebarBeamSchemal.SchemaField,
                        x);
                    if (info == null) return false;
                    var rebarBeamInfo = JsonConvert.DeserializeObject<BeamRebarInfo>(info.Value);
                    return rebarBeamInfo.RebarBeamLevel == (int)RebarBeamLevel.Top && rebarBeamInfo.RebarBeamGroup == (int)RebarBeamGroup.Level2;
                }).ToList();
            }
            catch (Exception)
            {
                results = new List<Rebar>();
            }
            return results;
        }

        public List<Rebar> GetRebarsTopClass3(List<Rebar> rebarsInBeamAssembly)
        {
            var results = new List<Rebar>();
            try
            {
                results = rebarsInBeamAssembly.Where(x =>
                {
                    var info = SchemaInfo.ReadAll(
                        _elementInstance.RebarBeamSchemal.SchemaBase,
                        _elementInstance.RebarBeamSchemal.SchemaField,
                        x);
                    if (info == null) return false;
                    var rebarBeamInfo = JsonConvert.DeserializeObject<BeamRebarInfo>(info.Value);
                    return rebarBeamInfo.RebarBeamLevel == (int)RebarBeamLevel.Top && rebarBeamInfo.RebarBeamGroup == (int)RebarBeamGroup.Level3;
                }).ToList();
            }
            catch (Exception)
            {
                results = new List<Rebar>();
            }
            return results;
        }

        public List<Rebar> GetRebarsBotClass1(List<Rebar> rebarsInBeamAssembly)
        {
            var results = new List<Rebar>();
            try
            {
                results = rebarsInBeamAssembly.Where(x =>
                {
                    var info = SchemaInfo.ReadAll(
                        _elementInstance.RebarBeamSchemal.SchemaBase,
                        _elementInstance.RebarBeamSchemal.SchemaField,
                        x);
                    if (info == null) return false;
                    var rebarBeamInfo = JsonConvert.DeserializeObject<BeamRebarInfo>(info.Value);
                    return rebarBeamInfo.RebarBeamLevel == (int)RebarBeamLevel.Bottom && rebarBeamInfo.RebarBeamGroup == (int)RebarBeamGroup.Level1;
                }).ToList();
            }
            catch (Exception)
            {
                results = new List<Rebar>();
            }
            return results;
        }

        public List<Rebar> GetRebarsBotClass2(List<Rebar> rebarsInBeamAssembly)
        {
            var results = new List<Rebar>();
            try
            {
                results = rebarsInBeamAssembly.Where(x =>
                {
                    var info = SchemaInfo.ReadAll(
                        _elementInstance.RebarBeamSchemal.SchemaBase,
                        _elementInstance.RebarBeamSchemal.SchemaField,
                        x);
                    if (info == null) return false;
                    var rebarBeamInfo = JsonConvert.DeserializeObject<BeamRebarInfo>(info.Value);
                    return rebarBeamInfo.RebarBeamLevel == (int)RebarBeamLevel.Bottom && rebarBeamInfo.RebarBeamGroup == (int)RebarBeamGroup.Level2;
                }).ToList();
            }
            catch (Exception)
            {
                results = new List<Rebar>();
            }
            return results;
        }

        public List<Rebar> GetRebarsBotClass3(List<Rebar> rebarsInBeamAssembly)
        {
            var results = new List<Rebar>();
            try
            {
                results = rebarsInBeamAssembly.Where(x =>
                {
                    var info = SchemaInfo.ReadAll(
                        _elementInstance.RebarBeamSchemal.SchemaBase,
                        _elementInstance.RebarBeamSchemal.SchemaField,
                        x);
                    if (info == null) return false;
                    var rebarBeamInfo = JsonConvert.DeserializeObject<BeamRebarInfo>(info.Value);
                    return rebarBeamInfo.RebarBeamLevel == (int)RebarBeamLevel.Bottom && rebarBeamInfo.RebarBeamGroup == (int)RebarBeamGroup.Level3;
                }).ToList();
            }
            catch (Exception)
            {
                results = new List<Rebar>();
            }
            return results;
        }

        public void InitRebarBeamInfo(ElementId rebarBeamAssembly)
        {
            try
            {
                var ass = _cmd.Document.GetElement(rebarBeamAssembly) as AssemblyInstance;

            }
            catch (Exception)
            {
            }
        }
    }
}
