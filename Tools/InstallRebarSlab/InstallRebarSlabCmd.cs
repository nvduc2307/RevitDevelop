using Autodesk.Revit.Attributes;
using HcBimUtils.DocumentUtils;
using Nice3point.Revit.Toolkit.External;
using RevitDevelop.InstallRebarSlab.models;
using RevitDevelop.InstallRebarSlab.viewModels;
using Utils.SelectionFilterInRevit;

namespace RevitDevelop.InstallRebarSlab
{
    [Transaction(TransactionMode.Manual)]
    public class InstallRebarSlabCmd : ExternalCommand
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
                    var mSLabElementIntance = new MSLabElementIntance();
                    var floors = UiDocument.Selection
                        .PickObjects(Autodesk.Revit.UI.Selection.ObjectType.Element, new GenericSelectionFilter(BuiltInCategory.OST_Floors))
                        .Select(x => Document.GetElement(x) as Floor)
                        .Select(x => new MSlab(x, mSLabElementIntance))
                        .ToList();
                    var viewModel = new InstallRebarSlabViewModel(mSLabElementIntance, floors);
                    viewModel.MainView.ShowDialog();
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
