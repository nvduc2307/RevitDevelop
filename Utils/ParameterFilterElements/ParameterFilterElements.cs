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
            var categoryIds = builtInCategories.Select(x => Category.GetCategory(document, x)).Select(x => x.Id).ToList();
            var elementParaFilter = new ElementParameterFilter(filterRule, false);
            ParameterFilterElement result = ParameterFilterElement.Create(document, filterName, categoryIds, elementParaFilter);
            return result;
        }
    }
}
