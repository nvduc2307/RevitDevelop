using Autodesk.Revit.DB;

namespace RIMT.InstallRebarSlab.models
{
    public class MSlabGrid
    {
        public Grid Grid { get; set; }
        public MSlabGrid(Grid grid)
        {
            Grid = grid;
        }
        public void DrawInCanvas()
        {

        }
    }
}
