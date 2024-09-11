using Autodesk.Revit.DB.Structure;

namespace Utils.RebarInRevits.Models
{
    public static class RebarHostCategoryInfo
    {
        public static int ToIntergerValue(this RebarHostCategory rebarHostCategory)
        {
            var result = 0;
            switch (rebarHostCategory)
            {
                case RebarHostCategory.Other:
                    result = 0;
                    break;
                case RebarHostCategory.Part:
                    result = 1;
                    break;
                case RebarHostCategory.StructuralColumn:
                    result = 2;
                    break;
                case RebarHostCategory.StructuralFraming:
                    result = 3;
                    break;
                case RebarHostCategory.Wall:
                    result = 4;
                    break;
                case RebarHostCategory.Floor:
                    result = 5;
                    break;
                case RebarHostCategory.StructuralFoundation:
                    result = 6;
                    break;
                case RebarHostCategory.Stairs:
                    result = 7;
                    break;
                case RebarHostCategory.SlabEdge:
                    result = 8;
                    break;
            }
            return result;
        }
    }
}
