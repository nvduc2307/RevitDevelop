namespace Utils.CurveInRevits
{
    public static class LineStyleInRevit
    {
        public static void Create(Document doc, string lineStyleName, Color color, string typePatternName = "")
        {
            // Use this to access the current document in a macro.
            //
            //Document doc = this.ActiveUIDocument.Document;

            // Find existing linestyle.  Can also opt to
            // create one with LinePatternElement.Create()

            FilteredElementCollector fec
              = new FilteredElementCollector(doc)
                .OfClass(typeof(LinePatternElement));

            // The new linestyle will be a subcategory 
            // of the Lines category        

            Categories categories = doc.Settings.Categories;

            Category lineCat = categories.get_Item(BuiltInCategory.OST_Lines);


            using (Transaction t = new Transaction(doc))
            {
                t.Start("Create LineStyle");

                // Add the new linestyle 

                Category newLineStyleCat = categories
              .NewSubcategory(lineCat, lineStyleName);

                doc.Regenerate();

                // Set the linestyle properties 
                // (weight, color, pattern).

                newLineStyleCat.SetLineWeight(8, GraphicsStyleType.Projection);

                newLineStyleCat.LineColor = color;

                if (typePatternName != "")
                {
                    LinePatternElement linePatternElem = fec.Cast<LinePatternElement>()
                                                            .FirstOrDefault<LinePatternElement>(linePattern => linePattern.Name.Contains(typePatternName));
                    if (linePatternElem != null) newLineStyleCat.SetLinePatternId(linePatternElem.Id, GraphicsStyleType.Projection);
                }

                t.Commit();
            }
        }
    }
}
