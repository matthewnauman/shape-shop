using Microsoft.Xna.Framework.Content;
using System;
using System.Collections.Generic;
using System.Text;

namespace ShapeShopData.Statistics
{
    /// The set of relevant statistics for game pieces.
    [Serializable]
    public struct PuzzleStatistics
    {
        [ContentSerializer(Optional = true)]
        public Int32 PuzzleKey;

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

        [ContentSerializer(Optional = true)]
        public List<int> ShapesUsedKeys;

        public PuzzleStatistics(int puzzleKey, int listSize)
        {
            SecondsTaken = 0;
            ShapesUsed = 0;
            CWRotations = 0;
            CCWRotations = 0;
            HorizontalFlips = 0;
            VerticalFlips = 0;
            ShapesUsedKeys = new List<int>(listSize);
            PuzzleKey = puzzleKey;
            HintsUsed = 0;
        }

        /// Returns true if this object is trivial - all values at zero.
        public bool IsZero
        {
            get
            {
                return ((SecondsTaken == 0) &&
                        (ShapesUsed == 0) && (CWRotations == 0) &&
                        (CCWRotations == 0) && (HorizontalFlips == 0) &&
                        (VerticalFlips == 0));
            }
        }

        // Operator: StatisticsValue + StatisticsValue
        /// Add one value to another, piecewise, and return the result.
        public static PuzzleStatistics Add(PuzzleStatistics value1, PuzzleStatistics value2)
        {
            PuzzleStatistics outputValue = new PuzzleStatistics();

            outputValue.CCWRotations = value1.CCWRotations + value2.CCWRotations;
            outputValue.CWRotations = value1.CWRotations + value2.CWRotations;
            outputValue.HorizontalFlips = value1.HorizontalFlips + value2.HorizontalFlips;
            outputValue.SecondsTaken = value1.SecondsTaken + value2.SecondsTaken;
            outputValue.ShapesUsed = value1.ShapesUsed + value2.ShapesUsed;
            outputValue.VerticalFlips = value1.VerticalFlips + value2.VerticalFlips;
            outputValue.HintsUsed = value1.HintsUsed + value2.HintsUsed;

            return outputValue;
        }

        /// Add one value to another, piecewise, and return the result.
        public static PuzzleStatistics operator +(PuzzleStatistics value1, PuzzleStatistics value2)
        {
            return Add(value1, value2);
        }

        // Operator: StatisticsValue - StatisticsValue
        /// Subtract one value from another, piecewise, and return the result.
        public static PuzzleStatistics Subtract(PuzzleStatistics value1, PuzzleStatistics value2)
        {
            PuzzleStatistics outputValue = new PuzzleStatistics();

            outputValue.CCWRotations = value1.CCWRotations - value2.CCWRotations;
            outputValue.CWRotations = value1.CWRotations - value2.CWRotations;
            outputValue.HorizontalFlips = value1.HorizontalFlips - value2.HorizontalFlips;
            outputValue.SecondsTaken = value1.SecondsTaken - value2.SecondsTaken;
            outputValue.ShapesUsed = value1.ShapesUsed - value2.ShapesUsed;
            outputValue.VerticalFlips = value1.VerticalFlips - value2.VerticalFlips;
            outputValue.HintsUsed = value1.HintsUsed - value2.HintsUsed;

            return outputValue;
        }

        /// Subtract one value from another, piecewise, and return the result.
        public static PuzzleStatistics operator -(PuzzleStatistics value1, PuzzleStatistics value2)
        {
            return Subtract(value1, value2);
        }

        // Compound assignment (+=, etc.) operators use the overloaded binary operators,
        // so there is no need in this case to override them explicitly

        // String Output
        /// Builds a string that describes this object.
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

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

            return sb.ToString();
        }

        /*
        public class PuzzleStatisticsReader : ContentTypeReader<PuzzleStatistics>
        {
            protected override PuzzleStatistics Read(ContentReader input, PuzzleStatistics existingInstance)
            {
                PuzzleStatistics puzzleStatistics = new PuzzleStatistics();

                puzzleStatistics.PuzzleKey = input.ReadInt32();
                puzzleStatistics.SecondsTaken = input.ReadDouble();
                puzzleStatistics.ShapesUsed = input.ReadInt32();
                puzzleStatistics.CWRotations = input.ReadInt32();
                puzzleStatistics.CCWRotations = input.ReadInt32();
                puzzleStatistics.HorizontalFlips = input.ReadInt32();
                puzzleStatistics.VerticalFlips = input.ReadInt32();
                puzzleStatistics.HintsUsed = input.ReadInt32();
                puzzleStatistics.ShapesUsedKeys = input.ReadObject<List<int>>();

                return puzzleStatistics;
            }
        }
        */
    }
}
