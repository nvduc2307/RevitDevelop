using Utils.Geometries;

namespace Utils.CompareElement
{
    public class XYZCompare
    {
    }
    public class XYZComparePosition : IEqualityComparer<XYZ>
    {
        public XYZ VtBase { get; }
        public XYZComparePosition(XYZ vtBase)
        {
            VtBase = vtBase;
        }
        public bool Equals(XYZ x, XYZ y)
        {
            return x.DotProduct(VtBase).IsAlmostEqual(y.DotProduct(VtBase));
        }

        public int GetHashCode(XYZ obj)
        {
            return 0;
        }
    }
    public class ComparePoint : IEqualityComparer<XYZ>
    {
        public bool Equals(XYZ x, XYZ y)
        {
            return x.IsSeem(y);
        }

        public int GetHashCode(XYZ obj)
        {
            return 0;
        }
    }
}
