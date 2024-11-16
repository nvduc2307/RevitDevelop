using Autodesk.Revit.Attributes;
using HcBimUtils;
using HcBimUtils.DocumentUtils;
using Nice3point.Revit.Toolkit.External;
using RevitDevelop.Tools.ATest.middlewares;
using Utils.BoundingBoxs;
using Utils.RevPoints;
using Utils.SelectionFilterInRevit;

namespace RevitDevelop.Tools.ATest
{
    [Transaction(TransactionMode.Manual)]
    public class ATestCmd : ExternalCommand
    {
        public override void Execute()
        {
            AC.GetInformation(UiDocument);
            using (var tsg = new TransactionGroup(Document, "name transaction group"))
            {
                tsg.Start();
                try
                {
                    //--------
                    var heightSolid = 500.MmToFoot();
                    var objs = UiDocument.Selection
                        .PickObjects(Autodesk.Revit.UI.Selection.ObjectType.Element, new GenericSelectionFilterFromCategory(BuiltInCategory.OST_StructuralColumns))
                        .Select(x=>x.ToElement());
                    var eleFails = new List<Element>();
                    foreach (var obj in objs) {
                        try
                        {
                            var box = new RevBoxElement(obj);
                            var isAnyBoxVectorParaWithVTZ =  ATestMiddleware.IsAnyBoxVectorParaWithVTZ(box);
                            if (!isAnyBoxVectorParaWithVTZ) throw new Exception();
                            var boxPoints = new List<XYZ>()
                            {
                                box.BoxElementPoint.P1,
                                box.BoxElementPoint.P2,
                                box.BoxElementPoint.P3,
                                box.BoxElementPoint.P4,
                                box.BoxElementPoint.P5,
                                box.BoxElementPoint.P6,
                                box.BoxElementPoint.P7,
                                box.BoxElementPoint.P8,
                            };

                            //chuyen lai truc toa do theo truc Z
                            box.GenerateCoordinateWithBaseVT(XYZ.BasisZ);
                            //get top points of box
                            var boxTopPoints = boxPoints
                                .GroupBy(x=> Math.Round(x.DotProduct(box.VTZ).MmToFoot(), 3))
                                .OrderBy(x=> Math.Round(x.First().DotProduct(box.VTZ).MmToFoot(), 3))
                                .Select(x=>x.ToList())
                                .LastOrDefault();

                            var boxTopPointsRefXs = boxTopPoints
                                .GroupBy(x => x.DotProduct(box.VTX))
                                .Select(x => x.OrderBy(y=>y.DotProduct(box.VTY)).ToList());
                            var plg = new List<XYZ>() {
                                boxTopPointsRefXs.FirstOrDefault().First(),
                                boxTopPointsRefXs.FirstOrDefault().Last(),
                                boxTopPointsRefXs.LastOrDefault().Last(),
                                boxTopPointsRefXs.LastOrDefault().First(),
                                boxTopPointsRefXs.FirstOrDefault().First(),
                            };

                            var cs = plg.PointsToCurves();

                            using (var ts = new Transaction(Document, "name transaction"))
                            {
                                ts.Start();
                                //--------
                                //var center = box.LineBox.Midpoint();
                                //var plane = Plane.CreateByNormalAndOrigin(box.VTY, center);
                                //var sket = SketchPlane.Create(Document, plane);
                                //Document.Create.NewModelCurve(Line.CreateBound(center, center + box.VTX * 1000.MmToFoot()), sket);
                                foreach (var c in cs)
                                {
                                    var normal = c.Direction().IsParallel(XYZ.BasisZ)
                                        ? c.Direction().CrossProduct(XYZ.BasisX)
                                        : c.Direction().CrossProduct(XYZ.BasisZ);
                                    var plane = Plane.CreateByNormalAndOrigin(normal, c.Midpoint());
                                    var sket = SketchPlane.Create(Document, plane);
                                    Document.Create.NewModelCurve(c, sket);
                                }
                                //--------
                                ts.Commit();
                            }
                        }
                        catch (Exception)
                        {
                            eleFails.Add(obj);
                        }
                    }
                    //--------
                    tsg.Assimilate();
                }
                catch (Autodesk.Revit.Exceptions.OperationCanceledException) { }
                catch (Exception)
                {
                    tsg.RollBack();
                }
            }
        }
    }
}
