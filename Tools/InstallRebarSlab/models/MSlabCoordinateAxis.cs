using System.Windows.Shapes;
using Utils.canvass;
using wd = System.Windows;

namespace RevitDevelop.InstallRebarSlab.models
{
    public class MSlabCoordinateAxis : ObservableObject
    {
        private XYZ _vtX;
        private XYZ _vtXBase;
        private XYZ _vtY;
        private XYZ _vtYBase;
        private int _angle;
        public XYZ VtX
        {
            get => _vtX;
            set
            {
                _vtX = value;
                OnPropertyChanged();
            }
        }
        public XYZ VtY
        {
            get => _vtY;
            set
            {
                _vtY = value;
                OnPropertyChanged();
            }
        }
        public XYZ VtXBase
        {
            get => _vtXBase;
            set
            {
                _vtXBase = value;
                OnPropertyChanged();
            }
        }
        public XYZ VtYBase
        {
            get => _vtYBase;
            set
            {
                _vtYBase = value;
                OnPropertyChanged();
            }
        }
        public XYZ VtZ { get; set; }
        public Path AxisX { get; set; }
        public Path AxisY { get; set; }
        public int Angle
        {
            get => _angle;
            set
            {
                _angle = value;
                OnPropertyChanged(nameof(Angle));
                AngleChangeEvent?.Invoke();
            }
        }
        public Action AngleChangeEvent { get; set; }
        public MSlabCoordinateAxis(XYZ vtX, XYZ vtY, XYZ vtZ)
        {
            VtX = vtX;
            VtY = vtY;
            VtZ = vtZ;
            VtXBase = vtX;
            VtYBase = vtY;
            Angle = 0;
        }
        public static void AngleChangeEventAction(CanvasPageBase canvasPageBase, XYZ vtXBase, XYZ vtYBase, int angle, XYZ CenterR, out XYZ vtxR, out XYZ vtyR)
        {
            var dmangle = angle * Math.PI / 180;
            var vtXBaseCanvas = new wd.Point(vtXBase.X, -vtXBase.Y);
            var vtYBaseCanvas = new wd.Point(vtYBase.X, -vtYBase.Y);
            var vtx = vtXBaseCanvas.RotateVector(canvasPageBase.Center, -dmangle);
            var vty = vtYBaseCanvas.RotateVector(canvasPageBase.Center, -dmangle);

            vtxR = new XYZ(vtx.X, -vtx.Y, 0);
            vtyR = new XYZ(vty.X, -vty.Y, 0);
        }
    }
}
