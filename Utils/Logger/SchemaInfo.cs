using Autodesk.Revit.DB.ExtensibleStorage;

namespace Utils.Logger
{
    public class SchemaInfo
    {
        public string SchemaGuidID { get; set; }
        public string SchemaName { get; set; }
        public List<string> SchemaKeys { get; set; }
        public SchemaInfo(string schemaGuidID, string schemaName, List<string> schemaKeys)
        {
            SchemaGuidID = schemaGuidID;
            SchemaName = schemaName;
            SchemaKeys = schemaKeys;
        }
        public Schema CreateBaseSchema()
        {
            var schemaBuilder = new SchemaBuilder(new Guid(SchemaGuidID));
            schemaBuilder.SetReadAccessLevel(AccessLevel.Public);
            schemaBuilder.SetWriteAccessLevel(AccessLevel.Public);
            schemaBuilder.SetSchemaName(SchemaName);
            foreach (var schemaAddField in SchemaKeys)
            {
                schemaBuilder.AddSimpleField(schemaAddField, typeof(string));
            }
            var schema = Schema.Lookup(new Guid(SchemaGuidID)) ?? schemaBuilder.Finish();// register the Schema object
            return schema;
        }
    }
}
