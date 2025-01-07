using Autodesk.Revit.Attributes;
using Nice3point.Revit.Toolkit.External;
using RevitDevelop.Utils.RevUtils;
using System.Windows.Controls;
using System.Windows.Media;
using UIFramework;

namespace RevitDevelop.Commands
{
    /// <summary>
    ///     External command entry point invoked from the Revit interface
    /// </summary>
    [UsedImplicitly]
    [Transaction(TransactionMode.Manual)]
    public class StartupCommand : ExternalCommand
    {
        public override void Execute()
        {
            try
            {
                var mainWindow = MainWindow.getMainWnd();
                var dcm = mainWindow.FindChildrenByType<StackPanel>();
                var dm = dcm.FirstOrDefault();
                dm.Background = Brushes.AliceBlue;
            }
            catch (Exception)
            {
            }
        }
    }
}