namespace RevitDevelop.Utils.Window2Ds.Compares
{
    public class PointCompare : IEqualityComparer<System.Windows.Point>
    {
        public bool Equals(System.Windows.Point x, System.Windows.Point y)
        {
            return x.X.IsAlmostEqual(y.X) && x.Y.IsAlmostEqual(y.Y);
        }

        public int GetHashCode(System.Windows.Point obj)
        {
            return 0;
        }
    }
}
