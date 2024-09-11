using Autodesk.Revit.DB.Structure;
using HcBimUtils;
using HcBimUtils.DocumentUtils;
using HcBimUtils.MoreLinq;
using Utils.canvass;
using Utils.CompareElement;
using Utils.Curveloops;
using Utils.Geometries;
using Utils.NumberUtils;
using Utils.RebarInRevits.Models;

namespace RIMT.InstallRebarSlab.models
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
            RebarBarTypeCustom = rebarBarTypeCustom;
            RebarLayer = rebarLayer;
            Spacing = spacing;
            Direction = direction;
            Normal = normal;
            RebarsInCanvas = new List<System.Windows.Shapes.Line>();

            FaceRebarOns = GetFaceRebarOns(Slab, Normal, spacing.MmToFoot());
            LineRebarOns = GetLineRebarOns(Slab, Normal, Direction, FaceRebarOns);
            MSlabRebars = GetMSlabRebars(this);
        }
        public static void NormalEventActionF(
            MSlabRebarLayer rebarLayerTarget,
            List<MSlab> mSlabs,
            CanvasPageBase canvasPageBase,
            XYZ rCenter,
            InstallRebarSlabModel installRebarSlabModel)
        {
            rebarLayerTarget.FaceRebarOns = GetFaceRebarOns(rebarLayerTarget.Slab, rebarLayerTarget.Normal, rebarLayerTarget.Spacing.MmToFoot());
            rebarLayerTarget.LineRebarOns = GetLineRebarOns(rebarLayerTarget.Slab, rebarLayerTarget.Normal, rebarLayerTarget.Direction, rebarLayerTarget.FaceRebarOns);
            rebarLayerTarget.MSlabRebars = GetMSlabRebars(rebarLayerTarget);
        }
        public static void SpacingEventActionF(MSlabRebarLayer rebarLayerTarget, List<MSlab> mSlabs, CanvasPageBase canvasPageBase, XYZ rCenter, OptionStyleInstanceInCanvas option, InstallRebarSlabModel installRebarSlabModel)
        {
            rebarLayerTarget.FaceRebarOns = GetFaceRebarOns(rebarLayerTarget.Slab, rebarLayerTarget.Normal, rebarLayerTarget.Spacing.MmToFoot());
            rebarLayerTarget.LineRebarOns = GetLineRebarOns(rebarLayerTarget.Slab, rebarLayerTarget.Normal, rebarLayerTarget.Direction, rebarLayerTarget.FaceRebarOns);
            rebarLayerTarget.MSlabRebars = GetMSlabRebars(rebarLayerTarget);
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
            MSlabRebarLayer rebarLayer, List<MSlab> mSlabs, OptionStyleInstanceInCanvas option, XYZ rCenter, CanvasPageBase canvasPageBase, InstallRebarSlabModel installRebarSlabModel)
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
        private static List<Line> GetLineRebarOns(MSlab mSlab, XYZ normal, XYZ direction, List<FaceCustom> faceRebarOns)
        {
            var results = new List<Line>();
            var openings = mSlab.Profiles.Count == 1
                    ? new List<Curve>()
                    : mSlab.Profiles
                    .Slice(0, mSlab.Profiles.Count - 1)
                    .Select(x => x.CreateOffset(mSlab.CoverMm.MmToFoot(), -mSlab.VTZ))
                    .Select(x => x.ToList())
                    .Aggregate((a, b) => a.Concat(b).ToList())
                    .ToList();
            var mSlabOutLine = mSlab.OutlineReal
                .Concat(openings);
            foreach (var faceRebarOn in faceRebarOns)
            {
                var ps = new List<XYZ>();
                foreach (var curve in mSlabOutLine)
                {
                    try
                    {
                        var p = curve.Midpoint().RayPointToFace(curve.Direction(), faceRebarOn);
                        if (p.IsSeem(curve.GetEndPoint(0)) || p.IsSeem(curve.GetEndPoint(1))) ps.Add(p);
                        var vt1 = curve.GetEndPoint(0) - p;
                        var vt2 = curve.GetEndPoint(1) - p;
                        if (vt1.DotProduct(vt2).IsSmaller(0)) ps.Add(p);
                    }
                    catch (Exception)
                    {
                    }
                }
                var psNew = ps
                    .OrderBy(x => x.DotProduct(direction))
                    .Distinct(new ComparePoint())
                    .ToList();
                var psNewCount = psNew.Count;
                if (psNewCount % 2 == 0)
                {
                    for (int i = 0; i < psNewCount; i += 2)
                    {
                        var j = i + 1;
                        results.Add(Line.CreateBound(psNew[i], psNew[j]));
                    }
                }
                //else
                //{
                //    IO.ShowInfo($"{psNewCount}");
                //}
            }
            return results;
        }
        public static List<MSlabRebarInfo> GetMSlabRebars(MSlabRebarLayer rebarLayerTarget)
        {
            var results = new List<MSlabRebarInfo>();
            var cover = rebarLayerTarget.CoverLayer;
            var z = rebarLayerTarget.Slab.FloorMaxZ - cover.MmToFoot();
            switch ((MSlabRebarLayerType)rebarLayerTarget.RebarLayer)
            {
                case MSlabRebarLayerType.Rebar_Slab_Top_X:
                    z = rebarLayerTarget.Slab.FloorMaxZ - cover.MmToFoot();
                    break;
                case MSlabRebarLayerType.Rebar_Slab_Top_Y:
                    z = rebarLayerTarget.Slab.FloorMaxZ - cover.MmToFoot();
                    break;
                case MSlabRebarLayerType.Rebar_Slab_Bot_X:
                    z = rebarLayerTarget.Slab.FloorMinZ + cover.MmToFoot();
                    break;
                case MSlabRebarLayerType.Rebar_Slab_Bot_Y:
                    z = rebarLayerTarget.Slab.FloorMinZ + rebarLayerTarget.CoverLayer.MmToFoot();
                    break;
            }
            foreach (var l in rebarLayerTarget.LineRebarOns)
            {
                var r = new MSlabRebarInfo()
                {
                    Slab = rebarLayerTarget.Slab,
                    CoverLayer = rebarLayerTarget.CoverLayer,
                    Direction = rebarLayerTarget.Direction,
                    Normal = rebarLayerTarget.Normal,
                    RebarLayer = rebarLayerTarget,
                    RebarBarTypeCustom = rebarLayerTarget.RebarBarTypeCustom,
                    RebarLineFake = Line.CreateBound(l.GetEndPoint(0).EditZ(z), l.GetEndPoint(1).EditZ(z))
                };
                results.Add(r);
            }
            return results;
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
        public List<XYZ> RebarShape { get; set; }
        public Line RebarLineFake { get; set; }
        public void CreateRebar()
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
                new List<Curve>() { RebarLineFake },
                RebarHookOrientation.Left,
                RebarHookOrientation.Right,
                true,
                true);
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
