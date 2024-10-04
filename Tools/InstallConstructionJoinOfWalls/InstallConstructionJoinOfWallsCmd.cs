using Autodesk.Revit.Attributes;
using HcBimUtils.DocumentUtils;
using Nice3point.Revit.Toolkit.External;
using RevitDevelop.Tools.InstallConstructionJoinOfWalls.viewModels;

namespace RevitDevelop.Tools.InstallConstructionJoinOfWalls
{
    [Transaction(TransactionMode.Manual)]
    public class InstallConstructionJoinOfWallsCmd : ExternalCommand
    {
        public override void Execute()
        {
            AC.GetInformation(UiDocument);
            using (var tsg = new TransactionGroup(Document, "Install_Construction_Join_Of_Walls"))
            {
                tsg.Start();
                try
                {
                    //--------
                    var vm = new InstallConstructionJoinOfWallViewModel();
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
