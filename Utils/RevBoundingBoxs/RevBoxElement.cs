using Autodesk.Revit.DB.Structure;
using HcBimUtils;
using HcBimUtils.DocumentUtils;
using HcBimUtils.GeometryUtils.Geometry;
using RevitDevelop.Utils.RevBoundingBoxs;
using Utils.Geometries;

namespace Utils.BoundingBoxs
{
    public class RevBoxElement
    {
        public int Id { get; }
        public XYZ VTX { get; set; }
        public XYZ VTY { get; set; }
        public XYZ VTZ { get; set; }
        public Element Element { get; }
        public List<Solid> Solids { get; }
        public List<Curve> Curves { get; }
        public Outline Outline { get; set; }
        public Line LineBox { get; set; }
        public RevBoxPoint BoxElementPoint { get; set; }
        public RevBoxElement(Element ele)
        {
            Element = ele;
            Id = int.Parse(Element.Id.ToString());
            Solids = GetSolids();
            Curves = GetCurves();
            VTX = GetVTX();
            VTY = !VTX.IsParallel(XYZ.BasisZ)
                ? VTX.CrossProduct(XYZ.BasisZ).Normalize()
                : VTX.CrossProduct(XYZ.BasisX).Normalize();
            if (!VTX.IsParallel(XYZ.BasisZ))
            {
                VTY = !VTX.IsParallel(XYZ.BasisY) ? VTY.DotProduct(XYZ.BasisY) <= 0 ? -VTY : VTY : VTY.DotProduct(XYZ.BasisX) <= 0 ? -VTY : VTY;
            }
            VTZ = VTX.CrossProduct(VTY).Normalize();
            Outline = GetOutLine(out RevBoxPoint boxElementPoint, out Line lineBox);
            BoxElementPoint = boxElementPoint;
            LineBox = lineBox;
        }
        public void GenerateCoordinateWithBaseVT(XYZ vtBase)
        {
            if (VTX.IsParallel(vtBase))
            {
                var vttg = VTZ;
                VTZ = vtBase;
                VTX = vttg;
                VTY = VTX.CrossProduct(VTZ);
            }
            if (VTY.IsParallel(vtBase))
            {
                var vttg = VTZ;
                VTZ = vtBase;
                VTY = vttg;
                VTX = VTY.CrossProduct(VTZ);
            }
            if (VTZ.IsParallel(vtBase))
            {
                VTZ = vtBase;
                VTY = VTX.CrossProduct(VTZ);
            }
        }
        private List<Solid> GetSolids()
        {
            var result = new List<Solid>();
            try
            {
                if (Element is FamilyInstance f)
                {
                    //result = new List<Solid>() { f.GetSolidOriginalFromFamilyInstance() };
                    result = Element.GetSolids()
                        .Where(x => x != null)
                        .Where(x => x.Volume > 0).ToList();
                }
            }
            catch (Exception)
            {
            }
            return result;
        }
        public Outline GetOutLine(out RevBoxPoint boxElementPoint, out Line lineBox)
        {
            lineBox = null;
            boxElementPoint = new RevBoxPoint();
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
                    var crs = ele.GetSolids()
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
}
