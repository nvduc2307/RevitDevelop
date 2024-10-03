using HcBimUtils;
using Newtonsoft.Json;
using RevitDevelop.SettingRuleRebarStandards.models;
using RevitDevelop.Tools.SettingRuleRebarStandards.views;
using RIMT.SettingRuleRebarStandards.viewModels;
using System.IO;
using System.Windows;
using Utils.Messages;
using Utils.RebarInRevits.Models;

namespace RevitDevelop.SettingRuleRebarStandards.viewModels
{
    public partial class SettingLapLengthAndDevelopRebarRuleAddViewModel : ObservableObject
    {
        private RebarBarTypeCustom _rebarBarTypeSeleced;
        public SettingLapLengthAndDevelopRebarRuleViewModel RuleViewModel { get; set; }
        public RebarBarTypeCustom RebarBarTypeSeleced
        {
            get => _rebarBarTypeSeleced;
            set
            {
                _rebarBarTypeSeleced = value;
                OnPropertyChanged();
            }
        }
        public SettingLapLengthAndDevelopRebarRule SettingLapLengthAndDevelopRebarRule { get; set; }

        public SettingLapLengthAndDevelopRebarRuleAddViewModel(SettingLapLengthAndDevelopRebarRuleViewModel ruleViewModel)
        {
            RuleViewModel = ruleViewModel;
            RebarBarTypeSeleced = RuleViewModel.ElementInstance.RebarBarTypes.FirstOrDefault();
            SettingLapLengthAndDevelopRebarRule = new SettingLapLengthAndDevelopRebarRule()
            {
                Id = 1,
                Diameter = RebarBarTypeSeleced.BarDiameter,
                Grade = RuleViewModel.ElementInstance.SettingRuleRebarGradesSelected.Grade,
                L1 = 4,
                L2 = 4,
                L3Frame = 4,
                L3Slab = 4,
                L1h = 4,
                L2h = 4,
                L3hFrame = 4,
                L3hSlab = 4,
                La = 4,
                Lb = 4,
            };
            var addView = new SettingLapLengthAndDevelopRebarRuleAddView() { DataContext = this };
            addView.ShowDialog();
        }
        [RelayCommand]
        private void AddOK(object obj)
        {
            try
            {
                //find grade
                var gradeGr = RuleViewModel.ElementInstance.SettingRuleRebarGrades.Find(x => x.Grade == SettingLapLengthAndDevelopRebarRule.Grade);

                //resetting diameter for new element
                SettingLapLengthAndDevelopRebarRule.Diameter = Math.Round(RebarBarTypeSeleced.ModelBarDiameter.FootToMm(), 1);
                //resetting id for new element
                SettingLapLengthAndDevelopRebarRule.Id = gradeGr.Rules.Count == 0 ? 1 : gradeGr.Rules.Max(x => x.Id) + 1;
                //check element is existed
                if (gradeGr.Rules.Any(x => x.Diameter == SettingLapLengthAndDevelopRebarRule.Diameter)) throw new Exception("Rule already is existed");

                gradeGr.Rules = gradeGr.Rules.Concat(new List<SettingLapLengthAndDevelopRebarRule>() { SettingLapLengthAndDevelopRebarRule }).ToList();
                var content = JsonConvert.SerializeObject(RuleViewModel.ElementInstance.SettingRuleRebarGrades);
                //write on database
                File.WriteAllText(ElementInstance.PATH_DATA, content);
                //close window
                if (obj is Window window) window.Close();
            }
            catch (Exception ex)
            {
                IO.ShowWarning(ex.Message);
            }
        }

        [RelayCommand]
        private void AddAll(object obj)
        {
            try
            {
                //find grade
                var gradeGr = RuleViewModel.ElementInstance.SettingRuleRebarGrades.Find(x => x.Grade == SettingLapLengthAndDevelopRebarRule.Grade);

                foreach (var rebarBarType in RuleViewModel.ElementInstance.RebarBarTypes)
                {
                    //resetting diameter for new element
                    var diameter = Math.Round(rebarBarType.ModelBarDiameter.FootToMm(), 1);
                    if (gradeGr.Rules.Count == 0 || !gradeGr.Rules.Any(x => x.Diameter == diameter))
                    {
                        var ruleNew = new SettingLapLengthAndDevelopRebarRule()
                        {
                            Id = SettingLapLengthAndDevelopRebarRule.Id,
                            Name = rebarBarType.NameStyle,
                            Diameter = diameter,
                            Grade = SettingLapLengthAndDevelopRebarRule.Grade,
                            L1 = SettingLapLengthAndDevelopRebarRule.L1,
                            L2 = SettingLapLengthAndDevelopRebarRule.L2,
                            L3Frame = SettingLapLengthAndDevelopRebarRule.L3Frame,
                            L3Slab = SettingLapLengthAndDevelopRebarRule.L3Slab,
                            L1h = SettingLapLengthAndDevelopRebarRule.L1h,
                            L2h = SettingLapLengthAndDevelopRebarRule.L2h,
                            L3hFrame = SettingLapLengthAndDevelopRebarRule.L3hFrame,
                            L3hSlab = SettingLapLengthAndDevelopRebarRule.L3hSlab,
                            La = SettingLapLengthAndDevelopRebarRule.La,
                            Lb = SettingLapLengthAndDevelopRebarRule.Lb,
                        };
                        gradeGr.Rules.Add(ruleNew);
                    }
                }
                //config Id rules
                gradeGr.Rules = gradeGr.Rules.OrderBy(rule => rule.Diameter).ToList();
                var c = 1;
                foreach (var rule in gradeGr.Rules)
                {
                    rule.Id = c++;
                }
                var content = JsonConvert.SerializeObject(RuleViewModel.ElementInstance.SettingRuleRebarGrades);
                //write on database
                File.WriteAllText(ElementInstance.PATH_DATA, content);
                //close window
                if (obj is Window window) window.Close();

            }
            catch (Exception ex)
            {
                IO.ShowWarning(ex.Message);
            }
        }

        [RelayCommand]
        private void AddCancel(object obj)
        {
            if (obj is Window window) window.Close();
        }
    }
}
