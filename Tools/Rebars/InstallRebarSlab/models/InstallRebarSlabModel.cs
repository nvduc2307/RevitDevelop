using HcBimUtils;
using Utils.CompareElement;

namespace RevitDevelop.Tools.Rebars.InstallRebarSlab.models
{
    public class InstallRebarSlabModel : ObservableObject
    {
        private MSlab _mSlabSelected;
        public MSlabCoordinateAxis MSlabCoordinateAxis { get; set; }
        public MSLabElementIntance MSLabElementIntance { get; set; }
        public List<MSlab> MSlabs { get; set; }
        public MSlab MSlabSelected
        {
            get => _mSlabSelected;
            set
            {
                _mSlabSelected = value;
                OnPropertyChanged();
            }
        }
        public List<MSlabGrid> MSlabGrids { get; private set; }
        public List<MSlabElementNeighborhood> MSlabElementNeighborhoods { get; set; }
        public XYZ MSlabCenter { get; set; }
        public BoundingBoxXYZ MSlabBox { get; set; }

        public InstallRebarSlabModel(MSLabElementIntance mSLabElementIntance, List<MSlab> mSlabs)
        {
            MSLabElementIntance = mSLabElementIntance;
            MSlabs = mSlabs;
            MSlabSelected = mSlabs.FirstOrDefault();
            MSlabGrids = MSlabs
                .Select(x => x.GridsAround)
                .Aggregate((a, b) => a.Concat(b).ToList())
                .Select(x => new MSlabGrid(x))
                .ToList();
            MSlabElementNeighborhoods = MSlabs
                .Select(x => x.ElementsAround)
                .Aggregate((a, b) => a.Concat(b).ToList())
                .Where(x => !MSlabs.Any(y => y.Floor.Id.ToString().Equals(x.Id)))
                .Select(x => new MSlabElementNeighborhood(x))
                .Where(x => x.AllCurveLoops != null)
                .ToList();
            MSlabBox = GetMSlabBox();
            MSlabCenter = MSlabBox.Min.Midpoint(MSlabBox.Max);
            MSlabCoordinateAxis = GetSlabCoordinateAxis();
        }
        private BoundingBoxXYZ GetMSlabBox()
        {
            BoundingBoxXYZ result = null;
            try
            {
                var points1 = MSlabs.Select(x => x.PointsSlabOnFloorPlan)
                    .Aggregate((a, b) => a.Concat(b).ToList());
                var points2 = MSlabElementNeighborhoods.Count > 0 ? MSlabElementNeighborhoods.Select(x => x.PointsOnFloorPlan)
                    .Aggregate((a, b) => a.Concat(b).ToList())
                    : new List<XYZ>();
                var points = points1.Concat(points2).ToList();

                var minx = points.Min(x => x.X);
                var miny = points.Min(x => x.Y);
                var minz = points.Min(x => x.Z);
                var maxx = points.Max(x => x.X);
                var maxy = points.Max(x => x.Y);
                var maxz = points.Max(x => x.Z);

                var min = new XYZ(minx, miny, maxz);
                var max = new XYZ(maxx, maxy, maxz);
                result = new BoundingBoxXYZ()
                {
                    Min = min,
                    Max = max
                };
            }
            catch (Exception)
            {
            }
            return result;
        }
        private MSlabCoordinateAxis GetSlabCoordinateAxis()
        {
            MSlabCoordinateAxis result = null;
            try
            {
                var vTX = GetVTX();
                var vTY = GetVTY();
                var vTZ = vTX != null && vTY != null
                    ? vTX.CrossProduct(vTY).Normalize()
                    : null;
                result = new MSlabCoordinateAxis(vTX, vTY, vTZ);
            }
            catch (Exception)
            {
            }
            return result;
        }
        private XYZ GetVTX()
        {
            XYZ result = null;
            try
            {
                if (MSlabs.Count > 0)
                {
                    var vtxs = MSlabs.Select(x => x.VTX)
                        .GroupBy(x => x, new ComparePoint())
                        .OrderBy(x => x.Count())
                        .LastOrDefault();
                    result = vtxs != null
                    ? vtxs.FirstOrDefault()
                    : null;
                }
            }
            catch (Exception)
            {
            }
            return result;
        }
        private XYZ GetVTY()
        {
            XYZ result = null;
            try
            {
                if (MSlabs.Count > 0)
                {
                    var vtys = MSlabs.Select(x => x.VTY)
                        .GroupBy(x => x, new ComparePoint())
                        .OrderBy(x => x.Count())
                        .LastOrDefault();
                    result = vtys != null
                    ? vtys.FirstOrDefault()
                    : null;
                }
            }
            catch (Exception)
            {
            }
            return result;
        }
    }
}
