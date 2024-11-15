using Utils.BoundingBoxs;

namespace RevitDevelop.Tools.Rebars.InstallRebarBeamV2.models
{
    public class RebarBeam : ObservableObject
    {
        public int BeamId { get; set; }
        public string Name { get; set; }
        public double BeamWidthMm { get; set; }
        public double BeamHeightMm { get; set; }
        public RebarBeamSectionStart RebarBeamSectionStart { get; set; }
        public RebarBeamSectionMid RebarBeamSectionMid { get; set; }
        public RebarBeamSectionEnd RebarBeamSectionEnd { get; set; }
        public RebarBeam(RevBoxElement revBoxBeam)
        {
            BeamId = int.Parse(revBoxBeam.Id.ToString(), System.Globalization.NumberStyles.Number);
            Name = revBoxBeam.Element.Name;
            BeamWidthMm = GetBeamWidthMm();
            BeamHeightMm = GetBeamHeightMm();
        }
        private double GetBeamWidthMm()
        {
            var result = 0.0;
            return result;
        }
        private double GetBeamHeightMm()
        {
            var result = 0.0;
            return result;
        }
    }
}
