using RevitDevelop.Tools.Rebars.InstallRebarBeamV2.models;

namespace RevitDevelop.Tools.Rebars.InstallRebarBeamV2.viewModels
{
    public class InstallRebarBeamV2ViewModel : ObservableObject
    {
        public ElementInstances ElementInstances { get; set; }
        public InstallRebarBeamV2ViewModel()
        {
            ElementInstances = new ElementInstances();
        }
    }
}
