using Autodesk.Revit.DB.Structure;
using Utils.Messages;

namespace RevitDevelop.Tools.Rebars.InstallRebarBeam.model
{
    public class RebarBeamInfo : ObservableObject
    {
        private bool _isSelected;

        public int Id { get; set; } // vi tri thep
        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                _isSelected = value;
                OnPropertyChanged();
            }
        }
        public RebarBarType RebarBarType { get; set; } // duong kinh
        public TypeRebarBeam TypeRebarBeam { get; set; } // loai thep
        public GroupRebarBeam GroupRebarBeam { get; set; } // nhom thep
        public SectionLocationRebarBeam SectionLocationRebarBeam { get; set; } // vi tri mat cat

        public RebarBeamInfo(int id, RebarBarType rebarBarType)
        {
            Id = id;
            RebarBarType = rebarBarType;
        }
    }

    public abstract class RebarType : ObservableObject
    {
        private BeamInfo _beamInfo;
        private int _rebarClassSelected;
        private RebarClass _rebarClassActive;
        public TypeRebarBeam TypeRebarBeam { get; set; }
        public RebarClass1 RebarClass1 { get; set; }
        public RebarClass2 RebarClass2 { get; set; }
        public RebarClass3 RebarClass3 { get; set; }
        public int RebarClassSelected
        {
            get => _rebarClassSelected;
            set
            {
                _rebarClassSelected = value;
                OnPropertyChanged();
                UpdateRebarClassActive();
            }
        }
        public RebarClass RebarClassActive
        {
            get => _rebarClassActive;
            set
            {
                _rebarClassActive = value;
                OnPropertyChanged();
            }
        }
        public BeamInfo BeamInfo
        {
            get => _beamInfo;
            set
            {
                _beamInfo = value;
                OnPropertyChanged();
                if (_beamInfo != null)
                {
                    RebarClass1.BeamInfo = _beamInfo;
                    RebarClass2.BeamInfo = _beamInfo;
                    RebarClass3.BeamInfo = _beamInfo;
                }
            }
        }
        public RebarType(RebarClass1 rebarClass1, RebarClass2 rebarClass2, RebarClass3 rebarClass3)
        {
            RebarClass1 = rebarClass1;
            RebarClass2 = rebarClass2;
            RebarClass3 = rebarClass3;
        }
        public virtual void UpdateTypeRebarBeam()
        {
            Update(RebarClass1);
            Update(RebarClass2);
            Update(RebarClass3);
            void Update(RebarClass rebarClass)
            {
                if (rebarClass != null)
                {
                    foreach (var rebarBeamInfo in rebarClass.RebarBeamInfos)
                    {
                        rebarBeamInfo.TypeRebarBeam = TypeRebarBeam;
                    }
                }
            }
        }
        public virtual void UpdateRebarClassActive()
        {
            switch (RebarClassSelected)
            {
                case (int)GroupRebarBeam.Class1:
                    RebarClassActive = RebarClass1;
                    break;
                case (int)GroupRebarBeam.Class2:
                    RebarClassActive = RebarClass2;
                    break;
                case (int)GroupRebarBeam.Class3:
                    RebarClassActive = RebarClass3;
                    break;
            }
            OnPropertyChanged(nameof(RebarClassActive));
        }
    }

    public class TopRebar : RebarType
    {
        public TopRebar(RebarClass1 rebarClass1, RebarClass2 rebarClass2, RebarClass3 rebarClass3) : base(rebarClass1, rebarClass2, rebarClass3)
        {
            TypeRebarBeam = TypeRebarBeam.TopRebar;
            UpdateTypeRebarBeam();
        }
    }

    public class BottomRebar : RebarType
    {
        public BottomRebar(RebarClass1 rebarClass1, RebarClass2 rebarClass2, RebarClass3 rebarClass3) : base(rebarClass1, rebarClass2, rebarClass3)
        {
            TypeRebarBeam = TypeRebarBeam.BottomRebar;
            UpdateTypeRebarBeam();
        }
    }

    public abstract class RebarClass : ObservableObject
    {
        private int _quantity;
        private double _spacing;
        private RebarBarType _rebarBarType;

        public int Id { get; set; }
        public RebarBarType RebarBarType
        {
            get => _rebarBarType;
            set
            {
                _rebarBarType = value;
                OnPropertyChanged();
                UpdateRebarBeamInfos();
            }
        }
        public int Quantity
        {
            get => _quantity;
            set
            {
                if (value < 50)
                {
                    if (value >= 2) Update();
                    else
                    {
                        if (GroupRebarBeam == GroupRebarBeam.SideBar) Update();
                        else if (GroupRebarBeam == GroupRebarBeam.Class2 || GroupRebarBeam == GroupRebarBeam.Class3)
                        {
                            if (value == 0) Update();
                            else IO.ShowWarning("So luong phai lon hon 1");
                        }
                        else IO.ShowWarning("So luong phai lon hon 1");
                    }
                    void Update()
                    {
                        _quantity = value;
                        OnPropertyChanged();
                        UpdateRebarBeamInfos();
                        if (BeamInfo != null)
                        {
                            BeamInfo.DrawRebarMainInCanvas(BeamInfo.UIElement.CanvasBaseSection, BeamInfo);
                        }
                    }
                }
                else IO.ShowWarning("So luong phai nho hon 50");
            }
        }
        public double Spacing
        {
            get => _spacing;
            set
            {
                _spacing = value;
                OnPropertyChanged();
            }
        }
        public GroupRebarBeam GroupRebarBeam { get; set; }
        public List<RebarBeamInfo> RebarBeamInfos { get; set; }

        public BeamInfo BeamInfo { get; set; }

        public RebarClass(RebarBarType rebarBarType, int quantity, double spacing)
        {
            RebarBarType = rebarBarType;
            Quantity = quantity;
            Spacing = spacing;
        }

        public void UpdateRebarBeamInfos()
        {
            RebarBeamInfos = new List<RebarBeamInfo>();
            for (int i = 0; i < Quantity; i++)
            {
                RebarBeamInfos.Add(new RebarBeamInfo(i, RebarBarType));
            }
        }

        public void UpdateGroupRebarBeam()
        {
            foreach (var rebarBeamInfo in RebarBeamInfos)
            {
                rebarBeamInfo.GroupRebarBeam = GroupRebarBeam;
            }
        }

        public static RebarBarType GetRebarBarTypeFromName(List<RebarBarType> rebarBarTypes, string nameBarType)
        {
            var result = rebarBarTypes.FirstOrDefault(x => x.Name == nameBarType);
            return result != null ? result : rebarBarTypes.FirstOrDefault();
        }
    }

    public class RebarClass1 : RebarClass
    {
        public RebarClass1(RebarBarType rebarBarType, int quantity, double spacing) : base(rebarBarType, quantity, spacing)
        {
            Id = 0;
            GroupRebarBeam = GroupRebarBeam.Class1;
            UpdateGroupRebarBeam();
        }
    }

    public class RebarClass2 : RebarClass
    {
        public RebarClass2(RebarBarType rebarBarType, int quantity, double spacing) : base(rebarBarType, quantity, spacing)
        {
            Id = 1;
            GroupRebarBeam = GroupRebarBeam.Class2;
            UpdateGroupRebarBeam();
        }
    }

    public class RebarClass3 : RebarClass
    {
        public RebarClass3(RebarBarType rebarBarType, int quantity, double spacing) : base(rebarBarType, quantity, spacing)
        {
            Id = 2;
            GroupRebarBeam = GroupRebarBeam.Class3;
            UpdateGroupRebarBeam();
        }
    }

    public class SideRebar : RebarClass
    {
        public SideRebar(RebarBarType rebarBarType, int quantity, double spacing) : base(rebarBarType, quantity, spacing)
        {
            Id = 3;
            GroupRebarBeam = GroupRebarBeam.SideBar;
            UpdateGroupRebarBeam();
        }
    }

    public class Stirrup : ObservableObject
    {
        private int _rebarClassSelected;
        private StirrupInfo _rebarClassActive;
        public TypeRebarBeam TypeRebarBeam { get; set; }
        public MainStirrup MainStirrup { get; set; }
        public TieMain TieMain { get; set; }
        public TieSide TieSide { get; set; }
        public TieSub TieSub { get; set; }
        public int RebarClassSelected
        {
            get => _rebarClassSelected;
            set
            {
                _rebarClassSelected = value;
                OnPropertyChanged();
                UpdateRebarClassActive();
            }
        }
        public StirrupInfo RebarClassActive
        {
            get => _rebarClassActive;
            set
            {
                _rebarClassActive = value;
                OnPropertyChanged();
            }
        }

        public Stirrup(MainStirrup mainStirrup, TieMain tieMain, TieSide tieSide, TieSub tieSub)
        {
            TypeRebarBeam = TypeRebarBeam.Stirrup;
            MainStirrup = mainStirrup;
            TieMain = tieMain;
            TieSide = tieSide;
            TieSub = tieSub;
            UpdateTypeRebarBeam();
        }
        public virtual void UpdateTypeRebarBeam()
        {
            Update(MainStirrup);
            Update(TieMain);
            Update(TieSide);
            Update(TieSub);
            void Update(RebarClass rebarClass)
            {
                if (rebarClass != null)
                {
                    foreach (var rebarBeamInfo in rebarClass.RebarBeamInfos)
                    {
                        rebarBeamInfo.TypeRebarBeam = TypeRebarBeam;
                    }
                }
            }
        }
        public virtual void UpdateRebarClassActive()
        {
            switch (RebarClassSelected)
            {
                case 0:
                    RebarClassActive = MainStirrup;
                    break;
                case 1:
                    RebarClassActive = TieMain;
                    break;
                case 2:
                    RebarClassActive = TieSide;
                    break;
                case 3:
                    RebarClassActive = TieSub;
                    break;
            }
            OnPropertyChanged(nameof(RebarClassActive));
        }

        public static MainStirrupType GetMainStirrupType(int typeValue)
        {
            var result = MainStirrupType.Type1;
            switch (typeValue)
            {
                case 0:
                    result = MainStirrupType.Type1;
                    break;
                case 1:
                    result = MainStirrupType.Type2;
                    break;
            }
            return result;
        }
        public static TieType GetTieType(int typeValue)
        {
            var result = TieType.Type1;
            switch (typeValue)
            {
                case 0:
                    result = TieType.Type1;
                    break;
                case 1:
                    result = TieType.Type2;
                    break;
            }
            return result;
        }
    }

    public class StirrupInfo : RebarClass
    {
        private object _stirrupType;
        public object StirrupType
        {
            get => _stirrupType;
            set
            {
                _stirrupType = value;
                OnPropertyChanged();
            }
        }
        public List<object> StirrupTypes { get; set; }
        public bool IsInstall { get; set; }
        public bool IsSelected { get; set; }
        public StirrupInfo(RebarBarType rebarBarType, int quantity, double spacing, object stirrupType) : base(rebarBarType, quantity, spacing)
        {
            StirrupType = stirrupType;
        }
    }

    public class MainStirrup : StirrupInfo
    {
        public MainStirrup(RebarBarType rebarBarType, int quantity, double spacing, MainStirrupType mainStirrupType) : base(rebarBarType, quantity, spacing, mainStirrupType)
        {
            Id = 4;
            GroupRebarBeam = GroupRebarBeam.MainStirrup;
            StirrupType = mainStirrupType;
            StirrupTypes = new List<object>() { MainStirrupType.Type1, MainStirrupType.Type2 };
            UpdateGroupRebarBeam();
        }
    }

    public class TieMain : StirrupInfo
    {
        public TieMain(RebarBarType rebarBarType, int quantity, double spacing, TieType tieType) : base(rebarBarType, quantity, spacing, tieType)
        {
            Id = 5;
            GroupRebarBeam = GroupRebarBeam.TieMain;
            StirrupType = tieType;
            StirrupTypes = new List<object>() { TieType.Type1, TieType.Type2 };
            UpdateGroupRebarBeam();
        }
    }

    public class TieSide : StirrupInfo
    {
        public TieSide(RebarBarType rebarBarType, int quantity, double spacing, TieType tieType) : base(rebarBarType, quantity, spacing, tieType)
        {
            Id = 6;
            GroupRebarBeam = GroupRebarBeam.TieSide;
            StirrupType = tieType;
            StirrupTypes = new List<object>() { TieType.Type1, TieType.Type2 };
            UpdateGroupRebarBeam();
        }
    }

    public class TieSub : StirrupInfo
    {
        public TieSub(RebarBarType rebarBarType, int quantity, double spacing, TieType tieType) : base(rebarBarType, quantity, spacing, tieType)
        {
            Id = 7;
            GroupRebarBeam = GroupRebarBeam.TieSub;
            StirrupType = tieType;
            StirrupTypes = new List<object>() { TieType.Type1, TieType.Type2 };
            UpdateGroupRebarBeam();
        }
    }

    public enum TypeRebarBeam
    {
        TopRebar = 0,
        BottomRebar = 1,
        Stirrup = 2,
    }
    public enum GroupRebarBeam
    {
        Class1 = 0, // thép chủ lớp 1
        Class2 = 1, // thép chủ lớp 2
        Class3 = 2, // thép chủ lớp 3
        SideBar = 3, // thép chống phình
        MainStirrup = 4, // đai chính
        TieMain = 5, // đai phụ dùng cho thép top kết nối thép bot
        TieSide = 6, // đai phụ dùng cho thép chống phình
        TieSub = 7 // đai dùng cho lớp thép class 2, class 3
    }
    public enum MainStirrupType
    {
        Type1 = 0, // 1 đai
        Type2 = 1, // 1 đai u + 1 đai c
    }
    public enum TieType
    {
        Type1 = 0, // 2 đầu hook bẻ 135
        Type2 = 1, // 1 đầu 135, một đầu 90
    }
    public enum SectionLocationRebarBeam
    {
        StartSection = 0,
        MiddleSection = 1,
        EndSection = 2,
    }
}
