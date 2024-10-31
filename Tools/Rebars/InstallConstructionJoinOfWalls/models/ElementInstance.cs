using HcBimUtils.DocumentUtils;
using RevitDevelop.Tools.Rebars.InstallConstructionJoinOfWalls.viewModels;
using RevitDevelop.Utils.RevColumns;
using RevitDevelop.Utils.RevGrids;
using RevitDevelop.Utils.RevWalls;
using Utils.canvass;
using Utils.FilterElements;
using Utils.RevPoints;

namespace RevitDevelop.Tools.Rebars.InstallConstructionJoinOfWalls.models
{
    public class ElementInstance : ObservableObject
    {
        private Level _levelSelected;
        private List<RColumn> _rColumnsPreviousLevel;
        private List<RColumn> _rColumnsCurrentLevel;
        private List<RWall> _rWallsCurrentLevel;
        private List<RWall> _rWallsPreviousLevel;
        public List<Grid> Grids { get; set; }
        public List<RGrid> RGrids { get; set; }
        public List<Level> Levels { get; set; }
        public Level LevelSelected
        {
            get => _levelSelected;
            set
            {
                _levelSelected = value;
                OnPropertyChanged();
                LevelSelectedChange?.Invoke();
            }
        }
        public Action LevelSelectedChange { get; set; }
        public XYZ RCenter { get; set; }
        public BoundingBoxXYZ RBox { get; set; }
        public double RCross { get; set; }
        public List<FamilyInstance> Columns { get; set; }
        public List<RColumn> RColumnsCurrentLevel
        {
            get => _rColumnsCurrentLevel;
            set
            {
                _rColumnsCurrentLevel = value;
                OnPropertyChanged();
            }
        }
        public List<RColumn> RColumnsPreviousLevel
        {
            get => _rColumnsPreviousLevel;
            set
            {
                _rColumnsPreviousLevel = value;
                OnPropertyChanged();
            }
        }
        public List<Wall> Walls { get; set; }
        public List<RWall> RWallsCurrentLevel
        {
            get => _rWallsCurrentLevel;
            set
            {
                _rWallsCurrentLevel = value;
                OnPropertyChanged();
            }
        }
        public List<RWall> RWallsPreviousLevel
        {
            get => _rWallsPreviousLevel;
            set
            {
                _rWallsPreviousLevel = value;
                OnPropertyChanged();
            }
        }
        public ElementInstance()
        {
            Levels = AC.Document.GetElementsFromClass<Level>(false).OrderBy(x => x.Elevation).ToList();
            LevelSelected = Levels.FirstOrDefault();
            Grids = AC.Document.GetElementsFromClass<Grid>(false);
            RCenter = GetRCenter(out BoundingBoxXYZ rBox, out double rCross);
            RBox = rBox;
            RCross = rCross;
            Columns = AC.Document.GetElementsFromCategory<FamilyInstance>(BuiltInCategory.OST_StructuralColumns, false);
            Walls = AC.Document.GetElementsFromClass<Wall>(false);
        }

        public static void LevelSelectedChangeAction(InstallConstructionJoinOfWallViewModel viewModel)
        {
            //UpdateRColumnsCurrentLevel
            viewModel.ElementInstance.RColumnsPreviousLevel = viewModel.ElementInstance.RColumnsCurrentLevel;
            viewModel.ElementInstance.RColumnsCurrentLevel = UpdateRColumnsCurrentLevel(
                viewModel.ElementInstance.LevelSelected,
                viewModel.ElementInstance.Columns,
                viewModel.ElementInstance.RCenter,
                viewModel.CanvasMain);
            DrawInCanvas_Column(viewModel);

            //UpdateRColumnsCurrentLevel
            viewModel.ElementInstance.RWallsPreviousLevel = viewModel.ElementInstance.RWallsCurrentLevel;
            viewModel.ElementInstance.RWallsCurrentLevel = UpdateRWallsCurrentLevel(
                viewModel.ElementInstance.LevelSelected,
                viewModel.ElementInstance.Walls,
                viewModel.ElementInstance.RCenter,
                viewModel.CanvasMain);
            DrawInCanvas_Wall(viewModel);
        }

        public static List<RColumn> UpdateRColumnsCurrentLevel(Level levelSelected, List<FamilyInstance> columns, XYZ rCenter, CanvasPageBase canvasMain)
        {
            var results = new List<RColumn>();
            try
            {
                var columnsCurrent = columns
                    .Where(x => x.LevelId.GetHashCode() == levelSelected.Id.GetHashCode())
                    .ToList();
                results = columnsCurrent.Select(x => new RColumn(1, x, rCenter, canvasMain)).ToList();
            }
            catch (Exception)
            {
            }
            return results;
        }
        public static List<RWall> UpdateRWallsCurrentLevel(Level levelSelected, List<Wall> walls, XYZ rCenter, CanvasPageBase canvasMain)
        {
            var results = new List<RWall>();
            try
            {
                var wallsCurrent = walls
                    .Where(x => x.LevelId.GetHashCode() == levelSelected.Id.GetHashCode())
                    .ToList();
                results = wallsCurrent.Select(x => new RWall(1, x, rCenter, canvasMain)).ToList();
            }
            catch (Exception)
            {
            }
            return results;
        }

        public static void DrawInCanvas_Grid(InstallConstructionJoinOfWallViewModel viewModel)
        {
            foreach (var rGrid in viewModel.ElementInstance.RGrids)
            {
                rGrid.DrawInCanvas();
            }
        }

        public static void DrawInCanvas_Column(InstallConstructionJoinOfWallViewModel viewModel)
        {
            //remove current column
            foreach (var item in viewModel.ElementInstance.RColumnsPreviousLevel)
            {
                viewModel.CanvasMain.Parent.Children.Remove(item.CColumn?.UIElement);
            }
            //draw column in canvas
            foreach (var rColumn in viewModel.ElementInstance.RColumnsCurrentLevel)
            {
                rColumn.DrawInCanvas();
            }
        }

        public static void DrawInCanvas_Wall(InstallConstructionJoinOfWallViewModel viewModel)
        {
            //remove current wall
            foreach (var item in viewModel.ElementInstance.RWallsPreviousLevel)
            {
                viewModel.CanvasMain.Parent.Children.Remove(item.CWall?.UIElement);
            }
            //draw wall in canvas
            foreach (var item in viewModel.ElementInstance.RWallsCurrentLevel)
            {
                item.DrawInCanvas();
            }
        }

        private XYZ GetRCenter(out BoundingBoxXYZ rBox, out double rCross)
        {
            rCross = 0;
            rBox = new BoundingBoxXYZ();
            XYZ result = null;
            try
            {
                var ps = Grids.Select(x => x.Curve)
                    .Select(x => new List<XYZ>() { x.GetEndPoint(0), x.GetEndPoint(1) })
                    .Aggregate((a, b) => a.Concat(b).ToList());
                result = ps.GetCenter();
                var minx = ps.Min(x => x.X);
                var miny = ps.Min(x => x.Y);
                var maxx = ps.Max(x => x.X);
                var maxy = ps.Max(x => x.Y);
                rBox.Max = new XYZ(maxx, maxy, 0);
                rBox.Min = new XYZ(minx, miny, 0);
                rCross = rBox.Min.DistanceTo(rBox.Max);
            }
            catch (Exception)
            {
            }
            return result;
        }
    }
}
