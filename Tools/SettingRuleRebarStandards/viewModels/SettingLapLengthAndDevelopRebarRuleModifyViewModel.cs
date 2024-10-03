using Newtonsoft.Json;
using RevitDevelop.SettingRuleRebarStandards.models;
using RevitDevelop.Tools.SettingRuleRebarStandards.views;
using System.IO;
using System.Windows;

namespace RIMT.SettingRuleRebarStandards.viewModels
{
    public partial class SettingLapLengthAndDevelopRebarRuleModifyViewModel : ObservableObject
    {
        public SettingLapLengthAndDevelopRebarRuleViewModel RuleViewModel { get; set; }
        public SettingLapLengthAndDevelopRebarRule SettingLapLengthAndDevelopRebarRule { get; set; }
        public SettingLapLengthAndDevelopRebarRuleModifyViewModel(SettingLapLengthAndDevelopRebarRuleViewModel ruleViewModel)
        {
            RuleViewModel = ruleViewModel;
            var rule = (SettingLapLengthAndDevelopRebarRule)RuleViewModel.DataGridTag.SelectedItem;
            SettingLapLengthAndDevelopRebarRule = new SettingLapLengthAndDevelopRebarRule()
            {
                Id = rule.Id,
                Grade = rule.Grade,
                L1 = rule.L1,
                L2 = rule.L2,
                L3Frame = rule.L3Frame,
                L3Slab = rule.L3Slab,
                L1h = rule.L1h,
                L2h = rule.L2h,
                L3hFrame = rule.L3hFrame,
                L3hSlab = rule.L3Slab,
                La = rule.La,
                Lb = rule.Lb,
            };
            var view = new SettingLapLengthAndDevelopRebarRuleModifyView() { DataContext = this };
            view.ShowDialog();
        }
        [RelayCommand]
        private void ModifyOK(object obj)
        {
            try
            {
                //find grade
                var rule = (SettingLapLengthAndDevelopRebarRule)RuleViewModel.DataGridTag.SelectedItem;
                rule.Id = SettingLapLengthAndDevelopRebarRule.Id;
                rule.Grade = SettingLapLengthAndDevelopRebarRule.Grade;
                rule.L1 = SettingLapLengthAndDevelopRebarRule.L1;
                rule.L2 = SettingLapLengthAndDevelopRebarRule.L2;
                rule.L3Frame = SettingLapLengthAndDevelopRebarRule.L3Frame;
                rule.L3Slab = SettingLapLengthAndDevelopRebarRule.L3Slab;
                rule.L1h = SettingLapLengthAndDevelopRebarRule.L1h;
                rule.L2h = SettingLapLengthAndDevelopRebarRule.L2h;
                rule.L3hFrame = SettingLapLengthAndDevelopRebarRule.L3hFrame;
                rule.L3hSlab = SettingLapLengthAndDevelopRebarRule.L3hSlab;
                rule.La = SettingLapLengthAndDevelopRebarRule.La;
                rule.Lb = SettingLapLengthAndDevelopRebarRule.Lb;
                var content = JsonConvert.SerializeObject(RuleViewModel.ElementInstance.SettingRuleRebarGrades);
                //write on database
                File.WriteAllText(ElementInstance.PATH_DATA, content);
                //close window
                if (obj is Window window) window.Close();
            }
            catch (Exception)
            {
            }
        }
        [RelayCommand]
        private void ModifyCancel(object obj)
        {
            if (obj is Window window) window.Close();
        }
    }
}
