﻿using RevitDevelop.Utils.RevElements.RevAssemblies;
using Utils.BoundingBoxs;

namespace RevitDevelop.Utils.RevElements
{
    public class RevElement
    {
        public ElementId Id { get; set; }
        public Element Element { get; set; }
        public RevBoxElement BoxElement { get; set; }
        public RevElementType RevElementType { get; set; }
        public RevAssemblyType RevAssemblyType { get; set; }
        public List<Element> ElementSubs { get; set; }
        public RevElement(Element element)
        {
            Id = element.Id;
            Element = element;
            BoxElement = new RevBoxElement(element);
        }
    }
}
