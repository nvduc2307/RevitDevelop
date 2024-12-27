using RevitDevelop.Utils.RevCurves;
using RevitDevelop.Utils.RevPoints;
using RevitDevelop.Utils.Revpolygons;

namespace RevitDevelop.AutoCutRebarBeamV2.models
{
    public class RebarBeamCutInfo
    {
        public int Id { get; set; }
        public bool IsCut { get; set; }
        public List<RevPolygon> ShapesOrigin { get; set; }
        public static RebarBeamCutInfo InitRebarBeamCutInfo(RebarBeamGroupInfo rebarBeamGroupInfo)
        {
            var results = new RebarBeamCutInfo()
            {
                Id = 0,
                IsCut = true,
                ShapesOrigin = new List<RevPolygon>()
            };
            try
            {
                foreach (var gr in rebarBeamGroupInfo.Groups)
                {
                    var c = 0;
                    foreach (var cur in gr.RevRebarCurves)
                    {
                        var ps = cur.LinesOrigin.GetPoints().Select(x => new RevPoint() { X = x.X, Y = x.Y, Z = x.Z });
                        var sh = new RevPolygon()
                        {
                            Id = c,
                            Shape = ps.ToList(),
                        };
                        results.ShapesOrigin.Add(sh);
                        c++;
                    }
                }
            }
            catch (Exception)
            {
            }
            return results;
        }
    }
}
