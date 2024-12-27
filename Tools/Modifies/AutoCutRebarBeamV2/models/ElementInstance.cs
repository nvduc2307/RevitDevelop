using RevitDevelop.BeamRebar.ViewModel;
using Utils.Entities;

namespace RevitDevelop.AutoCutRebarBeamV2.models
{
    public partial class ElementInstance : ObservableObject
    {
        public const double REBAR_LENGTH_CUT = 3500;
        public const double REBAR_LENGTH_MIN = 1000;
        public const double REBAR_LENGTH_MAX = 12000;
        public const double A_MAX = 100;
        public const double A_MIN = 0;
        public const double B_MAX = 100;
        public const double B_MIN = 0;
        private RebarBeamCutTypeInfo _rebarBeamCutTypeInfoSelected;
        private RebarBeamLevelInfo _rebarBeamLevelInfoSelected;
        private RebarBeamGroupTypeInfo _rebarBeamGroupInfoSelected;
        private int _rebarGroupCutSelected;
        private double _rebarLengthCut;
        [ObservableProperty]
        private string _aTitle;
        private double _a;
        private double _b;
        public double A
        {
            get => _a;
            set
            {
                _a = value;
                OnPropertyChanged();
            }
        }
        public double B
        {
            get => _b;
            set
            {
                _b = value;
                BChanged?.Invoke();
                OnPropertyChanged();
            }
        }
        public Action BChanged { get; set; }
        [ObservableProperty]
        private List<int> _rebarGroupCuts;
        [ObservableProperty]
        private bool _isSelectedRebarGroup;
        public double RebarLengthCut
        {
            get => _rebarLengthCut;
            set
            {
                if (value >= REBAR_LENGTH_MIN && value <= REBAR_LENGTH_MAX)
                {
                    _rebarLengthCut = value;
                    OnPropertyChanged();
                    RebarLengthCutChanged?.Invoke();
                }
            }
        }
        public Action RebarLengthCutChanged { get; set; }
        public int RebarGroupCutSelected
        {
            get => _rebarGroupCutSelected;
            set
            {
                _rebarGroupCutSelected = value;
                OnPropertyChanged();
                RebarGroupCutSelectedChanged?.Invoke();
            }
        }
        public Action RebarGroupCutSelectedChanged { get; set; }
        public List<RebarBeamCutTypeInfo> RebarBeamCutTypeInfos { get; set; }
        public List<RebarBeamLevelInfo> RebarBeamLevelInfos { get; set; }
        public List<RebarBeamGroupTypeInfo> RebarBeamGroupInfos { get; set; }
        public RebarBeamCutTypeInfo RebarBeamCutTypeInfoSelected
        {
            get => _rebarBeamCutTypeInfoSelected;
            set
            {
                _rebarBeamCutTypeInfoSelected = value;
                OnPropertyChanged();
                RebarBeamCutTypeInfoSelectedChange?.Invoke();
            }
        }
        public RebarBeamLevelInfo RebarBeamLevelInfoSelected
        {
            get => _rebarBeamLevelInfoSelected;
            set
            {
                _rebarBeamLevelInfoSelected = value;
                OnPropertyChanged();
                RebarBeamLevelInfoSelectedChange?.Invoke();
            }
        }
        public RebarBeamGroupTypeInfo RebarBeamGroupInfoSelected
        {
            get => _rebarBeamGroupInfoSelected;
            set
            {
                _rebarBeamGroupInfoSelected = value;
                OnPropertyChanged();
                RebarBeamGroupInfoSelectedChange?.Invoke();
            }
        }
        public Action RebarBeamCutTypeInfoSelectedChange { get; set; }
        public Action RebarBeamLevelInfoSelectedChange { get; set; }
        public Action RebarBeamGroupInfoSelectedChange { get; set; }
        public SchemaInfo RebarBeamSchemal { get; set; }
        public SchemaInfo LapSchemal { get; set; }
        public SchemaInfo RebarBeamCutSchemal { get; set; }

        public ElementInstance()
        {
            RebarBeamCutSchemal = new SchemaInfo(
                Properties.SchemaInfo.REBAR_BEAM_CUT_SCHEMAL_INFO_GUID,
                Properties.SchemaInfo.REBAR_BEAM_CUT_SCHEMAL_INFO_NAME,
                new SchemaField());
            LapSchemal = new SchemaInfo(
                Properties.SchemaInfo.LAP_SCHEMAL_INFO_GUID,
                Properties.SchemaInfo.LAP_SCHEMAL_INFO_NAME,
                new SchemaField());
            RebarBeamSchemal = new SchemaInfo(
                Properties.SchemaInfo.REBAR_BEAM_SCHEMAL_INFO_GUID,
                Properties.SchemaInfo.REBAR_BEAM_SCHEMAL_INFO_NAME,
                new SchemaField());
            RebarBeamCutTypeInfos = new List<RebarBeamCutTypeInfo>()
            {
                new RebarBeamCutTypeInfo(RebarBeamCutType.Weld,Properties.Langs.AutoCutRebarBeamV2.REBAR_BEAM_CUT_LAP_WELD) { Image = "../imgs/Welds.png"},
                new RebarBeamCutTypeInfo(RebarBeamCutType.Coupler, Properties.Langs.AutoCutRebarBeamV2.REBAR_BEAM_CUT_LAP_COUPLER) { Image = "../imgs/Couplers.png"},
                new RebarBeamCutTypeInfo(RebarBeamCutType.LapLength, Properties.Langs.AutoCutRebarBeamV2.REBAR_BEAM_CUT_LAP_LENGTH) { Image = "../imgs/LapLength.png"},
            };
            RebarBeamCutTypeInfoSelected = RebarBeamCutTypeInfos.FirstOrDefault();
            RebarBeamLevelInfos = new List<RebarBeamLevelInfo>()
            {
                new RebarBeamLevelInfo(RebarBeamLevel.Top, Properties.Langs.AutoCutRebarBeamV2.REBAR_BEAM_CUT_POSITION_TOP),
                new RebarBeamLevelInfo(RebarBeamLevel.Bottom, Properties.Langs.AutoCutRebarBeamV2.REBAR_BEAM_CUT_POSITION_BOTTOM),
            };
            RebarBeamLevelInfoSelected = RebarBeamLevelInfos.FirstOrDefault();
            RebarBeamGroupInfos = new List<RebarBeamGroupTypeInfo>()
            {
                new RebarBeamGroupTypeInfo(RebarBeamGroup.Level1),
                new RebarBeamGroupTypeInfo(RebarBeamGroup.Level2),
                new RebarBeamGroupTypeInfo(RebarBeamGroup.Level3),
            };
            RebarBeamGroupInfoSelected = RebarBeamGroupInfos.FirstOrDefault();
            InitDefaultAAndB(RebarBeamCutTypeInfoSelected, out double a, out double b, out string aTitle);
            A = a;
            B = b;
            ATitle = aTitle;
            RebarLengthCut = REBAR_LENGTH_CUT;
            RebarGroupCuts = new List<int>();
        }

        public static void InitDefaultAAndB(
            RebarBeamCutTypeInfo rebarBeamCutTypeInfo,
            out double a,
            out double b,
            out string aTitle)
        {
            a = 0;
            b = 0;
            aTitle = "A[mm]";
            switch (rebarBeamCutTypeInfo.Id)
            {
                case (int)RebarBeamCutType.Weld:
                case (int)RebarBeamCutType.Coupler:
                    a = 20;
                    b = 0;
                    aTitle = "A[mm]";
                    break;
                case (int)RebarBeamCutType.LapLength:
                    a = 40;
                    b = 0;
                    aTitle = "A[d]";
                    break;
            }
        }
        public static List<int> GetRebarGroupCuts(int count)
        {
            var results = new List<int>();
            for (int i = 0; i < count; i++)
            {
                results.Add(i + 1);
            }
            return results;
        }
    }
}
