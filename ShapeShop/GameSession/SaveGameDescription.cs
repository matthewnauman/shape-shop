using System;

namespace ShapeShop.GameSession
{
    /// The description of a save game file.
    /// This data is saved in a separate file, and loaded by 
    /// the Load and Save Game menu screens.
    public class SaveGameDescription
    {
        /// The name of the save file with the game data. 
        public string FileName = "";

        public string Gamertag;

        public string PuzzleSetName;

        public int PuzzlesSolved;

        public int PuzzlesTotal;

        public double SolveTime;

        public DateTime TimeStamp;

        public int SaveSlot;

        public override string ToString()
        {
            return "FileName: " + FileName +
                   " GamerTag: " + Gamertag +
                   " PuzzleSetName: " + PuzzleSetName +
                   " PuzzlesSolved: " + PuzzlesSolved +
                   " PuzzlesTotal: " + PuzzlesTotal +
                   " SolveTime: " + SolveTime +
                   " TimeStamp: " + TimeStamp +
                   " SaveSlot: " + SaveSlot;

        }
    }
}
