using RevitDevelop.Tools.InstallConstructionJoinOfWalls.views;
using RevitDevelop.Tools.Rebars.InstallConstructionJoinOfWalls.models;
using RevitDevelop.Utils.RevGrids;
using System.Windows.Controls;
using Utils.canvass;

namespace RevitDevelop.Tools.Rebars.InstallConstructionJoinOfWalls.viewModels
{
    public class InstallConstructionJoinOfWallViewModel : ObservableObject
    {
        public ElementInstance ElementInstance { get; set; }
        public InstallConstructionJoinOfWallView MainView { get; set; }
        public CanvasPageBase CanvasMain { get; set; }
        public InstallConstructionJoinOfWallViewModel()
        {
            ElementInstance = new ElementInstance();
            MainView = new InstallConstructionJoinOfWallView() { DataContext = this };
            MainView.Loaded += MainView_Loaded;
            ActionInit();

        }
        private void ActionInit()
        {
            //Level Selected Change
            ElementInstance.LevelSelectedChange = () =>
            {
                ElementInstance.LevelSelectedChangeAction(this);
            };
        }
        private void MainView_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            CanvasMain = new CanvasPageBase(MainView.FindName("MainCanvas") as Canvas);
            CanvasMain.Scale = CanvasMain.RatioScale * CanvasMain.DistanceCrossScreen / ElementInstance.RCross;
            ElementInstance.RGrids = ElementInstance.Grids.Select(x => new RGrid(1, x, ElementInstance.RCenter, CanvasMain)).ToList();
            ElementInstance.RColumnsCurrentLevel = ElementInstance.UpdateRColumnsCurrentLevel(
                ElementInstance.LevelSelected,
                ElementInstance.Columns,
                ElementInstance.RCenter,
                CanvasMain);
            ElementInstance.RColumnsPreviousLevel = ElementInstance.RColumnsCurrentLevel;

            ElementInstance.RWallsCurrentLevel = ElementInstance.UpdateRWallsCurrentLevel(
                ElementInstance.LevelSelected,
                ElementInstance.Walls,
                ElementInstance.RCenter,
                CanvasMain);
            ElementInstance.RWallsPreviousLevel = ElementInstance.RWallsCurrentLevel;

            ElementInstance.DrawInCanvas_Grid(this);
            ElementInstance.DrawInCanvas_Column(this);
            ElementInstance.DrawInCanvas_Wall(this);
        }
    }
}
