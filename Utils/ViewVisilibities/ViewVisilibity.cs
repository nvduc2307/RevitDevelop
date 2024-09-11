namespace Utils.ViewVisilibities
{
    public static class ViewVisilibity
    {
        public static void SetVisilibity(this View view, BuiltInCategory builtInCategory, bool visible)
        {
            var doc = view.Document;
            var category = Category.GetCategory(doc, builtInCategory);

            using (var ts = new Transaction(doc, "ViewVisilibity"))
            {
                ts.Start();
                //--------
                if (category != null && category.get_AllowsVisibilityControl(view))
                {
                    view.SetCategoryHidden(category.Id, !visible);
                }
                //--------
                ts.Commit();
            }
        }
        public static void SetVisilibity(this View view, IEnumerable<BuiltInCategory> builtInCategoryies, bool visible)
        {
            var doc = view.Document;
            var categories = builtInCategoryies.Select(x => Category.GetCategory(doc, x));

            using (var ts = new Transaction(doc, "ViewVisilibity"))
            {
                ts.Start();
                //--------
                foreach (var category in categories)
                {
                    if (category != null && category.get_AllowsVisibilityControl(view))
                    {
                        view.SetCategoryHidden(category.Id, !visible);
                    }
                }
                //--------
                ts.Commit();
            }
        }

    }
}
