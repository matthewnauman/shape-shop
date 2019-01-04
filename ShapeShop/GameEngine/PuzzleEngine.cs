using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using ShapeShop.GameSession;
using ShapeShop.UI;
using ShapeShopData.Models;
using ShapeShopData.Statistics;
using System;
using System.Collections.Generic;
using System.Text;

namespace ShapeShop.GameEngine
{
    /// Static class for a tileable map
    static class PuzzleEngine
    {
        private static RenderTarget2D renderTarget;
        private static readonly Vector2 TILE_OFFSET = new Vector2(-18, -18);
        public static readonly int OUTLINE_THICKNESS = 8;

        private static bool checkForSolved = false;
        public static double PuzzleTimer = 0f;

        private static MainGameScreen mainScreen = null;
        public static MainGameScreen MainScreen
        {
            get { return mainScreen; }
            set { mainScreen = value; }
        }

        public static PuzzleSet CurrentPuzzleSet
        {
            get { return puzzleSet; }
        }

        public static Puzzle CurrentPuzzle
        {
            get { return MainScreen.PickerPanel.SelectedPickerEntry.Puzzle; }
        }

        public static PickerEntry SelectedPickerEntry
        {
            get { return MainScreen.PickerPanel.SelectedPickerEntry; }
        }

        private static PuzzleSet puzzleSet;
        public static PuzzleSet PuzzleSet
        {
            get { return puzzleSet; }
        }

        /// The current position of the cursor
        private static Cursor cursor = new Cursor();
        public static Cursor Cursor
        {
            get { return cursor; }
            set { cursor = value; }
        }

        private static HintManager hintManager = new HintManager();
        public static HintManager HintManager
        {
            get { return hintManager; }
            set { hintManager = value; }
        }

        private static bool isOnReplay = false;
        public static bool IsOnReplay
        {
            get { return isOnReplay; }
            set { isOnReplay = value; }
        }
        
        private static bool isCheckRender = false;
        public static bool IsCheckRender
        {
            get { return isCheckRender; }
            set { isCheckRender = value; }
        }

        private static bool isTimerDisabled = false;
        public static bool IsTimerDisabled
        {
            get { return isTimerDisabled; }
            set { isTimerDisabled = value; }
        }

        private static bool isCursorModeActive = true;
        public static bool IsCursorModeActive
        {
            get { return isCursorModeActive; }
            set { isCursorModeActive = value; }
        }

        private static int selectedShapeKey = 0;
        public static int SelectedShapeKey
        {
            get { return selectedShapeKey; }
            set
            {
                if (value == 0)
                {
                    Cursor.CursorMode = CursorState.Free;
                }
                else
                {
                    Cursor.CursorMode = CursorState.Grab;
                }
                selectedShapeKey = value;
            }
        }

        public static Shape SelectedShape
        {
            get
            {
                if (CurrentPuzzleSet.ShapesDict.ContainsKey(SelectedShapeKey))
                {
                    return CurrentPuzzleSet.ShapesDict[SelectedShapeKey];
                }
                return null;
            }
        }

        private static Shape getShape(int key)
        {
            if (CurrentPuzzleSet.ShapesDict.ContainsKey(key))
            {
                return CurrentPuzzleSet.ShapesDict[key];
            }
            return null;
        }

        public static bool IsAShapeSelected
        {
            get
            {
                if (SelectedShapeKey == 0) return false;
                return true;
            }
        }

        public static bool IsSelectedShapeOverPuzzle
        {
            get
            {
                if (SelectedShape == null) return false;

                // added an extra half tilesize padding around board dimensiosn
                if ((SelectedShape.ScreenPosition.X - SelectedShape.ShapeOrigin.X < CurrentPuzzle.GridOrigin.X - CurrentPuzzle.TileSize.X / 2) ||
                    (SelectedShape.ScreenPosition.Y - SelectedShape.ShapeOrigin.Y < CurrentPuzzle.GridOrigin.Y - CurrentPuzzle.TileSize.Y / 2) ||
                    (SelectedShape.ScreenPosition.X + SelectedShape.ShapeOrigin.X > CurrentPuzzle.GridOrigin.X + CurrentPuzzle.Size.X + CurrentPuzzle.TileSize.X / 2) ||
                    (SelectedShape.ScreenPosition.Y + SelectedShape.ShapeOrigin.Y > CurrentPuzzle.GridOrigin.Y + CurrentPuzzle.Size.Y + CurrentPuzzle.TileSize.Y / 2))
                {
                    return false;
                }
                return true;
            }
        }

        // convenience properties for puzzle and puzzleBackground
        public static Vector2 PuzzleOrigin
        {
            get { return CurrentPuzzle.GridOrigin; }
        }

        public static Point SnapTilePosition
        {
            get
            {
                return new Point((int)((Cursor.ScreenPosition.X + (CurrentPuzzle.TileSize.X / 2) - CurrentPuzzle.GridOrigin.X) / CurrentPuzzle.TileSize.X),
                                 (int)((Cursor.ScreenPosition.Y + (CurrentPuzzle.TileSize.Y / 2) - CurrentPuzzle.GridOrigin.Y) / CurrentPuzzle.TileSize.Y));
            }
        }

        public static Point CursorTilePosition
        {
            get
            {
                return new Point((int)((Cursor.ScreenPosition.X - CurrentPuzzle.GridOrigin.X) / CurrentPuzzle.TileSize.X),
                                 (int)((Cursor.ScreenPosition.Y - CurrentPuzzle.GridOrigin.Y) / CurrentPuzzle.TileSize.Y));
            }
        }

        public static Point PuzzleTileOffset
        {
            get
            {
                return new Point((int)((Cursor.ScreenPosition.X - CurrentPuzzle.GridOrigin.X) % CurrentPuzzle.TileSize.X) - (CurrentPuzzle.TileSize.X / 2),
                                 (int)((Cursor.ScreenPosition.Y - CurrentPuzzle.GridOrigin.Y) % CurrentPuzzle.TileSize.Y) - (CurrentPuzzle.TileSize.Y / 2));

            }
        }

        public static Point GetPuzzleTilePosition(Vector2 position)
        {
            return new Point((int)((position.X - CurrentPuzzle.GridOrigin.X) / CurrentPuzzle.TileSize.X),
                             (int)((position.Y - CurrentPuzzle.GridOrigin.Y) / CurrentPuzzle.TileSize.Y));
        }

        // set the current board - refactor - rename this method?  move these 3 lines into a different method?
        public static void SetPuzzle()
        {
            if (CurrentPuzzle == null)
            {
                throw new ArgumentNullException("PuzzleEngine.currentpuzzle");
            }
            
//            AudioManager.PlayMusic(CurrentPuzzle.MusicCueName);
            PuzzleTimer = 0;
            CurrentPuzzle.Statistics = new PuzzleStatistics(CurrentPuzzle.Key, CurrentPuzzleSet.ShapesDict.Count);
            Cursor.CursorMode = CursorState.Free;
            Cursor.ScreenPosition = PuzzleEngine.ViewportCenter;

            MainScreen.PuzzlePanel.ResetSelectedCogIdx();

            hintManager.Initialize(CurrentPuzzle.OurSolution);
        }

        public static void LoadContent(ContentManager content, PuzzleSet newPuzzleSet)
        {
            if (content == null)
            {
                throw new ArgumentNullException("content");
            }
            if (newPuzzleSet == null)
            {
                throw new ArgumentNullException("newPuzzleSet");
            }
            if (newPuzzleSet.Puzzles.Count == 0)
            {
                throw new ArgumentException("No puzzles in puzzleSet");
            }

            cursor.LoadContent(content);
            puzzleSet = newPuzzleSet;            // shoudl i clone here?
        }

        /// The viewport that the tile engine is rendering within.
        private static Viewport viewport;
        public static Viewport Viewport
        {
            get { return viewport; }
            set
            {
                viewport = value;
                viewportCenter = new Vector2(viewport.X + ShapeShop.PREFERRED_WIDTH / 2f,
                                             viewport.Y + ShapeShop.PREFERRED_HEIGHT / 2f);
            }
        }

        /// The center of the current viewport.
        private static Vector2 viewportCenter;
        public static Vector2 ViewportCenter
        {
            get { return viewportCenter; }
        }

        public static void ShapeGrabTriggered()
        {
            if (MainScreen.PuzzlePanel.SelectedShapeKey == 0) { return; }

            Shape selectedShape = CurrentPuzzleSet.ShapesDict[MainScreen.PuzzlePanel.SelectedShapeKey];

            switch (selectedShape.State)
            {
                case Shape.ShapeState.Dropped:
                    Cursor.CursorMode = CursorState.Grab;
                    SelectedShapeKey = selectedShape.Key;
                    SelectedShape.State = Shape.ShapeState.Selected;
                    SelectedShape.CursorReturnOffset = Cursor.ScreenPosition - SelectedShape.Shape00Position;

                    Cursor.ScreenPosition = SelectedShape.Shape00Position;
                    CurrentPuzzle.RemoveShape(SelectedShape);

                    if (SelectedShape.IsSnapped)
                    {
                        AudioManager.PlayCue("grabSnapped");
                    }
                    else
                    {
                        AudioManager.PlayCue("grab");
                    }
//                    return;
                    break;
                case Shape.ShapeState.Waiting:
                    Cursor.CursorMode = CursorState.Grab;
                    SelectedShapeKey = selectedShape.Key;
                    SelectedShape.State = Shape.ShapeState.TransitionIn;
                    SelectedShape.CursorReturnOffset = SelectedShape.ShapeOrigin;

                    Cursor.ScreenPosition = SelectedShape.Shape00Position;

                    AudioManager.PlayCue("selectShape");
//                    return;
                    break;
            }

        }

        public static void ShapeDropTriggered()
        {
            if (SelectedShape.State == Shape.ShapeState.TransitionIn || SelectedShape.State == Shape.ShapeState.TransitionOut)
            {
                return;
            }

            if (IsSelectedShapeOverPuzzle && SelectedShape.IsSnapped)
            {
                // enter shape into shapelayer
                if (CurrentPuzzle.CanAddShape(CursorTilePosition, SelectedShape))
                {
                    CurrentPuzzle.AddShape(CursorTilePosition, SelectedShape);
                    SelectedShape.State = Shape.ShapeState.Dropped;
                    Cursor.ScreenPosition = SelectedShape.ScreenPosition;
                    SelectedShapeKey = 0;
                    Cursor.CursorMode = CursorState.Free;
                    AudioManager.PlayCue("dropSnapped");
                    checkForSolved = true;
                }
                else
                {
                    AudioManager.PlayCue("error");
                }
            }
            else
            {
                SelectedShape.State = Shape.ShapeState.Dropped;
                Cursor.ScreenPosition = SelectedShape.ScreenPosition;
                SelectedShapeKey = 0;
                Cursor.CursorMode = CursorState.Free;
                AudioManager.PlayCue("drop");
            }

        }

        public static void ShapeFlipTriggered(bool isHorizontal)
        {
            AudioManager.PlayCue("flip");

            if (isHorizontal)
            {
                CurrentPuzzle.CountHorizontalFlip();
                SelectedShape.StartFlipHorizontal();
            }
            else
            {
                CurrentPuzzle.CountVerticalFlip();
                SelectedShape.StartFlipVertical();
            }
        }

        public static void ShapeRotateTriggered(bool isCW)
        {
            AudioManager.PlayCue("rotate");

            if (isCW)
            {
                CurrentPuzzle.CountCWRotation();
                SelectedShape.StartRotationCW();
            }
            else
            {
                CurrentPuzzle.CountCCWRotation();
                SelectedShape.StartRotationCCW();
            }

            SelectedShape.ShapeOrigin = new Vector2(SelectedShape.ShapeOrigin.Y, SelectedShape.ShapeOrigin.X);
            Cursor.ScreenPosition = SelectedShape.Shape00Position;
        }

        public static void ShapeResetTriggered()
        {
            AudioManager.PlayCue("replaceShape");
            Cursor.CursorMode = CursorState.Reset;

            resetSelectedShape();
        }

        public static void ResetCurrentPuzzleOnReplay()
        {
            CurrentPuzzleSet.Statistics -= CurrentPuzzle.Statistics;
            ResetCurrentPuzzleStatistics();
            CurrentPuzzle.IsCleared = false;
            CurrentPuzzle.IsRenderPortrait = true;
            SelectedPickerEntry.IsRenderEntry = true;
            IsCheckRender = true;
        }

        public static void ResetCurrentPuzzleStatistics()
        {
            CurrentPuzzle.Statistics = new PuzzleStatistics(CurrentPuzzle.Key, CurrentPuzzleSet.ShapesDict.Count);
        }

        public static void ResetCurrentPuzzleShapes(bool invalidOnly, bool isHardReset)
        {
            bool playCue = false;

            Cursor.CursorMode = CursorState.Reset;

            if (SelectedShape != null)
            {
                resetSelectedShape();
                playCue = true;
            }

            foreach (Shape shape in CurrentPuzzleSet.ShapesDict.Values)
            {
                if (shape.State != Shape.ShapeState.Waiting)
                {
                    if (invalidOnly)
                    {
                        if (!shape.IsValid)
                        {
                            CurrentPuzzle.RemoveShape(shape);
                            if (isHardReset)
                            {
                                shape.Reset();
                                //                                shape.State = Shape.ShapeState.Resetting;
                            }
                            else
                            {
                                playCue = true;
                                shape.State = Shape.ShapeState.TransitionOut;
                            }
                        }
                    }
                    else
                    {
                        CurrentPuzzle.RemoveShape(shape);
                        if (isHardReset)
                        {
                            shape.Reset();
                        }
                        else
                        {
                            playCue = true;
                            shape.State = Shape.ShapeState.TransitionOut;
                        }
                    }
                }
            }

            if (playCue)
            {
                AudioManager.PlayCue("replaceShape");
            }

            Cursor.CursorMode = CursorState.Free;
        }

        private static void resetSelectedShape()
        {
            SelectedShape.IsDrawHighlighted = false;
            SelectedShape.State = Shape.ShapeState.TransitionOut;
            Cursor.ScreenPosition = SelectedShape.ScreenPosition;
            SelectedShapeKey = 0;
            Cursor.CursorMode = CursorState.Free;
        }

        /// Update the tile engine.
        public static void UpdatePuzzle(GameTime gameTime)
        {
            if (!isTimerDisabled)
            {
                PuzzleEngine.PuzzleTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
            }

            cursor.Update(gameTime);
            CurrentPuzzleSet.Update(gameTime);
            hintManager.Update(gameTime);

            // checkSolved will be set by shapeDropped handleInput action because any dropped shape can trigger a solution
            if (checkForSolved)
            {
                int solution = CurrentPuzzle.CheckSolved(CurrentPuzzleSet.ShapesDict);
                if (solution != 0)
                {                    
                    puzzleCleared(solution);
                }
                checkForSolved = false;
            }
        }

        public static void EndSession()
        {
            if (renderTarget != null)
            {
                renderTarget.Dispose();
                renderTarget = null;
            }

            if (puzzleSet != null)
            {
                for (int i = 0; i < puzzleSet.Puzzles.Count; i++)
                {
                    if (puzzleSet.Puzzles[i] != null && 
                        puzzleSet.Puzzles[i].PortraitTexture != null)
                    {
                        puzzleSet.Puzzles[i].PortraitTexture.Dispose();
                    }
                }

                if (puzzleSet.TileTexture != null)
                {
                    puzzleSet.TileTexture.Dispose();
                }
                puzzleSet = null;
            }
        }

        private static void puzzleCleared(int shapesUsed)
        {
            mainScreen.HelpPanel.Off(true);

            ResetCurrentPuzzleShapes(true, false);
            CurrentPuzzle.IsCleared = true;
            CurrentPuzzleSet.UnlockNextPuzzle();

            // update statistics & build solution
            PlayerSolution solution = new PlayerSolution();
            solution.PuzzleKey = CurrentPuzzle.Key;
            solution.SolutionShapes = new List<PlayerSolutionShape>();

            foreach (Shape shape in CurrentPuzzleSet.ShapesDict.Values)
            {
                if (shape.IsValid)
                {
                    Vector2 position = new Vector2(shape.DestRect.X, shape.DestRect.Y);

                    PlayerSolutionShape userShape = new PlayerSolutionShape();
                    userShape.ShapeKey = shape.Key;
                    userShape.OriginPoint = PuzzleEngine.GetPuzzleTilePosition(new Vector2(shape.DestRect.X, shape.DestRect.Y));
                    userShape.IsFlippedHorizontal = shape.IsFlippedHorizontal;
                    userShape.IsFlippedVertical = shape.IsFlippedVertical;
                    userShape.RotationIdx = shape.CurrentRotationIdx;

                    solution.SolutionShapes.Add(userShape);

                    CurrentPuzzle.Statistics.ShapesUsedKeys.Add(shape.Key);
                }
            }

            CurrentPuzzle.PlayerSolution = solution;
            CurrentPuzzle.SetCompletedStats(shapesUsed, PuzzleTimer);
            CurrentPuzzleSet.Statistics += CurrentPuzzle.Statistics;
            Cursor.CursorMode = CursorState.Hidden;
            CurrentPuzzle.IsRenderPortrait = true;
            SelectedPickerEntry.IsRenderEntry = true;
            IsCheckRender = true;
            MainScreen.PuzzlePanel.ResetPuzzleName();
            MainScreen.PuzzlePanel.InitSolutionDict(solution.SolutionShapes);
            MainScreen.PuzzlePanel.PanelState = PuzzlePanel.PuzzlePanelState.Cleared;
            MainScreen.PuzzlePanel.ClearedState = PuzzlePanel.PuzzleClearedState.Prepare;
        }

        public static void DrawTileLayers(SpriteBatch spriteBatch, Puzzle puzzle, Vector2 basePosition)
        {
            // check the parameters
            if (spriteBatch == null)
            {
                throw new ArgumentNullException("spriteBatch");
            }

            Rectangle destinationRectangle = new Rectangle(0, 0, puzzle.TileSize.X, puzzle.TileSize.Y);

            for (int y = 0; y < puzzle.Dimensions.Y; y++)
            {
                for (int x = 0; x < puzzle.Dimensions.X; x++)
                {
                    destinationRectangle.X = (int)(basePosition.X + TILE_OFFSET.X + (puzzle.IsGridShiftX ? puzzle.TileSize.X / 2 : 0)) + x * puzzle.TileSize.X;
                    destinationRectangle.Y = (int)(basePosition.Y + TILE_OFFSET.Y + (puzzle.IsGridShiftY ? puzzle.TileSize.Y / 2 : 0)) + y * puzzle.TileSize.Y;

                    Point puzzlePoint = new Point(x, y);

                    Rectangle tileSourceRectangle = puzzle.GetTileLayerSourceRectangle(puzzlePoint);

                    if (puzzle.GetPuzzleLayerValue(puzzlePoint) == 0)
                    {
                        Rectangle bgSourceRectangle = puzzle.GetBackgroundSourceRectangle(puzzlePoint);
                        spriteBatch.Draw(CurrentPuzzleSet.GetBackgroundTex(puzzle.BackgroundTextureName),
                                         destinationRectangle,
                                         bgSourceRectangle,
                                         Color.White);
                    }

                    if (tileSourceRectangle != Rectangle.Empty)
                    {
                        spriteBatch.Draw(puzzleSet.TileTexture,
                                         destinationRectangle,
                                         tileSourceRectangle,
                                         Color.White);
                    }
                }
            }

        }

        /// Drawing
        public static void DrawShapes(SpriteBatch spriteBatch)
        {
            // sort shapes with comparer for drawing (comparer sorts on isSnapped then timeIdle)
            List<Shape> shapes = new List<Shape>(CurrentPuzzleSet.ShapesDict.Values);
            shapes.Sort(new ShapeSnapAgeComparer());

            if (CurrentPuzzle.IsCleared)
            {
                for (int i = 0; i < shapes.Count; i++)
                {
                    if (shapes[i].State == Shape.ShapeState.Dropped ||
                        shapes[i].State == Shape.ShapeState.TransitionOut)
                    {
                        shapes[i].Draw(spriteBatch, Color.White, false);
                    }
                }
            }
            else if (IsAShapeSelected)
            {

                // draw dropped shapes and shapes transitioning off
                for (int i = 0; i < shapes.Count; i++)
                {
                    if (shapes[i].State == Shape.ShapeState.Dropped ||
                        shapes[i].State == Shape.ShapeState.TransitionOut || shapes[i].State == Shape.ShapeState.TransitionInAfterHint || shapes[i].State == Shape.ShapeState.TransitionOutForHint || shapes[i].State == Shape.ShapeState.WaitOnHint)
                    {
                        shapes[i].Draw(spriteBatch, Color.White, false);
                    }
                }

                // apply an effect to selected texture here
                SelectedShape.Draw(spriteBatch, Color.White, true);

            }
            else
            {
                // draw dropped shapes and shapes transitioning off

                int selectedCogShapeKey = MainScreen.PuzzlePanel.SelectedShapeKey;

                for (int i = 0; i < shapes.Count; i++)
                {
                    if (shapes[i].State == Shape.ShapeState.Dropped ||
                        shapes[i].State == Shape.ShapeState.TransitionOut ||
                        shapes[i].State == Shape.ShapeState.TransitionInAfterHint || 
                        shapes[i].State == Shape.ShapeState.TransitionOutForHint || 
                        shapes[i].State == Shape.ShapeState.WaitOnHint)
                    {
                        if (shapes[i].Key == selectedCogShapeKey &&
                            (shapes[i].State != Shape.ShapeState.TransitionInAfterHint && 
                             shapes[i].State != Shape.ShapeState.TransitionOutForHint && 
                             shapes[i].State != Shape.ShapeState.WaitOnHint))
                        {
                            shapes[i].Draw(spriteBatch, Color.White, true);
                        }
                        else // if (shapes[i].State == Shape.ShapeState.TransitionOut)
                        {
                            shapes[i].Draw(spriteBatch, Color.White, false);
                            //                            shapes[i].Draw(spriteBatch, Color.White, 1f, 1f);
                        }

                    }
                }

            }

            if (MainScreen.PuzzlePanel.PanelState == PuzzlePanel.PuzzlePanelState.Hint)
            {
                hintManager.Draw(spriteBatch);
            }

        }

        public static void RenderTextures()
        {
            // do rendering if necessary
            foreach (Puzzle puzzle in CurrentPuzzleSet.Puzzles)
            {
                if (puzzle.IsRenderPortrait)
                {
                    puzzle.PortraitTexture = renderPuzzlePortraitTexture(puzzle, CurrentPuzzleSet.ShapesDict);
                    puzzle.IsRenderPortrait = false;
                }
            }

            foreach (PickerEntry pe in mainScreen.PickerPanel.PickerEntries)
            {
                if (pe.IsRenderEntry)
                {
                    pe.EntryTexture = renderEntryTexture(pe.Puzzle);
                    pe.IsRenderEntry = false;
                }
            }

            IsCheckRender = false;
        }

        private static Texture2D renderPuzzlePortraitTexture(Puzzle puzzle, Dictionary<int, Shape> shapesDict)
        {
            SpriteBatch spriteBatch = Session.ScreenManager.SpriteBatch;
            GraphicsDevice device = Session.ScreenManager.GraphicsDevice;

            renderTarget = new RenderTarget2D(device,
                                             (int)PuzzleSet.BackgroundSize.X,
                                             (int)PuzzleSet.BackgroundSize.Y,
                                             false,
                                             SurfaceFormat.Color,
                                             DepthFormat.None,
                                             device.PresentationParameters.MultiSampleCount,
                                             RenderTargetUsage.DiscardContents);

            device.SetRenderTarget(renderTarget);

            spriteBatch.Begin();

            spriteBatch.Draw(PuzzleEngine.CurrentPuzzleSet.GetForegroundTex(puzzle.ForegroundTextureName),
                             Vector2.Zero,
                             null,
                             Color.White,
                             0f,
                             Vector2.Zero,
                             1f,
                             SpriteEffects.None,
                             0);

            PuzzleEngine.DrawTileLayers(spriteBatch, puzzle, Vector2.Zero);

            if (puzzle.IsCleared)
            {
                // no idea why i have to add 6 px to the offset??
                Vector2 offset = new Vector2(-puzzle.TileSize.X / 2 + 6, -puzzle.TileSize.Y / 2 + 6);

                // account for gridshift in puzzle
                if (puzzle.IsGridShiftX && puzzle.IsGridShiftY)
                {
                    offset += new Vector2(puzzle.TileSize.X / 2, puzzle.TileSize.Y / 2);
                }
                else if (puzzle.IsGridShiftX)
                {
                    offset.X += (puzzle.TileSize.X / 2);
                }
                else if (puzzle.IsGridShiftY)
                {
                    offset.Y += (puzzle.TileSize.Y / 2);
                }

                foreach (PlayerSolutionShape pss in puzzle.PlayerSolution.SolutionShapes)
                {
                    shapesDict[pss.ShapeKey].Render(spriteBatch, offset, pss, 1);
                }
            }

            spriteBatch.End();

            device.SetRenderTarget(null);
//            renderTarget.SaveAsJpeg(new FileStream("puzzle_"+puzzle.Place+"_"+puzzle.Key+".jpg", FileMode.Create), renderTarget.Width, renderTarget.Height);
            return renderTarget;
        }

        private static Texture2D renderEntryTexture(Puzzle puzzle)
        {
            SpriteBatch spriteBatch = Session.ScreenManager.SpriteBatch;

            GraphicsDevice device = Session.ScreenManager.GraphicsDevice;

            renderTarget = new RenderTarget2D(device,
                                              (int)(PuzzleSet.BackgroundSize.X * PickerEntry.RENDER_SCALE),
                                              (int)(ShapeShop.PREFERRED_HEIGHT / 2),
                                              false,
                                              SurfaceFormat.Color,
                                              DepthFormat.None,
                                              device.PresentationParameters.MultiSampleCount,
                                              RenderTargetUsage.DiscardContents);

            device.SetRenderTarget(renderTarget);
            device.Clear(Color.Transparent);

            spriteBatch.Begin();

            spriteBatch.Draw(puzzle.PortraitTexture,
                             PickerEntry.TOP_ENTRY_POSITION,
                             null,
                             Color.White,
                             0f,
                             Vector2.Zero,
                             PickerEntry.RENDER_SCALE,
                             SpriteEffects.None,
                             0);

            spriteBatch.End();

            device.SetRenderTarget(null);
            return renderTarget;
        }

        public static void Shuffle<T>(this IList<T> list)
        {
            Random rng = new Random();
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }

        public static string GetDebugInformation()
        {

            StringBuilder retStr = new StringBuilder("Lvl Timer : " + String.Format("{0:F1}", PuzzleEngine.PuzzleTimer) + "s" +
                   "\nPuzzle Origin : " + PuzzleOrigin.ToString() +
                   "\nSnap TilePosition : " + SnapTilePosition.ToString() +
                   "\nPuzzle TilePosition : " + CursorTilePosition.ToString() +
                   "\nPuzzle TileOffset : " + PuzzleTileOffset.ToString() +
                   "\nIsShapeOverPuzzle : " + IsSelectedShapeOverPuzzle);

            if (SelectedShape == null)
            {
                retStr.Append("\n\n\n\n\n\n\n\n\n\n\n");
                foreach (Shape shape in CurrentPuzzleSet.ShapesDict.Values)
                {
                    retStr.Append("\n" + shape.Name + " *" + (shape.IsValid ? "IsValid " : "NotValid") + "* [" + shape.State + "] (" + shape.TimeIdle + ")");
                }
            }

            return retStr.ToString();
        }

    }
}
