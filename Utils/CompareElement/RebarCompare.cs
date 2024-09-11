using Autodesk.Revit.DB.Structure;
using HcBimUtils.RebarUtils;

namespace Utils.CompareElement
{
    public class RebarCompare
    {
    }
    public class RebarCompareDiameter : IEqualityComparer<Rebar>
    {
        public bool Equals(Rebar x, Rebar y)
        {
            return x.BarDiameter().IsAlmostEqual(y.BarDiameter());
        }

        public int GetHashCode(Rebar obj)
        {
            return 0;
        }
    }
}
