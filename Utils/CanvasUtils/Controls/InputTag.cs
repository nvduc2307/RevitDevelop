using System.Text.RegularExpressions;
using System.Windows.Controls;
using System.Windows.Media;

namespace Utils.CanvasUtils.Controls
{
    public class InputTag : TextBox
    {
        private const string regex = "^-?(0|[1-9][0-9]*)$";
        public InputType Type { get; set; }
        public InputTag()
        {
            this.VerticalContentAlignment = System.Windows.VerticalAlignment.Center;
            this.TextChanged += (s, e) =>
            {
                if (Type == InputType.Number)
                {
                    var texts = this.Text.Split();
                    var inValid = texts.Any(x => !Regex.IsMatch(x.ToString(), regex));
                    if (inValid)
                    {
                        this.BorderBrush = Brushes.Red;
                        this.Text = this.Text.Length == 0
                        ? "0"
                        : this.Text.Substring(0, this.Text.Length - 1);
                        //IO.ShowWarning("Ky tu khong hop le");
                    }
                    else
                    {
                        this.BorderBrush = Brushes.Green;
                    }
                }
            };
        }
        public enum InputType
        {
            Text,
            Number
        }
    }
}