using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using ShapeShop.GameEngine;
using ShapeShop.GameSession;
using ShapeShopData.Models;
using System;

namespace ShapeShop.UI
{
    class PickerConsole
    {
        private readonly Vector2 CONSOLE_TEXT_START_POSITION = new Vector2(475, 410);
        private readonly Vector2 CONSOLE_TITLE_NEWLINE = new Vector2(0, Fonts.Console24Font.MeasureString("X").Y * 1.25f);
        private readonly Vector2 CONSOLE_TEXT_NEWLINE = new Vector2(0, Fonts.Console20Font.MeasureString("X").Y * 1.25f);
        private readonly Vector2 CONSOLE_TOP_NEWCOL = new Vector2(275, 0);
        private readonly Vector2 CONSOLE_BOTTOM_NEWCOL = new Vector2(250, 0);
        private Texture2D blankTex;

        private Viewport viewport;

        public PickerConsole()
        {
        }

        public void LoadContent(ContentManager content)
        {
            viewport = Session.ScreenManager.GraphicsDevice.Viewport;

            blankTex = content.Load<Texture2D>(@"Textures\1x1");
        }

        public void Update(GameTime gameTime)
        {

        }

        public void Draw(SpriteBatch spriteBatch, Vector2 basePosition, Puzzle puzzle)
        {
            if (puzzle != null)
            {
                // draw current puzzle name
                SpriteFont nameFont = Fonts.Console28Font;

                int nameWidth = (int)nameFont.MeasureString(puzzle.Name).X + 10;
                if (nameWidth > 554)
                {
                    nameFont = Fonts.Console24Font;
                    nameWidth = (int)nameFont.MeasureString(puzzle.Name).X + 10;
                }

                Vector2 puzzleNamePosition = new Vector2((ShapeShop.PREFERRED_WIDTH / 2) - (nameFont.MeasureString(puzzle.Name).X / 2), basePosition.Y + CONSOLE_TEXT_START_POSITION.Y + 10);

                spriteBatch.Draw(blankTex, 
                                 new Rectangle((int)puzzleNamePosition.X - 5, (int)puzzleNamePosition.Y, 
                                               (int)nameWidth, (int)nameFont.MeasureString(puzzle.Name).Y + 4), 
                                 Fonts.ConsoleTextColor);

                spriteBatch.DrawString(nameFont,
                                       puzzle.Name,
                                       puzzleNamePosition,
                                       Color.Black);

                String drawStr;
                Vector2 strPosition;

                // draw puzzle solved information
                if (puzzle.IsCleared)
                {
                    TimeSpan ptime = TimeSpan.FromSeconds(puzzle.Statistics.SecondsTaken);
                    drawStr = "Solve Time: " + new DateTime(ptime.Ticks).ToString("HH:mm:ss.ff");
                    strPosition = new Vector2((ShapeShop.PREFERRED_WIDTH / 2) - (Fonts.Console24Font.MeasureString(drawStr).X / 2),
                                                       4 + basePosition.Y + CONSOLE_TEXT_START_POSITION.Y + CONSOLE_TITLE_NEWLINE.Y * 1.25f);

                    spriteBatch.DrawString(Fonts.Console24Font,
                                           drawStr,
                                           //new Vector2(0, 4) + basePosition + CONSOLE_TEXT_START_POSITION + CONSOLE_TITLE_NEWLINE * 1.25f,
                                           strPosition,
                                           Fonts.ConsoleTextColor);
                }
                // draw locked information
                else if (puzzle.IsLocked)
                {
                    strPosition = new Vector2((ShapeShop.PREFERRED_WIDTH / 2) - (Fonts.Console28Font.MeasureString("LOCKED").X / 2),
                                                         4 + basePosition.Y + CONSOLE_TEXT_START_POSITION.Y + CONSOLE_TITLE_NEWLINE.Y * 1.25f);
                    spriteBatch.DrawString(Fonts.Console28Font,
                                           "LOCKED",
                                           strPosition,
                                           Fonts.ConsoleTextColor);
                }
                else
                {
                    drawStr = "Solve Time: " + "--:--:--.--";
                    strPosition = new Vector2((ShapeShop.PREFERRED_WIDTH / 2) - (Fonts.Console24Font.MeasureString(drawStr).X / 2),
                                                       4 + basePosition.Y + CONSOLE_TEXT_START_POSITION.Y + CONSOLE_TITLE_NEWLINE.Y * 1.25f);
                    spriteBatch.DrawString(Fonts.Console24Font,
                                           drawStr,
                                           strPosition,
                                           Fonts.ConsoleTextColor);

                }

                // draw divider text
                drawStr = "--------------------------------";
                strPosition = new Vector2((ShapeShop.PREFERRED_WIDTH / 2) - (Fonts.Console24Font.MeasureString(drawStr).X / 2),
                                                      basePosition.Y + CONSOLE_TEXT_START_POSITION.Y + CONSOLE_TITLE_NEWLINE.Y * 2.25f);
                spriteBatch.DrawString(Fonts.Console24Font,
                                       drawStr,
                                       strPosition,
                                       Fonts.ConsoleTextColor);

                // draw current puzzleSetStatistics info
                drawStr = "Puzzles Solved: (" + PuzzleEngine.CurrentPuzzleSet.Statistics.PuzzlesSolved + "/" + PuzzleEngine.CurrentPuzzleSet.Puzzles.Count + ")";
                strPosition = new Vector2((ShapeShop.PREFERRED_WIDTH / 2) - (Fonts.Console22Font.MeasureString(drawStr).X / 2),
                                             basePosition.Y + CONSOLE_TEXT_START_POSITION.Y + CONSOLE_TEXT_NEWLINE.Y * 4f);
                spriteBatch.DrawString(Fonts.Console22Font,
                                       drawStr,
                                       strPosition,
                                       //basePosition + CONSOLE_TEXT_START_POSITION + CONSOLE_TEXT_NEWLINE * 4.0f,
                                       Fonts.ConsoleTextColor);

                TimeSpan time = TimeSpan.FromSeconds(PuzzleEngine.CurrentPuzzleSet.Statistics.SecondsTaken);
                if (time.Ticks > 0)
                {
                    drawStr = "Time Taken: " + new DateTime(time.Ticks).ToString("HH:mm:ss.ff");
                }
                else
                {
                    drawStr = "Time Taken: " + "--:--:--.--";
                }
                strPosition = new Vector2((ShapeShop.PREFERRED_WIDTH / 2) - (Fonts.Console22Font.MeasureString(drawStr).X / 2),
                                          basePosition.Y + CONSOLE_TEXT_START_POSITION.Y + CONSOLE_TEXT_NEWLINE.Y * 5f);
                spriteBatch.DrawString(Fonts.Console22Font,
                                       drawStr,
                                       strPosition,
                                       //basePosition + CONSOLE_TEXT_START_POSITION + CONSOLE_TEXT_NEWLINE * 5.0f,
                                       Fonts.ConsoleTextColor);

                int saveGamePct = (int)(((float)PuzzleEngine.CurrentPuzzleSet.Statistics.PuzzlesSolved / (float)PuzzleEngine.CurrentPuzzleSet.Puzzles.Count) * 100);
                drawStr = saveGamePct.ToString() + "% Complete";
                strPosition = new Vector2((ShapeShop.PREFERRED_WIDTH / 2) - (Fonts.Console22Font.MeasureString(drawStr).X / 2),
                                                      basePosition.Y + CONSOLE_TEXT_START_POSITION.Y + CONSOLE_TEXT_NEWLINE.Y * 6.0f);
                spriteBatch.DrawString(Fonts.Console22Font,
                                       drawStr,
                                       strPosition,
                                       Fonts.ConsoleTextColor);

            }
        }
    }
}

