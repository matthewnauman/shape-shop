using System.Collections.Generic;

namespace ShapeShopData.Statistics
{
    public class PuzzleStatisticsSecondsTakenComparer : IComparer<PuzzleStatistics>
    {
        // sort by timeIdle, (oldest move to head)
        public int Compare(PuzzleStatistics ps1, PuzzleStatistics ps2)
        {
            return ps2.SecondsTaken.CompareTo(ps1.SecondsTaken);
        }

    }
}