using System.Text.RegularExpressions;

namespace Utils.NumberUtils
{
    public static class NumberUtil
    {
        public static int GetIntegerFromText(this string text)
        {
            var resultString = Regex.Match(text, @"\d+").Value;
            return !string.IsNullOrEmpty(resultString) ? int.Parse(resultString) : 0;
        }
        public static int DivInterger(this double num1, double num2, out double du)
        {
            du = num1 % num2;
            var per = du * 100 / num2;
            var n = int.Parse(Math.Round((num1 - du) / num2, 0).ToString(), System.Globalization.NumberStyles.Integer);
            return per <= 20 ? n : n + 1;
        }
        public static int SolveNumber(this double num, double spacing)
        {
            var d = num % spacing;
            var per = d * 100 / spacing;
            var n = int.Parse($"{Math.Round((num - d) / spacing, 0)}");
            return per > 20 ? n + 1 : n;
        }
        public static int SolveNumber(this int num, int spacing)
        {
            var d = num % spacing;
            var per = d * 100 / spacing;
            var n = (num - d) / spacing;
            return d > 0 ? n + 1 : n;
        }
        public static int FindInterger(this string t)
        {
            var re = new Regex(@"\d+");
            var vl = re.Match(t);
            return string.IsNullOrEmpty(vl.Value) ? 0 : int.Parse(vl.Value);
        }
        public static bool CheckNumberInStrings(this List<string> numberValues)
        {
            return numberValues.Any(x =>
            {
                var result = false;
                try
                {
                    var numberValid = double.Parse(x, System.Globalization.NumberStyles.Number);
                    result = false;
                }
                catch (Exception)
                {
                    result = true;
                }
                return result;
            });
        }
    }

    public class DoubleComparer(double tolerance = 1E-09) : EqualityComparer<double>
    {
        public override bool Equals(double x, double y)
        {
            return Math.Abs(x - y) < tolerance;
        }

        public override int GetHashCode(double obj)
        {
            return 0;
        }
    }
}
