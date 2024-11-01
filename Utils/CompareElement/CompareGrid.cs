using HcBimUtils;

namespace RevitDevelop.Utils.CompareElement
{
    public class CompareGrid : IEqualityComparer<Grid>
    {
        public bool Equals(Grid x, Grid y)
        {
            return x.Name == y.Name;
        }

        public int GetHashCode(Grid obj)
        {
            return 0;
        }
    }
    public class CompareGridDirection : IEqualityComparer<Grid>
    {
        public bool Equals(Grid x, Grid y)
        {
            var d1 = x.Curve.Direction();
            var d2 = y.Curve.Direction();
            return d1.IsSameDirection(d2);
        }

        public int GetHashCode(Grid obj)
        {
            return 0;
        }
    }
}
