using Autodesk.Revit.DB.ExtensibleStorage;

namespace Utils.Entities
{
    public class SchemaInfo
    {
        public string SchemaGuidID { get; }
        public string SchemaName { get; }
        public SchemaField SchemaField { get; }
        public Schema SchemaBase { get; }
        public SchemaInfo(string schemaGuidID, string schemaName, SchemaField schemaField)
        {
            SchemaGuidID = schemaGuidID;
            SchemaName = schemaName;
            SchemaField = schemaField;
            SchemaBase = CreateBaseSchema(SchemaGuidID, SchemaName, SchemaField);
        }

        public static Schema CreateBaseSchema(string schemaGuidID, string schemaName, SchemaField schemaField)
        {
            var schemaBuilder = new SchemaBuilder(new Guid(schemaGuidID));
            schemaBuilder.SetReadAccessLevel(AccessLevel.Public);
            schemaBuilder.SetWriteAccessLevel(AccessLevel.Public);
            schemaBuilder.SetSchemaName(schemaName);
            schemaBuilder.AddSimpleField(schemaField.Name, typeof(string));
            var schema = Schema.Lookup(new Guid(schemaGuidID)) ?? schemaBuilder.Finish();// register the Schema object
            return schema;
        }

        public static Schema Write(Schema schemaBase, Element element, SchemaField schemaField)
        {
            var entity = new Entity(schemaBase);
            var field = schemaBase.GetField(schemaField.Name);
            entity.Set(field, schemaField.Value);
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
    }
}
