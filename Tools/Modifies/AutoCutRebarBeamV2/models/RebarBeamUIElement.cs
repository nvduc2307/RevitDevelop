using HcBimUtils;
using RevitDevelop.BeamRebar.ViewModel;
using RevitDevelop.Tools.Modifies.AutoCutRebarBeamV2.models;
using RevitDevelop.Utils.Window2Ds;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using Utils.canvass;

namespace RevitDevelop.AutoCutRebarBeamV2.models
{
    public class RebarBeamUIElement
    {
        private readonly DispatcherTimer _debounceTimer;
        public int Id { get; set; }
        public RevRebarCurveCutUICanvas Parent { get; set; }
        public int RebarId { get; set; }
        public int LevelGroup { get; set; }
        public CanvasPageBase CanvasPage { get; set; }
        public UIElement Element { get; set; }
        public TextBox ElementValue { get; set; }
        public Action ElementValueChanged { get; set; }
        public event EventHandler EventElementValueChanged;
        public Autodesk.Revit.DB.XYZ CenterGroup { get; set; }
        public double LengthMm { get; set; }
        public RebarBeamUIElement(
            int id,
            RevRebarCurveCutUICanvas parent,
            UIElement element,
            CanvasPageBase canvasPage,
            int levelGroup,
            Autodesk.Revit.DB.XYZ centerGroup)
        {
            _debounceTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(0.5) // Set debounce interval
            };
            _debounceTimer.Tick += DebounceTimer_Tick;

            Id = id;
            Parent = parent;
            Element = element;
            LevelGroup = levelGroup;
            CenterGroup = centerGroup;
            CanvasPage = canvasPage;
            ElementValue = new TextBox();
            ElementValue.Width = 40;
            ElementValue.Height = 25;
            LengthMm = 0;
            if (Element is System.Windows.Shapes.Line l)
            {
                AlignElementValue(this);
                ElementValue.Text = LengthMm.ToString();
                ElementValue.Visibility = System.Windows.Visibility.Visible;
                ElementValue.TextChanged += ElementValue_TextChanged;
            }

            LevelGroup = levelGroup;
        }

        private void DebounceTimer_Tick(object sender, EventArgs e)
        {
            _debounceTimer.Stop();
            ElementValueChanged?.Invoke();
            EventElementValueChanged?.Invoke(this, e);
        }

        private void ElementValue_TextChanged(object sender, TextChangedEventArgs e)
        {
            _debounceTimer.Stop();
            _debounceTimer.Start();
        }
        public static void AlignElementValue(RebarBeamUIElement rebarBeamUIElement)
        {
            try
            {
                var l = rebarBeamUIElement.Element as System.Windows.Shapes.Line;
                var extent = 5;
                var ps = new System.Windows.Point(l.X1, l.Y1);
                var pe = new System.Windows.Point(l.X2, l.Y2);
                var mid = new System.Windows.Point(l.X1 * 0.5 + l.X2 * 0.5, l.Y1 * 0.5 + l.Y2 * 0.5);
                var vtx = ps.Direction(pe);
                var vty = (RebarBeamLevel)rebarBeamUIElement.LevelGroup == RebarBeamLevel.Top
                    ? vtx.CrossProduc()
                    : vtx.CrossProduc(new System.Windows.Point(), 1);
                Canvas.SetTop(rebarBeamUIElement.ElementValue, mid.Y - rebarBeamUIElement.ElementValue.Height / 2 + vty.Y * (rebarBeamUIElement.ElementValue.Height / 2 + extent));
                Canvas.SetLeft(rebarBeamUIElement.ElementValue, mid.X - rebarBeamUIElement.ElementValue.Width / 2 + vty.X * (rebarBeamUIElement.ElementValue.Width / 2 + extent));
                var psr = ps.ConvertPointCToR(rebarBeamUIElement.CenterGroup, rebarBeamUIElement.CanvasPage);
                var per = pe.ConvertPointCToR(rebarBeamUIElement.CenterGroup, rebarBeamUIElement.CanvasPage);
                rebarBeamUIElement.LengthMm = Math.Round(psr.DistanceTo(per).FootToMm(), 0);
            }
            catch (Exception)
            {
            }
        }
    }
}
