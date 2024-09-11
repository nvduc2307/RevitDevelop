using Autodesk.Revit.DB.Structure;
using FAMILY = Autodesk.Revit.DB.Family;

namespace Utils.FamilyInRevits
{
    public static class FamilyInRevits
    {
        public static FamilyInstance PushFamily(this Document doc, FamilySymbol symbol, XYZ basePoint)
        {
            symbol.Activate();
            var result = doc.Create.NewFamilyInstance(basePoint, symbol, StructuralType.NonStructural);
            return result;
        }
        public static FamilyInstance PushFamily(this Document doc, FamilySymbol symbol, XYZ basePoint, View view)
        {
            symbol.Activate();
            var result = doc.Create.NewFamilyInstance(basePoint, symbol, view);
            return result;
        }
        public static FamilyInstance PushFamily(this Document doc, FamilySymbol symbol, XYZ basePoint, double angle, View view)
        {
            symbol.Activate();
            var result = doc.Create.NewFamilyInstance(basePoint, symbol, view);
            result.Location.Rotate(Line.CreateBound(basePoint, new XYZ(basePoint.X, basePoint.Y, basePoint.Z + 1)), angle);
            return result;
        }
        public static void LoadFamily(this Document document, string nameFamily, string dirFamily)
        {
            var fileName = $"{nameFamily}.rfa";
            var path = $"{dirFamily}{fileName}";
            FAMILY family = null;
            var a = new FilteredElementCollector(document).OfClass(typeof(FAMILY));
            int n = a.Count<Element>(e => e.Name.Contains(fileName));
            if (0 > n)
                document.LoadFamily(path, out family);
        }
    }
}
