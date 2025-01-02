namespace RevitDevelop.Utils.RevElements
{
    public static class RevElementUtils
    {
        public static Element CreateHost(this Document document, BuiltInCategory builtInCategory)
        {
            return DirectShape.CreateElement(document, new ElementId(builtInCategory));
        }
    }
}
