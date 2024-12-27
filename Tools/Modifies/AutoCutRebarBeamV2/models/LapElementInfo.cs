namespace RevitDevelop.AutoCutRebarBeamV2.models
{
    public class LapElementInfo
    {
        public int Hostid { get; }
        public int LapType { get; set; }
        public LapElementInfo(int hostid, int lapType)
        {
            Hostid = hostid;
            LapType = lapType;
        }
    }
}
