using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using ShapeShop.GameEngine;
using ShapeShopData.Models;
using ShapeShopData.Statistics;
using System;
using System.Collections.Generic;

namespace ShapeShop.UI
{
    public class CreditsConsole
    {
        public enum ConsoleState
        {
            Waiting,
            Scrolling,
            FadeInA,
            Finished,
        }

        private readonly float TEXT_START_Y = 40;
        private const float HEADING_SCALE = .80f;
        private const float SUBHEADING_SCALE = .65f;
        private const float TEXT_SCALE = .55f; //.65f;
        private const float SHAPE_TEX_COL = 210;
        private const float SHAPE_TEXT_COL = 360;
        private const float SHAPE_VALUE_COL = 490;
        private const float SHAPE_TEXT_Y_OFFSET = 61f;
        private const float SHAPE_VALUE_Y_OFFSET = 49f;
        private const float FAVORITE_SCALE_FACTOR = 0.05f;
        private const float FAVORITE_BASE_SCALE = 2.0f;
        private const float ALPHA_STEP = .04f;
        private const float STATS_COL_X = 598;

        private readonly float NEWLINE_HEADING = Fonts.Console72OutlinedFont.MeasureString("X").Y * HEADING_SCALE;
        private readonly float NEWLINE_SUBHEADING = Fonts.Console72OutlinedFont.MeasureString("X").Y * SUBHEADING_SCALE;
        private readonly float NEWLINE_TEXT = Fonts.Console72OutlinedFont.MeasureString("X").Y * TEXT_SCALE;
        private const float WAIT_TIMEOUT = 5f;
        private const float SCROLL_SPEED = .5f;

        private ConsoleState state = ConsoleState.Waiting;
        public ConsoleState State
        {
            get { return state; }
        }

        private readonly string congrats1Text = "Congratulations!!!";
        private readonly string congrats2Text = "You have completed all of";
        private readonly string congrats3Text = "the puzzles!";

        private readonly string statsTitleText = "Game Statistics";
        private readonly string puzzlesSolvedText = "Puzzles Solved: ";
        private readonly string totalTimeText =     "Total Time Taken: ";
        private readonly string shapesUsedText =    "Shapes Used: ";
        private readonly string hintsUsedText = "Hints Used: ";

        private readonly string longestTime1Text = "Puzzle That Took The Longest";
        private readonly string longestTime2Text = "Time To Complete";

        private readonly string shapeBreakdownText = "Shape Breakdown";

        private readonly string shapeText = "Used     times.";
        private readonly string favoriteShapeText = "Favorite Shape";
        private readonly string creditsTitleText = "Credits";
        private readonly string creditsProgrammerText = "Programming";
        private readonly string creditsProgrammerNameText = "Matthew Nauman";
        private readonly string creditsDesignerText = "Graphics & Design";
        private readonly string creditsDesignerNameText = "William Gilbert";
        private readonly string creditsCreativeText = "Creative Consultant";
        private readonly string creditsCreativeNameText = "Ian Finch";
        private readonly string creditsThanksText = "Special Thanks";
        private readonly string creditsThanksName1Text = "Sarah Lisjak";
        private readonly string creditsThanksName2Text = "Katie Martin";

        private readonly string thanksForPlayingText = "Thank You For Playing!";

        private Vector2 congrats1Position, congrats2Position, congrats3Position;
        private Vector2 statsHeaderPosition;
        private Vector2 puzzlesSolvedTextPosition, puzzlesSolvedPosition;
        private Vector2 totalTimeTextPosition, totalTimePosition;
        private Vector2 shapesUsedTextPosition, shapesUsedPosition;
        private Vector2 shapeBreakdownTextPosition;
        private Vector2 favoriteShapeTextPosition;
        private Vector2 creditsHeaderPosition;
        private Vector2 creditsProgrammerTextPosition, creditsProgrammerNameTextPosition;
        private Vector2 creditsDesignerTextPosition, creditsDesignerNameTextPosition;
        private Vector2 creditsCreativeTextPosition, creditsCreativeNameTextPosition;
        private Vector2 creditsThanksTextPosition, creditsThanksName1TextPosition, creditsThanksName2TextPosition;
        private Vector2 thanksForPlayingTextPosition;
        private Vector2 aButtonTexPosition;
        private Vector2 longestTime1TextPosition, longestTime2TextPosition, hintsUsedTextPosition, hintsUsedPosition;
        private Vector2 longestPuzzleNamePosition, longestPuzzleTimePosition;

        private Vector2 shape1TexPosition, shape1TextPosition, shape1ValuePosition;
        private Vector2 shape2TexPosition, shape2TextPosition, shape2ValuePosition;
        private Vector2 shape3TexPosition, shape3TextPosition, shape3ValuePosition;
        private Vector2 shape4TexPosition, shape4TextPosition, shape4ValuePosition;
        private Vector2 shape5TexPosition, shape5TextPosition, shape5ValuePosition;
        private Vector2 shape6TexPosition, shape6TextPosition, shape6ValuePosition;
        private Vector2 shape7TexPosition, shape7TextPosition, shape7ValuePosition;
        private Vector2 shape8TexPosition, shape8TextPosition, shape8ValuePosition;
        private Vector2 shape9TexPosition, shape9TextPosition, shape9ValuePosition;
        private Vector2 shape10TexPosition, shape10TextPosition, shape10ValuePosition;
        private Vector2 shape11TexPosition, shape11TextPosition, shape11ValuePosition;
        private Vector2 shape12TexPosition, shape12TextPosition, shape12ValuePosition;

        private Texture2D aButtonTex;

        private Viewport viewport;
        private CreditsPanel parentPanel;

        private string totalTime;
        private int puzzlesSolved;
        private int hintsUsed;
        private int shapesUsed;
        private float waitTimer = 0;
        private Dictionary<int, Texture2D> shapeTexDict;
        List<KeyValuePair<int, int>> sortedShapeCountList;

        private float dScroll = 0;
        private float favoriteScale = 1;
        private Texture2D favoriteShapeTex;
        private Vector2 favoriteShapePosition;
        private Vector2 favoriteShapeOrigin;
        private ContentManager content;

        private const float LONGEST_PUZZLE_SCALE = .75f;
        private Texture2D longestPuzzleTexture;
        private Vector2 longestPuzzlePosition;
        private string longestPuzzleTime;
        private string longestPuzzleName;
        private float aButtonAlpha = 0;

        public CreditsConsole(CreditsPanel parentPanel)
        {
            this.parentPanel = parentPanel;
        }

        public void LoadContent(ContentManager content)
        {
            this.content = content;
            this.viewport = parentPanel.Viewport;

            shapeTexDict = new Dictionary<int, Texture2D>
            {
                { 1, content.Load<Texture2D>(@"Textures\Shapes\smShape1") },
                { 2, content.Load<Texture2D>(@"Textures\Shapes\smShape2") },
                { 3, content.Load<Texture2D>(@"Textures\Shapes\smShape3") },
                { 4, content.Load<Texture2D>(@"Textures\Shapes\smShape4") },
                { 5, content.Load<Texture2D>(@"Textures\Shapes\smShape5") },
                { 6, content.Load<Texture2D>(@"Textures\Shapes\smShape6") },
                { 7, content.Load<Texture2D>(@"Textures\Shapes\smShape7") },
                { 8, content.Load<Texture2D>(@"Textures\Shapes\smShape8") },
                { 9, content.Load<Texture2D>(@"Textures\Shapes\smShape9") },
                { 10, content.Load<Texture2D>(@"Textures\Shapes\smShape10") },
                { 11, content.Load<Texture2D>(@"Textures\Shapes\smShape11") },
                { 12, content.Load<Texture2D>(@"Textures\Shapes\smShape12") }
            };

            aButtonTex = content.Load<Texture2D>(@"Textures\Buttons\AButton");
        }

        public void LoadStatistics()
        {
            // init shapes dict
            Dictionary<int, int> shapeCountDict = new Dictionary<int, int>();
            foreach (Shape shape in PuzzleEngine.CurrentPuzzleSet.ShapesDict.Values)
            {
                shapeCountDict.Add(shape.Key, 0);
            }

            foreach (Puzzle puzzle in PuzzleEngine.CurrentPuzzleSet.Puzzles)
            {
                foreach (int key in puzzle.Statistics.ShapesUsedKeys)
                {
                    shapeCountDict[key]++;
                }
            }

            List<PuzzleStatistics> statsList = new List<PuzzleStatistics>();
            foreach (Puzzle p in PuzzleEngine.CurrentPuzzleSet.Puzzles)
            {
                statsList.Add(p.Statistics);
            }
            statsList.Sort(new PuzzleStatisticsSecondsTakenComparer());

            foreach (Puzzle p in PuzzleEngine.CurrentPuzzleSet.Puzzles)
            {
                if (p.Key == statsList[0].PuzzleKey)
                {
                    longestPuzzleTexture = p.PortraitTexture;
                    TimeSpan time = TimeSpan.FromSeconds(p.Statistics.SecondsTaken);
                    longestPuzzleTime = new DateTime(time.Ticks).ToString("HH:mm:ss.ff");
                    longestPuzzleName = p.Name;
                }
            }

            sortedShapeCountList = new List<KeyValuePair<int, int>>(shapeCountDict);
            sortedShapeCountList.Sort((firstPair, nextPair) =>
            {
                return firstPair.Value.CompareTo(nextPair.Value);
            }
            );

            favoriteShapeTex = content.Load<Texture2D>(@"Textures\Shapes\Shape" + sortedShapeCountList[11].Key + "HL");

            TimeSpan ptime = TimeSpan.FromSeconds(PuzzleEngine.CurrentPuzzleSet.Statistics.SecondsTaken);
            totalTime = new DateTime(ptime.Ticks).ToString("HH:mm:ss.ff");
            puzzlesSolved = PuzzleEngine.CurrentPuzzleSet.Statistics.PuzzlesSolved;
            shapesUsed = PuzzleEngine.CurrentPuzzleSet.Statistics.ShapesUsed;
            hintsUsed = PuzzleEngine.CurrentPuzzleSet.Statistics.HintsUsed;

            float yCursorPosition = TEXT_START_Y;

            // congrats
            congrats1Position = new Vector2(parentPanel.PanelCenter - (Fonts.Console72OutlinedFont.MeasureString(congrats1Text).X / 2), yCursorPosition);
            yCursorPosition += Fonts.Console72OutlinedFont.MeasureString("X").Y;
            congrats2Position = new Vector2(parentPanel.PanelCenter - ((Fonts.Console72OutlinedFont.MeasureString(congrats2Text).X * SUBHEADING_SCALE) / 2), yCursorPosition);
            yCursorPosition += NEWLINE_SUBHEADING;
            congrats3Position = new Vector2(parentPanel.PanelCenter - ((Fonts.Console72OutlinedFont.MeasureString(congrats3Text).X * SUBHEADING_SCALE) / 2), yCursorPosition);
            yCursorPosition += NEWLINE_HEADING * 1.25f;

            // puzzleSet stats
            statsHeaderPosition = new Vector2(parentPanel.PanelCenter - ((Fonts.Console72OutlinedFont.MeasureString(statsTitleText).X * HEADING_SCALE) / 2), yCursorPosition);
            yCursorPosition += NEWLINE_HEADING * 1f;

            puzzlesSolvedTextPosition = new Vector2(STATS_COL_X - (Fonts.Console72OutlinedFont.MeasureString(puzzlesSolvedText).X * TEXT_SCALE), yCursorPosition);
            puzzlesSolvedPosition = new Vector2(STATS_COL_X, puzzlesSolvedTextPosition.Y);
            yCursorPosition += NEWLINE_TEXT;

            hintsUsedTextPosition = new Vector2(STATS_COL_X - (Fonts.Console72OutlinedFont.MeasureString(hintsUsedText).X * TEXT_SCALE), yCursorPosition);
            hintsUsedPosition = new Vector2(STATS_COL_X, hintsUsedTextPosition.Y);
            yCursorPosition += NEWLINE_TEXT;

            totalTimeTextPosition = new Vector2(STATS_COL_X - (Fonts.Console72OutlinedFont.MeasureString(totalTimeText).X * TEXT_SCALE), yCursorPosition);
            totalTimePosition = new Vector2(STATS_COL_X, totalTimeTextPosition.Y);
            yCursorPosition += NEWLINE_TEXT;

            shapesUsedTextPosition = new Vector2(STATS_COL_X - (Fonts.Console72OutlinedFont.MeasureString(shapesUsedText).X * TEXT_SCALE), yCursorPosition);
            shapesUsedPosition = new Vector2(STATS_COL_X, shapesUsedTextPosition.Y);
            yCursorPosition += NEWLINE_HEADING * 1.25f;

            // shape breakdown
            shapeBreakdownTextPosition = new Vector2(parentPanel.PanelCenter - ((Fonts.Console72OutlinedFont.MeasureString(shapeBreakdownText).X * SUBHEADING_SCALE) / 2), yCursorPosition);

            yCursorPosition += NEWLINE_TEXT * .5f;

            shape1TexPosition = new Vector2(SHAPE_TEX_COL, yCursorPosition);
            shape1TextPosition = new Vector2(SHAPE_TEXT_COL, yCursorPosition + SHAPE_TEXT_Y_OFFSET);
            shape1ValuePosition = new Vector2(SHAPE_VALUE_COL, yCursorPosition + SHAPE_VALUE_Y_OFFSET);
            yCursorPosition += NEWLINE_HEADING;

            shape2TexPosition = new Vector2(SHAPE_TEX_COL, yCursorPosition);
            shape2TextPosition = new Vector2(SHAPE_TEXT_COL, yCursorPosition + SHAPE_TEXT_Y_OFFSET);
            shape2ValuePosition = new Vector2(SHAPE_VALUE_COL, yCursorPosition + SHAPE_VALUE_Y_OFFSET);
            yCursorPosition += NEWLINE_HEADING;

            shape3TexPosition = new Vector2(SHAPE_TEX_COL, yCursorPosition);
            shape3TextPosition = new Vector2(SHAPE_TEXT_COL, yCursorPosition + SHAPE_TEXT_Y_OFFSET);
            shape3ValuePosition = new Vector2(SHAPE_VALUE_COL, yCursorPosition + SHAPE_VALUE_Y_OFFSET);
            yCursorPosition += NEWLINE_HEADING;

            shape4TexPosition = new Vector2(SHAPE_TEX_COL, yCursorPosition);
            shape4TextPosition = new Vector2(SHAPE_TEXT_COL, yCursorPosition + SHAPE_TEXT_Y_OFFSET);
            shape4ValuePosition = new Vector2(SHAPE_VALUE_COL, yCursorPosition + SHAPE_VALUE_Y_OFFSET);
            yCursorPosition += NEWLINE_HEADING;

            shape5TexPosition = new Vector2(SHAPE_TEX_COL, yCursorPosition);
            shape5TextPosition = new Vector2(SHAPE_TEXT_COL, yCursorPosition + SHAPE_TEXT_Y_OFFSET);
            shape5ValuePosition = new Vector2(SHAPE_VALUE_COL, yCursorPosition + SHAPE_VALUE_Y_OFFSET);
            yCursorPosition += NEWLINE_HEADING;

            shape6TexPosition = new Vector2(SHAPE_TEX_COL, yCursorPosition);
            shape6TextPosition = new Vector2(SHAPE_TEXT_COL, yCursorPosition + SHAPE_TEXT_Y_OFFSET);
            shape6ValuePosition = new Vector2(SHAPE_VALUE_COL, yCursorPosition + SHAPE_VALUE_Y_OFFSET);
            yCursorPosition += NEWLINE_HEADING;

            shape7TexPosition = new Vector2(SHAPE_TEX_COL, yCursorPosition);
            shape7TextPosition = new Vector2(SHAPE_TEXT_COL, yCursorPosition + SHAPE_TEXT_Y_OFFSET);
            shape7ValuePosition = new Vector2(SHAPE_VALUE_COL, yCursorPosition + SHAPE_VALUE_Y_OFFSET);
            yCursorPosition += NEWLINE_HEADING;

            shape8TexPosition = new Vector2(SHAPE_TEX_COL, yCursorPosition);
            shape8TextPosition = new Vector2(SHAPE_TEXT_COL, yCursorPosition + SHAPE_TEXT_Y_OFFSET);
            shape8ValuePosition = new Vector2(SHAPE_VALUE_COL, yCursorPosition + SHAPE_VALUE_Y_OFFSET);
            yCursorPosition += NEWLINE_HEADING;

            shape9TexPosition = new Vector2(SHAPE_TEX_COL, yCursorPosition);
            shape9TextPosition = new Vector2(SHAPE_TEXT_COL, yCursorPosition + SHAPE_TEXT_Y_OFFSET);
            shape9ValuePosition = new Vector2(SHAPE_VALUE_COL, yCursorPosition + SHAPE_VALUE_Y_OFFSET);
            yCursorPosition += NEWLINE_HEADING;

            shape10TexPosition = new Vector2(SHAPE_TEX_COL, yCursorPosition);
            shape10TextPosition = new Vector2(SHAPE_TEXT_COL, yCursorPosition + SHAPE_TEXT_Y_OFFSET);
            shape10ValuePosition = new Vector2(SHAPE_VALUE_COL, yCursorPosition + SHAPE_VALUE_Y_OFFSET);
            yCursorPosition += NEWLINE_HEADING;

            shape11TexPosition = new Vector2(SHAPE_TEX_COL, yCursorPosition);
            shape11TextPosition = new Vector2(SHAPE_TEXT_COL, yCursorPosition + SHAPE_TEXT_Y_OFFSET);
            shape11ValuePosition = new Vector2(SHAPE_VALUE_COL, yCursorPosition + SHAPE_VALUE_Y_OFFSET);
            yCursorPosition += NEWLINE_HEADING;

            shape12TexPosition = new Vector2(SHAPE_TEX_COL, yCursorPosition);
            shape12TextPosition = new Vector2(SHAPE_TEXT_COL, yCursorPosition + SHAPE_TEXT_Y_OFFSET);
            shape12ValuePosition = new Vector2(SHAPE_VALUE_COL, yCursorPosition + SHAPE_VALUE_Y_OFFSET);

            yCursorPosition += NEWLINE_HEADING * 2f;

            // favorite shape
            favoriteShapeTextPosition = new Vector2(parentPanel.PanelCenter - ((Fonts.Console72OutlinedFont.MeasureString(favoriteShapeText).X * SUBHEADING_SCALE) / 2), yCursorPosition);
            yCursorPosition += NEWLINE_HEADING * 1.75f;

            favoriteShapePosition = new Vector2(parentPanel.PanelCenter - (favoriteShapeTex.Width / 2), yCursorPosition);
            favoriteShapeOrigin = new Vector2(favoriteShapeTex.Width / 2, favoriteShapeTex.Height / 2);
            yCursorPosition += (NEWLINE_HEADING * 2.5f);

            //longest puzzle
            longestTime1TextPosition = new Vector2(parentPanel.PanelCenter - ((Fonts.Console72OutlinedFont.MeasureString(longestTime1Text).X * SUBHEADING_SCALE) / 2), yCursorPosition);
            yCursorPosition += NEWLINE_TEXT;
            longestTime2TextPosition = new Vector2(parentPanel.PanelCenter - ((Fonts.Console72OutlinedFont.MeasureString(longestTime2Text).X * SUBHEADING_SCALE) / 2), yCursorPosition);
            yCursorPosition += NEWLINE_HEADING * 1.25f;
            longestPuzzleNamePosition = new Vector2(parentPanel.PanelCenter - ((Fonts.Console72OutlinedFont.MeasureString(longestPuzzleName).X * TEXT_SCALE) / 2), yCursorPosition);
            yCursorPosition += NEWLINE_TEXT;
            longestPuzzlePosition = new Vector2(parentPanel.PanelCenter - ((longestPuzzleTexture.Width / 2) * LONGEST_PUZZLE_SCALE), yCursorPosition);
            yCursorPosition += (longestPuzzleTexture.Height * LONGEST_PUZZLE_SCALE);
            longestPuzzleTimePosition = new Vector2(parentPanel.PanelCenter - ((Fonts.Console72OutlinedFont.MeasureString(longestPuzzleTime).X * TEXT_SCALE) / 2), yCursorPosition);

            yCursorPosition += (NEWLINE_HEADING * 4);

            // credits
            creditsHeaderPosition = new Vector2(parentPanel.PanelCenter - ((Fonts.Console72OutlinedFont.MeasureString(creditsTitleText).X * HEADING_SCALE) / 2), yCursorPosition);
            yCursorPosition += NEWLINE_HEADING * 1.5f;

            creditsDesignerTextPosition = new Vector2(parentPanel.PanelCenter - ((Fonts.Console72OutlinedFont.MeasureString(creditsDesignerText).X * TEXT_SCALE) / 2), yCursorPosition);
            yCursorPosition += NEWLINE_TEXT * 1.1f;

            creditsDesignerNameTextPosition = new Vector2(parentPanel.PanelCenter - ((Fonts.Console72OutlinedFont.MeasureString(creditsDesignerNameText).X * TEXT_SCALE) / 2), yCursorPosition);
            yCursorPosition += NEWLINE_TEXT * 2f;

            creditsProgrammerTextPosition = new Vector2(parentPanel.PanelCenter - ((Fonts.Console72OutlinedFont.MeasureString(creditsProgrammerText).X * TEXT_SCALE) / 2), yCursorPosition);
            yCursorPosition += NEWLINE_TEXT * 1.1f;

            creditsProgrammerNameTextPosition = new Vector2(parentPanel.PanelCenter - ((Fonts.Console72OutlinedFont.MeasureString(creditsProgrammerNameText).X * TEXT_SCALE) / 2), yCursorPosition);
            yCursorPosition += NEWLINE_TEXT * 2f;

            creditsCreativeTextPosition = new Vector2(parentPanel.PanelCenter - ((Fonts.Console72OutlinedFont.MeasureString(creditsCreativeText).X * TEXT_SCALE) / 2), yCursorPosition);
            yCursorPosition += NEWLINE_TEXT * 1.1f;

            creditsCreativeNameTextPosition = new Vector2(parentPanel.PanelCenter - ((Fonts.Console72OutlinedFont.MeasureString(creditsCreativeNameText).X * TEXT_SCALE) / 2), yCursorPosition);
            yCursorPosition += NEWLINE_TEXT * 2f;

            creditsThanksTextPosition = new Vector2(parentPanel.PanelCenter - ((Fonts.Console72OutlinedFont.MeasureString(creditsThanksText).X * TEXT_SCALE) / 2), yCursorPosition);
            yCursorPosition += NEWLINE_TEXT * 1.1f;

            creditsThanksName1TextPosition = new Vector2(parentPanel.PanelCenter - ((Fonts.Console72OutlinedFont.MeasureString(creditsThanksName1Text).X * TEXT_SCALE) / 2), yCursorPosition);
            yCursorPosition += NEWLINE_TEXT;
            creditsThanksName2TextPosition = new Vector2(parentPanel.PanelCenter - ((Fonts.Console72OutlinedFont.MeasureString(creditsThanksName2Text).X * TEXT_SCALE) / 2), yCursorPosition);
            yCursorPosition += (NEWLINE_HEADING * 5);

            thanksForPlayingTextPosition = new Vector2(parentPanel.PanelCenter - ((Fonts.Console72OutlinedFont.MeasureString(thanksForPlayingText).X * HEADING_SCALE) / 2), yCursorPosition);
            yCursorPosition += NEWLINE_HEADING;
            aButtonTexPosition = new Vector2(parentPanel.PanelCenter - (aButtonTex.Width / 2), yCursorPosition);

        }

        public void Update(GameTime gameTime)
        {
            if (parentPanel.PanelState == CreditsPanel.CreditsPanelState.Open)
            {
                // pulsate favorite shape
                double time = gameTime.TotalGameTime.TotalSeconds;
                float pulsate = (float)Math.Sin(time * 5) + 1;
                favoriteScale = FAVORITE_BASE_SCALE + pulsate * FAVORITE_SCALE_FACTOR;

//                double time = gameTime.TotalGameTime.TotalSeconds;
//                float pulsate = (float)Math.Sin(time * 6) + 1;
//                float scale = 1 + pulsate * 0.05f;

                switch (state)
                {
                    case ConsoleState.Waiting:
                        waitTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;

                        if (waitTimer >= WAIT_TIMEOUT)
                        {
                            state = ConsoleState.Scrolling;
                            waitTimer = 0;
                        }
                        break;
                    case ConsoleState.Scrolling:
                        dScroll -= SCROLL_SPEED;
                        break;
                    case ConsoleState.FadeInA:
                        aButtonAlpha += ALPHA_STEP;
                        if (aButtonAlpha >= 1)
                        {
                            aButtonAlpha = 1;
                            state = ConsoleState.Finished;
                        }
                        break;
                    case ConsoleState.Finished:
                        break;
                }

            }

        }

        public void NotifyBackgroundFinished()
        {
            state = ConsoleState.FadeInA;
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 basePosition)
        {
            basePosition.Y += dScroll;

            // congrats
            spriteBatch.DrawString(Fonts.Console72OutlinedFont,
                                   congrats1Text,
                                   basePosition + congrats1Position,
                                   Fonts.CreditsStatsHighlightColor,
                                   0,
                                   Vector2.Zero,
                                   1,
                                   SpriteEffects.None,
                                   0);

            spriteBatch.DrawString(Fonts.Console72OutlinedFont,
                                   congrats2Text,
                                   basePosition + congrats2Position,
                                   Fonts.CreditsStatsTextColor,
                                   0,
                                   Vector2.Zero,
                                   SUBHEADING_SCALE,
                                   SpriteEffects.None,
                                   0);


            spriteBatch.DrawString(Fonts.Console72OutlinedFont,
                                   congrats3Text,
                                   basePosition + congrats3Position,
                                   Fonts.CreditsStatsTextColor,
                                   0,
                                   Vector2.Zero,
                                   SUBHEADING_SCALE,
                                   SpriteEffects.None,
                                   0);

            // stats heading
            spriteBatch.DrawString(Fonts.Console72OutlinedFont,
                                   statsTitleText,
                                   basePosition + statsHeaderPosition,
                                   Fonts.CreditsStatsHeaderColor,
                                   0,
                                   Vector2.Zero,
                                   HEADING_SCALE,
                                   SpriteEffects.None,
                                   0);

            // puzzles solved text
            spriteBatch.DrawString(Fonts.Console72OutlinedFont,
                                   puzzlesSolvedText,// + puzzlesSolved,
                                   basePosition + puzzlesSolvedTextPosition,
                                   Fonts.CreditsStatsTextColor,
                                   0,
                                   Vector2.Zero,
                                   TEXT_SCALE,
                                   SpriteEffects.None,
                                   0);
            spriteBatch.DrawString(Fonts.Console72OutlinedFont,
                                   puzzlesSolved.ToString(),
                                   basePosition + puzzlesSolvedPosition,
                                   Fonts.CreditsStatsHighlightColor,
                                   0,
                                   Vector2.Zero,
                                   TEXT_SCALE,
                                   SpriteEffects.None,
                                   0);

            // hints used
            spriteBatch.DrawString(Fonts.Console72OutlinedFont,
                                   hintsUsedText,// + puzzlesSolved,
                                   basePosition + hintsUsedTextPosition,
                                   Fonts.CreditsStatsTextColor,
                                   0,
                                   Vector2.Zero,
                                   TEXT_SCALE,
                                   SpriteEffects.None,
                                   0);
            spriteBatch.DrawString(Fonts.Console72OutlinedFont,
                                   hintsUsed.ToString(),
                                   basePosition + hintsUsedPosition,
                                   Fonts.CreditsStatsHighlightColor,
                                   0,
                                   Vector2.Zero,
                                   TEXT_SCALE,
                                   SpriteEffects.None,
                                   0);

            // total time text
            spriteBatch.DrawString(Fonts.Console72OutlinedFont,
                                   totalTimeText,// + totalTime,
                                   basePosition + totalTimeTextPosition,
                                   Fonts.CreditsStatsTextColor,
                                   0,
                                   Vector2.Zero,
                                   TEXT_SCALE,
                                   SpriteEffects.None,
                                   0);
            spriteBatch.DrawString(Fonts.Console72OutlinedFont,
                                   totalTime,
                                   basePosition + totalTimePosition,
                                   Fonts.CreditsStatsHighlightColor,
                                   0,
                                   Vector2.Zero,
                                   TEXT_SCALE,
                                   SpriteEffects.None,
                                   0);

            // shapes used text
            spriteBatch.DrawString(Fonts.Console72OutlinedFont,
                                   shapesUsedText,// + shapesUsed,
                                   basePosition + shapesUsedTextPosition,
                                   Fonts.CreditsStatsTextColor,
                                   0,
                                   Vector2.Zero,
                                   TEXT_SCALE,
                                   SpriteEffects.None,
                                   0);
            spriteBatch.DrawString(Fonts.Console72OutlinedFont,
                                   shapesUsed.ToString(),
                                   basePosition + shapesUsedPosition,
                                   Fonts.CreditsStatsHighlightColor,
                                   0,
                                   Vector2.Zero,
                                   TEXT_SCALE,
                                   SpriteEffects.None,
                                   0);

            if (parentPanel.PanelState == CreditsPanel.CreditsPanelState.Open || parentPanel.PanelState == CreditsPanel.CreditsPanelState.Closing)
            {
                // shape breakdown sub heading
                spriteBatch.DrawString(Fonts.Console72OutlinedFont,
                                       shapeBreakdownText,
                                       basePosition + shapeBreakdownTextPosition,
                                       Fonts.CreditsStatsHeaderColor,
                                       0,
                                       Vector2.Zero,
                                       SUBHEADING_SCALE,
                                       SpriteEffects.None,
                                       0);

                spriteBatch.Draw(shapeTexDict[sortedShapeCountList[0].Key], basePosition + shape1TexPosition, Color.White);
                spriteBatch.DrawString(Fonts.Console72OutlinedFont,
                                       shapeText,
                                       basePosition + shape1TextPosition,
                                       Fonts.CreditsStatsTextColor,
                                       0,
                                       Vector2.Zero,
                                       TEXT_SCALE,
                                       SpriteEffects.None,
                                       0);
                spriteBatch.DrawString(Fonts.Console72OutlinedFont,
                                       sortedShapeCountList[0].Value.ToString(),
                                       basePosition + shape1ValuePosition,
                                       Fonts.ShapeColorDict[sortedShapeCountList[0].Key],
                                       0,
                                       Vector2.Zero,
                                       HEADING_SCALE,
                                       SpriteEffects.None,
                                       0);

                spriteBatch.Draw(shapeTexDict[sortedShapeCountList[1].Key], basePosition + shape2TexPosition, Color.White);
                spriteBatch.DrawString(Fonts.Console72OutlinedFont,
                                       shapeText,
                                       basePosition + shape2TextPosition,
                                       Fonts.CreditsStatsTextColor,
                                       0,
                                       Vector2.Zero,
                                       TEXT_SCALE,
                                       SpriteEffects.None,
                                       0);
                spriteBatch.DrawString(Fonts.Console72OutlinedFont,
                                       sortedShapeCountList[1].Value.ToString(),
                                       basePosition + shape2ValuePosition,
                                       Fonts.ShapeColorDict[sortedShapeCountList[1].Key],
                                       0,
                                       Vector2.Zero,
                                       HEADING_SCALE,
                                       SpriteEffects.None,
                                       0);

                spriteBatch.Draw(shapeTexDict[sortedShapeCountList[2].Key], basePosition + shape3TexPosition, Color.White);
                spriteBatch.DrawString(Fonts.Console72OutlinedFont,
                                       shapeText,
                                       basePosition + shape3TextPosition,
                                       Fonts.CreditsStatsTextColor,
                                       0,
                                       Vector2.Zero,
                                       TEXT_SCALE,
                                       SpriteEffects.None,
                                       0);
                spriteBatch.DrawString(Fonts.Console72OutlinedFont,
                                       sortedShapeCountList[2].Value.ToString(),
                                       basePosition + shape3ValuePosition,
                                       Fonts.ShapeColorDict[sortedShapeCountList[2].Key],
                                       0,
                                       Vector2.Zero,
                                       HEADING_SCALE,
                                       SpriteEffects.None,
                                       0);

                spriteBatch.Draw(shapeTexDict[sortedShapeCountList[3].Key], basePosition + shape4TexPosition, Color.White);
                spriteBatch.DrawString(Fonts.Console72OutlinedFont,
                                       shapeText,
                                       basePosition + shape4TextPosition,
                                       Fonts.CreditsStatsTextColor,
                                       0,
                                       Vector2.Zero,
                                       TEXT_SCALE,
                                       SpriteEffects.None,
                                       0);
                spriteBatch.DrawString(Fonts.Console72OutlinedFont,
                                       sortedShapeCountList[3].Value.ToString(),
                                       basePosition + shape4ValuePosition,
                                       Fonts.ShapeColorDict[sortedShapeCountList[3].Key],
                                       0,
                                       Vector2.Zero,
                                       HEADING_SCALE,
                                       SpriteEffects.None,
                                       0);
                
                spriteBatch.Draw(shapeTexDict[sortedShapeCountList[4].Key], basePosition + shape5TexPosition, Color.White);
                spriteBatch.DrawString(Fonts.Console72OutlinedFont,
                                       shapeText,
                                       basePosition + shape5TextPosition,
                                       Fonts.CreditsStatsTextColor,
                                       0,
                                       Vector2.Zero,
                                       TEXT_SCALE,
                                       SpriteEffects.None,
                                       0);
                spriteBatch.DrawString(Fonts.Console72OutlinedFont,
                                       sortedShapeCountList[4].Value.ToString(),
                                       basePosition + shape5ValuePosition,
                                       Fonts.ShapeColorDict[sortedShapeCountList[4].Key],
                                       0,
                                       Vector2.Zero,
                                       HEADING_SCALE,
                                       SpriteEffects.None,
                                       0);
                
                spriteBatch.Draw(shapeTexDict[sortedShapeCountList[5].Key], basePosition + shape6TexPosition, Color.White);
                spriteBatch.DrawString(Fonts.Console72OutlinedFont,
                                       shapeText,
                                       basePosition + shape6TextPosition,
                                       Fonts.CreditsStatsTextColor,
                                       0,
                                       Vector2.Zero,
                                       TEXT_SCALE,
                                       SpriteEffects.None,
                                       0);
                spriteBatch.DrawString(Fonts.Console72OutlinedFont,
                                       sortedShapeCountList[5].Value.ToString(),
                                       basePosition + shape6ValuePosition,
                                       Fonts.ShapeColorDict[sortedShapeCountList[5].Key],
                                       0,
                                       Vector2.Zero,
                                       HEADING_SCALE,
                                       SpriteEffects.None,
                                       0);
                
                spriteBatch.Draw(shapeTexDict[sortedShapeCountList[6].Key], basePosition + shape7TexPosition, Color.White);
                spriteBatch.DrawString(Fonts.Console72OutlinedFont,
                                       shapeText,
                                       basePosition + shape7TextPosition,
                                       Fonts.CreditsStatsTextColor,
                                       0,
                                       Vector2.Zero,
                                       TEXT_SCALE,
                                       SpriteEffects.None,
                                       0);
                spriteBatch.DrawString(Fonts.Console72OutlinedFont,
                                       sortedShapeCountList[6].Value.ToString(),
                                       basePosition + shape7ValuePosition,
                                       Fonts.ShapeColorDict[sortedShapeCountList[6].Key],
                                       0,
                                       Vector2.Zero,
                                       HEADING_SCALE,
                                       SpriteEffects.None,
                                       0);
                
                spriteBatch.Draw(shapeTexDict[sortedShapeCountList[7].Key], basePosition + shape8TexPosition, Color.White);
                spriteBatch.DrawString(Fonts.Console72OutlinedFont,
                                       shapeText,
                                       basePosition + shape8TextPosition,
                                       Fonts.CreditsStatsTextColor,
                                       0,
                                       Vector2.Zero,
                                       TEXT_SCALE,
                                       SpriteEffects.None,
                                       0);
                spriteBatch.DrawString(Fonts.Console72OutlinedFont,
                                       sortedShapeCountList[7].Value.ToString(),
                                       basePosition + shape8ValuePosition,
                                       Fonts.ShapeColorDict[sortedShapeCountList[7].Key],
                                       0,
                                       Vector2.Zero,
                                       HEADING_SCALE,
                                       SpriteEffects.None,
                                       0);
                
                spriteBatch.Draw(shapeTexDict[sortedShapeCountList[8].Key], basePosition + shape9TexPosition, Color.White);
                spriteBatch.DrawString(Fonts.Console72OutlinedFont,
                                       shapeText,
                                       basePosition + shape9TextPosition,
                                       Fonts.CreditsStatsTextColor,
                                       0,
                                       Vector2.Zero,
                                       TEXT_SCALE,
                                       SpriteEffects.None,
                                       0);
                spriteBatch.DrawString(Fonts.Console72OutlinedFont,
                                       sortedShapeCountList[8].Value.ToString(),
                                       basePosition + shape9ValuePosition,
                                       Fonts.ShapeColorDict[sortedShapeCountList[8].Key],
                                       0,
                                       Vector2.Zero,
                                       HEADING_SCALE,
                                       SpriteEffects.None,
                                       0);
                
                spriteBatch.Draw(shapeTexDict[sortedShapeCountList[9].Key], basePosition + shape10TexPosition, Color.White);
                spriteBatch.DrawString(Fonts.Console72OutlinedFont,
                                       shapeText,
                                       basePosition + shape10TextPosition,
                                       Fonts.CreditsStatsTextColor,
                                       0,
                                       Vector2.Zero,
                                       TEXT_SCALE,
                                       SpriteEffects.None,
                                       0);
                spriteBatch.DrawString(Fonts.Console72OutlinedFont,
                                       sortedShapeCountList[9].Value.ToString(),
                                       basePosition + shape10ValuePosition,
                                       Fonts.ShapeColorDict[sortedShapeCountList[9].Key],
                                       0,
                                       Vector2.Zero,
                                       HEADING_SCALE,
                                       SpriteEffects.None,
                                       0);
                
                spriteBatch.Draw(shapeTexDict[sortedShapeCountList[10].Key], basePosition + shape11TexPosition, Color.White);
                spriteBatch.DrawString(Fonts.Console72OutlinedFont,
                                       shapeText,
                                       basePosition + shape11TextPosition,
                                       Fonts.CreditsStatsTextColor,
                                       0,
                                       Vector2.Zero,
                                       TEXT_SCALE,
                                       SpriteEffects.None,
                                       0);
                spriteBatch.DrawString(Fonts.Console72OutlinedFont,
                                       sortedShapeCountList[10].Value.ToString(),
                                       basePosition + shape11ValuePosition,
                                       Fonts.ShapeColorDict[sortedShapeCountList[10].Key],
                                       0,
                                       Vector2.Zero,
                                       HEADING_SCALE,
                                       SpriteEffects.None,
                                       0);
                
                spriteBatch.Draw(shapeTexDict[sortedShapeCountList[11].Key], basePosition + shape12TexPosition, Color.White);
                spriteBatch.DrawString(Fonts.Console72OutlinedFont,
                                       shapeText,
                                       basePosition + shape12TextPosition,
                                       Fonts.CreditsStatsTextColor,
                                       0,
                                       Vector2.Zero,
                                       TEXT_SCALE,
                                       SpriteEffects.None,
                                       0);
                spriteBatch.DrawString(Fonts.Console72OutlinedFont,
                                       sortedShapeCountList[11].Value.ToString(),
                                       basePosition + shape12ValuePosition,
                                       Fonts.ShapeColorDict[sortedShapeCountList[11].Key],
                                       0,
                                       Vector2.Zero,
                                       HEADING_SCALE,
                                       SpriteEffects.None,
                                       0);

                // favorite shape
                spriteBatch.DrawString(Fonts.Console72OutlinedFont,
                                       favoriteShapeText,
                                       basePosition + favoriteShapeTextPosition,
                                       Fonts.CreditsStatsHeaderColor,
                                       0,
                                       Vector2.Zero,
                                       SUBHEADING_SCALE,
                                       SpriteEffects.None,
                                       0);
                spriteBatch.Draw(favoriteShapeTex,
                                 basePosition + favoriteShapePosition + favoriteShapeOrigin,
                                 null,
                                 Color.White,
                                 0,
                                 favoriteShapeOrigin,
                                 favoriteScale,
                                 SpriteEffects.None,
                                 0);

                // longest time
                spriteBatch.DrawString(Fonts.Console72OutlinedFont,
                                       longestTime1Text,
                                       basePosition + longestTime1TextPosition,
                                       Fonts.CreditsStatsHeaderColor,
                                       0,
                                       Vector2.Zero,
                                       SUBHEADING_SCALE,
                                       SpriteEffects.None,
                                       0);
                spriteBatch.DrawString(Fonts.Console72OutlinedFont,
                                       longestTime2Text,
                                       basePosition + longestTime2TextPosition,
                                       Fonts.CreditsStatsHeaderColor,
                                       0,
                                       Vector2.Zero,
                                       SUBHEADING_SCALE,
                                       SpriteEffects.None,
                                       0);
                spriteBatch.DrawString(Fonts.Console72OutlinedFont,
                                       longestPuzzleTime,
                                       basePosition + longestPuzzleTimePosition,
                                       Fonts.CreditsStatsTextColor,
                                       0,
                                       Vector2.Zero,
                                       TEXT_SCALE,
                                       SpriteEffects.None,
                                       0);
                spriteBatch.Draw(longestPuzzleTexture,
                                 basePosition + longestPuzzlePosition,
                                 null,
                                 Color.White,
                                 0,
                                 Vector2.Zero,
                                 LONGEST_PUZZLE_SCALE,
                                 SpriteEffects.None,
                                 0);
                spriteBatch.DrawString(Fonts.Console72OutlinedFont,
                                       longestPuzzleName,
                                       basePosition + longestPuzzleNamePosition,
                                       Fonts.CreditsDescriptionColor,
                                       0,
                                       Vector2.Zero,
                                       TEXT_SCALE,
                                       SpriteEffects.None,
                                       0);

                // credits
                spriteBatch.DrawString(Fonts.Console72OutlinedFont,
                                       creditsTitleText,
                                       basePosition + creditsHeaderPosition,
                                       Fonts.CreditsStatsHeaderColor,
                                       0,
                                       Vector2.Zero,
                                       HEADING_SCALE,
                                       SpriteEffects.None,
                                       0);

                spriteBatch.DrawString(Fonts.Console72OutlinedFont,
                                       creditsDesignerText,
                                       basePosition + creditsDesignerTextPosition,
                                       Fonts.CreditsDescriptionColor,
                                       0,
                                       Vector2.Zero,
                                       TEXT_SCALE,
                                       SpriteEffects.None,
                                       0);
                spriteBatch.DrawString(Fonts.Console72OutlinedFont,
                                       creditsDesignerNameText,
                                       basePosition + creditsDesignerNameTextPosition,
                                       Fonts.CreditsNameColor,
                                       0,
                                       Vector2.Zero,
                                       TEXT_SCALE,
                                       SpriteEffects.None,
                                       0);

                spriteBatch.DrawString(Fonts.Console72OutlinedFont,
                                       creditsProgrammerText,
                                       basePosition + creditsProgrammerTextPosition,
                                       Fonts.CreditsDescriptionColor,
                                       0,
                                       Vector2.Zero,
                                       TEXT_SCALE,
                                       SpriteEffects.None,
                                       0);
                spriteBatch.DrawString(Fonts.Console72OutlinedFont,
                                       creditsProgrammerNameText,
                                       basePosition + creditsProgrammerNameTextPosition,
                                       Fonts.CreditsNameColor,
                                       0,
                                       Vector2.Zero,
                                       TEXT_SCALE,
                                       SpriteEffects.None,
                                       0);

                spriteBatch.DrawString(Fonts.Console72OutlinedFont,
                                       creditsCreativeText,
                                       basePosition + creditsCreativeTextPosition,
                                       Fonts.CreditsDescriptionColor,
                                       0,
                                       Vector2.Zero,
                                       TEXT_SCALE,
                                       SpriteEffects.None,
                                       0);
                spriteBatch.DrawString(Fonts.Console72OutlinedFont,
                                       creditsCreativeNameText,
                                       basePosition + creditsCreativeNameTextPosition,
                                       Fonts.CreditsNameColor,
                                       0,
                                       Vector2.Zero,
                                       TEXT_SCALE,
                                       SpriteEffects.None,
                                       0);

                spriteBatch.DrawString(Fonts.Console72OutlinedFont,
                                       creditsThanksText,
                                       basePosition + creditsThanksTextPosition,
                                       Fonts.CreditsDescriptionColor,
                                       0,
                                       Vector2.Zero,
                                       TEXT_SCALE,
                                       SpriteEffects.None,
                                       0);
                spriteBatch.DrawString(Fonts.Console72OutlinedFont,
                                       creditsThanksName1Text,
                                       basePosition + creditsThanksName1TextPosition,
                                       Fonts.CreditsNameColor,
                                       0,
                                       Vector2.Zero,
                                       TEXT_SCALE,
                                       SpriteEffects.None,
                                       0);
                spriteBatch.DrawString(Fonts.Console72OutlinedFont,
                                       creditsThanksName2Text,
                                       basePosition + creditsThanksName2TextPosition,
                                       Fonts.CreditsNameColor,
                                       0,
                                       Vector2.Zero,
                                       TEXT_SCALE,
                                       SpriteEffects.None,
                                       0);
                
                // thanks for playing
                spriteBatch.DrawString(Fonts.Console72OutlinedFont,
                                       thanksForPlayingText,
                                       basePosition + thanksForPlayingTextPosition,
                                       Fonts.CreditsStatsHighlightColor,
                                       0,
                                       Vector2.Zero,
                                       HEADING_SCALE,
                                       SpriteEffects.None,
                                       0);

                spriteBatch.Draw(aButtonTex, 
                                 basePosition + aButtonTexPosition, 
                                 new Color(1, 1, 1, aButtonAlpha));

            }



        }


    }
}

