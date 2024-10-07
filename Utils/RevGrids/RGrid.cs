using RevitDevelop.Utils.RevElements;
using Utils.canvass;

namespace RevitDevelop.Utils.RevGrids
{
    public class RGrid : RElement
    {
        public XYZ PS { get; set; }
        public XYZ PE { get; set; }
        public InstanceInCanvasLine CGrid { get; set; }
        public InstanceInCanvasCircel StartNote { get; set; }
        public InstanceInCanvasCircel EndNote { get; set; }
        public RGrid(int id, object rGrid, XYZ rCenter, CanvasPageBase canvasPageBase) : base(id, rGrid, rCenter, canvasPageBase)
        {
            CGrid = GetCGrid(out XYZ ps, out XYZ pe, out string name);
            Name = name;
            PS = ps;
            PE = pe;
            StartNote = GetStartNote();
            EndNote = GetEndNote();
        }
        private InstanceInCanvasLine GetCGrid(out XYZ ps, out XYZ pe, out string name)
        {
            ps = null;
            pe = null;
            name = null;
            InstanceInCanvasLine result = null;
            try
            {
                if (RObject is Grid grid)
                {
                    name = grid.Name;
                    var curve = grid.Curve;
                    ps = curve.GetEndPoint(0);
                    pe = curve.GetEndPoint(1);
                    var p1 = curve.GetEndPoint(0).ConvertPointRToC(RCenter, CanvasPageBase);
                    var p2 = curve.GetEndPoint(1).ConvertPointRToC(RCenter, CanvasPageBase);
                    result = new InstanceInCanvasLine(CanvasPageBase, OptionStyleInstanceInCanvas.OPTION_GRID, p1, p2);
                    result.ClickLeftMouse = () =>
                    {
                        ClickLeftMouse();
                    };
                    result.ClickRightMouse = () =>
                    {
                        ClickRightMouse();
                    };
                }
            }
            catch (Exception)
            {
            }
            return result;
        }
        private InstanceInCanvasCircel GetStartNote()
        {
            InstanceInCanvasCircel result = null;
            try
            {
                var p1 = PS.ConvertPointRToC(RCenter, CanvasPageBase);
                var p2 = PE.ConvertPointRToC(RCenter, CanvasPageBase);
                var vt = p1.GetVector(p2).VtNormal();
                result = new InstanceInCanvasCircel(
                    CanvasPageBase,
                    OptionStyleInstanceInCanvas.OPTION_GRID_Note,
                    CanvasPageBase.Center,
                    15,
                    p1,
                    vt,
                    Name);
                result.ClickLeftMouse = () =>
                {
                    ClickLeftMouse();
                };
                result.ClickRightMouse = () =>
                {
                    ClickRightMouse();
                };
            }
            catch (Exception)
            {
            }
            return result;
        }
        private InstanceInCanvasCircel GetEndNote()
        {
            InstanceInCanvasCircel result = null;
            try
            {
                var p1 = PS.ConvertPointRToC(RCenter, CanvasPageBase);
                var p2 = PE.ConvertPointRToC(RCenter, CanvasPageBase);
                var vt = p1.GetVector(p2).VtNormal();
                result = new InstanceInCanvasCircel(
                    CanvasPageBase,
                    OptionStyleInstanceInCanvas.OPTION_GRID_Note,
                    CanvasPageBase.Center,
                    15,
                    p2,
                    new System.Windows.Point(-vt.X, -vt.Y),
                    Name);
                result.ClickLeftMouse = () =>
                {
                    ClickLeftMouse();
                };
                result.ClickRightMouse = () =>
                {
                    ClickRightMouse();
                };
            }
            catch (Exception)
            {
            }
            return result;
        }
        private void ClickLeftMouse()
        {
        }
        private void ClickRightMouse()
        {
        }

        public override void DrawInCanvas()
        {
            try
            {
                CGrid.DrawInCanvas();
                StartNote.DrawInCanvas();
                EndNote.DrawInCanvas();
            }
            catch (Exception)
            {
            }
        }
    }
}
