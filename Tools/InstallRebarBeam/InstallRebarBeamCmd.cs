using Autodesk.Revit.Attributes;
using HcBimUtils.DocumentUtils;
using Nice3point.Revit.Toolkit.External;
using RIMT.InstallRebarBeam.viewModel;
using Utils.Messages;

namespace RIMT.InstallRebarBeam
{
    [Transaction(TransactionMode.Manual)]
    public class InstallRebarBeamCmd : ExternalCommand
    {
        private InstallRebarBeamViewModel _viewModel;
        public override void Execute()
        {
            AC.GetInformation(UiDocument);
            using (var tsg = new TransactionGroup(AC.Document, "name transaction"))
            {
                tsg.Start();
                //--------
                try
                {
                    _viewModel = new InstallRebarBeamViewModel();
                    _viewModel.ShowUI();
                }
                catch (Autodesk.Revit.Exceptions.OperationCanceledException) { }
                catch (Exception ex)
                {
                    IO.ShowException(ex);
                    tsg.RollBack();
                }
            }
        }
    }

}
