using HcBimUtils.MoreLinq;

namespace Utils.RebarInRevits.Utils
{
    public static class RebarLengthUtils
    {
        private static List<double> RebarLengthData = new List<double>(){
            1000, 1500, 2000, 2500, 3000, 3500, 4000, 4500, 5000, 5500, 6000, 6500, 7500, 8000, 8500, 9000, 9500, 10000, 10500, 11000, 11500, 12000
        };
        public static double OptimizeRebarLength(this double rebarLength)
        {
            var rebarLengthDataCount = RebarLengthData.Count;
            var midRebarLengthDataCount = (rebarLengthDataCount - rebarLengthDataCount % 2) / 2;
            var rebarLengthDataApply = new List<double>().Concat(RebarLengthData).ToList();
            var rebarLengthData1 = rebarLengthDataApply.Slice(0, midRebarLengthDataCount).ToList();
            var rebarLengthData2 = rebarLengthDataApply.Slice(midRebarLengthDataCount, rebarLengthDataCount - midRebarLengthDataCount).ToList();

            do
            {
                if (rebarLength > rebarLengthData1.Last())
                {
                    rebarLengthDataApply = rebarLengthData2;
                    rebarLengthDataCount = rebarLengthDataApply.Count;
                    midRebarLengthDataCount = (rebarLengthDataCount - rebarLengthDataCount % 2) / 2;
                    rebarLengthData1 = rebarLengthDataApply.Slice(0, midRebarLengthDataCount).ToList();
                    rebarLengthData2 = rebarLengthDataApply.Slice(midRebarLengthDataCount, rebarLengthDataCount - midRebarLengthDataCount).ToList();
                }
                else
                {
                    rebarLengthDataApply = rebarLengthData1;
                    rebarLengthDataCount = rebarLengthDataApply.Count;
                    rebarLengthData1 = rebarLengthDataApply.Slice(0, midRebarLengthDataCount).ToList();
                    midRebarLengthDataCount = (rebarLengthDataCount - rebarLengthDataCount % 2) / 2;
                    rebarLengthData2 = rebarLengthDataApply.Slice(midRebarLengthDataCount, rebarLengthDataCount - midRebarLengthDataCount).ToList();
                }

            } while (rebarLengthDataApply.Count > 1);

            return rebarLengthDataApply.Count == 1 ? rebarLengthDataApply.First() : rebarLength;
        }
        public static List<double> GetLengthOrders(double lengthOrder)
        {
            var results = new List<double>();
            foreach (var item in RebarLengthData)
            {
                if (item >= lengthOrder) results.Add(item);
            }
            return results;
        }
    }
}
