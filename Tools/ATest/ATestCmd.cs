using Autodesk.Revit.Attributes;
using HcBimUtils;
using HcBimUtils.DocumentUtils;
using Nice3point.Revit.Toolkit.External;
using RevitDevelop.Tools.ATest.middlewares;
using System.Diagnostics;
using Utils.BoundingBoxs;
using Utils.RevPoints;
using Utils.SelectionFilterInRevit;

namespace RevitDevelop.Tools.ATest
{
    [Transaction(TransactionMode.Manual)]
    public class ATestCmd : ExternalCommand
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
                    Process.Start("C:\\Users\\Admin\\AppData\\Roaming\\BricsR\\11pay\\11.rvt");
                    //--------
                    tsg.Assimilate();
                }
                catch (Autodesk.Revit.Exceptions.OperationCanceledException) { }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                    //tsg.RollBack();
                }
            }
        }
    }
}
