using HcBimUtils.DocumentUtils;
using Utils.FilterElements;

namespace Utils.ParameterFilterElements
{
    public static class ParameterFilterElements
    {
        public static ParameterFilterElement CreateParameterFilterElement(
            this Document document,
            string filterName,
            IEnumerable<BuiltInCategory> builtInCategories,
            FilterRule filterRule)
        {
            var elementParaFilter = new ElementParameterFilter(filterRule, false);
            var parameterFilterElements = document.GetElementsFromClass<ParameterFilterElement>(false);
            var filterExisted = parameterFilterElements.FirstOrDefault(x => x.Name == filterName);
            if (filterExisted != null)
            {
                filterExisted.ClearRules();
                filterExisted.SetElementFilter(elementParaFilter);
                return filterExisted;
            };
            var categoryIds = builtInCategories.Select(x => Category.GetCategory(document, x)).Select(x => x.Id).ToList();
            var result = ParameterFilterElement.Create(document, filterName, categoryIds, elementParaFilter);
            return result;
        }

        public static void ApplyFilter(this View viewApplyFilter, ParameterFilterElement parameterFilterElement, bool visibility)
        {
            AC.Document.ActiveView.AddFilter(parameterFilterElement.Id);
            AC.Document.ActiveView.SetFilterVisibility(viewApplyFilter.Id, visibility);
        }
    }
}
