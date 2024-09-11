using Autodesk.Revit.Attributes;
using Autodesk.Revit.UI;
using Utils.Messages;

namespace RevitDev.Commands
{
    [Transaction(TransactionMode.Manual)]
    public class Command1 : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            var uIDocument = commandData.Application.ActiveUIDocument;
            var document = commandData.Application.ActiveUIDocument.Document;
            using (var tsg = new TransactionGroup(document, "name transaction group"))
            {
                tsg.Start();
                try
                {
                    //--------
                    IO.ShowInfo("hello");
                    //--------
                    tsg.Assimilate();
                    return Result.Succeeded;
                }
                catch (Autodesk.Revit.Exceptions.OperationCanceledException) { return Result.Failed; }
                catch (Exception ex)
                {
                    IO.ShowException(ex);
                    tsg.RollBack();
                    return Result.Failed;
                }
            }
        }
    }
}
