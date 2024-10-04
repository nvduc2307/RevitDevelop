using Utils.canvass;

namespace RevitDevelop.Utils.RevGrids
{
    public class RGrid
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public Grid Grid { get; set; }
        public XYZ RCenter { get; set; }
        public InstanceInCanvasLine CGrid { get; set; }
        public CanvasPageBase CanvasPageBase { get; set; }
        public RGrid(int id, Grid grid, XYZ rCenter, CanvasPageBase canvasPageBase)
        {
            Id = id;
            RCenter = rCenter;
            CanvasPageBase = canvasPageBase;
            Grid = grid;
            Name = grid.Name;
            CGrid = GetCGrid();
        }
        public void DrawInCanvas()
        {
            try
            {
                CGrid.DrawInCanvas();
            }
            catch (Exception)
            {
            }
        }
        private InstanceInCanvasLine GetCGrid()
        {
            InstanceInCanvasLine result = null;
            try
            {
                var curve = Grid.Curve;
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
    }
}
