using RevitDevelop.Utils.RevPoints;

namespace RevitDevelop.Tools.Rebars.InstallRebarBeamV2.models
{
    public abstract partial class RebarBaseInfo : ObservableObject
    {
        [ObservableProperty]
        private string _diameter;
        [ObservableProperty]
        private int _quantity;
        public int HostId;
        public int RebarBeamType { get; set; }
        public List<RevPoint> Shape { get; set; }
    }
}
