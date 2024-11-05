using Utils.BoundingBoxs;

namespace RevitDevelop.Utils.RevElements
{
    public class RevElement
    {
        public ElementId Id { get; set; }
        public Element Element { get; set; }
        public RevBoxElement BoxElement { get; set; }
        public RevElement(Element element)
        {
            Id = element.Id;
            Element = element;
            BoxElement = new RevBoxElement(element);
        }
    }
}
