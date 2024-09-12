namespace RevitDevelop.Utils.ParameterFilterElements
{
    public static class ParameterUtils
    {
        public static BuiltInParameter GetBuiltInParameter(this Parameter parameter)
        {
            return (BuiltInParameter)int.Parse(parameter.Id.ToString());
        }
    }
}
