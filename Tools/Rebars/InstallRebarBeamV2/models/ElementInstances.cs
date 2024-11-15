using HcBimUtils;
using HcBimUtils.DocumentUtils;
using RevitDevelop.Utils.RevElements;
using Utils.SelectionFilterInRevit;

namespace RevitDevelop.Tools.Rebars.InstallRebarBeamV2.models
{
    public class ElementInstances
    {
        public RevElement Beam { get; set; }
        public ElementInstances()
        {
            var obj = AC.UiDoc.Selection.PickObject(
                Autodesk.Revit.UI.Selection.ObjectType.Element,
                new GenericSelectionFilterFromCategory(BuiltInCategory.OST_StructuralFraming)).ToElement();
            Beam = new RevElement(obj);

            using (var ts = new Transaction(AC.Document, "name transaction"))
            {
                ts.Start();
                //--------
                var plane = Plane.CreateByNormalAndOrigin(Beam.BoxElement.LineBox.Direction.CrossProduct(XYZ.BasisZ), Beam.BoxElement.LineBox.Midpoint());
                var sk = SketchPlane.Create(AC.Document, plane);
                AC.Document.Create.NewModelCurve(Beam.BoxElement.LineBox, sk);
                //--------
                ts.Commit();
            }
        }
    }
}
