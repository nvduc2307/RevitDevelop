using HcBimUtils.DocumentUtils;
using Utils.FilterElements;
using Utils.RevPoints;

namespace RevitDevelop.Tools.InstallConstructionJoinOfWalls.models
{
    public class ElementInstance
    {
        public List<Grid> Grids { get; set; }
        public XYZ RCenter { get; set; }
        public BoundingBoxXYZ RBox { get; set; }
        public double RCross { get; set; }
        public ElementInstance()
        {
            Grids = AC.Document.GetElementsFromClass<Grid>(false);
            RCenter = GetRCenter(out BoundingBoxXYZ rBox, out double rCross);
            RBox = rBox;
            RCross = rCross;
        }
        private XYZ GetRCenter(out BoundingBoxXYZ rBox, out double rCross)
        {
            rCross = 0;
            rBox = new BoundingBoxXYZ();
            XYZ result = null;
            try
            {
                var ps = Grids.Select(x => x.Curve)
                    .Select(x => new List<XYZ>() { x.GetEndPoint(0), x.GetEndPoint(1) })
                    .Aggregate((a, b) => a.Concat(b).ToList());
                result = ps.GetCenter();
                var minx = ps.Min(x => x.X);
                var miny = ps.Min(x => x.Y);
                var maxx = ps.Max(x => x.X);
                var maxy = ps.Max(x => x.Y);
                rBox.Max = new XYZ(maxx, maxy, 0);
                rBox.Min = new XYZ(minx, miny, 0);
                rCross = rBox.Min.DistanceTo(rBox.Max);
            }
            catch (Exception)
            {
            }
            return result;
        }
    }
}
