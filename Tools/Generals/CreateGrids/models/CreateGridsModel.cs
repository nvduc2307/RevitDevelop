using HcBimUtils;
using RevitDevelop.Utils.RevGrids;
using Utils.Geometries;

namespace RevitDevelop.Tools.Generals.CreateGrids.models
{
    public class CreateGridsModel : ObservableObject
    {
        private ElementInstances _elementInstances;
        private string _gridXNames;
        private string _gridYNames;
        private string _gridXValues;
        private string _gridYValues;
        public string GridXNames
        {
            get => _gridXNames;
            set
            {
                _gridXNames = value;
                OnPropertyChanged();
            }
        }
        public string GridYNames
        {
            get => _gridYNames;
            set
            {
                _gridYNames = value;
                OnPropertyChanged();
            }
        }
        public string GridXValues
        {
            get => _gridXValues;
            set
            {
                _gridXValues = value;
                OnPropertyChanged();
            }
        }
        public string GridYValues
        {
            get => _gridYValues;
            set
            {
                _gridYValues = value;
                OnPropertyChanged();
            }
        }
        public List<RevGrid> GridXs { get; set; }
        public List<RevGrid> GridYs { get; set; }
        public CreateGridsModel(ElementInstances elementInstances)
        {
            _elementInstances = elementInstances;
            GridXs = _elementInstances.Grids.Count != 0 ?
                _elementInstances.Grids
                .FirstOrDefault()
                .Select(x => new RevGrid(x))
                .ToList()
                : new List<RevGrid>();
            GridYs = _elementInstances.Grids.Count != 0
                ? _elementInstances.Grids
                .LastOrDefault()
                .Select(x => new RevGrid(x))
                .ToList()
                : [];
            GridXNames = GridXs.Count != 0 ? GridXs.Select(x => x.Name).Aggregate((a, b) => $"{a} {b}") : "";
            GridYNames = GridXs.Count != 0 ? GridYs.Select(x => x.Name).Aggregate((a, b) => $"{a} {b}") : "";
            GridXValues = GetGridValues(GridXs);
            GridYValues = GetGridValues(GridYs);
        }
        private static string GetGridValues(List<RevGrid> grids)
        {
            var result = "";
            try
            {
                var gtg = grids.FirstOrDefault();
                var c = 0;
                foreach (var gr in grids)
                {
                    var gValue = gr.Curve.Midpoint().RayPointToFace(gtg.Face.Normal, gtg.Face).Distance(gr.Curve.Midpoint()).FootToMm();
                    result += c == 0 ? $"{gValue}" : $" {gValue}";
                    gtg = gr;
                    c++;
                }
            }
            catch (Exception)
            {
            }
            return result;
        }
    }
}
