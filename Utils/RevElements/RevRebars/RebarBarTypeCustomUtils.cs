namespace RevitDevelop.Utils.RevElements.RevRebars
{
    public static class RebarBarTypeCustomUtils
    {
        public static RebarBarTypeCustom GetRebarBarTypeCustom(string name, List<RebarBarTypeCustom> rebarBarTypeCustoms)
        {
            RebarBarTypeCustom result = null;
            try
            {
                result = rebarBarTypeCustoms.FirstOrDefault(x => x.NameStyle == name);

            }
            catch (Exception)
            {
                result = null;
            }
            return result;
        }
    }
}
