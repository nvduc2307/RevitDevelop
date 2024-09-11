using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Events;

namespace Utils.EventInReVit
{
    public class RevitCommandEndedMonitor
    {
        private readonly UIApplication _revitUiApplication;

        private bool _initializingCommandMonitor;

        public event EventHandler CommandEnded;

        public RevitCommandEndedMonitor(UIApplication revituiApplication)
        {
            _revitUiApplication = revituiApplication;

            _initializingCommandMonitor = true;

            _revitUiApplication.Idling += OnRevitUiApplicationIdling;
        }

        private void OnRevitUiApplicationIdling(object sender, IdlingEventArgs idlingEventArgs)
        {
            if (_initializingCommandMonitor)
            {
                _initializingCommandMonitor = false;
                return;
            }

            _revitUiApplication.Idling -= OnRevitUiApplicationIdling;

            OnCommandEnded();
        }

        protected virtual void OnCommandEnded()
        {
            CommandEnded?.Invoke(this, EventArgs.Empty);
        }
    }
}
