using System.Windows;

namespace Utils.canvass
{
    public abstract class InstanceInCanvas
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public CanvasPageBase CanvasPageBase { get; }
        public UIElement UIElement { get; set; }
        public OptionStyleInstanceInCanvas Options { get; set; }
        public Action ClickRightMouse { get; set; }
        public Action ClickLeftMouse { get; set; }
        public InstanceInCanvas(CanvasPageBase canvasPageBase, OptionStyleInstanceInCanvas options)
        {
            CanvasPageBase = canvasPageBase;
            Options = options;
        }

        public void DrawInCanvas()
        {
            if (CanvasPageBase != null)
                CanvasPageBase.Parent.Children.Add(UIElement);
        }
    }
}
