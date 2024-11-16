using HcBimUtils;
using Utils.BoundingBoxs;

namespace RevitDevelop.Tools.ATest.middlewares
{
    public static class ATestMiddleware
    {
        public static bool IsAnyBoxVectorParaWithVTZ(RevBoxElement revBoxElement)
        {
            if (revBoxElement.VTX.IsParallel(XYZ.BasisZ)) return true;
            if (revBoxElement.VTY.IsParallel(XYZ.BasisZ)) return true;
            if (revBoxElement.VTZ.IsParallel(XYZ.BasisZ)) return true;
            return false;
        }
    }
}
