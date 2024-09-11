using Autodesk.Revit.DB.Structure;

namespace Utils.RebarInRevits.Models
{
    public class RevRebar
    {
        /// <summary>
        /// Style = [Standard, StirrupTie].
        /// BarType is dimeter of rebar.
        /// StartHook is start hook.
        /// EndHook is start hook.
        /// StartHookOrientation
        /// EndHookOrientation
        /// Normal is vector normal of face has all rebar curve.
        /// IsUseExistingShapeIfPosible : if rebar shape is existed.
        /// IsCreateNewShape
        /// </summary>
        /// 
        public Element Host { get; set; }
        public RebarStyle Style { get; set; }
        public RebarBarType BarType { get; set; }
        public RebarHookType StartHook { get; set; }
        public RebarHookType EndHook { get; set; }
        public XYZ Normal { get; set; }
        public RebarHookOrientation StartHookOrientation { get; set; }
        public RebarHookOrientation EndHookOrientation { get; set; }
        public bool IsUseExistingShapeIfPosible { get; set; }
        public bool IsCreateNewShape { get; set; }
        public string Prefix { get; set; }
        public double Length { get; set; }
        public string RebarShape { get; set; }
        public double Diameter { get; set; }
        public string Zone { get; set; }
        public string GroupElevation { get; set; }
    }
}
