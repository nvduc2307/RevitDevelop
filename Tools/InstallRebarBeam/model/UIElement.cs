using Utils.canvass;
using wd = System.Windows;

namespace Tools.InstallRebarBeam.model
{
    public class UIElement
    {
        public const string CANVAS_TAG_NAME_BEAM_SECTION = "CanvasSection";
        public CanvasPageBase CanvasBaseSection { get; set; }
        public List<wd.UIElement> RebarStirrupMains { get; set; }
        public List<wd.UIElement> RebarMainTopClass1 { get; set; }
        public List<wd.UIElement> RebarMainTopClass2 { get; set; }
        public List<wd.UIElement> RebarMainTopClass3 { get; set; }
        public List<wd.UIElement> RebarMainBotClass1 { get; set; }
        public List<wd.UIElement> RebarMainBotClass2 { get; set; }
        public List<wd.UIElement> RebarMainBotClass3 { get; set; }
        public List<wd.UIElement> RebarSideBar { get; set; }
        public UIElement()
        {
            RebarStirrupMains = new List<wd.UIElement>();

            RebarMainTopClass1 = new List<wd.UIElement>();
            RebarMainTopClass2 = new List<wd.UIElement>();
            RebarMainTopClass3 = new List<wd.UIElement>();

            RebarMainBotClass1 = new List<wd.UIElement>();
            RebarMainBotClass2 = new List<wd.UIElement>();
            RebarMainBotClass3 = new List<wd.UIElement>();

            RebarSideBar = new List<wd.UIElement>();
        }
    }
}
