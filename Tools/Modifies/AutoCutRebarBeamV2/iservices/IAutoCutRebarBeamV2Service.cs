using Autodesk.Revit.DB.Structure;
using RevitDevelop.AutoCutRebarBeamV2.models;

namespace RevitDevelop.Tools.Modifies.AutoCutRebarBeamV2.iservices
{
    public interface IAutoCutRebarBeamV2Service
    {
        public List<Rebar> Cut(RebarBeamGroupInfo rebarBeamGroupInfo);

    }
}
