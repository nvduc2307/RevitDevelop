using Autodesk.Revit.UI;
using RevitDevelop.Utils.RevAssemblies;
using Utils.BoundingBoxs;

namespace Utils.RevAssemblies
{
    public class RevAssembly
    {
        private UIDocument _uiDocument;
        private Document _document;
        public AssemblyInstance AssemblyInstance { get; private set; }
        public RevAssemblyType AssemblyType { get; private set; }
        public RevBoxElement RevBoxElement { get; private set; }
        public IEnumerable<Element> Elements { get; private set; }
        public RevAssembly(UIDocument uiDocument, AssemblyInstance assemblyInstance)
        {
            _uiDocument = uiDocument;
            _document = _uiDocument.Document;
            AssemblyInstance = assemblyInstance;
            RevBoxElement = new RevBoxElement(assemblyInstance);
            AssemblyType = _document.GetAssemblyType(AssemblyInstance);
            Elements = GetElements();
        }
        private IEnumerable<Element> GetElements()
        {
            var results = new List<Element>();
            try
            {
                results = AssemblyInstance
                    .GetMemberIds()
                    .Select(x => x.ToElement(_document))
                    .Where(x => x != null)
                    .ToList();
            }
            catch (Exception)
            {
            }
            return results;
        }
    }
}
