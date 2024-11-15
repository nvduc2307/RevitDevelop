namespace RevitDevelop.Tools.Rebars.InstallRebarBeamV2.models
{
    public abstract class RebarBeamSection
    {
        public int RebarBeamSectionType { get; set; }
        public RebarBeamTop RebarBeamTop { get; set; }
        public RebarBeamBot RebarBeamBot { get; set; }
        public RebarBeamSideBar RebarBeamSideBar { get; set; }
        public RebarBeamStirrup RebarBeamStirrup { get; set; }
    }

    public class RebarBeamSectionStart : RebarBeamSection { }
    public class RebarBeamSectionMid : RebarBeamSection { }
    public class RebarBeamSectionEnd : RebarBeamSection { }
}
