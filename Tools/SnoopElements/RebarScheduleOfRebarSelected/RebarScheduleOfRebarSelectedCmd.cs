using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB.Structure;
using HcBimUtils.DocumentUtils;
using Nice3point.Revit.Toolkit.External;
using RevitDevelop.Tools.SnoopElements.RebarScheduleOfRebarSelected.viewModels;
using Utils.SelectionFilterInRevit;

namespace RevitDevelop.Tools.SnoopElements.RebarScheduleOfRebarSelected
{
    [Transaction(TransactionMode.Manual)]
    public class RebarScheduleOfRebarSelectedCmd : ExternalCommand
    {
        public override void Execute()
        {
            AC.GetInformation(UiDocument);
            using (var tsg = new TransactionGroup(Document, "Rebar Schedule Of Rebar Selected"))
            {
                tsg.Start();
                try
                {
                    //--------
                    var rebars = AC.UiDoc.Selection
                    .PickObjects(Autodesk.Revit.UI.Selection.ObjectType.Element, new GenericSelectionFilter(BuiltInCategory.OST_Rebar))
                    .Select(x => AC.Document.GetElement(x) as Rebar)
                    .Where(x => x is not null)
                    .ToList();
                    var viewModel = new RebarScheduleOfRebarSelectedViewModel(rebars);
                    viewModel.MainView.ShowDialog();
                    //--------
                    tsg.RollBack();
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
