using HcBimUtils;

namespace Utils.RevSolids
{
    public static class SolidUtils
    {
        public static DirectShape CreateDirectShape(this Solid solid, Document document, BuiltInCategory builtInCategory = BuiltInCategory.OST_GenericModel)
        {
            DirectShape result = null;
            try
            {
                result = DirectShape.CreateElement(document, new ElementId(builtInCategory));
                result.SetShape([solid]);
            }
            catch (Exception)
            {
            }
            return result;
        }
        public static Solid CreateSolid(this List<XYZ> polygons, XYZ normal, double thicknessMm)
        {
            Solid result = null;
            var polygonsCount = polygons.Count;
            if (polygonsCount > 2)
            {
                //create list curveloop
                var curveLoop = new CurveLoop();
                for (int i = 0; i < polygonsCount; i++)
                {
                    var j = i == 0 ? polygonsCount - 1 : i - 1;
                    curveLoop.Append(Line.CreateBound(polygons[j], polygons[i]));
                }
                //create solid
                result = GeometryCreationUtilities.CreateExtrusionGeometry(new List<CurveLoop>() { curveLoop }, normal, thicknessMm.MmToFoot());
            }
            return result;
        }
        public static Solid CreateSolidVertical(this List<XYZ> polygons, double heightMm)
        {
            Solid result = null;
            var polygonsCount = polygons.Count;
            if (polygonsCount > 2)
            {
                //create list curveloop
                var curveLoop = new CurveLoop();
                for (int i = 0; i < polygonsCount; i++)
                {
                    if (i != polygonsCount - 1)
                    {
                        var p1 = new XYZ(polygons[i].X, polygons[i].Y, polygons[0].Z);
                        var p2 = new XYZ(polygons[i + 1].X, polygons[i + 1].Y, polygons[0].Z);
                        curveLoop.Append(Line.CreateBound(p1, p2));
                    }
                    else
                    {
                        var p1 = new XYZ(polygons[i].X, polygons[i].Y, polygons[0].Z);
                        var p2 = new XYZ(polygons[0].X, polygons[0].Y, polygons[0].Z);
                        curveLoop.Append(Line.CreateBound(p1, p2));
                    }
                }
                //create solid
                result = GeometryCreationUtilities.CreateExtrusionGeometry(new List<CurveLoop>() { curveLoop }, XYZ.BasisZ, heightMm.MmToFoot());
            }
            return result;
        }
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
    }
}
