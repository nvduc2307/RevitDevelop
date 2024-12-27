using RevitDevelop.AutoCutRebarBeamV2.models;
using System.Windows.Controls;
using System.Windows;
using Utils.canvass;

namespace RevitDevelop.Utils.CanvasUtils.LapElements
{
    public class LapElement
    {
        private const double _h = 10;
        private const double _w = 10;
        public int Id { get; set; }
        public System.Windows.Point Location { get; set; }
        public RebarBeamCutType Type { get; set; }
        public CanvasPageBase CanvasPageBase { get; set; }
        public UIElement Lap { get; set; }

        public LapElement(RebarBeamCutType type, System.Windows.Point location, CanvasPageBase canvas)
        {
            Type = type;
            Location = location;
            CanvasPageBase = canvas;
        }
        public void Create()
        {
            var option = OptionStyleInstanceInCanvas.OPTION_CONCRETE;
            UIElement lap = null;
            switch (Type)
            {
                case RebarBeamCutType.Weld:
                    lap = InstanceInCanvasCircel(option);
                    break;
                case RebarBeamCutType.Coupler:
                    lap = InstanceInCanvasPolygon(option);
                    break;
                case RebarBeamCutType.LapLength:
                    break;
            }
            if (lap != null)
                CanvasPageBase.Parent.Children.Add(lap);
        }
        private UIElement InstanceInCanvasCircel(OptionStyleInstanceInCanvas option)
        {
            var lap = new System.Windows.Shapes.Ellipse()
            {
                Height = _h,
                Width = _w,
                StrokeThickness = option.Thickness,
                StrokeDashArray = option.LineStyle,
                Stroke = option.ColorBrush,
                Fill = option.Fill,
            };

            Canvas.SetTop(lap, Location.Y - _h / 2);
            Canvas.SetLeft(lap, Location.X - _w / 2);

            return lap;
        }
        private UIElement InstanceInCanvasPolygon(OptionStyleInstanceInCanvas option)
        {
            var ps = new List<System.Windows.Point>();
            ps.Add(new System.Windows.Point(Location.X - _w / 2, Location.Y - _h / 2));
            ps.Add(new System.Windows.Point(Location.X - _w / 2, Location.Y + _h / 2));
            ps.Add(new System.Windows.Point(Location.X + _w / 2, Location.Y + _h / 2));
            ps.Add(new System.Windows.Point(Location.X + _w / 2, Location.Y - _h / 2));
            var lap = new InstanceInCanvasPolygon(CanvasPageBase, option, ps);
            return lap.UIElement;
        }
    }
}
