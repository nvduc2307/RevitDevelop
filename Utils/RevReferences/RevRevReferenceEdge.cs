namespace RevitDevelop.Utils.RevReferences
{
    public class RevRevReferenceEdge : RevReferenceElement
    {
        public RevRevReferenceEdge(Edge revElement, Element host, Document document)
        {
            RevElement = revElement;
            Id = revElement.Id;
            Host = host;
            var content = host.UniqueId + CONTENT_REFERENCE + revElement.Reference.ConvertToStableRepresentation(document);
            Reference = Reference.ParseFromStableRepresentation(document, content);
        }
    }
}
