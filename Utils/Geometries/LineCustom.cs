using HcBimUtils;

namespace Utils.Geometries
{
    public class LineCustom
    {
        public XYZ Direction { get; set; }
        public XYZ BasePoint { get; set; }
        public Line LineBase { get; set; }
        public LineCustom(XYZ direction, XYZ basePoint)
        {
            Direction = direction;
            BasePoint = basePoint;
            LineBase = Line.CreateBound(BasePoint, BasePoint + Direction * 1000.MmToFoot());
        }
    }
}
