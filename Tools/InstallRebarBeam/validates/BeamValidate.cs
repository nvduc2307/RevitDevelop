using HcBimUtils;
using Tools.InstallRebarBeam.model;
using Utils.Geometries;

namespace RIMT.InstallRebarBeam.validates
{
    public class BeamValidate
    {
        public const string MESSAGE_BEAM_ERROR = "選択された梁は無効です。";
        public static bool Check(List<FamilyInstance> beams)
        {
            if (beams.Count == 1) return true;
            var beamInfos = beams.Select(x => new BeamInfo(x)).ToList();
            var vtXFirst = beamInfos.First().VtX;
            var vtYFirst = beamInfos.First().VtY;
            var vtZFirst = beamInfos.First().VtZ;
            var planeHor = new FaceCustom(vtZFirst, beamInfos.First().MidBeam);
            var planeVer = new FaceCustom(vtYFirst, beamInfos.First().MidBeam);
            var heightFirst = beamInfos.First().HeightMm;
            var widthFirst = beamInfos.First().WidthMm;

            var checkDirection = !beamInfos.Any(x => !x.VtX.IsParallel(vtXFirst));
            var checkHeight = !beamInfos.Any(x =>
            {
                var distance = Math.Abs(x.HeightMm - heightFirst);
                return (distance - 45) > 0;
            });
            var checkWidth = !beamInfos.Any(x =>
            {
                var distance = Math.Abs(x.WidthMm - widthFirst);
                return (distance - 45) > 0;
            });
            var checkPlaneHor = !beamInfos.Any(x =>
            {
                var p = x.MidBeam.RayPointToFace(vtZFirst, planeHor);
                var distance = p.DistanceTo(x.MidBeam);
                return (distance - 10.MmToFoot()) > 0;
            });
            var checkPlaneVer = !beamInfos.Any(x =>
            {
                var p = x.MidBeam.RayPointToFace(vtYFirst, planeVer);
                var distance = p.DistanceTo(x.MidBeam);
                return (distance - 10.MmToFoot()) > 0;
            });

            var check = new List<bool>()
            {
                checkDirection, checkHeight, checkWidth, checkPlaneHor, checkPlaneVer
            };
            return check.Any(x => !x) ? false : true;
        }
    }
}
