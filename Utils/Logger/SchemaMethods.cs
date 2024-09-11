using Autodesk.Revit.DB.ExtensibleStorage;

namespace Utils.Logger
{
    public static class SchemaMethods
    {
        public static Schema Write(Schema schemaBase, Element element, Dictionary<string, string> pairs)
        {
            var entity = new Entity(schemaBase);
            foreach (var pair in pairs)
            {
                var field = schemaBase.GetField(pair.Key);
                entity.Set(field, pair.Value);
            }
            element.SetEntity(entity); // store the entity in the element
            return schemaBase;
        }
        public static string Read(Schema schemaBase, Element element, string fieldName)
        {
            var views = "";
            if (schemaBase == null) return views;

            var field = schemaBase.GetField(fieldName);
            if (field == null) return views;

            var entity = element.GetEntity(schemaBase);
            if (entity != null && entity.IsValid())
                views = entity.Get<string>(field);
            return views;
        }
        public static Dictionary<string, string> ReadAll(Schema schemaBase, List<string> schemaBaseKeys, Element element)
        {
            var resultDict = new Dictionary<string, string>();
            if (schemaBase == null)
            {
                return resultDict;
            }
            foreach (var schemaAddField in schemaBaseKeys)
            {
                resultDict.Add(schemaAddField, "");
                var field = schemaBase.GetField(schemaAddField);
                if (field == null) continue;

                var entity = element?.GetEntity(schemaBase);
                if (entity != null && entity.IsValid())
                {
                    resultDict[schemaAddField] = entity.Get<string>(field);
                }
            }
            return resultDict;
        }
    }
}
