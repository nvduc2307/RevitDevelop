namespace RevitDevelop.Tools.Rebars.SettingRuleRebarStandards.models
{
    public class SettingRuleRebarGrade : ObservableObject
    {
        private List<SettingLapLengthAndDevelopRebarRule> _rules;
        public int Id { get; set; }
        public string Grade { get; set; }
        public List<SettingLapLengthAndDevelopRebarRule> Rules
        {
            get => _rules;
            set
            {
                _rules = value;
                OnPropertyChanged();
            }
        }
    }
    public class SettingLapLengthAndDevelopRebarRule : ObservableObject
    {
        private int _l1;
        private int _l2;
        private int _l3Frame;
        private int _l3Slab;
        private int _l1h;
        private int _l2h;
        private int _l3hFrame;
        private int _l3hSlab;
        private int _la;
        private int _lb;

        public int Id { get; set; }
        public string Grade { get; set; }
        public string Name { get; set; }
        public double Diameter { get; set; }
        public int L1
        {
            get => _l1;
            set
            {
                _l1 = value;
                OnPropertyChanged();
            }
        }
        public int L2
        {
            get => _l2;
            set
            {
                _l2 = value;
                OnPropertyChanged();
            }
        }
        public int L3Frame
        {
            get => _l3Frame;
            set
            {
                _l3Frame = value;
                OnPropertyChanged();
            }
        }
        public int L3Slab
        {
            get => _l3Slab;
            set
            {
                _l3Slab = value;
                OnPropertyChanged();
            }
        }
        public int L1h
        {
            get => _l1h;
            set
            {
                _l1h = value;
                OnPropertyChanged();
            }
        }
        public int L2h
        {
            get => _l2h;
            set
            {
                _l2h = value;
                OnPropertyChanged();
            }
        }
        public int L3hFrame
        {
            get => _l3hFrame;
            set
            {
                _l3hFrame = value;
                OnPropertyChanged();
            }
        }
        public int L3hSlab
        {
            get => _l3hSlab;
            set
            {
                _l3hSlab = value;
                OnPropertyChanged();
            }
        }
        public int La
        {
            get => _la;
            set
            {
                _la = value;
                OnPropertyChanged();
            }
        }
        public int Lb
        {
            get => _lb;
            set
            {
                _lb = value;
                OnPropertyChanged();
            }
        }
    }
    public enum GradeRebarType
    {
        SD259A = 0,
        SD259B = 1,
        SD345 = 2,
        SD390 = 3,
        SD490 = 4,
    }
    public enum LapType
    {
        LapLength = 1,
        Coupler = 2,
        Weld = 3
    }
    public enum LapStyle
    {
        OneHundredPercen = 1, //100%
        FityPercen = 2, //50%
    }
}
