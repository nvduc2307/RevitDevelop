using RevitDevelop.Utils.RevPoints;

namespace RevitDevelop.Utils.Revpolygons
{
    public class RevPolygon
    {
        public int Id { get; set; }
        public List<RevPoint> Shape { get; set; }
    }
}
