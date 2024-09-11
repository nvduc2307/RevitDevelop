using System.Windows.Media;

namespace Utils.canvass
{
    public class OptionStyleInstanceInCanvas
    {
        public static OptionStyleInstanceInCanvas OPTION_CONCRETE =
            new OptionStyleInstanceInCanvas(
                StyleThicknessInCanvas.Thickness_2,
                StyleDashInCanvas.Style_Solid,
                StyleColorInCanvas.Color_Concrete_OutLine,
                StyleColorInCanvas.Color_Concrete);
        public static OptionStyleInstanceInCanvas OPTION_OPENING =
            new OptionStyleInstanceInCanvas(
                StyleThicknessInCanvas.Thickness_2,
                StyleDashInCanvas.Style_Solid,
                StyleColorInCanvas.Color_Concrete_OutLine,
                StyleColorInCanvas.Color_BackGround);
        public static OptionStyleInstanceInCanvas OPTION_REBAR =
             new OptionStyleInstanceInCanvas(
                StyleThicknessInCanvas.Thickness_1,
                StyleDashInCanvas.Style_Solid,
                StyleColorInCanvas.Color_Rebar,
                StyleColorInCanvas.Color_Rebar);
        public double Thickness { get; set; }
        public DoubleCollection LineStyle { get; set; }
        public SolidColorBrush ColorBrush { get; set; }
        public SolidColorBrush Fill { get; set; }
        public OptionStyleInstanceInCanvas(
            double thickness,
            DoubleCollection lineStyle,
            SolidColorBrush colorBrush,
            SolidColorBrush fill)
        {
            Thickness = thickness;
            LineStyle = lineStyle;
            ColorBrush = colorBrush;
            Fill = fill;
        }
    }
}
