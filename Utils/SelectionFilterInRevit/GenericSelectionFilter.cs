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
            if (elem.Category == null) return false;
            return elem.Category.ToBuiltinCategory() == _category;
        }

        public bool AllowReference(Reference reference, XYZ position)
        {
            return false;
        }
    }
}
