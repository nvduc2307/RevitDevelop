using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using wd = System.Windows;

namespace Utils.canvass
{
    public class CanvasPageBase
    {
        public Canvas Parent { get; private set; }
        public double Height { get; private set; }
        public double Width { get; private set; }
        public double Scale { get; set; }
        public double RatioScale { get; set; }
        public wd.Point Center { get; private set; }
        public Vector VTX { get; private set; }
        public Vector VTY { get; private set; }
        public double DistanceCrossScreen { get; private set; }
        public CanvasPageBase(Canvas parent)
        {
            parent.Cursor = Cursors.Hand;
            Parent = parent;
            Width = parent.ActualWidth;
            Height = parent.ActualHeight;
            DistanceCrossScreen = Math.Sqrt(Width * Width + Height * Height);
            RatioScale = 0.7;
            Scale = 0.1;
            Center = new wd.Point(Width / 2, Height / 2);
            VTX = new Vector(1, 0);
            VTY = new Vector(0, 1);
        }

        public static double CalcularScale(double maximumLengthInRevit, double maximumLengthInCanvas)
        {
            return maximumLengthInCanvas / maximumLengthInRevit;
        }
        public static void ZoomIndexTarget(Canvas canvas, float zoomfactor)
        {
            var childs = canvas.Children;
            if (childs.Count > 0)
            {
                var polygon = childs[0] as Polygon;
                var width = polygon.Width;
                var height = polygon.Width;
                var location = new wd.Point(Canvas.GetLeft(polygon) + width / 2, Canvas.GetTop(polygon) + height / 2);
                var _transform = new MatrixTransform();
                Matrix matrix = _transform.Matrix;
                matrix.ScaleAt(zoomfactor, zoomfactor, location.X, location.Y);
                _transform.Matrix = matrix;
                foreach (UIElement child in canvas.Children)
                {
                    double left = Canvas.GetLeft(child);
                    double top = Canvas.GetTop(child);
                    double length = left * (double)zoomfactor;
                    double length2 = top * (double)zoomfactor;
                    Canvas.SetLeft(child, length);
                    Canvas.SetTop(child, length2);
                    child.RenderTransform = _transform;
                }
            }
        }
    }
}
