using Microsoft.Xna.Framework.Content;
using ShapeShop.GameEngine;
using ShapeShopData.Models;
using System;

namespace ShapeShop.GameSession
{
    /// All info for a player, including GamePieces
    public class Player
    {
        private string name = "Player Name";
        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        private int level = 1;
        public int Level
        {
            get { return level; }
            set { level = value; }
        }

        private readonly int slotNumber;
        public int SlotNumber
        {
            get { return slotNumber; }
        }

        public Player(int slotNumber)
        {
            this.slotNumber = slotNumber;
        }

        public Player(PlayerSaveData playerData, ContentManager content)
            : this(playerData.SlotNumber)
        {
            if (playerData == null)
            {
                throw new ArgumentNullException("playerData");
            }
            if (content == null)
            {
                throw new ArgumentNullException("content");
            }

            name = playerData.PlayerName;

            // load starting puzzleset
            PuzzleSet puzzleSet = content.Load<PuzzleSet>(playerData.PuzzleSetSaveData.assetName).Clone() as PuzzleSet;

            puzzleSet.LoadContent(content);

            // apply saved items to puzzleset
            puzzleSet.Statistics = playerData.PuzzleSetSaveData.puzzleSetStatistics;

            for (int i = 0; i < puzzleSet.Puzzles.Count; i++)
            {
                puzzleSet.Puzzles[i].IsCleared = playerData.PuzzleSetSaveData.IsPuzzleCleared[i];
                puzzleSet.Puzzles[i].IsLocked = playerData.PuzzleSetSaveData.IsPuzzleLocked[i];
                puzzleSet.Puzzles[i].Statistics = playerData.PuzzleSetSaveData.AllPuzzleStatistics[i];
                puzzleSet.Puzzles[i].PlayerSolution = playerData.PuzzleSetSaveData.PlayerSolutions[i];
                puzzleSet.Puzzles[i].IsRenderPortrait = true;
            }
            PuzzleEngine.IsCheckRender = true;

            // load updated puzzleset
            PuzzleEngine.LoadContent(content, puzzleSet);
        }        
    
    }
}
