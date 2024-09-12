namespace Utils.Entities
{
    public class SchemaField
    {
        public string Name { get; }
        public string Value { get; set; }
        public SchemaField()
        {
            Name = "Content";
            Value = "";
        }
    }
}
