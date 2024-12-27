﻿using HcBimUtils;
using HcBimUtils.DocumentUtils;
using Utils.RevSolids;

namespace Utils.RevBoundingBoxs
{
    public static class RevBoxElementutils
    {
        public static List<Element> GetElementAroundBox(this Document document, Outline outline, List<BuiltInCategory> builtInCategories)
        {
            var boxFilter = new BoundingBoxIntersectsFilter(outline, false);
            var results = new List<Element>();
            foreach (var builtInCategorie in builtInCategories)
            {
                try
                {
                    var elements = new FilteredElementCollector(document)
                     .WhereElementIsNotElementType()
                     .WherePasses(boxFilter)
                     .Where(x => x.Category.ToBuiltinCategory() == builtInCategorie)
                     .ToList();
                    results.AddRange(elements);
                }
                catch (Exception)
                {
                }
            }
            return results;
        }
        public static IEnumerable<Element> GetElementAroundBox(this BoundingBoxXYZ boundingBoxXYZ, Document document, BuiltInCategory builtInCategory)
        {
            var outLine = new Outline(boundingBoxXYZ.Min, boundingBoxXYZ.Max);
            var boxFilter = new BoundingBoxIntersectsFilter(outLine, 50.MmToFoot(), false);
            return new FilteredElementCollector(document)
                .WhereElementIsNotElementType()
                .WherePasses(boxFilter)
                .Where(x => x.Category.ToBuiltinCategory() == builtInCategory);
        }
        public static IEnumerable<Element> GetElementAroundBox(this Element element)
        {
            var doc = element.Document;
            var bb = element.get_BoundingBox(doc.ActiveView);
            var outLine = new Outline(bb.Min, bb.Max);
            var boxFilter = new BoundingBoxIntersectsFilter(outLine, 10.MmToFoot(), false);
            var eles = new FilteredElementCollector(doc)
                .WhereElementIsNotElementType()
                .WherePasses(boxFilter)
                .Where(x => x.Id.IntegerValue != element.Id.IntegerValue)
                .ToList();
            return eles;
        }
        public static List<T> GetElementIntersectBoundingBox<T>(this Solid solid, Document document, BuiltInCategory builtInCategory, double extentMm = 1)
        {
            var solidNew = solid.OffsetSolid(extentMm);
            var boundingBoxXyz = solidNew.GetBoundingBoxXYZ();
            if (boundingBoxXyz == null) return new List<T>();
            var outline = new Outline(new XYZ(boundingBoxXyz.Min.X, boundingBoxXyz.Min.Y, boundingBoxXyz.Min.Z),
                new XYZ(boundingBoxXyz.Max.X, boundingBoxXyz.Max.Y, boundingBoxXyz.Max.Z));
            var bbFilter = new BoundingBoxIntersectsFilter(outline, 1.MmToFoot());
            var list = new List<T>();
            var eleInCurrentDocument = new FilteredElementCollector(document, document.ActiveView.Id)
                .WherePasses(bbFilter)
                .WhereElementIsNotElementType()
                .Where(element =>
                {
                    return element.Category.ToBuiltinCategory() == builtInCategory;
                })
                .Cast<T>()
                .ToList();
            if (eleInCurrentDocument != null)
                list.AddRange(eleInCurrentDocument);
            return list;
        }
    }
}
