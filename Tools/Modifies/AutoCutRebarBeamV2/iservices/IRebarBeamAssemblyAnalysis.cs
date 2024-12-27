using Autodesk.Revit.DB.Structure;
using RevitDevelop.Utils.RevElements;

namespace RevitDevelop.Tools.Modifies.AutoCutRebarBeamV2.iservices
{
    public interface IRebarBeamAssemblyAnalysis
    {
        public RevElement SelecteBeamAssembly();
        public void InitRebarBeamInfo(ElementId rebarBeamAssembly);
        public ElementId GetRebarBeamAssembly(RevElement beam);
        public List<Rebar> GetRebarsInBeamAssembly(ElementId rebarBeamAssembly);
        public List<Rebar> GetRebarsTopClass1(List<Rebar> rebarsInBeamAssembly);
        public List<Rebar> GetRebarsTopClass2(List<Rebar> rebarsInBeamAssembly);
        public List<Rebar> GetRebarsTopClass3(List<Rebar> rebarsInBeamAssembly);
        public List<Rebar> GetRebarsBotClass1(List<Rebar> rebarsInBeamAssembly);
        public List<Rebar> GetRebarsBotClass2(List<Rebar> rebarsInBeamAssembly);
        public List<Rebar> GetRebarsBotClass3(List<Rebar> rebarsInBeamAssembly);
    }
}
