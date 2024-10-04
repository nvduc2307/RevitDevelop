using RevitDevelop.Tools.InstallConstructionJoinOfWalls.models;
using RevitDevelop.Tools.InstallConstructionJoinOfWalls.views;
using RevitDevelop.Utils.RevGrids;
using System.Windows.Controls;
using Utils.canvass;

namespace RevitDevelop.Tools.InstallConstructionJoinOfWalls.viewModels
{
    public class InstallConstructionJoinOfWallViewModel : ObservableObject
    {
        public ElementInstance ElementInstance { get; set; }
        public InstallConstructionJoinOfWallView MainView { get; set; }
        public CanvasPageBase CanvasMain { get; set; }
        public List<RGrid> RGrids { get; set; }
        public InstallConstructionJoinOfWallViewModel()
        {
            ElementInstance = new ElementInstance();
            MainView = new InstallConstructionJoinOfWallView() { DataContext = this };
            MainView.Loaded += MainView_Loaded;
        }

        private void MainView_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            CanvasMain = new CanvasPageBase(MainView.FindName("MainCanvas") as Canvas);
            CanvasMain.Scale = CanvasMain.RatioScale * CanvasMain.DistanceCrossScreen / ElementInstance.RCross;
            RGrids = ElementInstance.Grids.Select(x => new RGrid(1, x, ElementInstance.RCenter, CanvasMain)).ToList();
            foreach (var rGrid in RGrids)
            {
                rGrid.DrawInCanvas();
            }
        }
    }
}
