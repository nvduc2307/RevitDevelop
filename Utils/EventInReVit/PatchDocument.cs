using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB.Events;
using Autodesk.Revit.UI;
using HcBimUtils.DocumentUtils;
using Nice3point.Revit.Toolkit.External;

namespace Utils.EventInReVit
{
    [UsedImplicitly]
    [Transaction(TransactionMode.Manual)]
    public class PatchDocument : ExternalCommand
    {
        public static Application App { get; private set; }
        public static List<ElementId> ElemIds = new List<ElementId>();
        public static event EventHandler OnPostableCommandModelLineEnded;
        public override void Execute()
        {
            ElemIds = new List<ElementId>();
            OnPostableCommandModelLineEnded = null;
            RevitCommandEndedMonitor revitCommandEndedMonitor = new RevitCommandEndedMonitor(AC.UiApplication);
            revitCommandEndedMonitor.CommandEnded += RevitCommandEndedMonitor_CommandEnded;
            App.DocumentChanged += Application_DocumentChanged;
        }

        private void Application_DocumentChanged(object sender, DocumentChangedEventArgs e)
        {
            ElemIds.AddRange(e.GetAddedElementIds());
        }

        private void RevitCommandEndedMonitor_CommandEnded(object sender, EventArgs e)
        {
            App.DocumentChanged -= Application_DocumentChanged;
            OnPostableCommandModelLineEnded?.Invoke(this, EventArgs.Empty);
        }
        public static void Start(Application app, UIApplication UiApp, RevitCommandId CommandId)
        {
            App = app;
            UiApp.PostCommand(CommandId);
            new PatchDocument().Execute();
        }
    }
}
