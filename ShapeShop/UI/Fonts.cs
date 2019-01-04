using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace ShapeShop.UI
{
    /// Static storage of SpriteFont objects and colors for use throughout the game.
    static class Fonts
    {
        public static readonly string[] PlaceText = { "1st", "2nd", "3rd", "4th", "5th", "6th", "7th", "8th", "9th", "10th",
                                             "11th", "12th", "13th", "14th", "15th", "16th", "17th", "18th", "19th", "20th",
                                             "21st", "22nd", "23rd", "24th", "25th", "26th", "27th", "28th", "29th", "30th", };

        public static readonly string Puzzle_prefix = "_puzzle_";
        public static readonly string Cleared_prefix = "_cleared_";

        // Fonts
        // debug fonts
        private static SpriteFont debugPuzzleCoordinateFont;
        public static SpriteFont DebugPuzzleCoordinateFont
        {
            get { return debugPuzzleCoordinateFont; }
        }

        private static SpriteFont debugPanelFont;
        public static SpriteFont DebugPanelFont
        {
            get { return debugPanelFont; }
        }

        private static SpriteFont debugPanelMenuFont;
        public static SpriteFont DebugPanelMenuFont
        {
            get { return debugPanelMenuFont; }
        }

        private static SpriteFont frameRateCounterFont;
        public static SpriteFont FrameRateCounterFont
        {
            get { return frameRateCounterFont; }
        }


        // gamefonts
        private static SpriteFont messageBoxButtonFont;
        public static SpriteFont MessageBoxButtonFont
        {
            get { return messageBoxButtonFont; }
        }

        private static SpriteFont messageBoxMessageFont;
        public static SpriteFont MessageBoxMessageFont
        {
            get { return messageBoxMessageFont; }
        }

        private static SpriteFont console72OutlinedFont;
        public static SpriteFont Console72OutlinedFont
        {
            get { return console72OutlinedFont; }
        }

        private static SpriteFont console48Font;
        public static SpriteFont Console48Font
        {
            get { return console48Font; }
        }

        private static SpriteFont console42Font;
        public static SpriteFont Console42Font
        {
            get { return console42Font; }
        }

        private static SpriteFont console36Font;
        public static SpriteFont Console36Font
        {
            get { return console36Font; }
        }

        private static SpriteFont console30BoldFont;
        public static SpriteFont Console30BoldFont
        {
            get { return console30BoldFont; }
        }

        private static SpriteFont console28Font;
        public static SpriteFont Console28Font
        {
            get { return console28Font; }
        }

        private static SpriteFont console24Font;
        public static SpriteFont Console24Font
        {
            get { return console24Font; }
        }

        private static SpriteFont console22Font;
        public static SpriteFont Console22Font
        {
            get { return console22Font; }
        }

        private static SpriteFont console20Font;
        public static SpriteFont Console20Font
        {
            get { return console20Font; }
        }

        private static Dictionary<int, Color> shapeColorDict;
        public static Dictionary<int, Color> ShapeColorDict
        {
            get { return shapeColorDict; }
        }

        // Font Colors
        public static readonly Color DebugTileInfoColor = Color.White;
        public static readonly Color DebugPanelColor = new Color(0f, 0f, 0f, .8f);
        public static readonly Color DebugPanelTextColor = Color.White;

        public static readonly Color TitleColor = Color.Red;

        public static readonly Color SolvedTextColor = new Color(0, 0, 0);
        public static readonly Color SolvedStatColor = Color.DarkBlue;
        public static readonly Color SolvedSubTextColor = new Color(96, 96, 96);
        public static readonly Color SolvedSubStatColor = Color.Blue;

        public static readonly Color MessageBoxMessageTextColor = Color.White;
        public static readonly Color MessageBoxButtonTextColor = Color.White;

        public static readonly Color ShapeOutlineColor = new Color(1, 1, 1);

        public static readonly Color StatsPositiveColor = new Color(0, 255, 0);
        public static readonly Color StatsNegativeColor = new Color(255, 0, 0);
        public static readonly Color StatsNoChangeColor = new Color(255, 255, 255);

        public static readonly Color ConsoleTextColor = new Color(71, 182, 73);

        public static readonly Color CreditsStatsHighlightColor = Color.Yellow;
        public static readonly Color CreditsStatsTextColor = Color.Ivory;
        public static readonly Color CreditsStatsHeaderColor = Color.Red;
        public static readonly Color CreditsDescriptionColor = Color.Orange;
        public static readonly Color CreditsNameColor = Color.Ivory;

        public static readonly Color Shape1Color = new Color(205, 104, 6);
        public static readonly Color Shape2Color = new Color(255, 239, 0);
        public static readonly Color Shape3Color = new Color(144, 208, 32);
        public static readonly Color Shape4Color = new Color(158, 174, 43);
        public static readonly Color Shape5Color = new Color(0, 100, 44);
        public static readonly Color Shape6Color = new Color(0, 149, 179);
        public static readonly Color Shape7Color = new Color(38, 29, 215);
        public static readonly Color Shape8Color = new Color(0, 0, 98);
        public static readonly Color Shape9Color = new Color(116, 15, 130);
        public static readonly Color Shape10Color = new Color(196, 83, 158);
        public static readonly Color Shape11Color = new Color(243, 102, 114);
        public static readonly Color Shape12Color = new Color(176, 0, 0);

        // Initialization
        /// Load the fonts from the content pipeline.
        public static void LoadContent(ContentManager contentManager)
        {
            // check the parameters
            if (contentManager == null)
            {
                throw new ArgumentNullException("contentManager");
            }

            // load each font from the content pipeline
            debugPuzzleCoordinateFont = contentManager.Load<SpriteFont>("Fonts/debugCoordinateFont");
            debugPanelMenuFont = contentManager.Load<SpriteFont>("Fonts/debugPanelMenuFont");
            debugPanelFont = contentManager.Load<SpriteFont>("Fonts/debugPanelFont");
            frameRateCounterFont = contentManager.Load<SpriteFont>("Fonts/frameRateCounterFont");
            messageBoxButtonFont = contentManager.Load<SpriteFont>("Fonts/messageBoxButtonFont");
            messageBoxMessageFont = contentManager.Load<SpriteFont>("Fonts/messageBoxMessageFont");
            console72OutlinedFont = contentManager.Load<SpriteFont>("Fonts/console72OLBFont");
            console48Font = contentManager.Load<SpriteFont>("Fonts/console48Font");
            console42Font = contentManager.Load<SpriteFont>("Fonts/console42Font");
            console30BoldFont = contentManager.Load<SpriteFont>("Fonts/console30BoldFont");
            console36Font = contentManager.Load<SpriteFont>("Fonts/console36Font");
            console28Font = contentManager.Load<SpriteFont>("Fonts/console28Font");
            console24Font = contentManager.Load<SpriteFont>("Fonts/console24Font");
            console22Font = contentManager.Load<SpriteFont>("Fonts/console22Font");
            console20Font = contentManager.Load<SpriteFont>("Fonts/console20Font");

            shapeColorDict = new Dictionary<int, Color>
            {
                { 1, Shape1Color },
                { 2, Shape2Color },
                { 3, Shape3Color },
                { 4, Shape4Color },
                { 5, Shape5Color },
                { 6, Shape6Color },
                { 7, Shape7Color },
                { 8, Shape8Color },
                { 9, Shape9Color },
                { 10, Shape10Color },
                { 11, Shape11Color },
                { 12, Shape12Color }
            };
        }

        /// Release all references to the fonts.
        public static void UnloadContent()
        {
            debugPuzzleCoordinateFont = null;
            debugPanelMenuFont = null;
            debugPanelFont = null;
            frameRateCounterFont = null;
            messageBoxButtonFont = null;
            messageBoxMessageFont = null;
            console72OutlinedFont = null;
            console20Font = null;
            console22Font = null;
            console24Font = null;
            console28Font = null;
        }


        // Text Helper Methods

        /// Adds newline characters to a string so that it fits within a certain size.
        /// <param name="text">The text to be modified.</param>
        /// <param name="maximumCharactersPerLine">
        /// The maximum length of a single line of text.
        /// </param>
        /// <param name="maximumLines">The maximum number of lines to draw.</param>
        /// <returns>The new string, with newline characters if needed.</returns>
        public static string BreakTextIntoLines(string text, int maximumCharactersPerLine, int maximumLines)
        {
            if (maximumLines <= 0)
            {
                throw new ArgumentOutOfRangeException("maximumLines");
            }
            if (maximumCharactersPerLine <= 0)
            {
                throw new ArgumentOutOfRangeException("maximumCharactersPerLine");
            }

            // if the string is trivial, then this is really easy
            if (String.IsNullOrEmpty(text))
            {
                return String.Empty;
            }

            // if the text is short enough to fit on one line, then this is still easy
            if (text.Length < maximumCharactersPerLine)
            {
                return text;
            }

            // construct a new string with carriage returns
            StringBuilder stringBuilder = new StringBuilder(text);
            int currentLine = 0;
            int newLineIndex = 0;
            while (((text.Length - newLineIndex) > maximumCharactersPerLine) &&
                (currentLine < maximumLines))
            {
                text.IndexOf(' ', 0);
                int nextIndex = newLineIndex;
                while ((nextIndex >= 0) && (nextIndex < maximumCharactersPerLine))
                {
                    newLineIndex = nextIndex;
                    nextIndex = text.IndexOf(' ', newLineIndex + 1);
                }
                stringBuilder.Replace(' ', '\n', newLineIndex, 1);
                currentLine++;
            }

            return stringBuilder.ToString();
        }


        /// Adds new-line characters to a string to make it fit.
        /// <param name="text">The text to be drawn.</param>
        /// <param name="maximumCharactersPerLine">
        /// The maximum length of a single line of text.
        /// </param>
        public static string BreakTextIntoLines(string text, int maximumCharactersPerLine)
        {
            // check the parameters
            if (maximumCharactersPerLine <= 0)
            {
                throw new ArgumentOutOfRangeException("maximumCharactersPerLine");
            }

            // if the string is trivial, then this is really easy
            if (String.IsNullOrEmpty(text))
            {
                return String.Empty;
            }

            // if the text is short enough to fit on one line, then this is still easy
            if (text.Length < maximumCharactersPerLine)
            {
                return text;
            }

            // construct a new string with carriage returns
            StringBuilder stringBuilder = new StringBuilder(text);
            int currentLine = 0;
            int newLineIndex = 0;
            while (((text.Length - newLineIndex) > maximumCharactersPerLine))
            {
                text.IndexOf(' ', 0);
                int nextIndex = newLineIndex;
//                while (nextIndex < maximumCharactersPerLine)
                while ((nextIndex >= 0) && (nextIndex < maximumCharactersPerLine))
                {
                    newLineIndex = nextIndex;
                    nextIndex = text.IndexOf(' ', newLineIndex + 1);
                }
                stringBuilder.Replace(' ', '\n', newLineIndex, 1);
                currentLine++;
            }

            return stringBuilder.ToString();
        }


        /// Break text up into separate lines to make it fit.
        /// <param name="text">The text to be broken up.</param>
        /// <param name="font">The font used ot measure the width of the text.</param>
        /// <param name="rowWidth">The maximum width of each line, in pixels.</param>
        public static List<string> BreakTextIntoList(string text, SpriteFont font, int rowWidth)
        {
            // check parameters
            if (font == null)
            {
                throw new ArgumentNullException("font");
            }
            if (rowWidth <= 0)
            {
                throw new ArgumentOutOfRangeException("rowWidth");
            }

            // create the list
            List<string> lines = new List<string>();

            // check for trivial text
            if (String.IsNullOrEmpty("text"))
            {
                lines.Add(String.Empty);
                return lines;
            }

            // check for text that fits on a single line
            if (font.MeasureString(text).X <= rowWidth)
            {
                lines.Add(text);
                return lines;
            }

            // break the text up into words
            string[] words = text.Split(' ');

            // add words until they go over the length
            int currentWord = 0;
            while (currentWord < words.Length)
            {
                int wordsThisLine = 0;
                string line = String.Empty;
                while (currentWord < words.Length)
                {
                    string testLine = line;
                    if (testLine.Length < 1)
                    {
                        testLine += words[currentWord];
                    }
                    else if ((testLine[testLine.Length - 1] == '.') ||
                        (testLine[testLine.Length - 1] == '?') ||
                        (testLine[testLine.Length - 1] == '!'))
                    {
                        testLine += "  " + words[currentWord];
                    }
                    else
                    {
                        testLine += " " + words[currentWord];
                    }
                    if ((wordsThisLine > 0) &&
                        (font.MeasureString(testLine).X > rowWidth))
                    {
                        break;
                    }
                    line = testLine;
                    wordsThisLine++;
                    currentWord++;
                }
                lines.Add(line);                
            }
            return lines;
        }

        /// Break text up into separate lines to make it fit.
        /// <param name="text">The text to be broken up.</param>
        /// <param name="font">The font used ot measure the width of the text.</param>
        /// <param name="rowWidth">The maximum width of each line, in pixels.</param>
        public static List<string> BreakTextIntoList2Widths(string text, SpriteFont font, int rowWidth, int expandedWidth, int atLineNumber)
        {
            // check parameters
            if (font == null)
            {
                throw new ArgumentNullException("font");
            }
            if (rowWidth <= 0)
            {
                throw new ArgumentOutOfRangeException("rowWidth");
            }

            // create the list
            List<string> lines = new List<string>();

            // check for trivial text
            if (String.IsNullOrEmpty("text"))
            {
                lines.Add(String.Empty);
                return lines;
            }

            // check for text that fits on a single line
            if (font.MeasureString(text).X <= rowWidth)
            {
                lines.Add(text);
                return lines;
            }

            // break the text up into words
            string[] words = text.Split(' ');

            // add words until they go over the length
            int currentWord = 0;
            while (currentWord < words.Length)
            {
                int wordsThisLine = 0;
                string line = String.Empty;
                while (currentWord < words.Length)
                {
                    string testLine = line;
                    if (testLine.Length < 1)
                    {
                        testLine += words[currentWord];
                    }
                    else if ((testLine[testLine.Length - 1] == '.') ||
                        (testLine[testLine.Length - 1] == '?') ||
                        (testLine[testLine.Length - 1] == '!'))
                    {
                        testLine += "  " + words[currentWord];
                    }
                    else
                    {
                        testLine += " " + words[currentWord];
                    }
                    if ((wordsThisLine > 0) &&
                        (font.MeasureString(testLine).X > rowWidth))
                    {
                        break;
                    }
                    line = testLine;
                    wordsThisLine++;
                    currentWord++;
                }
                lines.Add(line);
                if (lines.Count >= atLineNumber)
                {
                    rowWidth = expandedWidth;
                }
            }
            return lines;
        }



        // Drawing Helper Methods


        /// Draws text centered at particular position.
        /// <param name="spriteBatch">The SpriteBatch object used to draw.</param>
        /// <param name="font">The font used to draw the text.</param>
        /// <param name="text">The text to be drawn</param>
        /// <param name="position">The center position of the text.</param>
        /// <param name="color">The color of the text.</param>
        public static void DrawCenteredText(SpriteBatch spriteBatch, SpriteFont font,
                                            string text, Vector2 position, Color color, 
                                            float scale)
        {
            // check the parameters
            if (spriteBatch == null)
            {
                throw new ArgumentNullException("spriteBatch");
            }
            if (font == null)
            {
                throw new ArgumentNullException("font");
            }

            // check for trivial text
            if (String.IsNullOrEmpty(text))
            {
                return;
            }

            // calculate the centered position
            Vector2 textSize = font.MeasureString(text) * scale;
            Vector2 centeredPosition = new Vector2(position.X - (int)textSize.X / 2,
                                                   position.Y - (int)textSize.Y / 2);

            // draw the string
            spriteBatch.DrawString(font, 
                                   text, 
                                   centeredPosition, 
                                   color, 
                                   0f,
                                   Vector2.Zero, 
                                   scale, 
                                   SpriteEffects.None, 
                                   0); //1f - position.Y / 720f);
        }

    }
}
