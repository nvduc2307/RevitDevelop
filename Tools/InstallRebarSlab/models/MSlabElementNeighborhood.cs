using HcBimUtils;
using HcBimUtils.GeometryUtils.Geometry;
using Utils.CompareElement;

namespace RIMT.InstallRebarSlab.models
{
    public class MSlabElementNeighborhood : ObservableObject
    {
        public List<BuiltInCategory> BuiltInCategoriesOfElementsAround = new List<BuiltInCategory>() {
            BuiltInCategory.OST_StructuralFraming,
            BuiltInCategory.OST_StructuralFoundation,
            BuiltInCategory.OST_StructuralColumns,
            BuiltInCategory.OST_Walls,
        };
        public Element Element { get; set; }
        public List<Solid> Solids { get; set; }
        public List<XYZ> Points { get; set; }
        public List<XYZ> PointsOnFloorPlan { get; set; }
        public System.Windows.Shapes.Polygon UIElement { get; set; }
        public bool IsSelected { get; set; }
        public Action EventArgsSelector { get; set; }

        public MSlabElementNeighborhood(Element element)
        {
            Element = element;
            Solids = Element.GetSolids().Where(x => x.Volume.IsGreater(0)).ToList();

            var curveLoops = Solids
                    .Select(x => x.GetFacesFromSolid())
                    .Aggregate((a, b) => a.Concat(b).ToList())
                    .Select(x => x.GetEdgesAsCurveLoops())
                    .Aggregate((a, b) => a.Concat(b).ToList())
                    .Where(x => x.GetPlane().Normal.AngleTo(XYZ.BasisZ).IsSmallerEqual(Math.PI / 18))
                    .ToList();
            Points = curveLoops.Select(x => x.GetPoints()).Aggregate((a, b) => a.Concat(b).ToList());
            PointsOnFloorPlan = Points
                .Select(x => x.EditZ(0))
                .Distinct(new ComparePoint())
                .ToList();
        }

        public void ActionElementSelected()
        {
            //EventArgsSelector?.Invoke();
            //IsSelected = !IsSelected;
            //UIElement.Stroke = IsSelected
            //        ? StyleColorInCanvas.Color_Selected_OutLine
            //        : StyleColorInCanvas.Color_Concrete_OutLine;
            //UIElement.Fill = IsSelected
            //        ? StyleColorInCanvas.Color_Selected
            //        : StyleColorInCanvas.Color_Concrete;
            //if (IsSelected) AC.UiDoc.Selection.SetElementIds(new List<ElementId>() { Element.Id });
            //if (!IsSelected) AC.UiDoc.Selection.SetElementIds(new List<ElementId>());
        }

        public static void MSlabElementNeighborhoodEventArgsSelector(List<MSlabElementNeighborhood> mSlabElementNeighborhoods, MSlabElementNeighborhood objTarget)
        {
            //foreach (var item in mSlabElementNeighborhoods)
            //{
            //    if (item.Element.Id == objTarget.Element.Id) continue;
            //    if (item.IsSelected)
            //    {
            //        item.IsSelected = false;
            //        item.UIElement.Stroke = item.IsSelected
            //        ? StyleColorInCanvas.Color_Selected_OutLine
            //        : StyleColorInCanvas.Color_Concrete_OutLine;
            //        item.UIElement.Fill = item.IsSelected
            //                ? StyleColorInCanvas.Color_Selected
            //                : StyleColorInCanvas.Color_Concrete;
            //    }
            //}
        }

        private List<XYZ> GetPoints()
        {
            var results = new List<XYZ>();
            try
            {
                var faces = Solids
                    .Select(x => x.GetFacesFromSolid())
                    .Aggregate((a, b) => a.Concat(b).ToList());
                results = faces
                    .Select(x => x.GetPoints())
                    .Aggregate((a, b) => a.Concat(b).ToList())
                    .ToList();
            }
            catch (Exception)
            {
            }
            return results;
        }
        public void DrawInCanvas()
        {

        }
    }
}
