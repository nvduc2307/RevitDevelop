using RevitDevelop.Utils.RevElements.RevAssemblies;

namespace RevitDevelop.Utils.RevElements
{
    public class RevBeam : RevElementGeneric<FamilyInstance>
    {
        public RevBeam(Element element) : base(element)
        {
        }

        public override RevAssemblyType GetRevAssemblyType()
        {
            throw new NotImplementedException();
        }

        public override RevElementType GetRevElementType()
        {
            throw new NotImplementedException();
        }
    }
}
