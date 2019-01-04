using ShapeShopData.Models;
using System;

namespace ShapeShop.GameSession
{
    public class PlayerSaveData
    {
        public string PlayerName;
        public int SlotNumber;
        public PuzzleSetSaveData PuzzleSetSaveData;

        /// Creates a new PlayerData object.
        public PlayerSaveData() { }

        /// Creates a new PlayerData object from the given Player object.
        public PlayerSaveData(Player player, PuzzleSet puzzleSet)
            : this()
        {
            // check the parameter
            if (player == null)
            {
                throw new ArgumentNullException("player");
            }

            PlayerName = player.Name;
            SlotNumber = player.SlotNumber;
            PuzzleSetSaveData = new PuzzleSetSaveData(puzzleSet);
        }


    }
}
