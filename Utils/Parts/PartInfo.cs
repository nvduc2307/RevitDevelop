using HcBimUtils;
using Utils.FilterElements;

namespace Utils.Parts
{
    public class PartInfo
    {
        public const string PARAM_NAME_ZONE = "工区";
        private Document _document;
        public Part PartZone { get; set; }
        public Solid SolidZone { get; set; }
        public View ViewZone { get; set; }
        public string NameZone { get; set; }
        public string ParamNameZone { get; set; }
        public PartInfo(Document document, View viewZone, Part partZone, string paramNameZone)
        {
            _document = document;
            ViewZone = viewZone;
            PartZone = partZone;
            SolidZone = partZone?.GetSingleSolid();
            ParamNameZone = paramNameZone;
            NameZone = GetNameZone();

        }
        private string GetNameZone()
        {
            var result = "";
            try
            {
                if (PartZone == null) return result;
                result = PartZone.LookupParameter(ParamNameZone).AsValueString();
            }
            catch (Exception)
            {
            }

            return result;
        }

        public static IEnumerable<Part> GetPartWithParamInView(Document document, View view, string paramName)
        {
            var results = new List<Part>();
            try
            {
                results = document
                    .GetElementsFromCategory<Part>(view, BuiltInCategory.OST_Parts, false)
                    .Where(x => ParameterUtilities.HasParameter(x, paramName))
                    .Where(x => !string.IsNullOrEmpty(x.LookupParameter(paramName).AsValueString()))
                    .ToList();
            }
            catch (Exception)
            {
            }
            return results;
        }
    }
    public class PartInfoRef
    {
        public PartInfo PartInfo { get; set; }
        public double Length { get; set; }
    }
}
