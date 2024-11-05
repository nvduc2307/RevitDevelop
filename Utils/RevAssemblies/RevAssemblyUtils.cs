using HcBimUtils.DocumentUtils;

namespace RevitDevelop.Utils.RevAssemblies
{
    public static class RevAssemblyUtils
    {
        public static RevAssemblyType GetAssemblyType(this Document document, AssemblyInstance assemblyInstance)
        {
            var result = RevAssemblyType.InValid;
            try
            {
                if (assemblyInstance == null) return RevAssemblyType.InValid;
                var members = assemblyInstance.GetMemberIds().Select(x => document.GetElement(x));

                var isRebar = members.Any(x => x.Category.ToBuiltinCategory() == BuiltInCategory.OST_Rebar);
                var isNotRebar = members.Any(x => x.Category.ToBuiltinCategory() != BuiltInCategory.OST_Rebar);

                var isBeam = members.Any(x => x.Category.ToBuiltinCategory() == BuiltInCategory.OST_StructuralFraming);
                var isNotBeam = members.Any(x => x.Category.ToBuiltinCategory() != BuiltInCategory.OST_StructuralFraming);

                var isColumn = members.Any(x => x.Category.ToBuiltinCategory() == BuiltInCategory.OST_StructuralColumns);
                var isNotColumn = members.Any(x => x.Category.ToBuiltinCategory() != BuiltInCategory.OST_StructuralColumns);

                var isFloor = members.Any(x => x.Category.ToBuiltinCategory() == BuiltInCategory.OST_Floors);
                var isNotFloor = members.Any(x => x.Category.ToBuiltinCategory() != BuiltInCategory.OST_Floors);

                if (isRebar == true && isNotRebar == false) result = RevAssemblyType.Rebar;
                if (isBeam == true && isNotBeam == false) result = RevAssemblyType.Beam;
                if (isRebar == true && isNotColumn == false) result = RevAssemblyType.Column;

                return result;
            }
            catch (Exception)
            {
                return RevAssemblyType.InValid;
            }
        }
        public static IEnumerable<T> GetElementInAssembly<T>(this Document document, AssemblyInstance assemblyInstance, BuiltInCategory builtInCategory)
        {
            var results = new List<T>();
            try
            {
                var members = assemblyInstance.GetMemberIds().Select(x => document.GetElement(x));
                results = members
                    .Where(x => x.Category.ToBuiltinCategory() == builtInCategory)
                    .Where(x => x is T)
                    .Cast<T>()
                    .ToList();
            }
            catch (Exception)
            {
            }
            return results;
        }

    }
}
