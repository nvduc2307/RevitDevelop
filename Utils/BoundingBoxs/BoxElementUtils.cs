using Autodesk.Revit.DB.Structure;
using HcBimUtils;
using HcBimUtils.DocumentUtils;
using HcBimUtils.GeometryUtils.Geometry;
using Utils.Geometries;

namespace Utils.BoundingBoxs
{
    public class BoxElementUtils
    {
    }
    public class BoxElement
    {
        public int Id { get; }
        public XYZ VTX { get; }
        public XYZ VTY { get; }
        public XYZ VTZ { get; }
        public Element Element { get; }
        public List<Solid> Solids { get; }
        public List<Curve> Curves { get; }
        public Outline Outline { get; set; }
        public Line LineBox { get; private set; }
        public BoxElementPoint BoxElementPoint { get; private set; }
        public BoxElement(Element ele)
        {
            Element = ele;
            Id = int.Parse(Element.Id.ToString());
            Solids = GetSolids();
            Curves = GetCurves();
            VTX = GetVTX();
            VTY = !VTX.IsParallel(XYZ.BasisZ) ? VTX.CrossProduct(XYZ.BasisZ).Normalize() : VTX.CrossProduct(XYZ.BasisX).Normalize();
            VTZ = VTX.CrossProduct(VTY).Normalize();
            Outline = GetOutLine(out BoxElementPoint boxElementPoint, out Line lineBox);
            BoxElementPoint = boxElementPoint;
            LineBox = lineBox;
        }
        private List<Solid> GetSolids()
        {
            var reuslts = new List<Solid>();
            try
            {
                reuslts = Element.GetSolids()
                    .Where(x => x != null)
                    .Where(x => x.Volume > 0).ToList();
            }
            catch (Exception)
            {
            }
            return reuslts;
        }
        private Outline GetOutLine(out BoxElementPoint boxElementPoint, out Line lineBox)
        {
            lineBox = null;
            boxElementPoint = new BoxElementPoint();
            try
            {
                var ps = Curves
                    .Where(x => x is Line)
                    .Select(x => new List<XYZ>() { x.GetEndPoint(0), x.GetEndPoint(1) })
                    .Aggregate((a, b) => a.Concat(b).ToList())
                    .ToList();
                var pxs = ps.OrderBy(x => x.DotProduct(VTX)).ToList();
                var pys = ps.OrderBy(x => x.DotProduct(VTY)).ToList();
                var pzs = ps.OrderBy(x => x.DotProduct(VTZ)).ToList();

                if (pxs.Count <= 0) return null;
                if (pys.Count <= 0) return null;
                if (pzs.Count <= 0) return null;

                var fxStart = new FaceCustom(VTX, pxs.FirstOrDefault());
                var fxEnd = new FaceCustom(VTX, pxs.LastOrDefault());
                var fyStart = new FaceCustom(VTY, pys.FirstOrDefault());
                var fyEnd = new FaceCustom(VTY, pys.LastOrDefault());
                var fzStart = new FaceCustom(VTZ, pzs.FirstOrDefault());
                var fzEnd = new FaceCustom(VTZ, pzs.LastOrDefault());

                var lb1 = fxStart.FaceIntersectFace(fzStart);
                var lb2 = fxEnd.FaceIntersectFace(fzStart);

                var pb1 = lb1.BasePoint.RayPointToFace(fyStart.Normal, fyStart);
                var pb2 = lb1.BasePoint.RayPointToFace(fyEnd.Normal, fyEnd);
                var pb3 = lb2.BasePoint.RayPointToFace(fyEnd.Normal, fyEnd);
                var pb4 = lb2.BasePoint.RayPointToFace(fyStart.Normal, fyStart);
                boxElementPoint.P1 = pb1;
                boxElementPoint.P2 = pb2;
                boxElementPoint.P3 = pb3;
                boxElementPoint.P4 = pb4;

                var lt1 = fxStart.FaceIntersectFace(fzEnd);
                var lt2 = fxEnd.FaceIntersectFace(fzEnd);

                var pt1 = lt1.BasePoint.RayPointToFace(fyStart.Normal, fyStart);
                var pt2 = lt1.BasePoint.RayPointToFace(fyEnd.Normal, fyEnd);
                var pt3 = lt2.BasePoint.RayPointToFace(fyEnd.Normal, fyEnd);
                var pt4 = lt2.BasePoint.RayPointToFace(fyStart.Normal, fyStart);
                boxElementPoint.P5 = pt1;
                boxElementPoint.P6 = pt2;
                boxElementPoint.P7 = pt3;
                boxElementPoint.P8 = pt4;
                lineBox = Line.CreateBound(pb1, pt3);

                return new Outline(pb1, pt3);
            }
            catch (Exception)
            {
                return null;
            }
        }
        private List<Curve> GetCurves()
        {
            var results = new List<Curve>();
            try
            {
                if (Element is AssemblyInstance ass)
                {
                    var eles = ass.GetMemberIds().Select(x => x.ToElement(AC.Document)).ToList();
                    foreach (var ele in eles)
                    {
                        var crs = GetCurvesFromElement(ele);
                        results.AddRange(crs);
                    }
                }
                else
                {
                    var crs = GetCurvesFromElement(Element);
                    results.AddRange(crs);
                }
            }
            catch (Exception)
            {
            }
            return results;
        }
        private List<Curve> GetCurvesFromElement(Element ele)
        {
            var results = new List<Curve>();
            if (ele is Rebar rb)
            {
                var crs = rb
                    .GetCenterlineCurves(false, false, false, MultiplanarOption.IncludeAllMultiplanarCurves, 0)
                    .ToList();
                results.AddRange(crs);
            }
            else
            {
                try
                {
                    var crs = Solids
                        .Select(x => x.GetFacesFromSolid())
                        .Aggregate((a, b) => a.Concat(b).ToList())
                        .Select(x => x.GetFirstCurveLoop().ToList())
                        .Select(x => x)
                        .Aggregate((a, b) => a.Concat(b).ToList());
                    results.AddRange(crs);
                }
                catch (Exception)
                {
                }
            }
            return results;
        }
        private XYZ GetVTX()
        {
            var result = new XYZ();
            try
            {
                var l = Curves
                    .Where(x => x is Line)
                    .OrderBy(x => x.Length)
                    .LastOrDefault();
                result = l.Direction();

            }
            catch (Exception)
            {
            }
            return result;
        }
    }
    public class BoxElementPoint
    {
        public XYZ P1 { get; set; }
        public XYZ P2 { get; set; }
        public XYZ P3 { get; set; }
        public XYZ P4 { get; set; }
        public XYZ P5 { get; set; }
        public XYZ P6 { get; set; }
        public XYZ P7 { get; set; }
        public XYZ P8 { get; set; }
    }
}
