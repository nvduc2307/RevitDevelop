﻿using HcBimUtils;
using HcBimUtils.DocumentUtils;

namespace Utils.RevBoundingBoxs
{
    public static class RevBoxElementutils
    {
        public static IEnumerable<Element> GetElementAroundBox(this BoundingBoxXYZ boundingBoxXYZ, Document document, BuiltInCategory builtInCategory)
        {
            var outLine = new Outline(boundingBoxXYZ.Min, boundingBoxXYZ.Max);
            var boxFilter = new BoundingBoxIntersectsFilter(outLine, 10.MmToFoot(), false);
            return new FilteredElementCollector(document)
                .WhereElementIsNotElementType()
                .WherePasses(boxFilter)
                .Where(x => x.Category.ToBuiltinCategory() == builtInCategory);
        }
    }
}
