namespace RevitDevelop.Utils.RevReferences
{
    /// <summary>
    /// RevElement can be a Edge, PanalFace
    /// </summary>
    public abstract class RevReferenceElement
    {
        public const string CONTENT_REFERENCE = ":0:INSTANCE:";
        public int Id { get; set; }
        public Element Host { get; set; }
        public object RevElement { get; set; }
        public Reference Reference { get; set; }
    }
}
