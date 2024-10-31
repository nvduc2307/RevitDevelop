using Autodesk.Revit.DB.Structure;
using HcBimUtils;
using HcBimUtils.DocumentUtils;
using HcBimUtils.MoreLinq;
using RevitDevelop.Utils.NumberingRevitElements;
using Utils.canvass;
using Utils.CompareElement;
using Utils.Curveloops;
using Utils.Geometries;
using Utils.NumberUtils;
using Utils.RebarInRevits.Models;

namespace RevitDevelop.Tools.Rebars.InstallRebarSlab.models
{
    public class MSlabRebar : ObservableObject
    {
        private MSlabRebarLayer _rebarLayerActive;
        public MSlab Slab { get; set; }
        public XYZ VTX { get; set; }
        public XYZ VTY { get; set; }
        public XYZ VTZ { get; set; }
        public MSlabRebarLayer TopX { get; set; }
        public MSlabRebarLayer TopY { get; set; }
        public MSlabRebarLayer BotX { get; set; }
        public MSlabRebarLayer BotY { get; set; }
        public MSlabRebarLayer RebarLayerActive
        {
            get => _rebarLayerActive;
            set
            {
                _rebarLayerActive = value;
                OnPropertyChanged();
            }
        }
        public List<NumberingRevitRebar> AllRebars { get; set; }

        public MSlabRebar(
            MSlab slab,
            XYZ vTX,
            XYZ vTY,
            XYZ vTZ,
            MSlabRebarLayer topX,
            MSlabRebarLayer topY,
            MSlabRebarLayer botX,
            MSlabRebarLayer botY)
        {
            Slab = slab;
            VTX = vTX;
            VTY = vTY;
            VTZ = vTZ;
            TopX = topX;
            TopY = topY;
            BotX = botX;
            BotY = botY;
            RebarLayerActive = topX;
            AllRebars = new List<NumberingRevitRebar>();
        }
    }
    public class MSlabRebarLayer : ObservableObject
    {
        private RebarBarTypeCustom _rebarBarTypeCustom;
        private double _spacing;
        private XYZ _normal;
        private XYZ _direction;
        public MSlab Slab { get; set; }
        public double CoverLayer { get; set; }
        public XYZ Direction
        {
            get => _direction;
            set
            {
                _direction = value;
                OnPropertyChanged();
            }
        }
        public XYZ Normal
        {
            get => _normal;
            set
            {
                _normal = value;
                OnPropertyChanged();
                NormalEventAction?.Invoke();
            }
        }
        public Action NormalEventAction { get; set; }
        public int RebarLayer { get; set; }
        public RebarBarTypeCustom RebarBarTypeCustom
        {
            get => _rebarBarTypeCustom;
            set
            {
                _rebarBarTypeCustom = value;
                OnPropertyChanged();
                UpdateLengthDevelopOfLayer();
                RebarBarTypeCustomEventAction?.Invoke();
            }
        }
        public Action RebarBarTypeCustomEventAction { get; set; }
        public List<System.Windows.Shapes.Line> RebarsInCanvas { get; set; }
        public double Spacing
        {
            get => _spacing;
            set
            {
                _spacing = value;
                OnPropertyChanged();
                SpacingEventAction?.Invoke();
            }
        }
        public Action SpacingEventAction { get; set; }
        public int Quantity { get; set; }
        public List<MSlabRebarInfo> MSlabRebars { get; set; }
        public List<FaceCustom> FaceRebarOns { get; set; }
        public List<Line> LineRebarOns { get; set; }

        public double L2 { get; set; }
        public double L3S { get; set; }
        public double Lb { get; set; }

        public MSlabRebarLayer(
            MSlab slab,
            double coverLayer,
            RebarBarTypeCustom rebarBarTypeCustom,
            XYZ direction,
            XYZ normal,
            int rebarLayer,
            double spacing)
        {
            Slab = slab;
            CoverLayer = coverLayer;
            RebarLayer = rebarLayer;
            RebarBarTypeCustom = rebarBarTypeCustom;
            Spacing = spacing;
            Direction = direction;
            Normal = normal;
            RebarsInCanvas = new List<System.Windows.Shapes.Line>();
            UpdateLengthDevelopOfLayer();

            FaceRebarOns = GetFaceRebarOns(Slab, Normal, spacing.MmToFoot());
            LineRebarOns = GetLineRebarOns(this, Slab, Normal, Direction, FaceRebarOns, out List<MSlabRebarInfo> mSlabRebarInfos);
            MSlabRebars = mSlabRebarInfos;
            UpdateLayerMSlabRebars(this);
        }
        private void UpdateLengthDevelopOfLayer()
        {
            //setting length develop
            var layerType = (MSlabRebarLayerType)RebarLayer;
            var ruleDevelopApply = Slab.MSLabElementIntance
                .SettingRuleRebarGrade.Rules
                .FirstOrDefault(x => x.Name == RebarBarTypeCustom.NameStyle);
            L2 = ruleDevelopApply.L2;
            L3S = ruleDevelopApply.L3Slab;
            Lb = ruleDevelopApply.Lb;
        }
        public static void NormalEventActionF(
            MSlabRebarLayer rebarLayerTarget,
            List<MSlab> mSlabs,
            CanvasPageBase canvasPageBase,
            XYZ rCenter,
            InstallRebarSlabModel installRebarSlabModel)
        {
            var layerType = (MSlabRebarLayerType)rebarLayerTarget.RebarLayer;

            rebarLayerTarget.FaceRebarOns = GetFaceRebarOns(rebarLayerTarget.Slab, rebarLayerTarget.Normal, rebarLayerTarget.Spacing.MmToFoot());
            rebarLayerTarget.LineRebarOns = GetLineRebarOns(
                rebarLayerTarget,
                rebarLayerTarget.Slab,
                rebarLayerTarget.Normal,
                rebarLayerTarget.Direction,
                rebarLayerTarget.FaceRebarOns,
                out List<MSlabRebarInfo> mSlabRebarInfos);
            rebarLayerTarget.MSlabRebars = mSlabRebarInfos;
            UpdateLayerMSlabRebars(rebarLayerTarget);
        }
        public static void SpacingEventActionF(
            MSlabRebarLayer rebarLayerTarget,
            List<MSlab> mSlabs,
            CanvasPageBase canvasPageBase,
            XYZ rCenter,
            OptionStyleInstanceInCanvas option,
            InstallRebarSlabModel installRebarSlabModel)
        {
            var layerType = (MSlabRebarLayerType)rebarLayerTarget.RebarLayer;

            rebarLayerTarget.FaceRebarOns = GetFaceRebarOns(rebarLayerTarget.Slab, rebarLayerTarget.Normal, rebarLayerTarget.Spacing.MmToFoot());
            rebarLayerTarget.LineRebarOns = GetLineRebarOns(
                rebarLayerTarget,
                rebarLayerTarget.Slab,
                rebarLayerTarget.Normal,
                rebarLayerTarget.Direction,
                rebarLayerTarget.FaceRebarOns,
                out List<MSlabRebarInfo> mSlabRebarInfos);
            rebarLayerTarget.MSlabRebars = mSlabRebarInfos;
            UpdateLayerMSlabRebars(rebarLayerTarget);
            if (rebarLayerTarget.RebarsInCanvas.Count > 0)
            {
                foreach (var item in rebarLayerTarget.RebarsInCanvas)
                {
                    canvasPageBase.Parent.Children.Remove(item);
                }
            }
            rebarLayerTarget.RebarsInCanvas = DrawRebarLayerInCanvas(rebarLayerTarget, mSlabs, option, rCenter, canvasPageBase, installRebarSlabModel);
        }
        public static List<System.Windows.Shapes.Line> DrawRebarLayerInCanvas(
            MSlabRebarLayer rebarLayer,
            List<MSlab> mSlabs,
            OptionStyleInstanceInCanvas option,
            XYZ rCenter,
            CanvasPageBase canvasPageBase,
            InstallRebarSlabModel installRebarSlabModel)
        {
            var results = new List<System.Windows.Shapes.Line>();
            foreach (var l in rebarLayer.LineRebarOns)
            {
                try
                {
                    var p1 = l.GetEndPoint(0).ConvertPointRToC(rCenter, canvasPageBase);
                    var p2 = l.GetEndPoint(1).ConvertPointRToC(rCenter, canvasPageBase);
                    var rebar = new InstanceInCanvasLine(canvasPageBase, option, p1, p2);
                    rebar.UIElement.MouseLeftButtonDown += (o, e) =>
                    {
                        MSlab.ActionElementSelected(rebarLayer.Slab, mSlabs, installRebarSlabModel);
                    };
                    rebar.DrawInCanvas();
                    results.Add(rebar.UIElement as System.Windows.Shapes.Line);
                }
                catch (Exception)
                {
                }
            }
            return results;
        }
        private static List<FaceCustom> GetFaceRebarOns(MSlab mSlab, XYZ normal, double spacing)
        {
            var offset = 50.MmToFoot();
            var results = new List<FaceCustom>();
            try
            {
                var ps = mSlab.PointsSlabOnFloorPlan.OrderBy(x => x.DotProduct(normal));
                var pMin = ps.First() + normal * offset;
                var pMax = ps.Last() - normal * offset;
                var length = pMin.DistanceTo(pMax);
                var faceQty = length.SolveNumber(spacing);
                for (int i = 0; i < faceQty; i++)
                {
                    results.Add(new FaceCustom(normal, pMin + normal * spacing * i));
                }
            }
            catch (Exception)
            {
            }
            return results;
        }
        private static MSlabRebarInfo CheckRebarDevelop(MSlabRebarLayer rebarLayer, Line lRebar)
        {
            var mSlabRebarInfo = new MSlabRebarInfo();
            var toleen = 1.2;
            var lNeo = rebarLayer.L2 * rebarLayer.RebarBarTypeCustom.ModelBarDiameter;
            var lNeoHook = rebarLayer.Lb * rebarLayer.RebarBarTypeCustom.ModelBarDiameter;
            var lHookMin = 10 * rebarLayer.RebarBarTypeCustom.ModelBarDiameter;

            var slabsAround = rebarLayer.Slab.ElementsAround
                .Where(x => x.Category.ToBuiltinCategory() == BuiltInCategory.OST_Floors)
                .Cast<Floor>()
                .ToList();

            var elementsAround = rebarLayer.Slab.ElementsAround
                .Where(x => x.Category.ToBuiltinCategory() != BuiltInCategory.OST_Floors)
                .Cast<FamilyInstance>()
                .ToList();

            switch ((MSlabRebarLayerType)rebarLayer.RebarLayer)
            {
                case MSlabRebarLayerType.Rebar_Slab_Top_X:
                case MSlabRebarLayerType.Rebar_Slab_Top_Y:
                    lNeo = rebarLayer.L2 * rebarLayer.RebarBarTypeCustom.ModelBarDiameter;
                    break;
                case MSlabRebarLayerType.Rebar_Slab_Bot_X:
                case MSlabRebarLayerType.Rebar_Slab_Bot_Y:
                    lNeo = rebarLayer.L3S * rebarLayer.RebarBarTypeCustom.ModelBarDiameter;
                    break;
            }

            //
            var vt = rebarLayer.Direction;
            var p1 = lRebar.GetEndPoint(0);
            var p2 = lRebar.GetEndPoint(1);
            //find start
            var lineStart = Line.CreateBound(p1 - vt * lNeo * toleen, p2);
            var lineEnd = Line.CreateBound(p1, p2 + vt * lNeo * toleen);
            XYZ pStart = p1;
            XYZ pEnd = p2;
            double lbhStart = 0.0;
            double lbhEnd = 0.0;
            bool isStartHook = false;
            bool isEndHook = false;
            double lHookStart = 0.0;
            double lHookEnd = 0.0;

            foreach (var ele in elementsAround)
            {
                var solid = ele.GetSingleSolid();
                var intersectIn = solid.IntersectWithCurve(lineStart, new SolidCurveIntersectionOptions());
                var intersectOut = solid.IntersectWithCurve(lineStart, new SolidCurveIntersectionOptions() { ResultType = SolidCurveIntersectionMode.CurveSegmentsOutside });
                var countIn = intersectIn.SegmentCount;
                var countOut = intersectOut.SegmentCount;
                if (countOut == 2)
                {
                    //truong hop neo co hook
                    pStart = p1 - vt * lNeoHook;
                    lbhStart = p1.Distance(pStart);
                    isStartHook = true;
                    lHookStart = Math.Abs(lNeo - lbhStart).IsSmaller(lHookMin) ? lHookMin : Math.Abs(lNeo - lbhStart);
                    break;
                }

                if (countIn == 1)
                {
                    //truong hop neo thang
                    pStart = p1 - vt * lNeo;
                    lbhStart = p1.Distance(pStart);
                    isStartHook = false;
                    lHookStart = Math.Abs(lNeo - lbhStart);
                    break;
                }
            }

            foreach (var ele in elementsAround)
            {
                var solid = ele.GetSingleSolid();
                var intersectIn = solid.IntersectWithCurve(lineEnd, new SolidCurveIntersectionOptions());
                var intersectOut = solid.IntersectWithCurve(lineEnd, new SolidCurveIntersectionOptions() { ResultType = SolidCurveIntersectionMode.CurveSegmentsOutside });
                var countIn = intersectIn.SegmentCount;
                var countOut = intersectOut.SegmentCount;
                if (countOut == 2)
                {
                    //truong hop neo co hook
                    pEnd = p2 + vt * lNeoHook;
                    lbhEnd = p2.Distance(pEnd);
                    isEndHook = true;
                    lHookEnd = Math.Abs(lNeo - lbhEnd).IsSmaller(lHookMin) ? lHookMin : Math.Abs(lNeo - lbhEnd);
                    break;
                }

                if (countIn == 1)
                {
                    //truong hop neo thang
                    pEnd = p2 + vt * lNeo;
                    lbhEnd = p2.Distance(pEnd);
                    isEndHook = false;
                    lHookEnd = Math.Abs(lNeo - lbhEnd);
                    break;
                }
            }

            foreach (var slab in slabsAround)
            {
                var solid = slab.GetSingleSolid();
                var intersect = solid.IntersectWithCurve(lineStart, new SolidCurveIntersectionOptions());
                var count = intersect.SegmentCount;
                if (count == 1)
                {
                    pStart = p1 - vt * lNeo;
                    lbhStart = p1.Distance(pStart);
                    isStartHook = false;
                    lHookStart = 0;
                    break;
                }
            }

            foreach (var slab in slabsAround)
            {
                var solid = slab.GetSingleSolid();
                var intersect = solid.IntersectWithCurve(lineEnd, new SolidCurveIntersectionOptions());
                var count = intersect.SegmentCount;
                if (count == 1)
                {
                    pEnd = p2 + vt * lNeo;
                    lbhEnd = p2.Distance(pEnd);
                    isEndHook = false;
                    lHookEnd = 0;
                    break;
                }
            }

            //kiem tra xem dau cat opening, neu cat boi opening thi hook min
            if (p1.IsSeem(pStart))
            {
                isStartHook = true;
                lbhStart = 0;
                lHookStart = lHookMin;
            }
            if (p2.IsSeem(pEnd))
            {
                isEndHook = true;
                lbhEnd = 0;
                lHookEnd = lHookMin;
            }

            var l = Line.CreateBound(pStart, pEnd);
            mSlabRebarInfo.Slab = rebarLayer.Slab;
            mSlabRebarInfo.RebarBarTypeCustom = rebarLayer.RebarBarTypeCustom;
            mSlabRebarInfo.RebarLayer = rebarLayer;
            mSlabRebarInfo.RebarLine = l;
            mSlabRebarInfo.IsStartHook = isStartHook;
            mSlabRebarInfo.IsEndHook = isEndHook;
            mSlabRebarInfo.LbhStart = lbhStart;
            mSlabRebarInfo.LbhEnd = lbhEnd;
            mSlabRebarInfo.LStartHook = lHookStart;
            mSlabRebarInfo.LEndHook = lHookEnd;
            mSlabRebarInfo.Direction = rebarLayer.Direction;
            mSlabRebarInfo.Normal = rebarLayer.Normal;
            return mSlabRebarInfo;
        }
        private static List<Line> GetLineRebarOns(MSlabRebarLayer rebarLayer, MSlab mSlab, XYZ normal, XYZ direction, List<FaceCustom> faceRebarOns, out List<MSlabRebarInfo> mSlabRebars)
        {
            mSlabRebars = new List<MSlabRebarInfo>();
            //tinh toan line thep
            var results = new List<Line>();
            var openings = mSlab.MSlabOpenings.Count > 0
                ? mSlab.MSlabOpenings
                .Select(x => x.CurveLoop.GenerateCurveLoop())
                .Select(x => x.CreateOffset(mSlab.CoverMm.MmToFoot(), x.GetNormal()))
                .Select(x => x.ToList())
                .Aggregate((a, b) => a.Concat(b).ToList())
                .ToList()
                : new List<Curve>();

            //var openings = mSlab.MSlabOpenings.Count > 0
            //    ? mSlab.MSlabOpenings
            //    .Select(x => x.CurveLoop.ToList())
            //    .Aggregate((a, b) => a.Concat(b).ToList())
            //    .ToList()
            //    : new List<Curve>();

            var mSlabOutLine = mSlab.OutlineReal
                .Concat(openings);
            foreach (var faceRebarOn in faceRebarOns)
            {
                var ps = new List<XYZ>();
                foreach (var curve in mSlabOutLine)
                {
                    try
                    {
                        var pointsIntersect = curve.CurveIntersectFace(faceRebarOn);
                        if (pointsIntersect.Count > 0)
                            ps.AddRange(pointsIntersect);
                    }
                    catch (Exception)
                    {
                    }
                }
                var psNew = ps
                    .Where(x => x != null)
                    .Distinct(new ComparePoint())
                    .OrderBy(x => x.DotProduct(direction))
                    .ToList();
                var psNewCount = psNew.Count;
                if (psNewCount % 2 == 0)
                {
                    for (int i = 0; i < psNewCount; i += 2)
                    {
                        var j = i + 1;
                        if (psNew[i].Distance(psNew[j]).IsGreaterEqual(rebarLayer.L2 * rebarLayer.RebarBarTypeCustom.ModelBarDiameter))
                        {
                            var l = Line.CreateBound(psNew[i], psNew[j]);
                            var mSlabRebar = CheckRebarDevelop(rebarLayer, l);
                            results.Add(mSlabRebar.RebarLine);
                            mSlabRebars.Add(mSlabRebar);
                        }
                    }
                }
            }
            return results;
        }
        public static void UpdateLayerMSlabRebars(MSlabRebarLayer rebarLayerTarget)
        {
            var cover = rebarLayerTarget.CoverLayer;
            var z = rebarLayerTarget.Slab.FloorMaxZ - cover.MmToFoot();
            var vtz = XYZ.BasisZ;
            switch ((MSlabRebarLayerType)rebarLayerTarget.RebarLayer)
            {
                case MSlabRebarLayerType.Rebar_Slab_Top_X:
                    z = rebarLayerTarget.Slab.FloorMaxZ - cover.MmToFoot();
                    vtz = -XYZ.BasisZ;
                    break;
                case MSlabRebarLayerType.Rebar_Slab_Top_Y:
                    z = rebarLayerTarget.Slab.FloorMaxZ - cover.MmToFoot();
                    vtz = -XYZ.BasisZ;
                    break;
                case MSlabRebarLayerType.Rebar_Slab_Bot_X:
                    z = rebarLayerTarget.Slab.FloorMinZ + cover.MmToFoot();
                    vtz = XYZ.BasisZ;
                    break;
                case MSlabRebarLayerType.Rebar_Slab_Bot_Y:
                    z = rebarLayerTarget.Slab.FloorMinZ + cover.MmToFoot();
                    vtz = XYZ.BasisZ;
                    break;
            }
            foreach (var r in rebarLayerTarget.MSlabRebars)
            {
                var shape = new List<Curve>();
                var l = Line.CreateBound(r.RebarLine.GetEndPoint(0).EditZ(z), r.RebarLine.GetEndPoint(1).EditZ(z));
                try
                {
                    var p1 = l.GetEndPoint(0) + vtz * r.LStartHook;
                    var p2 = l.GetEndPoint(0);
                    var p3 = l.GetEndPoint(1);
                    var p4 = l.GetEndPoint(1) + vtz * r.LEndHook;

                    if (r.IsStartHook) shape.Add(Line.CreateBound(p1, p2));
                    shape.Add(Line.CreateBound(p2, p3));
                    if (r.IsEndHook) shape.Add(Line.CreateBound(p3, p4));
                }
                catch (Exception)
                {
                    shape = new List<Curve>() { l };
                }
                r.RebarShape = shape;
            }
        }
    }
    public class MSlabRebarInfo
    {
        public MSlab Slab { get; set; }
        public double CoverLayer { get; set; }
        public XYZ Direction { get; set; }
        public XYZ Normal { get; set; }
        public MSlabRebarLayer RebarLayer { get; set; }
        public RebarBarTypeCustom RebarBarTypeCustom { get; set; }
        public List<Curve> RebarShape { get; set; }
        public Line RebarLine { get; set; }
        public bool IsStartHook { get; set; }
        public bool IsEndHook { get; set; }
        public double LbhStart { get; set; }
        public double LbhEnd { get; set; }
        public double LStartHook { get; set; }
        public double LEndHook { get; set; }
        public NumberingRevitRebar RebarNumbering { get; set; }
        public MSlabRebarInfo()
        {
            RebarShape = new List<Curve>();
        }
        public void CreateRebar()
        {
            try
            {
                var host = DirectShape.CreateElement(AC.Document, new ElementId(BuiltInCategory.OST_Floors));
                var rebar = Rebar.CreateFromCurves(
                    AC.Document,
                    RebarStyle.Standard,
                    RebarLayer.RebarBarTypeCustom.RebarBarType,
                    null,
                    null,
                    host,
                    Normal,
                    RebarShape,
                    RebarHookOrientation.Left,
                    RebarHookOrientation.Right,
                    true,
                    true);
                RebarNumbering = new NumberingRevitRebar(rebar)
                {
                    RebarLayer = RebarLayer.RebarLayer,
                    HostId = int.Parse(Slab.Floor.Id.ToString())
                };

            }
            catch (Exception ex)
            {
                //Debug.WriteLine(JsonConvert.SerializeObject(RebarShape.ToList().GetPoints()));
            }
        }
    }

    public class MSlabRebarLayerUi
    {
        public string Name { get; }
        public MSlabRebarLayerType MSlabRebarLayerType { get; }
        public MSlabRebarLayerUi(MSlabRebarLayerType mSlabRebarLayerType, string name)
        {
            MSlabRebarLayerType = mSlabRebarLayerType;
            Name = name;
        }

        public static MSlabRebarLayer GetMSlabRebarLayer(MSlabRebarLayerType mSlabRebarLayerType, MSlabRebar mSlabRebar)
        {
            var result = mSlabRebar.TopX;
            switch (mSlabRebarLayerType)
            {
                case MSlabRebarLayerType.Rebar_Slab_Top_X:
                    result = mSlabRebar.TopX;
                    break;
                case MSlabRebarLayerType.Rebar_Slab_Top_Y:
                    result = mSlabRebar.TopY;
                    break;
                case MSlabRebarLayerType.Rebar_Slab_Bot_X:
                    result = mSlabRebar.BotX;
                    break;
                case MSlabRebarLayerType.Rebar_Slab_Bot_Y:
                    result = mSlabRebar.BotY;
                    break;
            }
            return result;
        }

        public static List<MSlabRebarLayerUi> GetMSlabRebarLayerUi()
        {
            return new List<MSlabRebarLayerUi>()
            {
                new MSlabRebarLayerUi(MSlabRebarLayerType.Rebar_Slab_Top_X, "Rebar_Top_X"),
                new MSlabRebarLayerUi(MSlabRebarLayerType.Rebar_Slab_Top_Y, "Rebar_Top_Y"),
                new MSlabRebarLayerUi(MSlabRebarLayerType.Rebar_Slab_Bot_X, "Rebar_Bot_X"),
                new MSlabRebarLayerUi(MSlabRebarLayerType.Rebar_Slab_Bot_Y, "Rebar_Bot_Y"),
            };
        }
    }
    public enum MSlabRebarLayerType
    {
        Rebar_Slab_Top_X = 0,
        Rebar_Slab_Top_Y = 1,
        Rebar_Slab_Bot_X = 2,
        Rebar_Slab_Bot_Y = 3,
    }
    public enum RebarTypeNameType
    {
        Rebar_Slab_Main = 0,
        Rebar_Slab_Additional = 1,
        Rebar_Slab_Leg_Dog = 2
    }
}
