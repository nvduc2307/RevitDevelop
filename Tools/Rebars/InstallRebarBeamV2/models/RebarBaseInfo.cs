using RevitDevelop.Utils.RevPoints;

namespace RevitDevelop.Tools.Rebars.InstallRebarBeamV2.models
{
    public abstract partial class RebarBaseInfo : ObservableObject
    {
        private int _quantity;
        public int Quantity
        {
            get => _quantity;
            set
            {
                _quantity = value == 1 ? 2 : value;
                OnDiameterChanged(nameof(Quantity));
                QuantityChange?.Invoke();
            }
        }
        public Action QuantityChange { get; set; }
        [ObservableProperty]
        private string _diameter;
        public int HostId;
        public int RebarBeamType { get; set; }
        public List<RevPoint> Shape { get; set; }
    }
}
