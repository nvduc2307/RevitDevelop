using HcBimUtils;
using Utils.BoundingBoxs;
using Utils.Geometries;

namespace RevitDevelop.Tools.Rebars.InstallRebarBeamV2.models
{
    public class RebarBeam : ObservableObject
    {
        public int BeamId { get; set; }
        public string Name { get; set; }
        public string NameType { get; set; }
        public double BeamWidthMm { get; set; }
        public double BeamHeightMm { get; set; }
        public RebarBeamSectionStart RebarBeamSectionStart { get; set; }
        public RebarBeamSectionMid RebarBeamSectionMid { get; set; }
        public RebarBeamSectionEnd RebarBeamSectionEnd { get; set; }
        public BeamStressRule BeamStressRule { get; set; }
        public RebarBeam(RevBoxElement revBoxBeam)
        {
            BeamId = int.Parse(revBoxBeam.Id.ToString(), System.Globalization.NumberStyles.Number);
            Name = revBoxBeam.Element.Name;
            NameType = "";
            BeamWidthMm = GetBeamWidthMm(revBoxBeam, out double beamHeightMm);
            BeamHeightMm = beamHeightMm;
            BeamStressRule = new BeamStressRule()
            {
                Id = 0,
                Stress = new List<double> { 0.25, 0.5, 0.25 }
            };
        }
        public RebarBeam()
        {

        }
        private double GetBeamWidthMm(RevBoxElement revBoxBeam, out double beamHeightMm)
        {
            var result = 0.0;
            beamHeightMm = 0.0;
            try
            {
                var face = new FaceCustom(revBoxBeam.VTX, revBoxBeam.LineBox.Midpoint());
                var faceOXY = new FaceCustom(revBoxBeam.VTZ, revBoxBeam.LineBox.Midpoint());
                var faceOXZ = new FaceCustom(revBoxBeam.VTY, revBoxBeam.LineBox.Midpoint());
                var faceOYZ = new FaceCustom(revBoxBeam.VTX, revBoxBeam.LineBox.Midpoint());

                var p1OYZ = revBoxBeam.LineBox.GetEndPoint(0).RayPointToFace(revBoxBeam.VTX, faceOYZ);
                var p2OYZ = revBoxBeam.LineBox.GetEndPoint(1).RayPointToFace(revBoxBeam.VTX, faceOYZ);

                var p1OXZ = p1OYZ.RayPointToFace(revBoxBeam.VTY, faceOXZ);
                var p2OXZ = p2OYZ.RayPointToFace(revBoxBeam.VTY, faceOXZ);

                var p1OXY = p1OYZ.RayPointToFace(revBoxBeam.VTZ, faceOXY);
                var p2OXY = p2OYZ.RayPointToFace(revBoxBeam.VTZ, faceOXY);

                result = Math.Round(p1OXY.Distance(p2OXY).FootToMm(), 0);
                beamHeightMm = Math.Round(p1OXZ.Distance(p2OXZ).FootToMm(), 0);
            }
            catch (Exception)
            {
            }
            return result;
        }
        public static void ResetActionChange(RebarBeam rebarBeam)
        {
            if (rebarBeam.RebarBeamSectionStart != null)
            {
                rebarBeam.RebarBeamSectionStart.RebarBeamTop.RebarBeamTopLevel1.QuantityChange = null;
                rebarBeam.RebarBeamSectionStart.RebarBeamTop.RebarBeamTopLevel2.QuantityChange = null;
                rebarBeam.RebarBeamSectionStart.RebarBeamTop.RebarBeamTopLevel3.QuantityChange = null;

                rebarBeam.RebarBeamSectionStart.RebarBeamBot.RebarBeamBotLevel1.QuantityChange = null;
                rebarBeam.RebarBeamSectionStart.RebarBeamBot.RebarBeamBotLevel2.QuantityChange = null;
                rebarBeam.RebarBeamSectionStart.RebarBeamBot.RebarBeamBotLevel3.QuantityChange = null;

                rebarBeam.RebarBeamSectionStart.RebarBeamSideBar.QuantitySideChange = null;
                rebarBeam.RebarBeamSectionStart.RebarBeamTop.RebarGroupTypeChange = null;
                rebarBeam.RebarBeamSectionStart.RebarBeamBot.RebarGroupTypeChange = null;
            }
            if (rebarBeam.RebarBeamSectionMid != null)
            {
                rebarBeam.RebarBeamSectionMid.RebarBeamTop.RebarBeamTopLevel1.QuantityChange = null;
                rebarBeam.RebarBeamSectionMid.RebarBeamTop.RebarBeamTopLevel2.QuantityChange = null;
                rebarBeam.RebarBeamSectionMid.RebarBeamTop.RebarBeamTopLevel3.QuantityChange = null;

                rebarBeam.RebarBeamSectionMid.RebarBeamBot.RebarBeamBotLevel1.QuantityChange = null;
                rebarBeam.RebarBeamSectionMid.RebarBeamBot.RebarBeamBotLevel2.QuantityChange = null;
                rebarBeam.RebarBeamSectionMid.RebarBeamBot.RebarBeamBotLevel3.QuantityChange = null;

                rebarBeam.RebarBeamSectionMid.RebarBeamSideBar.QuantitySideChange = null;
                rebarBeam.RebarBeamSectionMid.RebarBeamTop.RebarGroupTypeChange = null;
                rebarBeam.RebarBeamSectionMid.RebarBeamBot.RebarGroupTypeChange = null;
            }
            if (rebarBeam.RebarBeamSectionEnd != null)
            {
                rebarBeam.RebarBeamSectionEnd.RebarBeamTop.RebarBeamTopLevel1.QuantityChange = null;
                rebarBeam.RebarBeamSectionEnd.RebarBeamTop.RebarBeamTopLevel2.QuantityChange = null;
                rebarBeam.RebarBeamSectionEnd.RebarBeamTop.RebarBeamTopLevel3.QuantityChange = null;

                rebarBeam.RebarBeamSectionEnd.RebarBeamBot.RebarBeamBotLevel1.QuantityChange = null;
                rebarBeam.RebarBeamSectionEnd.RebarBeamBot.RebarBeamBotLevel2.QuantityChange = null;
                rebarBeam.RebarBeamSectionEnd.RebarBeamBot.RebarBeamBotLevel3.QuantityChange = null;

                rebarBeam.RebarBeamSectionEnd.RebarBeamSideBar.QuantitySideChange = null;
                rebarBeam.RebarBeamSectionEnd.RebarBeamTop.RebarGroupTypeChange = null;
                rebarBeam.RebarBeamSectionEnd.RebarBeamBot.RebarGroupTypeChange = null;
            }
        }
    }
}
