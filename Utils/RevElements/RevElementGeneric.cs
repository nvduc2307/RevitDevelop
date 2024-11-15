using RevitDevelop.Utils.RevElements.RevAssemblies;
using Utils.BoundingBoxs;

namespace RevitDevelop.Utils.RevElements
{
    public abstract class RevElementGeneric<T>
    {
        public ElementId Id { get; set; }
        public Element Element { get; set; }
        public RevBoxElement BoxElement { get; set; }
        public RevElementType RevElementType { get; set; }
        public RevAssemblyType RevAssemblyType { get; set; }
        public List<T> ElementSubs { get; set; }
        protected RevElementGeneric(Element element)
        {
            Id = element.Id;
            Element = element;
            BoxElement = new RevBoxElement(element);
        }
        public abstract RevElementType GetRevElementType();
        public abstract RevAssemblyType GetRevAssemblyType();
    }
}
