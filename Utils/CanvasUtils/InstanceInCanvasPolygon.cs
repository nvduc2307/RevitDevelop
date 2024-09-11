using System.Windows.Input;
using System.Windows.Shapes;

namespace Utils.canvass
{
    public class InstanceInCanvasPolygon : InstanceInCanvas
    {
        public IEnumerable<System.Windows.Point> Points { get; set; }
        public InstanceInCanvasPolygon(CanvasPageBase canvasPageBase, OptionStyleInstanceInCanvas options, IEnumerable<System.Windows.Point> points) : base(canvasPageBase, options)
        {
            Points = points;
            var plg = new Polygon();
            foreach (System.Windows.Point p in points)
            {
                plg.Points.Add(p);
            }
            plg.StrokeThickness = Options.Thickness;
            plg.StrokeDashArray = Options.LineStyle;
            plg.Stroke = Options.ColorBrush;

            if (options.Fill != null) plg.Fill = options.Fill;

            UIElement = plg;

            //action
            plg.MouseMove += Plg_MouseMove;
        }

        private void Plg_MouseMove(object sender, MouseEventArgs e)
        {
            if (sender is Polygon plg) plg.Cursor = Cursors.Hand;
        }
    }
}
