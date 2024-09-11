namespace Utils.EventInReVit
{
    public static class OverrideGraphics
    {
        public static void SetTransparenColorElement(Document doc, Element ele, Color color, int transparency)
        {
            var graphic = new OverrideGraphicSettings();
            graphic.SetSurfaceTransparency(transparency);
            graphic.SetSurfaceBackgroundPatternColor(color);
            FillPatternElement fillPattern = new FilteredElementCollector(doc).OfClass(typeof(FillPatternElement)).Cast<FillPatternElement>().FirstOrDefault();
            if (fillPattern != null) graphic.SetSurfaceBackgroundPatternId(fillPattern.Id);
            doc.ActiveView.SetElementOverrides(ele.Id, graphic);
        }
    }
}
