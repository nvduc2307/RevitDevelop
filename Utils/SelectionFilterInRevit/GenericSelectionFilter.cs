using Autodesk.Revit.UI.Selection;
using HcBimUtils.DocumentUtils;

namespace Utils.SelectionFilterInRevit
{
    public class GenericSelectionFilter : ISelectionFilter
    {
        private BuiltInCategory _category;
        public GenericSelectionFilter(BuiltInCategory category)
        {
            _category = category;
        }
        public bool AllowElement(Element elem)
        {
            return elem.Category == null ? false : elem.Category.ToBuiltinCategory() == _category;
        }

        public bool AllowReference(Reference reference, XYZ position)
        {
            return false;
        }
    }
    public class GenericSelectionFilterFromCategory : ISelectionFilter
    {
        private BuiltInCategory _category;
        public GenericSelectionFilterFromCategory(BuiltInCategory category)
        {
            _category = category;
        }
        public bool AllowElement(Element elem)
        {
            if (elem.Category == null) return false;
            if (elem is AssemblyInstance ass)
            {
                var els = ass.GetMemberIds().Select(x => x.ToElement());
                return !els.Any(x => x.Category.ToBuiltinCategory() != _category);
            }
            else
            {
                return elem.Category.ToBuiltinCategory() == _category;
            }
        }

        public bool AllowReference(Reference reference, XYZ position)
        {
            return false;
        }
    }
}
