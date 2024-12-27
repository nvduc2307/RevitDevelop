using RevitDevelop.AutoCutRebarBeamV2.models;
using RevitDevelop.Utils.Regexs;
using RevitDevelop.Utils.Window2Ds;
using System.Text.RegularExpressions;

namespace RevitDevelop.AutoCutRebarBeamV2.middleWares
{
    public static class RebarBeamUIElementMiddleWare
    {
        public static void CheckValueHasText(this RebarBeamUIElement item, EventHandler eventHandler)
        {
            var textValue = item.ElementValue.Text;
            var checkDouble = Regex.Match(textValue, RegexUtils.REGEX_DOUBLE);
            if (!checkDouble.Success)
            {
                item.EventElementValueChanged -= eventHandler;
                item.ElementValue.Text = item.LengthMm.ToString();
                item.EventElementValueChanged += eventHandler;
            }
        }
        public static void CheckValueLimit(this RebarBeamUIElement item, EventHandler eventHandler)
        {
            var limit = 1000;
            var textValue = item.ElementValue.Text;
            var curveValue = double.Parse(textValue, System.Globalization.NumberStyles.Number);
            var l = item.Element as System.Windows.Shapes.Line;
            if (curveValue < limit)
            {
                if (!l.Direction().DotProduct(new System.Windows.Point(1, 0)).IsAlmostEqual(0))
                {
                    item.EventElementValueChanged -= eventHandler;
                    item.ElementValue.Text = item.LengthMm.ToString();
                    item.EventElementValueChanged += eventHandler;
                }
            }
        }
    }
}
