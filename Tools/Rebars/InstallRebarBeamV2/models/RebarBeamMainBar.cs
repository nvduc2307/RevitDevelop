namespace RevitDevelop.Tools.Rebars.InstallRebarBeamV2.models
{
    public partial class RebarBeamMainBar : RebarBaseInfo
    {
        [ObservableProperty]
        private int _rebarGroupType;
        [ObservableProperty]
        private int _rebarLevelType;
    }
    public partial class RebarBeamTop : ObservableObject
    {
        private int _rebarGroupTypeActive;
        public int RebarGroupTypeActive
        {
            get => _rebarGroupTypeActive;
            set
            {
                _rebarGroupTypeActive = value;
                OnPropertyChanged();
                RebarGroupTypeChange?.Invoke();
            }
        }
        public RebarBeamMainBar RebarBeamTopLevel1 { get; set; }
        public RebarBeamMainBar RebarBeamTopLevel2 { get; set; }
        public RebarBeamMainBar RebarBeamTopLevel3 { get; set; }
        [ObservableProperty]
        public RebarBeamMainBar _rebarBeamTopLevelActive;
        public Action RebarGroupTypeChange { get; set; }
        public static void TopRebarGroupTypeChangeFunc(RebarBeamTop rebarBeam)
        {
            switch (rebarBeam.RebarGroupTypeActive)
            {
                case 1:
                    rebarBeam.RebarBeamTopLevelActive = rebarBeam.RebarBeamTopLevel1;
                    break;
                case 2:
                    rebarBeam.RebarBeamTopLevelActive = rebarBeam.RebarBeamTopLevel2;
                    break;
                case 3:
                    rebarBeam.RebarBeamTopLevelActive = rebarBeam.RebarBeamTopLevel3;
                    break;
            }
        }

    }
    public partial class RebarBeamBot : ObservableObject
    {
        private int _rebarGroupTypeActive;
        public int RebarGroupTypeActive
        {
            get => _rebarGroupTypeActive;
            set
            {
                _rebarGroupTypeActive = value;
                OnPropertyChanged();
                RebarGroupTypeChange?.Invoke();
            }
        }
        public RebarBeamMainBar RebarBeamBotLevel1 { get; set; }
        public RebarBeamMainBar RebarBeamBotLevel2 { get; set; }
        public RebarBeamMainBar RebarBeamBotLevel3 { get; set; }
        [ObservableProperty]
        public RebarBeamMainBar _rebarBeamBotLevelActive;
        public Action RebarGroupTypeChange { get; set; }
        public static void BotRebarGroupTypeChangeFunc(RebarBeamBot rebarBeam)
        {
            switch (rebarBeam.RebarGroupTypeActive)
            {
                case 1:
                    rebarBeam.RebarBeamBotLevelActive = rebarBeam.RebarBeamBotLevel1;
                    break;
                case 2:
                    rebarBeam.RebarBeamBotLevelActive = rebarBeam.RebarBeamBotLevel2;
                    break;
                case 3:
                    rebarBeam.RebarBeamBotLevelActive = rebarBeam.RebarBeamBotLevel3;
                    break;
            }
        }
    }
}
