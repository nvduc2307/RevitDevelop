using System.Windows.Shapes;

namespace Utils.canvass
{
    public class InstanceInCanvasPolyline : InstanceInCanvas
    {
        public List<System.Windows.Point> Points { get; set; }
        public InstanceInCanvasPolyline(CanvasPageBase canvasPageBase, OptionStyleInstanceInCanvas options, List<System.Windows.Point> points) : base(canvasPageBase, options)
        {
            Points = points;
            var pll = new Polyline();
            foreach (System.Windows.Point p in points)
            {
                pll.Points.Add(p);
            }
            pll.StrokeThickness = Options.Thickness;
            pll.StrokeDashArray = Options.LineStyle;
            pll.Stroke = Options.ColorBrush;

            UIElement = pll;
        }
    }
}
