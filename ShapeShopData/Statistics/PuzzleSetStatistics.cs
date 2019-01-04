using Microsoft.Xna.Framework.Content;
using System;
using System.Collections.Generic;
using System.Text;

namespace ShapeShopData.Statistics
{
    /// The set of relevant statistics for game pieces.
    [Serializable]
    public struct PuzzleSetStatistics
    {
        [ContentSerializer(Optional = true)]
        public Int32 PuzzlesSolved;

        [ContentSerializer(Optional = true)]
        public double SecondsTaken;

        [ContentSerializer(Optional = true)]
        public Int32 ShapesUsed;

        [ContentSerializer(Optional = true)]
        public Int32 CWRotations;

        [ContentSerializer(Optional = true)]
        public Int32 CCWRotations;

        [ContentSerializer(Optional = true)]
        public Int32 HorizontalFlips;

        [ContentSerializer(Optional = true)]
        public Int32 VerticalFlips;

        [ContentSerializer(Optional = true)]
        public Int32 HintsUsed;

        /// Returns true if this object is trivial - all values at zero.
        public bool IsZero
        {
            get
            {
                return (PuzzlesSolved == 0);
            }
        }

        public PuzzleSetStatistics(List<int> shapeKeys)
        {
            PuzzlesSolved = 0;
            SecondsTaken = 0;
            ShapesUsed = 0;
            CWRotations = 0;
            CCWRotations = 0;
            HorizontalFlips = 0;
            VerticalFlips = 0;
            HintsUsed = 0;
        }

        // Operator: StatisticsValue + StatisticsValue
        /// Add one value to another, piecewise, and return the result.
        public static PuzzleSetStatistics Add(PuzzleSetStatistics value1, PuzzleStatistics value2)
        {
            PuzzleSetStatistics outputValue = new PuzzleSetStatistics();
            
            outputValue.CCWRotations = value1.CCWRotations + value2.CCWRotations;
            outputValue.CWRotations = value1.CWRotations + value2.CWRotations;
            outputValue.HorizontalFlips = value1.HorizontalFlips + value2.HorizontalFlips;
            outputValue.SecondsTaken = value1.SecondsTaken + value2.SecondsTaken;
            outputValue.ShapesUsed = value1.ShapesUsed + value2.ShapesUsed;
            outputValue.VerticalFlips = value1.VerticalFlips + value2.VerticalFlips;
            outputValue.HintsUsed = value1.HintsUsed + value2.HintsUsed;

            outputValue.PuzzlesSolved = value1.PuzzlesSolved;
            outputValue.PuzzlesSolved++;

            return outputValue;
        }

        /// Add one value to another, piecewise, and return the result.
        public static PuzzleSetStatistics operator +(PuzzleSetStatistics value1, PuzzleStatistics value2)
        {
            return Add(value1, value2);
        }

        // Operator: StatisticsValue - StatisticsValue
        /// Subtract one value from another, piecewise, and return the result.
        public static PuzzleSetStatistics Subtract(PuzzleSetStatistics value1, PuzzleStatistics value2)
        {
            PuzzleSetStatistics outputValue = new PuzzleSetStatistics();

            outputValue.CCWRotations = value1.CCWRotations - value2.CCWRotations;
            outputValue.CWRotations = value1.CWRotations - value2.CWRotations;
            outputValue.HorizontalFlips = value1.HorizontalFlips - value2.HorizontalFlips;
            outputValue.SecondsTaken = value1.SecondsTaken - value2.SecondsTaken;
            outputValue.ShapesUsed = value1.ShapesUsed - value2.ShapesUsed;
            outputValue.VerticalFlips = value1.VerticalFlips - value2.VerticalFlips;
            outputValue.HintsUsed = value1.HintsUsed - value2.HintsUsed;

            outputValue.PuzzlesSolved = value1.PuzzlesSolved;
            outputValue.PuzzlesSolved--;

            return outputValue;
        }

        /// Subtract one value from another, piecewise, and return the result.
        public static PuzzleSetStatistics operator -(PuzzleSetStatistics value1, PuzzleStatistics value2)
        {
            return Subtract(value1, value2);
        }

        // String Output
        /// Builds a string that describes this object.
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("; PuzzlesSolved:");
            sb.Append(PuzzlesSolved.ToString());

            sb.Append("; ShapesUsed:");
            sb.Append(ShapesUsed.ToString());

            sb.Append("; SecondsTaken:");
            sb.Append(SecondsTaken.ToString());

            sb.Append("; CWRotations:");
            sb.Append(CWRotations.ToString());

            sb.Append("; CCWRotations:");
            sb.Append(CCWRotations.ToString());

            sb.Append("; HorizontalFlips:");
            sb.Append(HorizontalFlips.ToString());

            sb.Append("; VerticalFlips:");
            sb.Append(VerticalFlips.ToString());

            sb.Append("; HintsUsed:");
            sb.Append(HintsUsed.ToString());

            return sb.ToString();
        }

        /*
        public class PuzzleSetStatisticsReader : ContentTypeReader<PuzzleSetStatistics>
        {
            protected override PuzzleSetStatistics Read(ContentReader input, PuzzleSetStatistics existingInstance)
            {
                PuzzleSetStatistics puzzleSetStatistics = new PuzzleSetStatistics();

                puzzleSetStatistics.PuzzlesSolved = input.ReadInt32();
                puzzleSetStatistics.SecondsTaken = input.ReadDouble();
                puzzleSetStatistics.ShapesUsed = input.ReadInt32();
                puzzleSetStatistics.CWRotations = input.ReadInt32();
                puzzleSetStatistics.CCWRotations = input.ReadInt32();
                puzzleSetStatistics.HorizontalFlips = input.ReadInt32();
                puzzleSetStatistics.VerticalFlips = input.ReadInt32();
                puzzleSetStatistics.HintsUsed = input.ReadInt32();

                return puzzleSetStatistics;
            }
        }
        */

    }
}
