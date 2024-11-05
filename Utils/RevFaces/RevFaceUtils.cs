namespace RevitDevelop.Utils.RevFaces
{
    public static class RevFaceUtils
    {
        public static List<PlanarFace> GetPlanarFaces(this Solid solid)
        {
            var result = new List<PlanarFace>();
            var faces = solid.Faces;
            foreach (PlanarFace face in solid.Faces)
            {
                try
                {
                    result.Add(face);
                }
                catch (Exception)
                {
                }
            }
            return result;
        }
    }
}
