using HcBimUtils;
using HcBimUtils.DocumentUtils;
using HcBimUtils.GeometryUtils.Geometry;
using RevitDevelop.Utils.RevElements;
using Utils.canvass;
using Utils.CompareElement;
using Utils.Geometries;

namespace RevitDevelop.Utils.RevColumns
{
    public class RColumn : RElement
    {
        public double WidthMm { get; set; }
        public double HeightMm { get; set; }
        public double LengthMm { get; set; }
        public XYZ VtX { get; set; }
        public XYZ VtY { get; set; }
        public List<XYZ> Polygon { get; set; }
        public InstanceInCanvasPolygon CColumn { get; set; }
        public RColumn(int id, object rColumn, XYZ rCenter, CanvasPageBase canvasPageBase) : base(id, rColumn, rCenter, canvasPageBase)
        {
            IsValidObject = validColumn();
            LevelId = GetLevel();
            GetVector(out XYZ vtx, out XYZ vty);
            VtX = vtx;
            VtY = vty;
            LengthMm = GetLength();
            GetWidthHeight(out double width, out double height, out List<XYZ> plg);
            WidthMm = width;
            HeightMm = height;
            Polygon = plg;
            CColumn = GetCColumn();
        }
        private void GetWidthHeight(out double width, out double height, out List<XYZ> plg)
        {
            width = 0.0;
            height = 0.0;
            plg = new List<XYZ>();
            try
            {
                if (IsValidObject)
                {
                    var column = RObject as FamilyInstance;
                    var document = column.Document;
                    var plan = new FaceCustom(XYZ.BasisZ, new XYZ());
                    var faceX = new FaceCustom(VtY, new XYZ());
                    var faceY = new FaceCustom(VtX, new XYZ());
                    var ps = column
                        .GetSolidOriginalFromFamilyInstance()
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

                    width = Math.Round(px1.DistanceTo(px2).FootToMm(), 0).FootToMm();
                    height = Math.Round(py1.DistanceTo(py2).FootToMm(), 0).FootToMm();
                    plg = new List<XYZ>()
                    {
                        minX.FirstOrDefault(),
                        minX.LastOrDefault(),
                        maxX.LastOrDefault(),
                        maxX.FirstOrDefault(),
                    };
                }
            }
            catch (Exception)
            {
            }
        }
        private void GetVector(out XYZ vtx, out XYZ vty)
        {
            vtx = XYZ.BasisX;
            vty = XYZ.BasisY;
            try
            {
                if (IsValidObject)
                {
                    var column = RObject as FamilyInstance;
                    var transform = column.GetTransform();
                    vtx = transform.OfVector(XYZ.BasisX);
                    vty = transform.OfVector(XYZ.BasisY);
                }
            }
            catch (Exception)
            {
            }
        }
        public double GetLength()
        {
            double lengthMm = 0.0;
            try
            {
                if (IsValidObject)
                {
                    var column = RObject as FamilyInstance;
                    lengthMm = Math.Round(column.get_Parameter(BuiltInParameter.INSTANCE_LENGTH_PARAM).AsDouble().FootToMm(), 0);
                }
            }
            catch (Exception)
            {
            }
            return lengthMm;
        }
        private bool validColumn()
        {
            var dk1 = RObject is FamilyInstance;
            var dk2 = (RObject as FamilyInstance).Category.ToBuiltinCategory() == BuiltInCategory.OST_StructuralColumns;
            return dk1 && dk2;
        }
        private ElementId GetLevel()
        {
            ElementId result = null;
            try
            {
                if (IsValidObject)
                {
                    var column = RObject as FamilyInstance;
                    result = column.LevelId;
                }
            }
            catch (Exception)
            {
            }
            return result;
        }
        private InstanceInCanvasPolygon GetCColumn()
        {
            InstanceInCanvasPolygon reuslt = null;
            try
            {
                if (RObject is FamilyInstance column)
                {
                    var pls = Polygon.Select(x => x.ConvertPointRToC(RCenter, CanvasPageBase));
                    reuslt = new InstanceInCanvasPolygon(CanvasPageBase, OptionStyleInstanceInCanvas.OPTION_CONCRETE_STRUCTURE, pls);
                }
            }
            catch (Exception)
            {
            }
            return reuslt;
        }

        public override void DrawInCanvas()
        {
            try
            {
                CColumn.DrawInCanvas();
            }
            catch (Exception)
            {
            }
        }
    }
}
