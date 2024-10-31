namespace RevitDevelop.Tools.Rebars.InstallRebarBeam.model
{
    public class BeamSetting : ObservableObject
    {
        private double _coverLeft;
        private double _coverTop;
        private double _coverRight;
        private double _coverBot;
        private double _distanceDevelopSideTop; // tinh theo duong kinh
        private double _distanceDevelopMiddleTop; // tinh theo duong kinh
        private double _distanceDevelopSideBot; // tinh theo duong kinh
        private double _distanceDevelopMiddleBot; // tinh theo duong kinh
        private double _distanceRebarToRebar; // khoang cach giua 2 lop thep chu
        private double _tyLeVungKeo; // tinh theo chieu dai dam
        private double _tyLeVungNen; // tinh theo chieu dai dam

        public double CoverLeft
        {
            get => _coverLeft;
            set
            {
                _coverLeft = value;
                OnPropertyChanged();
                if (UIElement.CanvasBaseSection != null && BeamInfo != null)
                {
                    BeamInfo.DrawRebarStirrupInCanvas(UIElement.CanvasBaseSection, BeamInfo);
                }
            }
        }
        public double CoverTop
        {
            get => _coverTop;
            set
            {
                _coverTop = value;
                OnPropertyChanged();
                if (UIElement.CanvasBaseSection != null && BeamInfo != null)
                {
                    BeamInfo.DrawRebarStirrupInCanvas(UIElement.CanvasBaseSection, BeamInfo);
                }
            }
        }
        public double CoverRight
        {
            get => _coverRight;
            set
            {
                _coverRight = value;
                OnPropertyChanged();
                if (UIElement.CanvasBaseSection != null && BeamInfo != null)
                {
                    BeamInfo.DrawRebarStirrupInCanvas(UIElement.CanvasBaseSection, BeamInfo);
                }
            }
        }
        public double CoverBot
        {
            get => _coverBot;
            set
            {
                _coverBot = value;
                OnPropertyChanged();
                if (UIElement.CanvasBaseSection != null && BeamInfo != null)
                {
                    BeamInfo.DrawRebarStirrupInCanvas(UIElement.CanvasBaseSection, BeamInfo);
                }
            }
        }
        public double DistanceDevelopSideTop
        {
            get => _distanceDevelopSideTop;
            set
            {
                _distanceDevelopSideTop = value;
                OnPropertyChanged();
            }
        }
        public double DistanceDevelopMiddleTop
        {
            get => _distanceDevelopMiddleTop;
            set
            {
                _distanceDevelopMiddleTop = value;
                OnPropertyChanged();
            }
        }
        public double DistanceDevelopSideBot
        {
            get => _distanceDevelopSideBot;
            set
            {
                _distanceDevelopSideBot = value;
                OnPropertyChanged();
            }
        }
        public double DistanceDevelopMiddleBot
        {
            get => _distanceDevelopMiddleBot;
            set
            {
                _distanceDevelopMiddleBot = value;
                OnPropertyChanged();
            }
        }
        public double DistanceRebarToRebar
        {
            get => _distanceRebarToRebar;
            set
            {
                _distanceRebarToRebar = value;
                OnPropertyChanged();
            }
        }
        public double TyLeVungKeo
        {
            get => _tyLeVungKeo;
            set
            {
                _tyLeVungKeo = value;
                OnPropertyChanged();
            }
        }
        public double TyLeVungNen
        {
            get => _tyLeVungNen;
            set
            {
                _tyLeVungNen = value;
                OnPropertyChanged();
            }
        }
        public UIElement UIElement { get; set; }
        public BeamInfo BeamInfo { get; set; }
        public BeamSetting(UIElement uIElement)
        {
            UIElement = uIElement;
            CoverLeft = 30;
            CoverTop = 30;
            CoverRight = 30;
            CoverBot = 30;
            DistanceDevelopSideTop = 15;
            DistanceDevelopMiddleTop = 20;
            DistanceDevelopSideBot = 15;
            DistanceDevelopMiddleBot = 20;
            DistanceRebarToRebar = 100;
            TyLeVungKeo = 0.25;
            TyLeVungNen = 0.5;
        }
    }
}
