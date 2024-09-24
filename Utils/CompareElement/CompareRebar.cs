using Autodesk.Revit.DB.Structure;
using HcBimUtils;
using HcBimUtils.RebarUtils;

namespace Utils.CompareElement
{
    public class CompareRebar : IEqualityComparer<Rebar>
    {
        private BuiltInParameter _builtInParameter = BuiltInParameter.INVALID;
        private string _builtInParameterName;
        public CompareRebar(BuiltInParameter builtInParameter)
        {
            _builtInParameter = builtInParameter;
        }
        public CompareRebar(string builtInParameterName)
        {
            _builtInParameterName = builtInParameterName;
        }
        public bool Equals(Rebar x, Rebar y)
        {
            try
            {
                var paraX = _builtInParameter != BuiltInParameter.INVALID
                    ? x.get_Parameter(_builtInParameter)
                    : x.LookupParameter(_builtInParameterName);
                var paraY = _builtInParameter != BuiltInParameter.INVALID
                    ? y.get_Parameter(_builtInParameter)
                    : y.LookupParameter(_builtInParameterName);

                if (paraX == null && paraY == null) return true;
                if (paraX == null && paraY != null) return false;
                if (paraX != null && paraY == null) return false;

                var paraType = paraX.StorageType;
                var result = false;
                switch (paraType)
                {
                    case StorageType.None:
                        result = true;
                        break;
                    case StorageType.Integer:
                        result = paraX.AsInteger().Equals(paraY.AsInteger());
                        break;
                    case StorageType.Double:
                        result = paraX.AsDouble().IsAlmostEqual(paraY.AsDouble(), 5.MmToFoot());
                        break;
                    case StorageType.String:
                        result = paraX.AsValueString().IsEqual(paraY.AsValueString());
                        break;
                    case StorageType.ElementId:
                        result = paraX.AsElementId().ToString().IsEqual(paraY.AsElementId().ToString());
                        break;
                }
                return result;
            }
            catch (System.Exception)
            {
                return true;
            }
        }

        public int GetHashCode(Rebar obj)
        {
            return 0;
        }
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
