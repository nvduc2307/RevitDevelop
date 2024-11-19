using HcBimUtils;
using Utils.BoundingBoxs;
using Utils.CompareElement;
using Utils.Geometries;
using Utils.RevPoints;

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
            BeamWidthMm = GetBeamWidthMm(revBoxBeam, out double beamHeightMm);
            BeamHeightMm = beamHeightMm;
        }
        private double GetBeamWidthMm(RevBoxElement revBoxBeam, out double beamHeightMm)
        {
            var result = 0.0;
            beamHeightMm = 0.0;
            try
            {
                var face = new FaceCustom(revBoxBeam.VTX, revBoxBeam.LineBox.Midpoint());
                var ps = new List<XYZ>() {
                    revBoxBeam.BoxElementPoint.P1,
                    revBoxBeam.BoxElementPoint.P2,
                    revBoxBeam.BoxElementPoint.P3,
                    revBoxBeam.BoxElementPoint.P4,
                    revBoxBeam.BoxElementPoint.P5,
                    revBoxBeam.BoxElementPoint.P6,
                    revBoxBeam.BoxElementPoint.P7,
                    revBoxBeam.BoxElementPoint.P8,
                };
                var psf = ps
                    .Select(x => x.RayPointToFace(revBoxBeam.VTX, face))
                    .Distinct(new ComparePoint())
                    .ToList();
                if (psf.Count != 4) throw new Exception();
                var curves = psf.PointsToCurves();
                result = curves.Max(x => x.Length).FootToMm();
                beamHeightMm = curves.Min(x => x.Length).FootToMm();
            }
            catch (Exception)
            {
            }
            return result;
        }
    }
}
