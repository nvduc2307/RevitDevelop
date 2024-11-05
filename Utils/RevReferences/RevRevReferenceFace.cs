namespace RevitDevelop.Utils.RevReferences
{
    public class RevRevReferenceFace : RevReferenceElement
    {
        public RevRevReferenceFace(PlanarFace revElement, Element host, Document document)
        {
            RevElement = revElement;
            Id = revElement.Id;
            Host = host;
            var content = host.UniqueId + CONTENT_REFERENCE + revElement.Reference.ConvertToStableRepresentation(document);
            Reference = Reference.ParseFromStableRepresentation(document, content);
        }
    }
}
