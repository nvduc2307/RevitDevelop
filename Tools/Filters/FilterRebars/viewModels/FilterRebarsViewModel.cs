using HcBimUtils.DocumentUtils;
using Newtonsoft.Json;
using RIMT.Filters.FilterRebars.iservices;
using RIMT.Filters.FilterRebars.model;
using RIMT.Filters.FilterRebars.views;
using System.IO;
using Utils.Messages;

namespace RIMT.Filters.FilterRebars.viewModels
{
    public partial class FilterRebarsViewModel : ObservableObject
    {
        private IFilterRebarPropertyTypeSevice _filterRebarPropertyTypeSevice;
        public ElementInstances ElementInstances { get; set; }
        public FilterRebarsView MainView { get; set; }
        public bool IsComplete { get; set; }
        public FilterRebarsViewModel(IFilterRebarPropertyTypeSevice filterRebarPropertyTypeSevice)
        {
            _filterRebarPropertyTypeSevice = filterRebarPropertyTypeSevice;
            ElementInstances = new ElementInstances();
            MainView = new FilterRebarsView() { DataContext = this };
        }

        [RelayCommand]
        private void AddProp()
        {
            try
            {
                List<FilterRebarProperty> propApplies = [.. ElementInstances.FilterRebarPropertyApplies];
                var propTarget = ElementInstances.FilterRebarPropertySelected;
                var isExist = propApplies.Any(x => x.Name == propTarget.Name);
                if (isExist) throw new Exception("Prop is existed");
                propApplies.Add(propTarget);
                ElementInstances.FilterRebarPropertyApplies = [.. propApplies];
                ElementInstances.FilterRebarPropertyApplySelected = ElementInstances.FilterRebarPropertyApplies.FirstOrDefault(x => x.Name == propTarget.Name);
            }
            catch (Exception ex)
            {
                IO.ShowWarning(ex.Message);
            }
        }
        [RelayCommand]
        private void RemoveProp()
        {
            try
            {
                List<FilterRebarProperty> propApplies = [.. ElementInstances.FilterRebarPropertyApplies];
                var propTarget = ElementInstances.FilterRebarPropertyApplySelected;
                var isExist = propApplies.Any(x => x.Name == propTarget.Name);
                if (!isExist) throw new Exception("Prop is not existed");
                propApplies.Remove(propApplies.FirstOrDefault(x => x.Name == propTarget.Name));
                ElementInstances.FilterRebarPropertyApplies = [.. propApplies];
                ElementInstances.FilterRebarPropertyApplySelected = ElementInstances.FilterRebarPropertyApplies.FirstOrDefault();
            }
            catch (Exception ex)
            {
                IO.ShowWarning(ex.Message);
            }
        }

        [RelayCommand]
        private void OK()
        {
            var parameterFilterElementsActive = AC.Document.ActiveView
                .GetFilters()
                .Select(x => x.ToElement() as ParameterFilterElement)
                .Where(x => x != null)
                .ToList();
            using (var ts = new Transaction(AC.Document, "name transaction"))
            {
                ts.Start();
                //--------
                foreach (var paraFilter in ElementInstances.ParameterFilterElements)
                {
                    try
                    {
                        var nameFilter = paraFilter.Name.Split('_').LastOrDefault();
                        var isVisibility = string.IsNullOrEmpty(nameFilter) ? false : ElementInstances.FilterRebarPropertyApplies.Any(x => nameFilter == x.Name);
                        if (ElementInstances.FilterRebarPropertyApplies.Count == 0) isVisibility = true;
                        if (parameterFilterElementsActive.Any(x => x.Name == paraFilter.Name))
                        {
                            var parameterFilterElementActive = parameterFilterElementsActive.FirstOrDefault(x => x.Name == paraFilter.Name);
                            if (parameterFilterElementActive != null)
                                AC.Document.ActiveView.SetFilterVisibility(parameterFilterElementActive.Id, isVisibility);
                        }
                        else
                        {
                            AC.Document.ActiveView.AddFilter(paraFilter.Id);
                            AC.Document.ActiveView.SetFilterVisibility(paraFilter.Id, isVisibility);
                        }
                    }
                    catch (Exception)
                    {
                    }
                }
                //--------
                ts.Commit();
            }
            IsComplete = true;
            MainView.Close();
        }
        [RelayCommand]
        private void Cancel()
        {
            MainView.Close();
        }
        [RelayCommand]
        private void SaveAs()
        {
            try
            {
                _filterRebarPropertyTypeSevice.SaveAs(ElementInstances.FilterRebarPropertyTypeName, ElementInstances.PathFilterRebarPropertyType);
                ElementInstances.FilterRebarPropertyTypes = JsonConvert.DeserializeObject<List<FilterRebarPropertyType>>(File.ReadAllText(ElementInstances.PathFilterRebarPropertyType));
                ElementInstances.FilterRebarPropertyTypeSeleceted = ElementInstances.FilterRebarPropertyTypes.LastOrDefault();
                ElementInstances.FilterRebarPropertyTypeName = "";
            }
            catch (Exception)
            {
            }
        }
        [RelayCommand]
        private void Save()
        {
            try
            {
                var currentName = ElementInstances.FilterRebarPropertyTypeSeleceted.Name;
                var props = ElementInstances.FilterRebarPropertyApplies.Select(x => x.Name).ToList();
                _filterRebarPropertyTypeSevice.Save(
                    currentName,
                    props,
                    ElementInstances.PathFilterRebarPropertyType);
                ElementInstances.FilterRebarPropertyTypes = JsonConvert.DeserializeObject<List<FilterRebarPropertyType>>(File.ReadAllText(ElementInstances.PathFilterRebarPropertyType));
                ElementInstances.FilterRebarPropertyTypeSeleceted = ElementInstances.FilterRebarPropertyTypes.FirstOrDefault(x => x.Name == currentName);
            }
            catch (Exception)
            {
            }
        }
        [RelayCommand]
        private void Apply()
        {
            try
            {
                var currentName = ElementInstances.FilterRebarPropertyTypeSeleceted.Name;
                _filterRebarPropertyTypeSevice.Apply(currentName, ElementInstances.PathFilterRebarPropertyType, ElementInstances.FilterRebarProperties, out List<FilterRebarProperty> filterRebarPropertySelecteds);
                if (filterRebarPropertySelecteds.Count > 0)
                {
                    ElementInstances.FilterRebarPropertyApplies = [.. filterRebarPropertySelecteds];
                    ElementInstances.FilterRebarPropertyApplySelected = ElementInstances.FilterRebarPropertyApplies.FirstOrDefault();
                }
            }
            catch (Exception)
            {
            }
        }
        [RelayCommand]
        private void Delete()
        {
            try
            {
                _filterRebarPropertyTypeSevice.Delete(ElementInstances.FilterRebarPropertyTypeSeleceted.Name, ElementInstances.PathFilterRebarPropertyType);
                ElementInstances.FilterRebarPropertyTypes = JsonConvert.DeserializeObject<List<FilterRebarPropertyType>>(File.ReadAllText(ElementInstances.PathFilterRebarPropertyType));
                ElementInstances.FilterRebarPropertyTypeSeleceted = ElementInstances.FilterRebarPropertyTypes.FirstOrDefault();
            }
            catch (Exception)
            {
            }
        }
    }
}
