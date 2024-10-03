using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.UI;
using HcBimUtils;
using HcBimUtils.DocumentUtils;
using HcBimUtils.GeometryUtils.Geometry;
using Utils.FilterElements;
using Utils.Parts;
using Utils.RebarInRevits.Models;

namespace Utils.Assemblies
{
    public class AssemblyInfo
    {
        private UIDocument _uiDocument;
        private Document _document;
        public AssemblyInstance AssemblyInstance { get; private set; }
        public AssemblyType1 AssemblyType { get; private set; }
        public Outline AssemblyOutline { get; private set; }
        public Line AssemblyCurveBoundingBox { get; private set; }
        public string AssemblyZone { get; set; }
        public AssemblyInfo(UIDocument uiDocument, AssemblyInstance assemblyInstance)
        {
            _uiDocument = uiDocument;
            _document = _uiDocument.Document;
            AssemblyInstance = assemblyInstance;
            AssemblyType = GetAssemblyType(_document, AssemblyInstance);
            AssemblyOutline = GetAssemblyOutLine();
            AssemblyCurveBoundingBox = GetAssemblyCurveBoundingBox();
        }
        public void SetAssemblyZone(List<PartInfo> partInfos)
        {
            var options = new SolidCurveIntersectionOptions();
            var obs = new List<PartInfoRef>();
            foreach (var item in partInfos)
            {
                var curveInterset = item.SolidZone.IntersectWithCurve(AssemblyCurveBoundingBox, options);
                var curveIntersetCount = curveInterset.Count();
                var length = 0.0;
                for (int i = 0; i < curveIntersetCount; i++)
                {
                    length += curveInterset.GetCurveSegment(i).Length;
                }
                obs.Add(new PartInfoRef() { PartInfo = item, Length = length });
            }
            var partInfo = obs.OrderBy(x => x.Length).LastOrDefault();
            AssemblyZone = partInfo != null ? partInfo.PartInfo.NameZone : "";
        }
        public void SetAssemblyZoneForRebarInAssembly(List<PartInfo> partInfos)
        {
            var options = new SolidCurveIntersectionOptions();
            var partInfosActive = partInfos.Where(item =>
            {
                var curveInterset = item.SolidZone.IntersectWithCurve(AssemblyCurveBoundingBox, options);
                var curveIntersetCount = curveInterset.Count();
                return curveIntersetCount > 0;
            }).ToList();

            if (AssemblyType == AssemblyType1.Rebar)
            {
                if (!string.IsNullOrEmpty(AssemblyZone))
                {
                    using (var ts = new Transaction(_document, "name transaction"))
                    {
                        ts.Start();
                        //--------
                        var rebars = GetElementInAssembly<Rebar>(_document, AssemblyInstance, BuiltInCategory.OST_Rebar).ToList();
                        foreach (var rebar in rebars)
                        {
                            var pathsRebar = rebar.GetCenterlineCurves(true, true, true, MultiplanarOption.IncludeAllMultiplanarCurves, 0);
                            var partInfoActiveOrder = partInfosActive.OrderBy(x =>
                            {
                                var length = 0.0;
                                foreach (var curve in pathsRebar)
                                {
                                    var curveInterset = x.SolidZone.IntersectWithCurve(curve, options);
                                    var curveIntersetCount = curveInterset.Count();
                                    for (int i = 0; i < curveIntersetCount; i++)
                                    {
                                        length += curveInterset.GetCurveSegment(i).Length;
                                    }
                                }
                                return length;
                            }).LastOrDefault();
                            if (partInfoActiveOrder != null)
                            {
                                var hasParamZone = ParameterUtilities.HasParameter(rebar, PartInfo.PARAM_NAME_ZONE);
                                if (hasParamZone)
                                {
                                    rebar.LookupParameter(PartInfo.PARAM_NAME_ZONE).Set(partInfoActiveOrder.NameZone);
                                }
                            }
                        }
                        //--------
                        ts.Commit();
                    }
                }
            }
        }
        public void SetAssemblyZoneForRebarInAssembly()
        {
            if (AssemblyType == AssemblyType1.Rebar)
            {
                if (!string.IsNullOrEmpty(AssemblyZone))
                {
                    using (var ts = new Transaction(_document, "name transaction"))
                    {
                        ts.Start();
                        //--------
                        var rebars = GetElementInAssembly<Rebar>(_document, AssemblyInstance, BuiltInCategory.OST_Rebar).ToList();
                        foreach (var rebar in rebars)
                        {
                            var hasParamZone = ParameterUtilities.HasParameter(rebar, PartInfo.PARAM_NAME_ZONE);
                            if (hasParamZone)
                            {
                                rebar.LookupParameter(PartInfo.PARAM_NAME_ZONE).Set(AssemblyZone);
                            }
                        }
                        //--------
                        ts.Commit();
                    }
                }
            }
        }
        private Outline GetAssemblyOutLine()
        {
            Outline result = null;
            try
            {
                var points = new List<XYZ>();
                switch (AssemblyType)
                {
                    case AssemblyType1.InValid:
                        break;
                    case AssemblyType1.Rebar:
                        var rebars = GetElementInAssembly<Rebar>(_document, AssemblyInstance, BuiltInCategory.OST_Rebar);
                        points = GetPoint(rebars).ToList();
                        break;
                    case AssemblyType1.Beam:
                        var beams = GetElementInAssembly<FamilyInstance>(_document, AssemblyInstance, BuiltInCategory.OST_StructuralFraming);
                        points = GetPoint(beams).ToList();
                        break;
                    case AssemblyType1.Column:
                        var columns = GetElementInAssembly<FamilyInstance>(_document, AssemblyInstance, BuiltInCategory.OST_StructuralColumns);
                        points = GetPoint(columns).ToList();
                        break;
                }

                var minx = points.Min(x => x.X);
                var miny = points.Min(x => x.Y);
                var minz = points.Min(x => x.Z);

                var maxx = points.Max(x => x.X);
                var maxy = points.Max(x => x.Y);
                var maxz = points.Max(x => x.Z);

                result = new Outline(new XYZ(minx, miny, minz), new XYZ(maxx, maxy, maxz));

                return result;
            }
            catch (Exception)
            {
                return null;
            }
        }
        private Line GetAssemblyCurveBoundingBox()
        {
            Line result = null;
            try
            {
                if (AssemblyOutline != null) result = Line.CreateBound(AssemblyOutline.MinimumPoint, AssemblyOutline.MaximumPoint);
            }
            catch (Exception)
            {
            }
            return result;
        }
        private IEnumerable<XYZ> GetPoint(IEnumerable<Rebar> rebars)
        {
            var results = new List<XYZ>();
            foreach (var rebar in rebars)
            {
                try
                {
                    var paths = rebar.GetCenterlineCurves(true, true, false, MultiplanarOption.IncludeAllMultiplanarCurves, 0);
                    foreach (var curve in paths)
                    {
                        results.Add(curve.GetEndPoint(0));
                        results.Add(curve.GetEndPoint(1));
                    }
                }
                catch (Exception)
                {
                }
            }
            return results;
        }
        private IEnumerable<XYZ> GetPoint(IEnumerable<FamilyInstance> familyInstances)
        {
            var results = new List<XYZ>();
            foreach (var familyInstance in familyInstances)
            {
                try
                {
                    var solid = familyInstance.GetSingleSolid();
                    var faces = solid.GetFacesFromSolid();
                    foreach (var face in faces)
                    {
                        var points = face.GetPoints();
                        results.AddRange(points);
                    }
                }
                catch (Exception)
                {
                }
            }
            return results;
        }
        public static IEnumerable<AssemblyInstance> GetAssemblyInstance(Document document)
        {
            return document.GetElementsFromClass<AssemblyInstance>(false);
        }
        public static AssemblyType1 GetAssemblyType(Document document, AssemblyInstance assemblyInstance)
        {
            var result = AssemblyType1.InValid;
            try
            {
                if (assemblyInstance == null) return AssemblyType1.InValid;
                var members = assemblyInstance.GetMemberIds().Select(x => document.GetElement(x));

                var isRebar = members.Any(x => x.Category.ToBuiltinCategory() == BuiltInCategory.OST_Rebar);
                var isNotRebar = members.Any(x => x.Category.ToBuiltinCategory() != BuiltInCategory.OST_Rebar);

                var isBeam = members.Any(x => x.Category.ToBuiltinCategory() == BuiltInCategory.OST_StructuralFraming);
                var isNotBeam = members.Any(x => x.Category.ToBuiltinCategory() != BuiltInCategory.OST_StructuralFraming);

                var isColumn = members.Any(x => x.Category.ToBuiltinCategory() == BuiltInCategory.OST_StructuralColumns);
                var isNotColumn = members.Any(x => x.Category.ToBuiltinCategory() != BuiltInCategory.OST_StructuralColumns);

                var isFloor = members.Any(x => x.Category.ToBuiltinCategory() == BuiltInCategory.OST_Floors);
                var isNotFloor = members.Any(x => x.Category.ToBuiltinCategory() != BuiltInCategory.OST_Floors);

                if (isRebar == true && isNotRebar == false) result = AssemblyType1.Rebar;
                if (isBeam == true && isNotBeam == false) result = AssemblyType1.Beam;
                if (isRebar == true && isNotColumn == false) result = AssemblyType1.Column;

                return result;
            }
            catch (Exception)
            {
                return AssemblyType1.InValid;
            }
        }
        public static IEnumerable<AssemblyInstance> GetAssemblyInstanceFormCategory(Document document, BuiltInCategory builtInCategory)
        {
            var results = new List<AssemblyInstance>();
            try
            {
                var asses = document.GetElementsFromClass<AssemblyInstance>(false);
                foreach (var asse in asses)
                {
                    var members = asse.GetMemberIds().Select(x => document.GetElement(x));
                    var isRebar = members.Any(x => x.Category.ToBuiltinCategory() == builtInCategory);
                    var isNotRebar = members.Any(x => x.Category.ToBuiltinCategory() != builtInCategory);

                }
                results = asses.Where(asse =>
                {
                    var members = asse.GetMemberIds().Select(x => document.GetElement(x));
                    var isRebar = members.Any(x => x.Category.ToBuiltinCategory() == builtInCategory);
                    var isNotRebar = members.Any(x => x.Category.ToBuiltinCategory() != builtInCategory);
                    return isRebar & !isNotRebar;
                }).ToList();
            }
            catch (Exception)
            {
            }
            return results;
        }
        public static IEnumerable<T> GetElementInAssembly<T>(Document document, AssemblyInstance assemblyInstance, BuiltInCategory builtInCategory)
        {
            var results = new List<T>();
            try
            {
                var members = assemblyInstance.GetMemberIds().Select(x => document.GetElement(x));
                results = members
                    .Where(x => x.Category.ToBuiltinCategory() == builtInCategory)
                    .Where(x => x is T)
                    .Cast<T>()
                    .ToList();
            }
            catch (Exception)
            {
            }
            return results;
        }
        public static IEnumerable<AssemblyInstance> GetRebarAssemblyInstanceFormRebarHostCategory(
            Document document,
            IEnumerable<AssemblyInstance> rebarAssemblyInstances,
            RebarHostCategory rebarHostCategory,
            out List<Rebar> rebarFalseHost)
        {
            var results = new List<AssemblyInstance>();
            var rebarAssemblyInstancesFailT = new List<Rebar>();
            foreach (var rebarAssemblyInstance in rebarAssemblyInstances)
            {
                try
                {
                    var assType = GetAssemblyType(document, rebarAssemblyInstance);
                    if (assType == AssemblyType1.Rebar)
                    {
                        var rebars = GetElementInAssembly<Rebar>(document, rebarAssemblyInstance, BuiltInCategory.OST_Rebar);
                        var isRebarWithHostCategory = rebars.Any(x => x.get_Parameter(BuiltInParameter.REBAR_HOST_CATEGORY).AsInteger() == rebarHostCategory.ToIntergerValue());
                        var isNotRebarWithHostCategory = rebars.Any(x => x.get_Parameter(BuiltInParameter.REBAR_HOST_CATEGORY).AsInteger() != rebarHostCategory.ToIntergerValue());
                        if (isRebarWithHostCategory == true && isNotRebarWithHostCategory == false)
                        {
                            results.Add(rebarAssemblyInstance);
                        }
                        else if (isRebarWithHostCategory == true && isNotRebarWithHostCategory == true)
                        {
                            rebarAssemblyInstancesFailT.AddRange(rebars.Where(x => x.get_Parameter(BuiltInParameter.REBAR_HOST_CATEGORY).AsInteger() != rebarHostCategory.ToIntergerValue()));
                        }
                    }
                }
                catch (Exception)
                {
                }
            }
            rebarFalseHost = rebarAssemblyInstancesFailT;
            return results;
        }
    }
    public enum AssemblyType1
    {
        InValid,
        Rebar,
        Beam,
        Column,
        Floor,
        Wall,
        Foundation
    }
}
