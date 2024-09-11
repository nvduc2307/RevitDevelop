using Nice3point.Revit.Toolkit.External;
using RevitDevelop.Commands;

namespace RevitDevelop
{
    /// <summary>
    ///     Application entry point
    /// </summary>
    [UsedImplicitly]
    public class Application : ExternalApplication
    {
        public override void OnStartup()
        {
            CreateRibbon();
        }

        private void CreateRibbon()
        {
            var panel = Application.CreatePanel("Commands", "RevitDevelop");

            panel.AddPushButton<StartupCommand>("Execute")
                .SetImage("/RevitDevelop;component/Resources/Icons/RibbonIcon16.png")
                .SetLargeImage("/RevitDevelop;component/Resources/Icons/RibbonIcon32.png");
        }
    }
}