using Autodesk.Revit.DB.Structure;
using HcBimUtils.RebarUtils;
using Utils.CurveInRevits;
using Utils.RebarInRevits.IRepositories;
using Utils.RebarInRevits.Models;
using Utils.RebarInRevits.Utils;

namespace Utils.RebarInRevits.Repositories
{
    public class RevRebarRepository : IRevRebarRepository
    {
        private readonly Document _document;
        public RevRebar RevRebar { get; private set; }
        public RevRebarRepository(Document document, RevRebar revRebar)
        {
            _document = document;
            RevRebar = revRebar;

        }
        public Rebar CreateRebar(List<Curve> polygon, Element host)
        {
            Rebar rebar = null;
            try
            {
                rebar = Rebar.CreateFromCurves(
                    _document,
                    RevRebar.Style,
                    RevRebar.BarType,
                    RevRebar.StartHook,
                    RevRebar.EndHook,
                    host,
                    RevRebar.Normal,
                    polygon,
                    RevRebar.StartHookOrientation,
                    RevRebar.EndHookOrientation,
                    RevRebar.IsUseExistingShapeIfPosible,
                    RevRebar.IsCreateNewShape);
            }
            catch (System.Exception)
            {
                rebar = null;
            }
            return rebar;
        }
        public Rebar CreateRebar(List<Curve> polygon, BuiltInCategory builtInCategoryHost)
        {
            Rebar rebar = null;
            try
            {
                rebar = Rebar.CreateFromCurves(
                    _document,
                    RevRebar.Style,
                    RevRebar.BarType,
                    RevRebar.StartHook,
                    RevRebar.EndHook,
                    null,
                    RevRebar.Normal,
                    polygon,
                    RevRebar.StartHookOrientation,
                    RevRebar.EndHookOrientation,
                    RevRebar.IsUseExistingShapeIfPosible,
                    RevRebar.IsCreateNewShape);
            }
            catch (System.Exception)
            {
                rebar = null;
            }
            return rebar;
        }

        public Rebar CreateRebarLayoutAsMaximumSpacing(List<Curve> polygon, double spacingFt, double lengthArrFt, BuiltInCategory builtInCategoryHost)
        {
            Rebar rebar = null;
            try
            {
                rebar = Rebar.CreateFromCurves(
                    _document,
                    RevRebar.Style,
                    RevRebar.BarType,
                    RevRebar.StartHook,
                    RevRebar.EndHook,
                    null,
                    RevRebar.Normal,
                    polygon,
                    RevRebar.StartHookOrientation,
                    RevRebar.EndHookOrientation,
                    RevRebar.IsUseExistingShapeIfPosible,
                    RevRebar.IsCreateNewShape);

                rebar.SetRebarLayoutAsMaximumSpacing(spacingFt, lengthArrFt, true, true, true);
            }
            catch (System.Exception)
            {
                rebar = null;
            }

            return rebar;
        }

        public Rebar CreateRebarLayoutAsFixedNumber(List<Curve> polygon, int qty, double lengthArrFt, BuiltInCategory builtInCategoryHost)
        {
            Rebar rebar = null;
            try
            {
                rebar = Rebar.CreateFromCurves(
                    _document,
                    RevRebar.Style,
                    RevRebar.BarType,
                    RevRebar.StartHook,
                    RevRebar.EndHook,
                    null,
                    RevRebar.Normal,
                    polygon,
                    RevRebar.StartHookOrientation,
                    RevRebar.EndHookOrientation,
                    RevRebar.IsUseExistingShapeIfPosible,
                    RevRebar.IsCreateNewShape);

                rebar.SetRebarLayoutAsFixedNumber(qty, lengthArrFt, true, true, true);
            }
            catch (System.Exception)
            {
                rebar = null;
            }
            return rebar;
        }

        public List<Rebar> CreateRebarTapper(List<Curve> polygon1, List<Curve> polygon2, double spacingFt, BuiltInCategory builtInCategoryHost)
        {
            var results = new List<Rebar>();
            try
            {
                var points1 = polygon1.Select(x => x.GetEndPoint(0)).ToList();
                var points2 = polygon2.Select(x => x.GetEndPoint(0)).ToList();

                var condittion1 = polygon1.Count > 0 && polygon2.Count > 0 && polygon1.Count == polygon2.Count;


                var lines = new List<Line>();
                if (condittion1)
                {

                    //create list lines
                    var dem = 0;
                    points1.ForEach(point =>
                    {
                        lines.Add(Line.CreateBound(point, points2[dem]));
                        dem++;
                    });

                    //
                    if (lines.Count > 0)
                    {
                        var maxLenght = lines.OrderBy(line => line.Length).Last().Length;
                        var rebarQty = maxLenght.GetQtyRebar(spacingFt, out double phandu);
                        var plgs = new List<List<XYZ>>();
                        for (int i = 0; i < rebarQty + 1; i++)
                        {
                            var plg = new List<XYZ>();
                            lines.ForEach(line =>
                            {
                                var p = line.GetEndPoint(0);
                                var spacing = line.Length / rebarQty - 1;
                                plg.Add(p + line.Direction * (spacing * i));
                            });
                            plgs.Add(plg);
                        }

                        plgs.ForEach(plg =>
                        {
                            var curves = plg.CreateLineClose();
                            if (curves.Count > 0)
                            {
                                results.Add(CreateRebar(curves, builtInCategoryHost));
                            }
                        });
                    }
                }
            }
            catch (System.Exception)
            {
                results = new List<Rebar>();
            }
            return results;
        }

        public Rebar CreateRebar(List<XYZ> ps, Element host)
        {
            var polygon = ps.CreateLine();
            return CreateRebar(polygon, host);
        }

        public Rebar CreateRebar(List<XYZ> ps, BuiltInCategory builtInCategoryHost)
        {
            var polygon = ps.CreateLine();
            return CreateRebar(polygon, builtInCategoryHost);
        }
    }
}
