using Autodesk.Revit.UI;
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
        public RibbonPanel PANEL_GENERAL { get; private set; }
        public RibbonPanel PANEL_CONCRETE { get; private set; }
        public RibbonPanel PANEL_STEEL { get; private set; }
        public RibbonPanel PANEL_REBAR { get; private set; }
        public RibbonPanel PANEL_SCHEDULE { get; private set; }
        public override void OnStartup()
        {
            InitPanel();
            CreateRibbon();
        }
        private void InitPanel()
        {
            PANEL_GENERAL = Application.CreatePanel(Properties.Langs.Appplication.PANEL_GENERAL_NAME, Properties.Langs.Appplication.TAB_NAME);
            PANEL_CONCRETE = Application.CreatePanel(Properties.Langs.Appplication.PANEL_CONCRETE_NAME, Properties.Langs.Appplication.TAB_NAME);
            PANEL_STEEL = Application.CreatePanel(Properties.Langs.Appplication.PANEL_STEEL_NAME, Properties.Langs.Appplication.TAB_NAME);
            PANEL_REBAR = Application.CreatePanel(Properties.Langs.Appplication.PANEL_REBAR_NAME, Properties.Langs.Appplication.TAB_NAME);
            PANEL_SCHEDULE = Application.CreatePanel(Properties.Langs.Appplication.PANEL_SCHEDULE_NAME, Properties.Langs.Appplication.TAB_NAME);
        }

        private void CreateRibbon()
        {

            PANEL_GENERAL.AddPushButton<StartupCommand>("Execute")
                .SetImage("/RevitDevelop;component/Resources/Icons/RibbonIcon16.png")
                .SetLargeImage("/RevitDevelop;component/Resources/Icons/RibbonIcon32.png");
        }
    }
}