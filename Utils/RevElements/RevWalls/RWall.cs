using HcBimUtils;
using HcBimUtils.GeometryUtils.Geometry;
using RevitDevelop.Utils.RevElements;
using Utils.canvass;
using Utils.CompareElement;
using Utils.Geometries;

namespace RevitDevelop.Utils.RevElements.RevWalls
{
    public class RWall : RElement
    {
        public XYZ VtX { get; set; }
        public XYZ VtY { get; set; }
        public double ThicknessMm { get; set; }
        public double LengthMm { get; set; }
        public List<XYZ> Polygon { get; set; }
        public InstanceInCanvasPolygon CWall { get; set; }
        public RWall(int id, object rWall, XYZ rCenter, CanvasPageBase canvasPageBase) : base(id, rWall, rCenter, canvasPageBase)
        {
            IsValidObject = ValidWall();
            GetVector(out XYZ vtX, out XYZ vtY);
            VtX = vtX;
            VtY = vtY;
            GetPolygon(out double thicknessMm, out double lengthMm, out List<XYZ> plg);
            ThicknessMm = thicknessMm;
            LengthMm = lengthMm;
            Polygon = plg;
            LevelId = GetLevel();
            CWall = GetCWall();
        }
        private ElementId GetLevel()
        {
            ElementId result = null;
            try
            {
                if (IsValidObject)
                {
                    var wall = RObject as Wall;
                    result = wall.LevelId;
                }
            }
            catch (Exception)
            {
            }
            return result;
        }
        private bool ValidWall()
        {
            try
            {
                if (RObject is not Wall) return false;
                if (RObject is Wall wall)
                {
                    var location = wall.Location;
                    if (location is LocationPoint) return false;
                    if (location is LocationCurve locationCurve) return true;
                }
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }
        private void GetVector(out XYZ vtX, out XYZ vtY)
        {
            vtX = null;
            vtY = null;
            try
            {
                if (IsValidObject)
                {
                    var location = (LocationCurve)(RObject as Wall).Location;
                    vtX = location.Curve.Direction();
                    vtY = vtX.CrossProduct(XYZ.BasisZ);
                }
            }
            catch (Exception)
            {
            }
        }
        private void GetPolygon(out double thicknessMm, out double lengthMm, out List<XYZ> plg)
        {
            thicknessMm = 0.0;
            lengthMm = 0.0;
            plg = new List<XYZ>();
            try
            {
                if (IsValidObject)
                {
                    var wall = RObject as Wall;
                    var document = wall.Document;
                    var plan = new FaceCustom(XYZ.BasisZ, new XYZ());
                    var faceX = new FaceCustom(VtY, new XYZ());
                    var faceY = new FaceCustom(VtX, new XYZ());
                    var ps = wall
                        .GetSolidOriginalWall()
                        .GetFacesFromSolid()
                        .Where(x => x != null)
                        .Select(x => x.GetPoints())
                        .Aggregate((a, b) => a.Concat(b).ToList())
                        .Select(x => x.RayPointToFace(XYZ.BasisZ, plan))
                        .Distinct(new ComparePoint())
                        .ToList();

                    var psX = ps
                        .GroupBy(x => x.DotProduct(VtX))
                        .Select(x => x.ToList())
                        .OrderBy(x => x.FirstOrDefault().DotProduct(VtX))
                        .ToList();
                    var psY = ps.GroupBy(x => x.DotProduct(VtX))
                        .Select(x => x.ToList())
                        .OrderBy(x => x.FirstOrDefault().DotProduct(VtX))
                        .ToList();

                    var maxX = psX.LastOrDefault().OrderBy(x => x.DotProduct(VtY)).ToList();
                    var minX = psX.FirstOrDefault().OrderBy(x => x.DotProduct(VtY)).ToList();
                    var maxY = psY.LastOrDefault().OrderBy(x => x.DotProduct(VtX)).ToList();
                    var minY = psY.FirstOrDefault().OrderBy(x => x.DotProduct(VtX)).ToList();

                    var px1 = minX.First().RayPointToFace(VtY, faceX);
                    var px2 = maxX.First().RayPointToFace(VtY, faceX);
                    var py1 = minY.First().RayPointToFace(VtX, faceY);
                    var py2 = maxY.First().RayPointToFace(VtX, faceY);

                    thicknessMm = Math.Round(px1.DistanceTo(px2).FootToMm(), 0).FootToMm();
                    lengthMm = Math.Round(py1.DistanceTo(py2).FootToMm(), 0).FootToMm();
                    plg = ps;
                }
            }
            catch (Exception)
            {
            }
        }
        private InstanceInCanvasPolygon GetCWall()
        {
            InstanceInCanvasPolygon result = null;
            try
            {
                if (RObject is Wall wall)
                {
                    var pls = Polygon.Select(x => x.ConvertPointRToC(RCenter, CanvasPageBase));
                    result = new InstanceInCanvasPolygon(CanvasPageBase, OptionStyleInstanceInCanvas.OPTION_CONCRETE_STRUCTURE, pls);
                }
            }
            catch (Exception)
            {
            }
            return result;
        }
        public override void DrawInCanvas()
        {
            try
            {
                CWall.DrawInCanvas();
            }
            catch (Exception)
            {
            }
        }
    }
}
