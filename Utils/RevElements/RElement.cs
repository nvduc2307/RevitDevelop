using Utils.canvass;

namespace RevitDevelop.Utils.RevElements
{
    public abstract class RElement
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public object RObject { get; set; }
        public XYZ RCenter { get; set; }
        public CanvasPageBase CanvasPageBase { get; set; }
        public bool IsValidObject { get; set; }
        public ElementId LevelId { get; set; }

        protected RElement(int id, object rObject, XYZ rCenter, CanvasPageBase canvasPageBase)
        {
            Id = id;
            RObject = rObject;
            RCenter = rCenter;
            CanvasPageBase = canvasPageBase;
        }
        public abstract void DrawInCanvas();

    }
}
