namespace Utils.RevElementNumberings
{
    public abstract class RevElementNumbering
    {
        public int ElementId { get; set; }
        public int HostId { get; set; }
        public string HostName { get; set; }
        public string Name { get; set; }
        public string Prefix { get; set; }
        public string Zone { get; set; }
        public string ElementPosition { get; set; } //[prefix + auto number]
    }
}
