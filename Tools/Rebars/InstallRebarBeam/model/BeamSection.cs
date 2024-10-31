namespace RevitDevelop.Tools.Rebars.InstallRebarBeam.model
{
    public class StartBeamSection : BeamSection
    {
        public StartBeamSection(TopRebar topRebar, BottomRebar bottomRebar, Stirrup stirrup, SideRebar sideRebar) : base(topRebar, bottomRebar, stirrup, sideRebar)
        {
            Id = 0;
            SectionLocationRebarBeam = SectionLocationRebarBeam.StartSection;
            UpdateSectionLocationRebarBeam();
        }
    }

    public class MiddleBeamSection : BeamSection
    {
        public MiddleBeamSection(TopRebar topRebar, BottomRebar bottomRebar, Stirrup stirrup, SideRebar sideRebar) : base(topRebar, bottomRebar, stirrup, sideRebar)
        {
            Id = 1;
            SectionLocationRebarBeam = SectionLocationRebarBeam.MiddleSection;
            UpdateSectionLocationRebarBeam();
        }
    }

    public class EndBeamSection : BeamSection
    {
        public EndBeamSection(TopRebar topRebar, BottomRebar bottomRebar, Stirrup stirrup, SideRebar sideRebar) : base(topRebar, bottomRebar, stirrup, sideRebar)
        {
            Id = 2;
            SectionLocationRebarBeam = SectionLocationRebarBeam.EndSection;
            UpdateSectionLocationRebarBeam();
        }
    }

    public abstract class BeamSection : ObservableObject
    {
        private BeamInfo _beamInfo;
        public int Id { get; set; }
        public SectionLocationRebarBeam SectionLocationRebarBeam { get; set; }
        public TopRebar TopRebar { get; set; }
        public BottomRebar BottomRebar { get; set; }
        public Stirrup Stirrup { get; set; }
        public SideRebar SideRebar { get; set; }
        public BeamInfo BeamInfo
        {
            get => _beamInfo;
            set
            {
                _beamInfo = value;
                OnPropertyChanged();
                if (_beamInfo != null)
                {
                    TopRebar.BeamInfo = _beamInfo;
                    BottomRebar.BeamInfo = _beamInfo;
                    SideRebar.BeamInfo = _beamInfo;
                }
            }
        }
        public BeamSection(TopRebar topRebar, BottomRebar bottomRebar, Stirrup stirrup, SideRebar sideRebar)
        {
            TopRebar = topRebar;
            BottomRebar = bottomRebar;
            Stirrup = stirrup;
            SideRebar = sideRebar;

        }
        public void UpdateSectionLocationRebarBeam()
        {
            Update(TopRebar.RebarClass1);
            Update(TopRebar.RebarClass2);
            Update(TopRebar.RebarClass3);

            Update(BottomRebar.RebarClass1);
            Update(BottomRebar.RebarClass2);
            Update(BottomRebar.RebarClass3);

            Update(Stirrup.MainStirrup);
            Update(Stirrup.TieMain);
            Update(Stirrup.TieSide);
            Update(Stirrup.TieSub);

            Update(SideRebar);

            void Update(RebarClass rebarClass)
            {
                if (rebarClass != null)
                {
                    foreach (var item in rebarClass.RebarBeamInfos)
                    {
                        item.SectionLocationRebarBeam = SectionLocationRebarBeam;
                    }
                }
            }
        }
    }
}
