using Autodesk.Revit.Attributes;
using HcBimUtils;
using HcBimUtils.DocumentUtils;
using HcBimUtils.GeometryUtils.Geometry;
using Nice3point.Revit.Toolkit.External;

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
                    var obj = UiDocument.Selection.PickObject(Autodesk.Revit.UI.Selection.ObjectType.Element).ToElement();
                    var solids = obj.GetSolids();
                    var curves = solids.Select(x => x.GetFacesFromSolid())
                        .Aggregate((a, b) => a.Concat(b).ToList())
                        .Select(x => x.GetFirstCurveLoop().ToList())
                        .Aggregate((a, b) => a.Concat(b).ToList())
                        .OrderBy(x => x.Length)
                        .ToList();
                    var vtx = curves.LastOrDefault().Direction();
                    var vty = vtx.CrossProduct(XYZ.BasisZ);
                    var vtz = vtx.CrossProduct(vty);
                    var max = curves
                        .Select(x => new List<XYZ>() { x.GetEndPoint(0), x.GetEndPoint(1) })
                        .Aggregate((a, b) => a.Concat(b).ToList())
                        .ToList()
                        .OrderBy(x => x.Z)
                        .LastOrDefault();
                    var edgeArrs = solids
                        .Select(x => x.Edges)
                        .ToList();
                    var refArr = new ReferenceArray();
                    foreach (var edgeArr in edgeArrs)
                    {
                        foreach (Edge edge in edgeArr)
                        {
                            var c = edge.AsCurve();
                            var dr = edge.AsCurve().Direction();
                            var dk1 = vtx.IsParallel(dr);
                            var dk2 = c.GetEndPoint(0).Z.IsAlmostEqual(max.Z) || c.GetEndPoint(1).Z.IsAlmostEqual(max.Z);
                            if (dk1 && dk2)
                            {
                                var content = obj.UniqueId + ":0:INSTANCE:" + edge.Reference.ConvertToStableRepresentation(Document);
                                refArr.Append(Reference.ParseFromStableRepresentation(Document, content));
                            }
                        }
                    }

                    using (var ts = new Transaction(Document, "name transaction"))
                    {
                        ts.Start();
                        //--------
                        var l = Line.CreateBound(max, max + vty * 1000.MmToFoot());
                        var dim = Document.Create.NewDimension(Document.ActiveView, l, refArr);
                        //--------
                        ts.Commit();
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
