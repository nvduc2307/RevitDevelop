using RevitDevelop.Tools.Rebars.InstallRebarBeamV2.models;
using RevitDevelop.Tools.Rebars.InstallRebarBeamV2.views;

namespace RevitDevelop.Tools.Rebars.InstallRebarBeamV2.viewModels
{
    public class InstallRebarBeamV2ViewModel : ObservableObject
    {
        public ElementInstances ElementInstances { get; set; }
        public InstallRebarBeamView MainView { get; set; }
        public InstallRebarBeamV2ViewModel()
        {
            ElementInstances = new ElementInstances();
            MainView = new InstallRebarBeamView() { DataContext = this };
        }
    }
}
