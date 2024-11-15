using Autodesk.Revit.Attributes;
using HcBimUtils.DocumentUtils;
using Nice3point.Revit.Toolkit.External;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace RevitDevelop.Tools.ATest
{
    [Transaction(TransactionMode.Manual)]
    public class ATestCmd : ExternalCommand
    {
        [DllImport("user32.dll")]
        private static extern bool ShowWindow(IntPtr hwnd, int nCmdShow);
        public override void Execute()
        {
            AC.GetInformation(UiDocument);
            using (var tsg = new TransactionGroup(Document, "name transaction group"))
            {
                tsg.Start();
                try
                {
                    //--------
                    Process[] processlist = Process.GetProcesses();
                    foreach (Process process in processlist)
                    {
                        if (!String.IsNullOrEmpty(process.MainWindowTitle))
                        {
                            //Debug.WriteLine(process.MainWindowTitle);
                            if (process.MainWindowTitle.Contains("Notepad"))
                            {
                                ShowWindow(process.MainWindowHandle, 3);
                            }
                        }
                    }
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
