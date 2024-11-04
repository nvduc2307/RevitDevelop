using HcBimUtils;
using HcBimUtils.DocumentUtils;
using HcBimUtils.GeometryUtils.Geometry;
using HcBimUtils.MoreLinq;
using RevitDevelop.Utils.Floors;
using Utils.Assemblies;
using Utils.canvass;
using Utils.CompareElement;
using Utils.Curveloops;
using Utils.Geometries;
using Utils.Solids;

namespace RevitDevelop.Tools.Rebars.InstallRebarSlab.models
{
    public class MSlab
    {
        public double CoverMm { get; set; }
        public Floor Floor { get; set; }
        public List<Curve> FloorCurves { get; set; }
        public List<XYZ> FloorPoints { get; set; }
        public double FloorMinZ { get; set; }
        public double FloorMaxZ { get; set; }
        public double Thickness { get; set; }
        public Level Level { get; set; }
        public Outline Outline { get; set; }
        public CurveLoop OutlineReal { get; set; }
        public List<CurveLoop> Profiles { get; set; }
        public XYZ Center { get; set; }
        public XYZ VTX { get; set; }
        public XYZ VTY { get; set; }
        public XYZ VTZ { get; set; }
        public double Angle { get; set; }
        public List<Element> ElementsAround { get; set; }
        public List<XYZ> PointsAround { get; set; }
        public List<XYZ> PointsSlabOnFloorPlan { get; set; }
        public List<MSlabOpening> MSlabOpenings { get; set; }
        public List<Grid> GridsAround { get; set; }
        public MSLabElementIntance MSLabElementIntance { get; set; }
        public List<AssemblyInfo> AssemblyRebarFloors { get; set; }
        public List<FaceCustom> FaceSections { get; set; }
        public List<System.Windows.Shapes.Polygon> OpeningsInCanvas { get; set; } = new List<System.Windows.Shapes.Polygon>();
        public System.Windows.Shapes.Polygon SlabInCanvas { get; set; }
        public bool IsSelected { get; set; }
        public MSlabRebar MSlabRebar { get; set; }
        public Line LineRayTopX { get; set; }
        public Line LineRayTopY { get; set; }
        public Line LineRayBotX { get; set; }
        public Line LineRayBotY { get; set; }

        public MSlab(Floor floor, MSLabElementIntance mSLabElementIntance)
        {
            CoverMm = 30;
            MSLabElementIntance = mSLabElementIntance;
            Floor = floor;
            Profiles = GetProfiles();
            Level = floor.LevelId.ToElement(AC.Document) as Level;
            Outline = GetOutLine(100.MmToFoot());
            Center = Outline.MinimumPoint.Midpoint(Outline.MaximumPoint).EditZ(0);
            FloorCurves = GetFloorCurves();
            FloorPoints = GetFloorPoints();
            FloorMinZ = FloorPoints.Min(x => x.Z);
            FloorMaxZ = FloorPoints.Max(x => x.Z);
            Thickness = FloorMaxZ - FloorMinZ;
            OutlineReal = Profiles.LastOrDefault();
            VTX = GetVTX(out XYZ vty);
            VTY = vty;
            VTZ = XYZ.BasisZ;
            Angle = GetAngle();
            ElementsAround = GetElementsAround();
            PointsAround = GetPointsAround();
            GridsAround = GetGridsAround();
            AssemblyRebarFloors = GetAssemblyRebarFloor();
            FaceSections = GetFaceSections();
            PointsSlabOnFloorPlan = GetPointsSlabOnFloorPlan(out List<MSlabOpening> openings);
            MSlabOpenings = openings;
            MSlabRebar = GetMSlabRebar();
            //LineRayTopX = GetLineRayTop(FloorCurves, Center, VTY, VTX, out Line lineRayBotX);
            //LineRayBotX = lineRayBotX;
            //LineRayTopY = GetLineRayTop(FloorCurves, Center, VTX, VTY, out Line lineRayBotY);
            //LineRayBotY = lineRayBotY;
        }
        public static void ActionElementSelected(MSlab mSlabTarget, List<MSlab> mSlabs, InstallRebarSlabModel installRebarSlabModel)
        {
            installRebarSlabModel.MSlabSelected = mSlabTarget;
            mSlabTarget.IsSelected = !mSlabTarget.IsSelected;
            mSlabTarget.SlabInCanvas.Stroke = mSlabTarget.IsSelected
                    ? StyleColorInCanvas.Color_Selected_OutLine
                    : StyleColorInCanvas.Color_Concrete_OutLine;
            mSlabTarget.SlabInCanvas.Fill = mSlabTarget.IsSelected
                    ? StyleColorInCanvas.Color_Selected
                    : StyleColorInCanvas.Color_Concrete;
            foreach (var opening in mSlabTarget.OpeningsInCanvas)
            {
                opening.Stroke = mSlabTarget.IsSelected
                    ? StyleColorInCanvas.Color_Selected_OutLine
                    : StyleColorInCanvas.Color_Concrete_OutLine;
            }

            foreach (var mSlab in mSlabs)
            {
                if (mSlab.Floor.Id.ToString() != mSlabTarget.Floor.Id.ToString())
                {
                    mSlab.IsSelected = false;
                    mSlab.SlabInCanvas.Stroke = StyleColorInCanvas.Color_Concrete_OutLine;
                    mSlab.SlabInCanvas.Fill = StyleColorInCanvas.Color_Concrete;
                    foreach (var opening in mSlab.OpeningsInCanvas)
                    {
                        opening.Stroke = StyleColorInCanvas.Color_Concrete_OutLine;
                    }
                }
            }
            if (mSlabTarget.IsSelected) AC.UiDoc.Selection.SetElementIds(new List<ElementId>() { mSlabTarget.Floor.Id });
            if (!mSlabTarget.IsSelected) AC.UiDoc.Selection.SetElementIds(new List<ElementId>());
        }
        private List<CurveLoop> GetProfiles()
        {
            var results = new List<CurveLoop>();
            try
            {
                Sketch sket = null;
#if REVIT2021
                var elementFilter = new ElementClassFilter(typeof(Sketch), false);
                var skets = Floor.GetDependentElements(elementFilter).Select(x => AC.Document.GetElement(x) as Sketch);
                sket = AC.Document.GetElement(skets.FirstOrDefault().Id) as Sketch;
#else  
                sket = AC.Document.GetElement(Floor.SketchId) as Sketch;
#endif
                var profile = sket.Profile;
                foreach (CurveArray curveArray in profile)
                {
                    var cvl = new CurveLoop();
                    foreach (var curve in curveArray.ToCurves())
                    {
                        cvl.Append(curve);
                    }
                    results.Add(cvl);
                }
            }
            catch (Exception)
            {
            }
            return results.OrderBy(x => x.GetArea()).ToList();
        }
        private MSlabRebar GetMSlabRebar()
        {
            var spacing = 200;

            var topX_diameter = MSLabElementIntance.RebarBarTypes.FirstOrDefault();
            var topY_diameter = MSLabElementIntance.RebarBarTypes.FirstOrDefault();
            var botX_diameter = MSLabElementIntance.RebarBarTypes.FirstOrDefault();
            var botY_diameter = MSLabElementIntance.RebarBarTypes.FirstOrDefault();

            var topX_cover = CoverMm + topY_diameter.BarDiameter.FootToMm() + topX_diameter.BarDiameter.FootToMm() / 2;
            var topY_cover = CoverMm + topY_diameter.BarDiameter.FootToMm() / 2;
            var botX_cover = CoverMm + botY_diameter.BarDiameter.FootToMm() + botX_diameter.BarDiameter.FootToMm() / 2;
            var botY_cover = CoverMm + topY_diameter.BarDiameter.FootToMm() / 2;

            var topX = new MSlabRebarLayer(this, topX_cover, topX_diameter, VTX, VTY, 0, spacing);
            var topY = new MSlabRebarLayer(this, topY_cover, topY_diameter, VTY, VTX, 1, spacing);
            var botX = new MSlabRebarLayer(this, botX_cover, botX_diameter, VTX, VTY, 2, spacing);
            var botY = new MSlabRebarLayer(this, botY_cover, botY_diameter, VTY, VTX, 3, spacing);

            topX.RebarBarTypeCustomEventAction = () =>
            {
                topX.CoverLayer = CoverMm + topY.RebarBarTypeCustom.BarDiameter.FootToMm() + topX.RebarBarTypeCustom.BarDiameter.FootToMm() / 2;
                topY.CoverLayer = CoverMm + topY.RebarBarTypeCustom.BarDiameter.FootToMm() / 2;

                MSlabRebarLayer.UpdateLayerMSlabRebars(topY);
                MSlabRebarLayer.UpdateLayerMSlabRebars(topX);
            };
            topY.RebarBarTypeCustomEventAction = () =>
            {
                topY.CoverLayer = CoverMm + topY.RebarBarTypeCustom.BarDiameter.FootToMm() / 2;
                topX.CoverLayer = CoverMm + topY.RebarBarTypeCustom.BarDiameter.FootToMm() + topX.RebarBarTypeCustom.BarDiameter.FootToMm() / 2;

                MSlabRebarLayer.UpdateLayerMSlabRebars(topY);
                MSlabRebarLayer.UpdateLayerMSlabRebars(topX);
            };

            botX.RebarBarTypeCustomEventAction = () =>
            {
                botY.CoverLayer = CoverMm + botY.RebarBarTypeCustom.BarDiameter.FootToMm() / 2;
                botX.CoverLayer = CoverMm + botY.RebarBarTypeCustom.BarDiameter.FootToMm() + botX.RebarBarTypeCustom.BarDiameter.FootToMm() / 2;

                MSlabRebarLayer.UpdateLayerMSlabRebars(botY);
                MSlabRebarLayer.UpdateLayerMSlabRebars(botX);
            };
            botY.RebarBarTypeCustomEventAction = () =>
            {
                botY.CoverLayer = CoverMm + botY.RebarBarTypeCustom.BarDiameter.FootToMm() / 2;
                botX.CoverLayer = CoverMm + botY.RebarBarTypeCustom.BarDiameter.FootToMm() + botX.RebarBarTypeCustom.BarDiameter.FootToMm() / 2;

                MSlabRebarLayer.UpdateLayerMSlabRebars(botY);
                MSlabRebarLayer.UpdateLayerMSlabRebars(botX);
            };

            return new MSlabRebar(this, VTX, VTY, VTZ, topX, topY, botX, botY);
        }
        private List<XYZ> GetPointsSlabOnFloorPlan(out List<MSlabOpening> openings)
        {
            openings = new List<MSlabOpening>();
            var result = new List<XYZ>();
            try
            {
                result = CurveloopExt.GetPoints(Profiles.LastOrDefault());
                openings = Profiles.Count == 1
                ? new List<MSlabOpening>()
                : Profiles.Slice(0, Profiles.Count - 1).Select(x => new MSlabOpening(Floor, x)).ToList();
            }
            catch (Exception)
            {
            }
            return result;
        }
        private List<FaceCustom> GetFaceSections()
        {
            var areaMin = 90000.MmToFoot();
            var result = new List<FaceCustom>();
            try
            {
                var curveLoops = Floor.GetSolids()
                    .Select(x => x.GetFacesFromSolid())
                    .Aggregate((a, b) => a.Concat(b).ToList())
                    .Select(x => x.GetEdgesAsCurveLoops())
                    .Aggregate((a, b) => a.Concat(b).ToList())
                    .OrderBy(x => x.GetArea())
                    .ToList();
                var normal = curveLoops.Last().GetNormal();
                var curveLoopsActive = curveLoops.Where(x =>
                {
                    var normalCheck = x.GetNormal();
                    return normalCheck.DotProduct(normal).IsAlmostEqual(1) || normalCheck.DotProduct(normal).IsAlmostEqual(-1);
                })
                    .GroupBy(x => x.GetArea())
                    .Select(x => x.First())
                    .OrderBy(x => x.GetArea())
                    .ToList();
                var curveLoopsActiveCount = curveLoopsActive.Count;
                if (curveLoopsActiveCount > 1)
                {
                    var opening = curveLoopsActive[curveLoopsActiveCount - 2];
                    var openingCenter = opening.GetCenter();

                    var face1 = new FaceCustom(VTX, openingCenter);
                    var face2 = new FaceCustom(VTY, openingCenter);
                    result.Add(face1);
                    result.Add(face2);

                    var ps_face1_1 = GetPointIntersect(face1, curveLoopsActive[curveLoopsActiveCount - 1]);
                    var ps_face1_2 = GetPointIntersect(face1, curveLoopsActive[curveLoopsActiveCount - 2]);
                    var ps_face2_1 = GetPointIntersect(face2, curveLoopsActive[curveLoopsActiveCount - 1]);
                    var ps_face2_2 = GetPointIntersect(face2, curveLoopsActive[curveLoopsActiveCount - 2]);

                    var ps_face1 = ps_face1_1.Concat(ps_face1_2).OrderBy(x => x.DotProduct(VTY)).ToList();
                    var ps_face2 = ps_face2_1.Concat(ps_face2_2).OrderBy(x => x.DotProduct(VTX)).ToList();

                    if (ps_face1.Count == 4)
                    {
                        var l1 = ps_face1[0].DistanceTo(ps_face1[1]);
                        var l2 = ps_face1[2].DistanceTo(ps_face1[3]);

                        var mid = l1 >= l2 ? ps_face1[0].MidPoint(ps_face1[1]) : ps_face1[2].MidPoint(ps_face1[3]);

                        var f = new FaceCustom(VTY, mid);
                        result.Add(f);
                    }

                    if (ps_face2.Count == 4)
                    {
                        var l1 = ps_face2[0].DistanceTo(ps_face2[1]);
                        var l2 = ps_face2[2].DistanceTo(ps_face2[3]);

                        var mid = l1 >= l2 ? ps_face2[0].MidPoint(ps_face2[1]) : ps_face2[2].MidPoint(ps_face2[3]);

                        var f = new FaceCustom(VTX, mid);
                        result.Add(f);
                    }

                    List<XYZ> GetPointIntersect(FaceCustom face, CurveLoop curveLoop)
                    {
                        var result = new List<XYZ>();
                        try
                        {
                            var curves = curveLoop.Select(x => x);
                            foreach (var curve in curves)
                            {
                                var p = curve.Midpoint().RayPointToFace(curve.Direction(), face);
                                if (p != null)
                                {
                                    var vt1 = (curve.GetEndPoint(0) - p).Normalize();
                                    var vt2 = (curve.GetEndPoint(1) - p).Normalize();
                                    if (vt1.DotProduct(vt2) < 0) result.Add(p);
                                }
                            }
                        }
                        catch (Exception)
                        {
                        }
                        return result;
                    }
                }
                else
                {
                    var floor = curveLoopsActive[curveLoopsActiveCount - 1];
                    var floorCenter = floor.GetCenter();
                    result.Add(new FaceCustom(VTX, floorCenter));
                    result.Add(new FaceCustom(VTY, floorCenter));
                }
            }
            catch (Exception)
            {
            }
            return result;
        }
        private List<XYZ> GetPointsAround()
        {
            var result = new List<XYZ>();
            try
            {
                result = ElementsAround
                .Select(x => x.GetSolids())
                .Aggregate((a, b) => a.Concat(b).ToList())
                .Select(x => x.GetFacesFromSolid())
                .Aggregate((a, b) => a.Concat(b).ToList())
                .Select(x => x.GetEdgesAsCurveLoops())
                .Aggregate((a, b) => a.Concat(b).ToList())
                .Select(x => CurveloopExt.GetPoints(x))
                .Aggregate((a, b) => a.Concat(b).ToList())
                .Select(x => x.EditZ(FloorMinZ))
                .ToList();
            }
            catch (Exception)
            {
            }
            return result;
        }
        private List<AssemblyInfo> GetAssemblyRebarFloor()
        {
            var result = new List<AssemblyInfo>();
            try
            {
                var minz = FloorMinZ - 1000.MmToFoot();
                var maxz = FloorMaxZ + 1000.MmToFoot();
                var plg = CurveloopExt.GetPoints(OutlineReal)
                    .Select(x => x.EditZ(minz))
                    .ToList();
                var solid = plg.CreateSolid(XYZ.BasisZ, (maxz - minz).FootToMm());

                result = MSLabElementIntance.AssemblyRebarFloors
                    .Where(x =>
                    {
                        var l = x.AssemblyCurveBoundingBox;
                        var intersect = solid.IntersectWithCurve(l, new SolidCurveIntersectionOptions());
                        if (intersect.SegmentCount == 0) return false;
                        var length = 0.0;
                        for (int i = 0; i < intersect.SegmentCount; i++)
                        {
                            length += intersect.GetCurveSegment(i).Length;
                        }
                        return length * 100 / l.Length >= 50;
                    })
                    .ToList();
            }
            catch (Exception)
            {
            }
            return result;
        }
        private List<Grid> GetGridsAround()
        {
            var solidThickness = 1000;
            var grids = new List<Grid>();
            try
            {
                var isClockwise = OutlineReal.IsCounterclockwise(XYZ.BasisZ);
                var outlineRealOffset = isClockwise
                    ? OutlineReal.CreateOffset(2000.MmToFoot(), XYZ.BasisZ)
                    : OutlineReal.CreateOffset(2000.MmToFoot(), -XYZ.BasisZ);
                var ps = CurveloopExt.GetPoints(outlineRealOffset)
                    .Select(x => x.EditZ(FloorMinZ))
                    .ToList();
                var solid = ps.CreateSolid(XYZ.BasisZ, solidThickness);

                grids = MSLabElementIntance.Grids
                .Where(x =>
                {
                    var curve = x.Curve;
                    var l = Line.CreateBound(
                        curve.GetEndPoint(0).EditZ(FloorMinZ + solidThickness.MmToFoot() / 2),
                        curve.GetEndPoint(1).EditZ(FloorMinZ + solidThickness.MmToFoot() / 2));
                    var isIntersect = solid.IntersectWithCurve(l, new SolidCurveIntersectionOptions());
                    return isIntersect.SegmentCount > 0;
                })
                .ToList();
            }
            catch (Exception)
            {
            }
            return grids;
        }
        private List<Element> GetElementsAround()
        {
            var maxDev = 2000.MmToFoot();
            var results = new List<Element>();
            try
            {
                //offset curveloop
                var isClockwise = OutlineReal.IsCounterclockwise(XYZ.BasisZ);
                var outlineRealOffset = isClockwise
                    ? OutlineReal.CreateOffset(maxDev, XYZ.BasisZ)
                    : OutlineReal.CreateOffset(maxDev, -XYZ.BasisZ);
                var ps = CurveloopExt.GetPoints(outlineRealOffset)
                    .Select(x => x.EditZ(FloorMinZ))
                    .ToList();
                var solid = ps.CreateSolid(XYZ.BasisZ, 50.MmToFoot());
                var solidFilter = new ElementIntersectsSolidFilter(solid);
                results = new FilteredElementCollector(AC.Document)
                    .WhereElementIsNotElementType()
                    .WherePasses(solidFilter)
                    .Where(x => x.Id.ToString() != Floor.Id.ToString())
                    .ToList();
            }
            catch (Exception)
            {
            }
            return results;
        }
        private double GetAngle()
        {
            var result = 0.0;
            try
            {
                result = VTX.Y > 0
                    ? VTX.AngleTo(XYZ.BasisX)
                    : 2 * Math.PI - VTX.AngleTo(XYZ.BasisX);
            }
            catch (Exception)
            {
                result = 0.0;
            }
            return result;
        }
        private List<XYZ> GetFloorPoints()
        {
            var result = new List<XYZ>();
            try
            {
                result = FloorCurves.Select(x => new List<XYZ>() { x.GetEndPoint(0), x.GetEndPoint(1) })
                    .Aggregate((a, b) => a.Concat(b).ToList())
                    .ToList();
            }
            catch (Exception)
            {
            }
            return result;
        }
        private List<Curve> GetFloorCurves()
        {
            var results = new List<Curve>();
            try
            {
                var curveLoops = Floor.GetSolids()
                    .Select(x => x.GetFacesFromSolid())
                    .Aggregate((a, b) => a.Concat(b).ToList())
                    .Select(x => x.GetEdgesAsCurveLoops())
                    .Aggregate((a, b) => a.Concat(b).ToList())
                    .OrderBy(x => x.GetArea())
                    .ToList();
                results = curveLoops.Select(x => x.Select(y => y).ToList())
                    .Aggregate((a, b) => a.Concat(b).ToList())
                    .ToList();
            }
            catch (Exception)
            {
            }
            return results;
        }
        private XYZ GetVTX(out XYZ vty)
        {
            // vtx là vector có các đường thẳng song song và dài nhất
            var curvesGr = OutlineReal
                .OrderBy(x => x.Length)
                .ToList();
            var lineMain = curvesGr.Last();
            var vtx = lineMain.Direction();
            vty = vtx.CrossProduct(XYZ.BasisZ);
            vty = (Center - lineMain.Midpoint()).Normalize().DotProduct(vty) >= 0 ? -vty : vty;
            return vtx.EditZ(0);
        }
        private Outline GetOutLine(double extFt)
        {
            Outline result = null;
            try
            {
                var bb = Floor.get_BoundingBox(AC.Document.ActiveView);
                var vt = (bb.Max - bb.Min).Normalize();
                result = new Outline(bb.Min - vt * extFt, bb.Max + vt * extFt);
            }
            catch (Exception)
            {
            }
            return result;
        }

        public static Line GetLineRayTop(List<Curve> floorCurves, XYZ centerSlab, XYZ normal, XYZ direction, out Line lineRayBot)
        {
            lineRayBot = null;
            var faceCut = new FaceCustom(normal, centerSlab);
            var ps = new List<XYZ>();
            foreach (var c in floorCurves)
            {
                var p = c.Midpoint().RayPointToFace(c.Direction(), faceCut);
                if (p.IsSeem(c.GetEndPoint(0)) || p.IsSeem(c.GetEndPoint(1)))
                {
                    ps.Add(p);
                    break;
                }
                var vt1 = p - c.GetEndPoint(0);
                var vt2 = p - c.GetEndPoint(1);
                if (vt1.DotProduct(vt2).IsSmaller(0)) ps.Add(p);
            }
            var psg = ps.Distinct(new ComparePoint())
                .GroupBy(x => x.DotProduct(direction))
                .Where(x => x.Count() > 1)
                .OrderBy(x => x.First().DotProduct(direction))
                .ToList();
            var psgF = psg.FirstOrDefault().OrderBy(x => x.Z).ToList();
            var psgE = psg.LastOrDefault().OrderBy(x => x.Z).ToList();

            var p1F = psgF.FirstOrDefault();
            var p1E = psgF.LastOrDefault();
            var p2F = psgE.FirstOrDefault();
            var p2E = psgE.LastOrDefault();
            lineRayBot = Line.CreateBound(p1F, p2F);
            return Line.CreateBound(p1E, p2E);
        }
    }
    public class MSlabOpening
    {
        public Floor Floor { get; }
        public List<XYZ> Points { get; }
        public CurveLoop CurveLoop { get; }
        public XYZ Normal { get; }
        public System.Windows.Shapes.Polygon OpeningsInCanvas { get; set; }
        public MSlabOpening(Floor floor, List<XYZ> points)
        {
            Floor = floor;
            Points = points;
        }
        public MSlabOpening(Floor floor, CurveLoop curveLoop)
        {
            Floor = floor;
            CurveLoop = curveLoop;
            Normal = curveLoop.GetNormal();
            Points = CurveloopExt.GetPoints(curveLoop);
        }
        public void DrawInCanvas()
        {

        }
    }
}
