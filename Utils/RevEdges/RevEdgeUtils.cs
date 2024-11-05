namespace RevitDevelop.Utils.RevEdges
{
    public static class RevEdgeUtils
    {
        public static List<Edge> GetEdges(Solid solid)
        {
            var result = new List<Edge>();
            foreach (Edge edge in solid.Edges)
            {
                try
                {
                    result.Add(edge);
                }
                catch (Exception)
                {
                }
            }
            return result;
        }
    }
}
