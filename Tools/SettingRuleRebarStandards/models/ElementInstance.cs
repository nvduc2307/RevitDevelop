using Autodesk.Revit.DB.Structure;
using HcBimUtils.DocumentUtils;
using HcBimUtils.MoreLinq;
using Newtonsoft.Json;
using System.IO;
using Utils.Directionaries;
using Utils.FilterElements;
using Utils.RebarInRevits.Models;

namespace RevitDevelop.SettingRuleRebarStandards.models
{
    public class ElementInstance : ObservableObject
    {
        public const string SCHEMAL_INFO_RULE_DEVELOP_LAP = "66affeb2-4ecb-471d-81f3-63e67abfe33b";
        public const string REBAR_RULE_DEVELOP_LAP = "RebarRuleDevelopLap";

        public static string PATH_DATA = $"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}\\RIMT\\data\\SettingRuleRebarStandardsData.json";
        private string _nameGradeSaveAs;
        private SettingRuleRebarGrade _settingRuleRebarGradesSelected;
        private List<SettingRuleRebarGrade> _settingRuleRebarGrades;
        public List<RebarBarTypeCustom> RebarBarTypes { get; set; }
        public List<SettingRuleRebarGrade> SettingRuleRebarGrades
        {
            get => _settingRuleRebarGrades;
            set
            {
                _settingRuleRebarGrades = value;
                OnPropertyChanged();
            }

        }
        public string NameGradeSaveAs
        {
            get => _nameGradeSaveAs;
            set
            {
                _nameGradeSaveAs = value;
                OnPropertyChanged();
            }
        }

        public SettingRuleRebarGrade SettingRuleRebarGradesSelected
        {
            get => _settingRuleRebarGradesSelected;
            set
            {
                _settingRuleRebarGradesSelected = value;
                OnPropertyChanged();
                SettingRuleRebarGradesSelectedActionRun?.Invoke();
            }
        }

        public Action SettingRuleRebarGradesSelectedActionRun { get; set; }

        public ElementInstance()
        {
            RebarBarTypes = AC.Document.GetElementsFromClass<RebarBarType>()
                .OrderBy(x => x.Name)
                .Select(x => new RebarBarTypeCustom(x))
                .ToList();
            //Check File data is existed
            DirectionaryExt.CreateDirectory(PATH_DATA);
            var content = File.ReadAllText(PATH_DATA);
            SettingRuleRebarGrades = JsonConvert.DeserializeObject<List<SettingRuleRebarGrade>>(content);
            SettingRuleRebarGradesSelected = SettingRuleRebarGrades.FirstOrDefault();
            NameGradeSaveAs = string.Empty;
        }
    }
}
