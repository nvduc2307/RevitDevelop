using Newtonsoft.Json;
using RIMT.Filters.FilterRebars.iservices;
using RIMT.Filters.FilterRebars.model;
using System.IO;
using Utils.Messages;

namespace RIMT.Filters.FilterRebars.services
{
    public class FilterRebarPropertyTypeSevice : IFilterRebarPropertyTypeSevice
    {
        public void Apply(string namePropertyType, string pathFilterPropertyTypes, List<FilterRebarProperty> filterRebarProperties, out List<FilterRebarProperty> filterRebarPropertiesApply)
        {
            filterRebarPropertiesApply = new List<FilterRebarProperty>();
            try
            {
                var filterPropertyTypes = JsonConvert.DeserializeObject<List<FilterRebarPropertyType>>(File.ReadAllText(pathFilterPropertyTypes));
                var propTypeExist = filterPropertyTypes.FirstOrDefault(x => x.Name == namePropertyType);
                if (propTypeExist == null) throw new Exception("PropType is not exist");
                foreach (var filterPropertyType in propTypeExist.FilterRebarProperteis)
                {
                    var propApply = filterRebarProperties.FirstOrDefault(x => x.Name == filterPropertyType);
                    if (propApply != null) filterRebarPropertiesApply.Add(propApply);
                }
            }
            catch (Exception)
            {
            }
        }

        public void Delete(string namePropertyType, string pathFilterPropertyTypes)
        {
            try
            {
                var filterPropertyTypes = JsonConvert.DeserializeObject<List<FilterRebarPropertyType>>(File.ReadAllText(pathFilterPropertyTypes));
                var propTypeExist = filterPropertyTypes.FirstOrDefault(x => x.Name == namePropertyType);
                if (propTypeExist == null) throw new Exception("PropType is not exist");
                filterPropertyTypes.Remove(propTypeExist);
                var content = JsonConvert.SerializeObject(filterPropertyTypes);
                File.WriteAllText(pathFilterPropertyTypes, content);
            }
            catch (Exception ex)
            {
                IO.ShowWarning(ex.Message);
            }
        }

        public void Save(string namePropertyType, List<string> properties, string pathFilterPropertyTypes)
        {
            try
            {
                var filterPropertyTypes = JsonConvert.DeserializeObject<List<FilterRebarPropertyType>>(File.ReadAllText(pathFilterPropertyTypes));
                var propTypeExist = filterPropertyTypes.FirstOrDefault(x => x.Name == namePropertyType);
                if (propTypeExist == null) throw new Exception("PropType is not exist");
                propTypeExist.FilterRebarProperteis = [.. properties];
                var content = JsonConvert.SerializeObject(filterPropertyTypes);
                File.WriteAllText(pathFilterPropertyTypes, content);
            }
            catch (Exception ex)
            {
                IO.ShowWarning(ex.Message);
            }
        }

        public void SaveAs(string namePropertyType, string pathFilterPropertyTypes)
        {
            try
            {
                if (string.IsNullOrEmpty(namePropertyType)) throw new Exception("namePropertyType is not empty");
                var filterPropertyTypes = JsonConvert.DeserializeObject<List<FilterRebarPropertyType>>(File.ReadAllText(pathFilterPropertyTypes));
                var propTypeExist = filterPropertyTypes.FirstOrDefault(x => x.Name == namePropertyType);
                if (propTypeExist != null) throw new Exception("PropType is existed");
                var propType = new FilterRebarPropertyType()
                {
                    Id = 1,
                    Name = namePropertyType
                };
                filterPropertyTypes.Add(propType);
                var content = JsonConvert.SerializeObject(filterPropertyTypes);
                File.WriteAllText(pathFilterPropertyTypes, content);
            }
            catch (Exception ex)
            {
                IO.ShowWarning(ex.Message);
            }
        }
    }
}
