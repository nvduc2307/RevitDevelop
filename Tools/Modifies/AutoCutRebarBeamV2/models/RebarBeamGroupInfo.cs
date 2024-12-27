using Autodesk.Revit.DB.Structure;
using HcBimUtils;
using RevitDevelop.Tools.Modifies.AutoCutRebarBeamV2.models;
using RevitDevelop.Utils.RevElements.RevRebars;
using Utils.CompareElement;
using Utils.Geometries;
using Utils.RevPoints;

namespace RevitDevelop.AutoCutRebarBeamV2.models
{
    public class RebarBeamGroupInfo
    {
        public List<Rebar> Rebars { get; }
        public XYZ VTX { get; set; }
        public XYZ VTY { get; set; }
        public XYZ VTZ { get; set; }
        public int RebarBeamLevel { get; set; }
        public int RebarBeamGroup { get; set; }
        public double BoxWidth { get; set; }
        public double BoxHeight { get; set; }
        public XYZ CenterGroup { get; set; }
        public List<RebarBeamGroupSubInfo> Groups { get; set; }
        public List<List<RebarBeamGroupSubInfo>> GroupCuts { get; set; }
        public RebarBeamGroupInfo(List<Rebar> rebars, XYZ vTX, XYZ vTY, XYZ vTZ, int rebarBeamLevel, int rebarBeamGroup)
        {
            Rebars = rebars;
            RebarBeamLevel = rebarBeamLevel;
            RebarBeamGroup = rebarBeamGroup;
            VTX = vTX;
            VTY = vTY;
            VTZ = vTZ;
            Groups = GetGroups(rebars);
            GroupCuts = GetGroupCuts(Groups);
            CenterGroup = GetCenterGroup(Groups, out double boxWidth, out double boxHeight);
            BoxWidth = boxWidth;
            BoxHeight = boxHeight;
        }
        private List<List<RebarBeamGroupSubInfo>> GetGroupCuts(List<RebarBeamGroupSubInfo> groups)
        {
            var results = new List<List<RebarBeamGroupSubInfo>>();
            try
            {
                results = groups
                    .GroupBy(x => x.RevRebarCurves.Count)
                    .Select(x => x.ToList())
                    .ToList();
            }
            catch (Exception)
            {

            }
            return results;
        }
        private List<RebarBeamGroupSubInfo> GetGroups(List<Rebar> rebars)
        {
            var results = new List<RebarBeamGroupSubInfo>();
            try
            {
                var allPs = rebars.GetPoint();
                var center = allPs.ToList().CenterPoint();
                var facePlan = new FaceCustom(VTZ, center);
                var faceX = new FaceCustom(VTY, center);
                var faceY = new FaceCustom(VTX, center);
                results = rebars.GroupBy(x =>
                {
                    var ps = x.GetRebarPoints().CenterPoint();
                    var pp = ps.RayPointToFace(VTZ, facePlan);
                    var py = pp.RayPointToFace(VTX, faceY);
                    return Math.Round(py.DotProduct(VTY).FootToMm(), 0);
                })
                .OrderBy(x =>
                {
                    var ps = x.First().GetRebarPoints().CenterPoint();
                    var pp = ps.RayPointToFace(VTZ, facePlan);
                    var py = pp.RayPointToFace(VTX, faceY);
                    return Math.Round(py.DotProduct(VTY).FootToMm(), 0);
                })
                .Select(x => x.ToList())
                .Select(x => new RebarBeamGroupSubInfo(this, x, RebarBeamLevel, CenterGroup))
                .ToList();
            }
            catch (Exception)
            {
            }
            return results;
        }
        private XYZ GetCenterGroup(List<RebarBeamGroupSubInfo> groups, out double boxWidth, out double boxHeight)
        {
            boxWidth = 0;
            boxHeight = 0;
            XYZ result = null;
            try
            {
                var ps = groups.Select(x => x.RevRebarCurves.Select(y => y.CurvesGenerate))
                    .Aggregate((a, b) => a.Concat(b))
                    .Aggregate((a, b) => a.Concat(b).ToList())
                    .Select(x => new List<XYZ>() { x.GetEndPoint(0), x.GetEndPoint(1) })
                    .Aggregate((a, b) => a.Concat(b).ToList())
                    .Distinct(new ComparePoint())
                    .Select(x => x.EditZ(0))
                    .ToList();
                var minx = ps.Min(x => x.X);
                var miny = ps.Min(x => x.Y);
                var maxx = ps.Max(x => x.X);
                var maxy = ps.Max(x => x.Y);

                var p1 = new XYZ(minx, miny, 0);
                var p2 = new XYZ(minx, maxy, 0);
                var p3 = new XYZ(maxx, maxy, 0);
                var p4 = new XYZ(maxx, miny, 0);

                result = p1.MidPoint(p3);
                boxHeight = Math.Abs(maxy - miny);
                boxWidth = Math.Abs(maxx - minx);
            }
            catch (Exception)
            {
            }
            return result;
        }
    }
    public class RebarBeamGroupSubInfo
    {
        public int LevelGroup { get; set; }
        public RebarBeamGroupInfo Parent { get; set; }
        public List<Rebar> Rebars { get; }
        public List<RevRebarCurve> RevRebarCurves { get; set; }
        public XYZ CenterGroup { get; set; }
        public RebarBeamGroupSubInfo(RebarBeamGroupInfo parent, List<Rebar> rebars, int levelGroup, XYZ centerGroup)
        {
            Parent = parent;
            Rebars = rebars;
            LevelGroup = levelGroup;
            CenterGroup = centerGroup;
            RevRebarCurves = rebars.Select((x, index) => new RevRebarCurve(this, index, x, LevelGroup, CenterGroup)).ToList();
        }
    }
}
