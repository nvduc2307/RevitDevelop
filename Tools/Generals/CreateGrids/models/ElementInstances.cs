using HcBimUtils.DocumentUtils;
using RevitDevelop.Utils.CompareElement;
using Utils.FilterElements;

namespace RevitDevelop.Tools.Generals.CreateGrids.models
{
    public class ElementInstances
    {
        public List<List<Grid>> Grids { get; set; }
        public List<Level> Levels { get; set; }
        public ElementInstances()
        {
            Grids = AC.Document.GetElementsFromClass<Grid>()
                .GroupBy(x => x, new CompareGridDirection())
                .OrderBy(x => x.FirstOrDefault().Name)
                .Select(x => x.OrderBy(y => y.Name).ToList())
                .ToList();
            Levels = AC.Document.GetElementsFromClass<Level>();
        }
    }
}
