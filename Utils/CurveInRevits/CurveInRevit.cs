namespace Utils.CurveInRevits
{
    public static class CurveInRevit
    {
        public static bool SortCurvesContinuous(IList<Curve> curves)
        {
            var r = true;

            int n = curves.Count;

            // Walk through each curve (after the first) 
            // to match up the curves in order

            for (int i = 0; i < n; ++i)
            {
                Curve curve = curves[i];
                XYZ endPoint = curve.GetEndPoint(1);

                XYZ p;

                // Find curve with start point = end point

                bool found = i + 1 >= n;

                for (int j = i + 1; j < n; ++j)
                {
                    p = curves[j].GetEndPoint(0);

                    // If there is a match end->start, 
                    // this is the next curve

                    if (0.001 > p.DistanceTo(endPoint))
                    {
                        if (i + 1 != j)
                        {
                            Curve tmp = curves[i + 1];
                            curves[i + 1] = curves[j];
                            curves[j] = tmp;
                        }
                        found = true;
                        break;
                    }

                    p = curves[j].GetEndPoint(1);
                    // If there is a match end->end, 
                    // reverse the next curve

                    if (0.001 > p.DistanceTo(endPoint))
                    {
                        if (i + 1 == j)
                        {
                            curves[i + 1] = curves[j].CreateReversed();
                        }
                        else
                        {
                            Curve tmp = curves[i + 1];
                            curves[i + 1] = curves[j].CreateReversed();
                            curves[j] = tmp;
                        }
                        found = true;
                        break;
                    }
                }
                if (!found)
                {
                    //MessageBox.Show("non - contiguous input curves", "Error");
                    r = false;
                }
            }

            return r;
        }
        public static List<Curve> CreateLineClose(this List<XYZ> points)
        {
            var pointsCount = points.Count;
            var results = new List<Curve>();
            if (pointsCount > 2)
            {
                for (int i = 0; i < pointsCount; i++)
                {
                    if (i == pointsCount - 1)
                    {
                        results.Add(Line.CreateBound(points[i], points[0]));

                    }
                    else
                    {
                        results.Add(Line.CreateBound(points[i], points[i + 1]));
                    }
                }
            }
            return results;
        }
        public static List<Curve> CreateLine(this List<XYZ> points)
        {
            var pointsCount = points.Count;
            var results = new List<Curve>();
            if (pointsCount > 1)
            {
                for (int i = 0; i < pointsCount - 1; i++)
                {
                    results.Add(Line.CreateBound(points[i], points[i + 1]));
                }
            }
            return results;
        }
    }
}
