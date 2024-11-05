using Autodesk.Revit.DB.Structure;
using HcBimUtils;
using HcBimUtils.DocumentUtils;
using RevitDevelop.Utils.NumberingRevitElements;
using RevitDevelop.Utils.RevReferences.RevRebars;
using System.Drawing;
using Utils.canvass;
using Utils.Geometries;
using Utils.RevArcs;

namespace RevitDevelop.Utils.BitMaps
{
    public class RebarCustomImage
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public Size Size { get; set; }
        public Pen Pen { get; set; }
        public Font Font { get; set; }
        public Brush Brush { get; set; }
        System.Drawing.Imaging.ImageFormat ImageFormat { get; set; }
        public double ScaleX { get; set; }
        public double ScaleY { get; set; }
        public NumberingRevitRebar NumberingRevitRebar { get; set; }
        public Rebar Rebar { get; set; }
        public XYZ VTX { get; set; }
        public XYZ VTY { get; set; }
        public XYZ VTZ { get; set; }
        public XYZ Origin { get; set; }
        public double Height { get; set; }
        public double Width { get; set; }
        public List<Curve> CurvesOrgin { get; set; } //doi voi cac thanh thep khep kin thi cac line co the khac mat phang
        public List<Curve> CurvesGenerate { get; set; } // xoay truc z, xoay truc x, move ve goc toa do
        public Line Axis { get; set; }
        public RebarCustomImage(int id, NumberingRevitRebar numberingRevitRebar)
        {
            Id = numberingRevitRebar.ElementId;
            NumberingRevitRebar = numberingRevitRebar;
            Rebar = new Autodesk.Revit.DB.ElementId(Id).ToElement(AC.Document) as Rebar;
            Name = ParameterUtilities.HasParameter(Rebar, "RebarScheduleShareParameter.SCHEDULE_REBAR_MARK")
                ? Rebar.LookupParameter("RebarScheduleShareParameter.SCHEDULE_REBAR_MARK").AsString()
                : Id.ToString();
            Size = new Size(200, 60);
            Pen = new Pen(Brushes.Red, 2);
            Font = new Font("ISOCPEUR", 13);
            Brush = Brushes.Black;
            VTY = Rebar.GetNormal();
            VTX = GetVectorX();
            VTZ = GetVectorZ();
            Origin = GetOrigin(out double width, out double height);
            Width = width;
            Height = height;
            CurvesOrgin = GetCurvesOrgin();
            CurvesGenerate = GetCurvesGenerate();
        }
        public void GetRebarInBitMap()
        {
            try
            {
                if (VTY == null) throw new Exception();

                using (var ts = new Transaction(AC.Document, "name transaction"))
                {
                    ts.Start();
                    //--------
                    var plane = Autodesk.Revit.DB.Plane.CreateByNormalAndOrigin(XYZ.BasisZ, new XYZ());
                    var sket = SketchPlane.Create(AC.Document, plane);
                    foreach (var item in CurvesGenerate)
                    {
                        try
                        {
                            AC.Document.Create.NewModelCurve(item, sket);
                        }
                        catch (Exception)
                        {
                        }
                    }
                    //--------
                    ts.Commit();
                }
            }
            catch (Exception)
            {
            }
        }
        public void CreateImage(string nameImg)
        {
            try
            {
                var rect = new RectangleF(0, 0, Size.Width, Size.Height);
                var bm = new Bitmap(Size.Width, Size.Height);
                var gMain = Graphics.FromImage(bm);
                //BackGround
                gMain.FillRectangle(Brushes.White, rect);
                gMain.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.GammaCorrected;
                //rebar
                var canvasBase = new CanvasPageBase(Size);
                canvasBase.RatioScale = 0.7;
                canvasBase.ScaleX = Size.Width * canvasBase.RatioScale / Width;
                canvasBase.ScaleY = Size.Height * 0.4 / Height;
                foreach (var curve in CurvesGenerate)
                {
                    if (curve is Line l)
                    {
                        var p1 = l.GetEndPoint(0).ConvertPointRToC(new XYZ(), canvasBase);
                        var p2 = l.GetEndPoint(1).ConvertPointRToC(new XYZ(), canvasBase);
                        gMain.DrawLine(
                            Pen,
                            new PointF(float.Parse(p1.X.ToString()), float.Parse(p1.Y.ToString())), new PointF(float.Parse(p2.X.ToString()), float.Parse(p2.Y.ToString())));
                    }
                    if (curve is Arc arc)
                    {
                        var arcCustom = new ArcCustom(arc);
                        var start = arcCustom.Start.ConvertPointRToC(new XYZ(), canvasBase);
                        var end = arcCustom.End.ConvertPointRToC(new XYZ(), canvasBase);
                        var mid = arcCustom.Mid.ConvertPointRToC(new XYZ(), canvasBase);

                        gMain.DrawArc(Pen,
                            float.Parse(start.X.ToString()),
                            float.Parse(start.Y.ToString()),
                            float.Parse(end.X.ToString()),
                            float.Parse(end.Y.ToString()),
                            0,
                            float.Parse(Math.PI.ToString()));
                    }
                }
                BitMapUtils.CreateImage(bm, "C:\\Users\\HC - 09\\Desktop\\New folder\\", nameImg, System.Drawing.Imaging.ImageFormat.Png, new List<Graphics>() { gMain });
            }
            catch (Exception)
            {
            }
        }
        private List<Curve> GetCurvesGenerate()
        {
            var results = new List<Curve>();
            var resultsTG = new List<Curve>();
            var fCustom = new FaceCustom(VTY, Origin);
            var plane = new FaceCustom(XYZ.BasisZ, Origin);
            var angle = fCustom.Normal.AngleTo(plane.Normal);
            var deg = angle * 180 / Math.PI;
            var axisRotate = plane.FaceIntersectFace(fCustom);
            Axis = axisRotate.LineBase;
            foreach (var curve in CurvesOrgin)
            {
                try
                {
                    if (curve is Line l)
                    {
                        var p1 = l.GetEndPoint(0).RayPointToFace(VTY, fCustom);
                        var p2 = l.GetEndPoint(1).RayPointToFace(VTY, fCustom);
                        //transform to plane
                        var p1New = p1.Rotate(fCustom, plane);
                        var p2New = p2.Rotate(fCustom, plane);
                        var lNew = Line.CreateBound(p1New, p2New);
                        resultsTG.Add(lNew);
                    }
                    if (curve is Arc arc)
                    {
                        var arcCustom = new ArcCustom(arc);
                        var start = arcCustom.Start.RayPointToFace(VTY, fCustom);
                        var end = arcCustom.End.RayPointToFace(VTY, fCustom);
                        var mid = arcCustom.Mid.RayPointToFace(VTY, fCustom);
                        //transform to plane
                        var startNew = start.Rotate(fCustom, plane);
                        var endNew = end.Rotate(fCustom, plane);
                        var midNew = mid.Rotate(fCustom, plane);
                        var arcNew = Arc.Create(startNew, endNew, midNew);
                        resultsTG.Add(arcNew);
                    }
                }
                catch (Exception)
                {
                }
            }
            //ReCalcular coordinate
            VTX = resultsTG.OrderBy(x => x.Length).LastOrDefault().Direction();
            VTZ = XYZ.BasisZ;
            VTY = VTX.CrossProduct(VTZ);
            //translate OX and translate global coordinate
            var angleX = VTX.Y > 0
                ? -VTX.AngleTo(XYZ.BasisX)
                : VTX.AngleTo(XYZ.BasisX);

            foreach (var curve in resultsTG)
            {
                try
                {
                    if (curve is Line l)
                    {
                        //transform to plane
                        var p1 = l.GetEndPoint(0).Rotate(Origin, angleX) - Origin;
                        var p2 = l.GetEndPoint(1).Rotate(Origin, angleX) - Origin;
                        var lNew = Line.CreateBound(p1, p2);
                        results.Add(lNew);

                    }
                    if (curve is Arc arc)
                    {
                        var arcCustom = new ArcCustom(arc);
                        var start = arcCustom.Start.Rotate(Origin, angleX) - Origin;
                        var end = arcCustom.End.Rotate(Origin, angleX) - Origin;
                        var mid = arcCustom.Mid.Rotate(Origin, angleX) - Origin;
                        var arcNew = Arc.Create(start, end, mid);
                        results.Add(arcNew);
                    }
                }
                catch (Exception)
                {
                }
            }

            return results;
        }
        private List<Curve> GetCurvesOrgin()
        {
            var results = new List<Curve>();
            try
            {
                results = Rebar.GetCenterlineCurves(true, false, false, MultiplanarOption.IncludeOnlyPlanarCurves, 0).ToList();
            }
            catch (Exception)
            {
            }
            return results;
        }
        private XYZ GetOrigin(out double width, out double height)
        {
            width = 0;
            height = 0;
            XYZ result = null;
            try
            {
                var ps = Rebar.GetRebarPoints();
                var psX = ps.OrderBy(x => x.DotProduct(VTX));
                var psY = ps.OrderBy(x => x.DotProduct(VTZ));

                var minX = psX.FirstOrDefault();
                var maxX = psX.LastOrDefault();
                var minY = psY.FirstOrDefault();
                var maxY = psY.LastOrDefault();
                if (minX.IsSeem(maxX))
                {
                    width = 0;
                    height = minY.Distance(maxY);
                    return minY.MidPoint(maxY);
                }
                if (minY.IsSeem(maxY))
                {
                    height = 0;
                    width = minX.Distance(maxX);
                    return minX.MidPoint(maxX);
                }

                var fx1 = new FaceCustom(VTX, minX);
                var fx2 = new FaceCustom(VTX, maxX);
                var fy1 = new FaceCustom(VTZ, minY);
                var fy2 = new FaceCustom(VTZ, maxY);

                var l1 = fx1.FaceIntersectFace(fy1);
                var l2 = fx1.FaceIntersectFace(fy2);
                var l3 = fx2.FaceIntersectFace(fy1);
                var l4 = fx2.FaceIntersectFace(fy2);

                height = l1.BasePoint.Distance(l2.BasePoint);
                width = l1.BasePoint.Distance(l3.BasePoint);
                result = l1.BasePoint.MidPoint(l4.BasePoint);
            }
            catch (Exception)
            {
                result = null;
            }
            return result;
        }
        private XYZ GetVectorZ()
        {
            XYZ result = null;
            try
            {
                result = VTX.CrossProduct(VTY);
            }
            catch (Exception)
            {
            }
            return result;
        }
        private XYZ GetVectorX()
        {
            XYZ result = null;
            try
            {
                result = Rebar.GetCenterlineCurves(true, true, false, MultiplanarOption.IncludeOnlyPlanarCurves, 0)
                    .Where(x => x is Line)
                    .OrderBy(x => x.Length)
                    .LastOrDefault()
                    .Direction();
            }
            catch (Exception)
            {
            }
            return result;
        }
    }
}
