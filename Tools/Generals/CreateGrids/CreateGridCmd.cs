using Autodesk.Revit.Attributes;
using HcBimUtils.DocumentUtils;
using Nice3point.Revit.Toolkit.External;
using RevitDevelop.Tools.Generals.CreateGrids.modelViews;

namespace RevitDevelop.Tools.Generals.CreateGrids
{
    [Transaction(TransactionMode.Manual)]
    public class CreateGridCmd : ExternalCommand
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
                    var vm = new CreateGridsModelViews();
                    vm.MainView.ShowDialog();
                    //--------
                    if (vm.IsComplete) tsg.Assimilate();
                    else tsg.RollBack();
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
