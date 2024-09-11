using HcBimUtils;
using Autodesk.Revit.DB.IFC;

namespace RevitDevelop.Utils.Floors
{
    public static class FloorUtils
    {
        private static Solid UnionSolid(Solid source, List<Solid> solids)
        {
            if (source == null) return null;
            if (solids == null || solids.Count == 0) return source;
            Solid result = source;
            Solid sl1 = null;

            foreach (var sl in solids)
            {
                if (sl == null) continue;
                try
                {
                    sl1 = BooleanOperationsUtils.ExecuteBooleanOperation(result, sl, BooleanOperationsType.Union);
                }
                catch
                {
                }
                if (sl1 != null)
                {
                    result = sl1;
                }
            }

            return result;
        }

        public static Solid CreateSolidFromCurveLoops(CurveLoop mainLoop, List<CurveLoop> loops, double height)
        {
            XYZ vertical = (height >= 0 ? 1 : -1) * XYZ.BasisZ;
            double h = Math.Abs(height);

            var mainSolid = GeometryCreationUtilities.CreateExtrusionGeometry(new List<CurveLoop>() { mainLoop }, vertical, h);
            var subSolids = loops.Select(x => GeometryCreationUtilities.CreateExtrusionGeometry(new List<CurveLoop>() { x }, vertical, h));
            Solid solid = mainSolid;
            foreach (var sl in subSolids)
            {
                solid = BooleanOperationsUtils.ExecuteBooleanOperation(solid, sl, BooleanOperationsType.Difference);
            }
            return solid;
        }

        public static List<PlanarFace> TopFaces(this Solid solid)
        {
            return (from Autodesk.Revit.DB.Face x in solid.Faces
                    where x.ComputeNormal(Autodesk.Revit.DB.UV.Zero).IsCodirectionalTo(XYZ.BasisZ)
                    orderby x.Evaluate(Autodesk.Revit.DB.UV.Zero).Z descending
                    select x).Cast<PlanarFace>().ToList();
        }

        public static List<PlanarFace> BottomFaces(this Solid solid)
        {
            return (from Autodesk.Revit.DB.Face x in solid.Faces
                    where x.ComputeNormal(Autodesk.Revit.DB.UV.Zero).IsCodirectionalTo(XYZ.BasisZ.Negate())
                    orderby x.Evaluate(Autodesk.Revit.DB.UV.Zero).Z
                    select x).Cast<PlanarFace>().ToList();
        }

        public static Solid Combine(this List<Solid> solids)
        {
            if (solids == null)
            {
                return null;
            }

            var solid = solids.FirstOrDefault();
            for (int i = 1; i < solids.Count; i++)
            {
                solid = BooleanOperationsUtils.ExecuteBooleanOperation(solid, solids[i], BooleanOperationsType.Union);
            }

            return solid;
        }

        public static double GetArea(this CurveLoop loop)
        {
            return ExporterIFCUtils.ComputeAreaOfCurveLoops(new List<CurveLoop>() { loop });
        }
    }
}
