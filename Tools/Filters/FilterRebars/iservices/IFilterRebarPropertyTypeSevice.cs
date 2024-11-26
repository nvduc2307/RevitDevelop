using RIMT.Filters.FilterRebars.model;

namespace RIMT.Filters.FilterRebars.iservices
{
    public interface IFilterRebarPropertyTypeSevice
    {
        public void SaveAs(string namePropertyType, string pathFilterPropertyTypes);
        public void Save(string namePropertyType, List<string> properties, string pathFilterPropertyTypes);
        public void Apply(string namePropertyType, string pathFilterPropertyTypes, List<FilterRebarProperty> filterRebarProperties, out List<FilterRebarProperty> filterRebarPropertiesSelected);
        public void Delete(string namePropertyType, string pathFilterPropertyTypes);
    }
}
