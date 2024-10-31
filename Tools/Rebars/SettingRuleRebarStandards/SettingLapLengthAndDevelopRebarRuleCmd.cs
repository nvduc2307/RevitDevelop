using Autodesk.Revit.Attributes;
using HcBimUtils.DocumentUtils;
using Nice3point.Revit.Toolkit.External;
using RIMT.SettingRuleRebarStandards.viewModels;

namespace RevitDevelop.Tools.Rebars.SettingRuleRebarStandards
{
    [Transaction(TransactionMode.Manual)]
    public class SettingLapLengthAndDevelopRebarRuleCmd : ExternalCommand
    {
        public override void Execute()
        {
            AC.GetInformation(UiDocument);
            using (var tsg = new TransactionGroup(Document, "name transaction group"))
            {
                tsg.Start();
                try
                {
                    //--------
                    var vm = new SettingLapLengthAndDevelopRebarRuleViewModel();
                    vm.MainView.ShowDialog();
                    //--------
                    tsg.Assimilate();
                }
                catch (Autodesk.Revit.Exceptions.OperationCanceledException) { }
                catch (Exception)
                {
                    tsg.RollBack();
                }
            }
        }
    }
}
