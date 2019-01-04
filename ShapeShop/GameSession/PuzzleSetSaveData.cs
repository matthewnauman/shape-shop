using ShapeShopData.Models;
using ShapeShopData.Statistics;
using System.Collections.Generic;

namespace ShapeShop.GameSession
{
    public class PuzzleSetSaveData
    {
        public string assetName;

        public PuzzleSetStatistics puzzleSetStatistics = new PuzzleSetStatistics();

        public List<PuzzleStatistics> AllPuzzleStatistics = new List<PuzzleStatistics>();
        public List<PlayerSolution> PlayerSolutions = new List<PlayerSolution>();
        public List<bool> IsPuzzleCleared = new List<bool>();
        public List<bool> IsPuzzleLocked = new List<bool>();

        public PuzzleSetSaveData() { }

        public PuzzleSetSaveData(PuzzleSet puzzleSet)
            : this()
        {
            this.assetName = puzzleSet.AssetName;

            this.puzzleSetStatistics = puzzleSet.Statistics;

            foreach (Puzzle puzzle in puzzleSet.Puzzles)
            {
                AllPuzzleStatistics.Add(puzzle.Statistics);
                PlayerSolutions.Add(puzzle.PlayerSolution);
                IsPuzzleCleared.Add(puzzle.IsCleared);
                IsPuzzleLocked.Add(puzzle.IsLocked);
            }
        }

    }
}
