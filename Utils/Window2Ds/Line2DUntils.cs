using System.Windows.Media;

namespace RevitDevelop.Utils.Window2Ds
{
    public static class Line2DUntils
    {
        public static System.Windows.Shapes.Line CreateLine(
            this System.Windows.Point ps,
            System.Windows.Point pe,
            SolidColorBrush color,
            double thickness)
        {
            System.Windows.Shapes.Line result = null;
            try
            {
                result = new System.Windows.Shapes.Line();
                result.X1 = ps.X;
                result.X2 = pe.X;
                result.Y1 = ps.Y;
                result.Y2 = pe.Y;
                result.Stroke = color;
                result.StrokeThickness = thickness;
            }
            catch (Exception)
            {
            }
            return result;
        }
    }
}
