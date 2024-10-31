using HcBimUtils.DocumentUtils;
using Newtonsoft.Json;
using RevitDevelop.SettingRuleRebarStandards.exceptions;
using RevitDevelop.SettingRuleRebarStandards.viewModels;
using RevitDevelop.Tools.Rebars.SettingRuleRebarStandards.models;
using RevitDevelop.Tools.SettingRuleRebarStandards.views;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using Utils.Entities;
using Utils.Messages;

namespace RIMT.SettingRuleRebarStandards.viewModels
{
    public partial class SettingLapLengthAndDevelopRebarRuleViewModel : ObservableObject
    {
        public SettingLapLengthAndDevelopRebarRuleView MainView { get; set; }
        public DataGrid DataGridTag { get; set; }
        public ElementInstance ElementInstance { get; set; }
        public SettingLapLengthAndDevelopRebarRuleViewModel()
        {
            ElementInstance = new ElementInstance();
            MainView = new SettingLapLengthAndDevelopRebarRuleView() { DataContext = this };
            MainView.Loaded += MainView_Loaded;
        }

        private void MainView_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            DataGridTag = MainView.FindName("DataGridMain") as DataGrid;
            DataGridTag.SelectedIndex = 0;
            ElementInstance.SettingRuleRebarGradesSelectedActionRun = () =>
            {
                //xet lai gia tri index cho data grid khi thay doi grade thep.
                DataGridTag.SelectedIndex = 0;
            };
        }

        [RelayCommand]
        private void SaveAsRule()
        {
            try
            {
                var isGradeExisted = ElementInstance.SettingRuleRebarGrades.Any(x => x.Grade == ElementInstance.NameGradeSaveAs);
                if (isGradeExisted) throw new Exception("Element is  Existed");
                if (string.IsNullOrEmpty(ElementInstance.NameGradeSaveAs)) throw new Exception("Grade name is not empty");
                var grade = new SettingRuleRebarGrade()
                {
                    Id = ElementInstance.SettingRuleRebarGrades.Count == 0 ? 1 : ElementInstance.SettingRuleRebarGrades.Max(x => x.Id) + 1,
                    Grade = ElementInstance.NameGradeSaveAs,
                    Rules = new List<SettingLapLengthAndDevelopRebarRule>()
                };
                ElementInstance.SettingRuleRebarGrades = ElementInstance.SettingRuleRebarGrades
                    .Concat(new List<SettingRuleRebarGrade>() { grade })
                    .ToList();
                var content = JsonConvert.SerializeObject(ElementInstance.SettingRuleRebarGrades);
                File.WriteAllText(ElementInstance.PATH_DATA, content);
                ElementInstance.SettingRuleRebarGradesSelected = ElementInstance.SettingRuleRebarGrades.LastOrDefault();
            }
            catch (Exception ex)
            {
                IO.ShowWarning(ex.Message);
            }
        }
        [RelayCommand]
        private void AddRule()
        {
            try
            {
                if (ElementInstance.SettingRuleRebarGrades.Count == 0) throw new SettingRuleRebarGradesEmptyException();
                var addViewModel = new SettingLapLengthAndDevelopRebarRuleAddViewModel(this);
            }
            catch (Exception ex)
            {
                IO.ShowWarning(ex.Message);
            }
        }
        [RelayCommand]
        private void ApplyRule()
        {
            try
            {
                if (ElementInstance.SettingRuleRebarGrades.Count == 0) throw new SettingRuleRebarGradesEmptyException();
                if (ElementInstance.SettingRuleRebarGradesSelected.Rules.Count == 0) throw new SettingRuleRebarGradesEmptyException();
                var schemaRebarNumberingInfo = new SchemaInfo(
                    ElementInstance.SCHEMAL_INFO_RULE_DEVELOP_LAP,
                    ElementInstance.REBAR_RULE_DEVELOP_LAP,
                    new SchemaField());

                var rebarInfos = SchemaInfo.Read(schemaRebarNumberingInfo.SchemaBase, AC.Document.ProjectInformation, schemaRebarNumberingInfo.SchemaField.Name);

                using (var ts = new Transaction(AC.Document, "name transaction"))
                {
                    ts.Start();
                    //--------
                    schemaRebarNumberingInfo.SchemaField.Value = JsonConvert.SerializeObject(ElementInstance.SettingRuleRebarGradesSelected);
                    SchemaInfo.Write(schemaRebarNumberingInfo.SchemaBase, AC.Document.ProjectInformation, schemaRebarNumberingInfo.SchemaField);
                    //--------
                    ts.Commit();
                }
            }
            catch (Exception ex)
            {
                IO.ShowWarning(ex.Message);
            }
        }
        [RelayCommand]
        private void OkRule(object obj)
        {
            ApplyRule();
            if (obj is Window window) window.Close();
        }
        [RelayCommand]
        private void ModifyRule()
        {
            try
            {
                if (ElementInstance.SettingRuleRebarGrades.Count == 0) throw new SettingRuleRebarGradesEmptyException();
                var modifyViewModel = new SettingLapLengthAndDevelopRebarRuleModifyViewModel(this);
            }
            catch (Exception ex)
            {
                IO.ShowWarning(ex.Message);
            }
        }
        [RelayCommand]
        private void DeleteRule()
        {
            try
            {
                if (ElementInstance.SettingRuleRebarGrades.Count == 0) throw new SettingRuleRebarGradesEmptyException();
                var resultDialog = IO.ShowQuestion("Are you sure to Delete rule?");
                if (resultDialog == System.Windows.Forms.DialogResult.Yes)
                {
                    ElementInstance.SettingRuleRebarGrades.Remove(ElementInstance.SettingRuleRebarGradesSelected);
                    var content = ElementInstance.SettingRuleRebarGrades.Count == 0 ? "[]"
                        : JsonConvert.SerializeObject(ElementInstance.SettingRuleRebarGrades);
                    File.WriteAllText(ElementInstance.PATH_DATA, content);
                    ElementInstance.SettingRuleRebarGrades = ElementInstance.SettingRuleRebarGrades.ToList();
                    ElementInstance.SettingRuleRebarGradesSelected = ElementInstance.SettingRuleRebarGrades.FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                IO.ShowWarning(ex.Message);
            }
        }
    }
}
