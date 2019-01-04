using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using ShapeShop.GameSession;

namespace ShapeShop.UI
{
    public class SaveLoadSlot
    {
        private readonly Vector2 SLOT_1_OFFSET = new Vector2(105, 80); // (115, 80)
        private readonly Vector2 SLOT_STEP = new Vector2(0, SlotPanel.STEP_OFFSETY);
        private readonly Vector2 TEXT_INDENT = new Vector2(310, 0); // (335, 0)
        private readonly string NEW_GAME_TEXT = "New";

        private readonly GameScreen parentScreen;
        private Vector2 timestampOffset, percentOffset, outOfOffset, newGameOffset, gamertagOffset;
        private Vector2 saveSlotTextOffset;

        private SaveGameDescription saveGameDescription;
        public SaveGameDescription SaveGameDescription
        {
            get { return saveGameDescription; }
            set { saveGameDescription = value; }
        }

        private bool isSelected = false;
        public bool IsSelected
        {
            get { return isSelected; }
            set { isSelected = value; }
        }

        private readonly int slotNumber;
        public int SlotNumber
        {
            get { return slotNumber; }
        }

        private Vector2 selectorOffset;
        public Vector2 SelectorOffset
        {
            get { return selectorOffset; }
        }

        private string saveSlotText
        {
            get { return "Slot " + slotNumber; }
        }

        public SaveLoadSlot(GameScreen parentScreen, int slotNumber, Vector2 selectorOffset) 
        {
            this.parentScreen = parentScreen;
            this.slotNumber = slotNumber;
            this.selectorOffset = selectorOffset;
            this.saveGameDescription = null;
        }

        public void LoadContent(ContentManager content)
        {
            Vector2 offset = Vector2.Zero;
            if (slotNumber == 1)
            {
                offset = SLOT_1_OFFSET;
            }
            else
            {
                offset = SLOT_1_OFFSET;
                offset.X += ((slotNumber - 1) * SLOT_STEP.X);
                offset.Y += ((slotNumber - 1) * SLOT_STEP.Y);
            }

            saveSlotTextOffset = offset;
            newGameOffset = offset + TEXT_INDENT;

            gamertagOffset = offset + new Vector2(0, 40);
            percentOffset = offset + TEXT_INDENT;
            outOfOffset = percentOffset + new Vector2(107, 0);
            timestampOffset = percentOffset + new Vector2(0, 40);
        }

        public void RefreshSlot(SaveGameDescription saveGameDescription)
        {
            this.saveGameDescription = saveGameDescription;
        }

        public void Update(GameTime gameTime)
        {
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 basePosition)
        {
            if (saveGameDescription == null)
            {
                spriteBatch.DrawString(Fonts.Console24Font, 
                                       NEW_GAME_TEXT,
                                       basePosition + saveSlotTextOffset, 
                                       Fonts.ConsoleTextColor);
            }
            else
            {
                spriteBatch.DrawString(Fonts.Console24Font, 
                                       "Load", 
                                       basePosition + saveSlotTextOffset,
                                       Fonts.ConsoleTextColor);
                spriteBatch.DrawString(Fonts.Console24Font, 
                                       SaveGameDescription.Gamertag, 
                                       basePosition + gamertagOffset,
                                       Fonts.ConsoleTextColor);
                int saveGamePcnt = (int)(((float)saveGameDescription.PuzzlesSolved / (float)saveGameDescription.PuzzlesTotal) * 100);
                spriteBatch.DrawString(Fonts.Console24Font, 
                                       saveGamePcnt.ToString() + "%", 
                                       basePosition + percentOffset, 
                                       Fonts.ConsoleTextColor);
                spriteBatch.DrawString(Fonts.Console24Font, 
                                       saveGameDescription.PuzzlesSolved + "/" + saveGameDescription.PuzzlesTotal, 
                                       basePosition + outOfOffset, 
                                       Fonts.ConsoleTextColor);
                spriteBatch.DrawString(Fonts.Console24Font, 
                                       saveGameDescription.TimeStamp.ToString(), 
                                       basePosition + timestampOffset, 
                                       Fonts.ConsoleTextColor);
            }
        }

        public override string ToString()
        {
            return "Slot.SaveGameDescription: " + saveGameDescription.ToString();
        }

    }
}
