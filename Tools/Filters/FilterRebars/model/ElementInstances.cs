using Autodesk.Revit.DB.Structure;
using HcBimUtils.DocumentUtils;
using Newtonsoft.Json;
using RevitDevelop.Utils.ParameterFilterElements;
using System.IO;
using Utils.Directionaries;
using Utils.FilterElements;
using Utils.ParameterFilterElements;
using Utils.PathInWindows;

namespace RIMT.Filters.FilterRebars.model
{
    public partial class ElementInstances : ObservableObject
    {
        public static string DirFilterRebarPropertyType = $"{PathInWindow.AppDataRimTFilterRebar}";
        private IList<RebarHostCategory> _rebarFilterRebarHostCategories =
        [
            RebarHostCategory.StructuralFraming,
            RebarHostCategory.StructuralFoundation,
            RebarHostCategory.StructuralColumn,
            RebarHostCategory.Floor,
            RebarHostCategory.Wall
        ];
        public IList<RebarBarType> RebarBarTypes { get; set; }
        public List<FilterRebarProperty> FilterRebarProperties { get; set; }
        [ObservableProperty]
        private FilterRebarProperty _filterRebarPropertySelected;
        [ObservableProperty]
        private List<FilterRebarProperty> _filterRebarPropertyApplies;
        [ObservableProperty]
        private FilterRebarProperty _filterRebarPropertyApplySelected;
        [ObservableProperty]
        private List<FilterRebarPropertyType> _filterRebarPropertyTypes;
        [ObservableProperty]
        private FilterRebarPropertyType _filterRebarPropertyTypeSeleceted;
        [ObservableProperty]
        private string _filterRebarPropertyTypeName;
        public string PathFilterRebarPropertyType { get; set; }
        public List<ParameterFilterElement> ParameterFilterElements { get; set; }
        public ElementInstances()
        {
            PathFilterRebarPropertyType = $"{DirFilterRebarPropertyType}\\{AC.Document.ProjectInformation.UniqueId}\\FilterRebarData.json";
            DirectionaryExt.CreateDirectory(PathFilterRebarPropertyType);
            FilterRebarPropertyTypes = JsonConvert.DeserializeObject<List<FilterRebarPropertyType>>(File.ReadAllText(PathFilterRebarPropertyType));
            FilterRebarPropertyTypeSeleceted = FilterRebarPropertyTypes.FirstOrDefault();
            FilterRebarPropertyTypeName = "";
            RebarBarTypes = AC.Document.GetElementsFromClass<RebarBarType>().OrderBy(x => x.Name).ToList();
            FilterRebarProperties = GetFilterRebarProperties();
            FilterRebarPropertySelected = FilterRebarProperties.FirstOrDefault();
            FilterRebarPropertyApplies = new List<FilterRebarProperty>();
            FilterRebarPropertyApplySelected = FilterRebarPropertyApplies.FirstOrDefault();
            ParameterFilterElements = ParameterFilterElementInit();
        }
        private List<FilterRebarProperty> GetFilterRebarProperties()
        {
            var results = new List<FilterRebarProperty>();
            try
            {
                foreach (var item in _rebarFilterRebarHostCategories)
                {
                    try
                    {
                        var eleFilter = new FilterRebarProperty()
                        {
                            Id = 1,
                            Name = item.ToString(),
                            Element = item,
                        };
                        results.Add(eleFilter);
                    }
                    catch (Exception)
                    {
                    }
                }
                foreach (var item in RebarBarTypes)
                {
                    try
                    {
                        var eleFilter = new FilterRebarProperty()
                        {
                            Id = 1,
                            Name = item.Name,
                            Element = item,
                        };
                        results.Add(eleFilter);
                    }
                    catch (Exception)
                    {
                    }
                }
            }
            catch (Exception)
            {
            }
            return results;
        }
        private List<ParameterFilterElement> ParameterFilterElementInit()
        {
            var results = new List<ParameterFilterElement>();
            using (var ts = new Transaction(AC.Document, "name transaction"))
            {
                ts.Start();
                //--------
                foreach (var item in FilterRebarProperties)
                {
                    try
                    {
                        var nameFilter = $"HC_Rebar_Filter_{item.Name}";
                        FilterRule filterRule = null;
                        if (item.Element is RebarHostCategory rebarHostCate) filterRule = FilterRuleUtils.CreateFilterRule(BuiltInParameter.REBAR_HOST_CATEGORY, FilterRuleEnum.CreateEqualsRule, (int)rebarHostCate);
                        if (item.Element is RebarBarType rebarbarType) filterRule = FilterRuleUtils.CreateFilterRule(BuiltInParameter.ALL_MODEL_TYPE_NAME, FilterRuleEnum.CreateEqualsRule, item.Name);
                        if (filterRule != null) results.Add(AC.Document.CreateParameterFilterElement(nameFilter, new List<BuiltInCategory>() { BuiltInCategory.OST_Rebar }, filterRule));
                    }
                    catch (Exception)
                    {
                    }
                }
                //--------
                ts.Commit();
            }
            return results;
        }
    }
}
