namespace RevitDevelop.AutoCutRebarBeamV2.models
{
    public class RebarBeamCutTypeInfo
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Image { get; set; }
        public RebarBeamCutTypeInfo(RebarBeamCutType rebarBeamCutType, string name = "")
        {
            Id = (int)rebarBeamCutType;
            Name = string.IsNullOrEmpty(name) ? rebarBeamCutType.ToString() : name;
        }
    }
    public enum RebarBeamCutType
    {
        Weld = 0,
        Coupler = 1,
        LapLength = 2,
    }
}
