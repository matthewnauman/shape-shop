using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using ShapeShop.GameEngine;
using ShapeShop.GameSession;
using ShapeShopData.Models;
using System.Collections.Generic;
using System.Text;

namespace ShapeShop.UI
{
    public class PuzzlePanel
    {
        public enum PuzzlePanelState
        {
            Opening,
            Open,
            Startup,
            Running,
            Hint,
            Cleared,
            Stamp,
            PreShutdown,
            Shutdown,
            Closing,
            Closed,
            Ending,
        }

        public enum GameEndingState
        {
            PanelPhase2Rev,
            PanelRetract,
            PanelPhase1Rev,
        }

        public enum PuzzleRunningState
        {
            FadeInName,
            ShowName,
            FadeOutName,
            NameFinished,
        }

        public enum PuzzleHintState
        {
            StartHint,
            FadeInHint,
            ShowHint,
            FadeOutHint,
            HintFinished,
        }

        public enum PuzzleStampState
        {
            Deploy,
            Deployed,
            Wait1,
            Clamp,
            Wait2_1,
            PuzzleChange,
            Wait2_2,
            Unclamp,
            Wait3,
            Retract,
            Finished,
        }

        public enum PuzzleClearedState
        {
            Prepare,
            FlashSolution,
            FadeInMessage,
            ShowMessage,
            FadeOutMessage,
            Finished,
        }

        public enum PanelStartupState
        {
            Phase1,
            Extend,
            Phase2,
        }

        public enum PanelShutdownState
        {
            Phase2Rev,
            Retract,
            Phase1Rev,
        }

        // puzzleCog shapeKey that selector will default to
        public static readonly int DEFAULT_COG_KEY = 2;
        private const float ANCHOR_STAMP_X = 140;
        private const float ANCHOR_STAMP_CLOSED_Y = 724; // can be arbitrarily large as long as > 724, bigger int = longer wait before stamp comes in
        private const float ANCHOR_STAMP_OPEN_Y = 4;
        private readonly Vector2 STAMP_POSITION_STEP = new Vector2(0, -12f);
        private const float STAMP_CLAMP_SCALE_STEP = .05f;
        private const float STAMP_UNCLAMP_SCALE_STEP = .01f;
        private const float STAMP_SCALE_MIN = .664f;
        private const float STAMP_WAIT2_LIMIT = .75f; //.5f;

        public static readonly int SLIDE_ARM_OFFSET = 90;
        public static readonly int ROTATION_ARM_OFFSET = 130;
        private const float PUZZLECOG_STARTSCALE = .85f;
        private const float ANCHOR_PANEL_X = 298;
        private const float ANCHOR_PANELCLOSED_Y = 894; //864; // -581
        private const float ANCHOR_PANELOPEN_Y = 114; //144;
        private const float PANEL_SPEED = 14f;
        private const float SELECTOR_ALPHA = 1;
        private const float SHUFFLE_MAX = 7f;
        private const string CLEARED_MESSAGE = "Completed!";
        private const float CLEARED_MESSAGE_ALPHA_STEP = .075f;
        private readonly Vector2 CLEARED_MESSAGE_START_OFFSET = new Vector2(0, 50);
        private readonly Vector2 CLEARED_MESSAGE_POSITION_STEP = new Vector2(0, -1);
        private const int CLEARED_MESSAGE_SHOW_TIME = 100;

        private const float PUZZLE_NAME_ALPHA_STEP = .075f;
        private readonly Vector2 PUZZLE_NAME_POSITION = PuzzleEngine.ViewportCenter + new Vector2(0, 200);
        private const int PUZZLE_NAME_SHOW_TIME = 175;

        private const float RESET_PUZZLE_BUTTON_TIMER_LIMIT = 1.5f;

        private float shuffleTimer = 0;
        private float stampWaitTimer = 0;
        
        private Sprite stampSprite;
        private Sprite selectorSprite;
        private Texture2D puzzleFrameTexture;
        private Vector2 puzzleFrameOffset;

        private Vector2 basePosition;
        public Vector2 BasePosition
        {
            get { return basePosition; }
        }

        private Vector2 clearedMessagePosition = Vector2.Zero;
        private float clearedMessageAlpha = 0f;
        private int clearedMessageShowTimer = CLEARED_MESSAGE_SHOW_TIME;

        private float puzzleNameAlpha = 0f;
        private int puzzleNameShowTimer = PUZZLE_NAME_SHOW_TIME;

        private MainGameScreen parentScreen;

        private int oldSelectedCogIdx;
        private int selectedCogIdx;
        public int SelectedShapeKey
        {
            get { return selectedCogIdx + 1; }
        }

        public PuzzleCog SelectedCog
        {
            get 
            {
                if (selectedCogIdx < 0) { return null; }
                return cogs[selectedCogIdx];
            }
        }

        private List<PuzzleCog> cogs;
        public List<PuzzleCog> Cogs
        {
            get { return cogs; }
        }

        private PanelStartupState startupState; // = StartupState.Phase1;
        public PanelStartupState StartupState
        {
            get { return startupState; }
        }

        private PanelShutdownState shutdownState; // = ShutdownState.Phase2Rev;
        public PanelShutdownState ShutdownState
        {
            get { return shutdownState; }
        }

        private PuzzlePanelState panelState = PuzzlePanelState.Closed;
        public PuzzlePanelState PanelState
        {
            get { return panelState; }
            set { panelState = value; }
        }

        private PuzzleClearedState clearedState = PuzzleClearedState.Prepare;
        public PuzzleClearedState ClearedState
        {
            get { return clearedState; }
            set { clearedState = value; }
        }

        private PuzzleHintState hintState = PuzzleHintState.HintFinished;
        public PuzzleHintState HintState
        {
            get { return hintState; }
            set { hintState = value; }
        }

        private PuzzleStampState stampState = PuzzleStampState.Deploy;
        private PuzzleRunningState runningState = PuzzleRunningState.FadeInName;
        
        private GameEndingState endingState;
        public GameEndingState EndingState
        {
            get { return endingState; }
        }

        private bool isReadyForCredits = false;

        private const float FLASH_TIMER_MAX = 0.2f;
        private float flashTimer = 0;
        private Dictionary<int, bool> flashShapeDict;

        private bool isFlashSolutionComplete
        {
            get
            {
                foreach (bool isDone in flashShapeDict.Values)
                {
                    if (!isDone) { return false; }
                }
                return true;
            }
        }

        private bool isClearedMessageState
        {
            get
            {
                if (clearedState == PuzzleClearedState.FadeInMessage ||
                    clearedState == PuzzleClearedState.ShowMessage ||
                    clearedState == PuzzleClearedState.FadeOutMessage)
                {
                    return true;
                }
                return false;
            }
        }

        // states that indicate the puzzlePanel is runnign, used in draw methods
        public bool IsRunning
        {
            get 
            { 
                return (panelState == PuzzlePanelState.Running || 
                        panelState == PuzzlePanelState.PreShutdown || 
                        panelState == PuzzlePanelState.Cleared ||
                        panelState == PuzzlePanelState.Stamp ||
                        panelState == PuzzlePanelState.Hint); 
            }
        }

        private bool isPhase1Finished
        {
            get
            {
                foreach (PuzzleCog cog in cogs)
                {
                    if (cog.MovementType == PuzzleCog.CogMovementType.Slide && !cog.IsPhase1Finished) { return false; }
                }
                return true;
            }

            set
            {
                foreach (PuzzleCog cog in cogs) { cog.IsPhase1Finished = value; }
            }
        }

        private bool isPhaseExtendFinished
        {
            get
            {
                foreach (PuzzleCog cog in cogs)
                {
                    if (cog.MovementType == PuzzleCog.CogMovementType.Slide && !cog.IsPhaseExtendFinished) { return false; }
                }
                return true;
            }

            set
            {
                foreach (PuzzleCog cog in cogs) { cog.IsPhaseExtendFinished = value; }
            }
        }

        private bool isPhase2Finished
        {
            get
            {
                foreach (PuzzleCog cog in cogs)
                {
                    if (cog.MovementType == PuzzleCog.CogMovementType.Slide && !cog.IsPhase2Finished) { return false; }
                }
                return true;
            }

            set
            {
                foreach (PuzzleCog cog in cogs) { cog.IsPhase2Finished = value; }
            }
        }

        private bool isPhase2RevFinished
        {
            get
            {
                foreach (PuzzleCog cog in cogs)
                {
                    if (cog.MovementType == PuzzleCog.CogMovementType.Slide && !cog.IsPhase2RevFinished) { return false; }
                }
                return true;
            }

            set
            {
                foreach (PuzzleCog cog in cogs) { cog.IsPhase2RevFinished = value; }
            }
        }

        private bool isPhaseRetractFinished
        {
            get
            {
                foreach (PuzzleCog cog in cogs)
                {
                    if (cog.MovementType == PuzzleCog.CogMovementType.Slide && !cog.IsPhaseRetractFinished) { return false; }
                }
                return true;
            }

            set
            {
                foreach (PuzzleCog cog in cogs) { cog.IsPhaseRetractFinished = value; }
            }
        }

        private bool isPhase1RevFinished
        {
            get
            {
                foreach (PuzzleCog cog in cogs)
                {
                    if (cog.MovementType == PuzzleCog.CogMovementType.Slide && !cog.IsPhase1RevFinished) { return false; }
                }
                return true;
            }

            set
            {
                foreach (PuzzleCog cog in cogs) { cog.IsPhase1RevFinished = value; }
            }
        }

        private bool isCogsZeroed
        {
            get
            {
                foreach (PuzzleCog cog in cogs)
                {
                    if (cog.CogRotation != 0) { return false; }
                }
                return true;
            }
        }

        /// Create a new GameplayScreen object from a new-game description.
        public PuzzlePanel(MainGameScreen parentScreen)
        {
            this.parentScreen = parentScreen;

            cogs = new List<PuzzleCog>();
        }

        public void LoadContent(ContentManager content)
        {

            selectorSprite = new Sprite(content.Load<Texture2D>(@"Textures\Cogs\pCogSelector"),
                                        new Color(1, 1, 1, SELECTOR_ALPHA),
                                        Vector2.Zero,
                                        null,
                                        1,
                                        0);

            stampSprite = new Sprite(content.Load<Texture2D>(@"Textures\GameScreens\Puzzle\puzzleStamp"),
                                     Color.White,
                                     new Vector2(ANCHOR_STAMP_X, ANCHOR_STAMP_CLOSED_Y),
                                     new Vector2(500, 356),
                                     1,
                                     0);

            puzzleFrameTexture = content.Load<Texture2D>(@"Textures\GameScreens\Puzzle\PuzzleFrame");

            basePosition = new Vector2(ANCHOR_PANEL_X, ANCHOR_PANELCLOSED_Y);

            puzzleFrameOffset = Vector2.Zero;

            PuzzleCog cog;
            cog = new PuzzleCog(this, 1, Color.DeepSkyBlue,
                                new Vector2(150, 65),
                                new Vector2(150, -315),
                                new Vector2(90, 280),
                                new Vector2(150, -65),
                                PuzzleCog.CogRotationDirection.CCW,
                                PuzzleCog.CogRotationDirection.CW);
            cogs.Add(cog);

            cog = new PuzzleCog(this, 2, Color.Crimson,
                                new Vector2(326, 154),
                                new Vector2(326, -44),
                                new Vector2(328, -13),
                                new Vector2(326, 154) + new Vector2(0, SLIDE_ARM_OFFSET),
                                PUZZLECOG_STARTSCALE, // 96
                                1f,
                                PuzzleCog.CogSlideDirection.Up,
                                PuzzleCog.CogRotationDirection.CW);
            cogs.Add(cog);
            
            cog = new PuzzleCog(this, 3, Color.DarkGreen,
                                new Vector2(548, 142),
                                new Vector2(548, -56),
                                new Vector2(550, -25),
                                new Vector2(548, 142) + new Vector2(0, SLIDE_ARM_OFFSET),
                                PUZZLECOG_STARTSCALE, // 96
                                1f,
                                PuzzleCog.CogSlideDirection.Up,
                                PuzzleCog.CogRotationDirection.CW);
            cogs.Add(cog);

            cog = new PuzzleCog(this, 4, Color.DarkSalmon,
                                new Vector2(771, 154),
                                new Vector2(771, -44),
                                new Vector2(773, -13),
                                new Vector2(771, 154) + new Vector2(0, SLIDE_ARM_OFFSET),
                                PUZZLECOG_STARTSCALE, // 96
                                1f,
                                PuzzleCog.CogSlideDirection.Up,
                                PuzzleCog.CogRotationDirection.CW);
            cogs.Add(cog);

            cog = new PuzzleCog(this, 5, Color.DarkOrange,
                                new Vector2(950, 65),
                                new Vector2(950, -315),
                                new Vector2(90, 280),
                                new Vector2(950, -65),
                                PuzzleCog.CogRotationDirection.CW,
                                PuzzleCog.CogRotationDirection.CW);
            cogs.Add(cog);

            cog = new PuzzleCog(this, 6, Color.MediumBlue,
                                new Vector2(777, 272),
                                new Vector2(972, 272),
                                new Vector2(950, 272),
                                new Vector2(777, 272) + new Vector2(-SLIDE_ARM_OFFSET, 0),
                                PUZZLECOG_STARTSCALE, // 92
                                1f,
                                PuzzleCog.CogSlideDirection.Right,
                                PuzzleCog.CogRotationDirection.CW);
            cogs.Add(cog);

            cog = new PuzzleCog(this, 7, Color.Lime,
                                new Vector2(950, 475),
                                new Vector2(950, 725),
                                new Vector2(90, 30),
                                new Vector2(950, 475) + new Vector2(0, ROTATION_ARM_OFFSET),
                                PuzzleCog.CogRotationDirection.CCW,
                                PuzzleCog.CogRotationDirection.CW);
            cogs.Add(cog);

            cog = new PuzzleCog(this, 8, Color.Gold,
                                new Vector2(771, 388),
                                new Vector2(771, 592),
                                new Vector2(773, 555),
                                new Vector2(771, 388) + new Vector2(0, -SLIDE_ARM_OFFSET),
                                PUZZLECOG_STARTSCALE, // 96
                                1f,
                                PuzzleCog.CogSlideDirection.Down,
                                PuzzleCog.CogRotationDirection.CW);
            cogs.Add(cog);
    
            cog = new PuzzleCog(this, 9, Color.Indigo,
                                new Vector2(548, 397),
                                new Vector2(548, 601),
                                new Vector2(550, 564),
                                new Vector2(548, 397) + new Vector2(0, -SLIDE_ARM_OFFSET),
                                PUZZLECOG_STARTSCALE, // 96
                                1f,
                                PuzzleCog.CogSlideDirection.Down,
                                PuzzleCog.CogRotationDirection.CW);
            cogs.Add(cog);

            cog = new PuzzleCog(this, 10, Color.DeepPink,
                                new Vector2(326, 388),
                                new Vector2(326, 592),
                                new Vector2(328, 555),
                                new Vector2(326, 388) + new Vector2(0, -SLIDE_ARM_OFFSET),
                                PUZZLECOG_STARTSCALE, // 96
                                1f,
                                PuzzleCog.CogSlideDirection.Down,
                                PuzzleCog.CogRotationDirection.CW);
            cogs.Add(cog);

            cog = new PuzzleCog(this, 11, Color.Coral,
                                new Vector2(150, 475),
                                new Vector2(150, 725),
                                new Vector2(90, 30),
                                new Vector2(150, 475) + new Vector2(0, ROTATION_ARM_OFFSET),
                                PuzzleCog.CogRotationDirection.CW,
                                PuzzleCog.CogRotationDirection.CW);
            cogs.Add(cog);

            cog = new PuzzleCog(this, 12, Color.SpringGreen,
                                new Vector2(325, 272),
                                new Vector2(142, 272),
                                new Vector2(150, 272), // new Vector2(170, 270),
                                new Vector2(325, 272) + new Vector2(SLIDE_ARM_OFFSET, 0),
                                PUZZLECOG_STARTSCALE, // 92
                                1f,
                                PuzzleCog.CogSlideDirection.Left,
                                PuzzleCog.CogRotationDirection.CW);
            cogs.Add(cog);

            foreach (PuzzleCog c in cogs)
            {
                c.LoadContent(content);
            }

            // default to cog1
            selectedCogIdx = 0;
            SelectedCog.IsSelected = true;
            selectorSprite.Position = SelectedCog.Position;
        }

        public void ResetSelectedCogIdx()
        {
            selectedCogIdx = -1;
        }

        public void Open()
        {
            AudioManager.PlayCue("trackSlide");
            panelState = PuzzlePanelState.Opening;
        }

        public void Close()
        {
            AudioManager.PlayCue("trackSlide");
            panelState = PuzzlePanelState.Closing;
        }

        public void ResetPuzzleName()
        {
            puzzleNameAlpha = 0f;
            puzzleNameShowTimer = PUZZLE_NAME_SHOW_TIME;
        }

        public void InitSolutionDict(List<PlayerSolutionShape> shapes)
        {
            flashTimer = 0;
            flashShapeDict = new Dictionary<int, bool>(shapes.Count);
            PuzzleEngine.Shuffle<PlayerSolutionShape>(shapes);
            foreach (PlayerSolutionShape pss in shapes)
            {
                flashShapeDict.Add(pss.ShapeKey, false);
            }

        }

        private void changePuzzle()
        {
            PuzzleEngine.ResetCurrentPuzzleShapes(false, true);
            PuzzleEngine.MainScreen.PickerPanel.GoToNext(true);
            PuzzleEngine.SetPuzzle();
        }

        private void updateFocus(Vector2 position)
        {
            oldSelectedCogIdx = selectedCogIdx;
            selectedCogIdx = -1;

            // updateShapeFocus
            List<Shape> shapes = new List<Shape>(PuzzleEngine.CurrentPuzzleSet.ShapesDict.Values);
            shapes.Sort(new ShapeSnapAgeComparer());
            shapes.Reverse();

            for (int i = 0; i < shapes.Count; i++)
            {
                if (shapes[i].State == Shape.ShapeState.Dropped && 
                    shapes[i].HasFocus(position))
                {
                    selectedCogIdx = shapes[i].Key - 1;
                    SelectedCog.IsSelected = true;
                    selectorSprite.Position = SelectedCog.Position;
                    if (oldSelectedCogIdx != selectedCogIdx)
                    {
                        AudioManager.PlayCue(SelectedCog.PShape.MusicCueName);
                    }
                    return;
                }
                else
                {
                    cogs[shapes[i].Key - 1].IsSelected = false;
                }
            }

            // updateCogFocus
            for (int i = 0; i < cogs.Count; i++)
            {
                if (cogs[i].HasFocus(position))
                {
                    selectedCogIdx = i;
                    SelectedCog.IsSelected = true;
                    selectorSprite.Position = SelectedCog.Position;
                    if (selectedCogIdx != oldSelectedCogIdx)
                    {
                        AudioManager.PlayCue(SelectedCog.PShape.MusicCueName);
                    }
                    return;
                }
                else
                {
                    cogs[i].IsSelected = false;
                }
            }

        }

        private void updateShapeFocus(Vector2 position)
        {
            List<Shape> shapes = new List<Shape>(PuzzleEngine.CurrentPuzzleSet.ShapesDict.Values);
            shapes.Sort(new ShapeSnapAgeComparer());
            shapes.Reverse();

            for (int i = 0; i < shapes.Count; i++)
            {
                if (shapes[i].State == Shape.ShapeState.Dropped && 
                    shapes[i].HasFocus(position))
                {
                    selectedCogIdx = shapes[i].Key - 1;
                    SelectedCog.IsSelected = true;
                    selectorSprite.Position = SelectedCog.Position;
                    if (oldSelectedCogIdx != selectedCogIdx)
                    {
                        AudioManager.PlayCue(SelectedCog.PShape.MusicCueName);
                    }
                    return;
                }
                else
                {
                    cogs[shapes[i].Key - 1].IsSelected = false;
                }
            }
        }

        private void updateCogFocus(Vector2 position)
        {
            oldSelectedCogIdx = selectedCogIdx;

            selectedCogIdx = -1;

            for (int i = 0; i < cogs.Count; i++)
            {
                if (cogs[i].HasFocus(position))
                {
                    selectedCogIdx = i;
                    SelectedCog.IsSelected = true;
                    selectorSprite.Position = SelectedCog.Position;
                    if (selectedCogIdx != oldSelectedCogIdx)
                    {
                        AudioManager.PlayCue(SelectedCog.PShape.MusicCueName);
                    }
                }
                else
                {
                    cogs[i].IsSelected = false;
                }
            }
        }

        public void Update(GameTime gameTime)
        {
            switch (panelState)
            {
                case PuzzlePanelState.Opening:
                    basePosition.Y -= PANEL_SPEED;
                    if (basePosition.Y <= ANCHOR_PANELOPEN_Y)
                    {
                        basePosition.Y = ANCHOR_PANELOPEN_Y;
                        panelState = PuzzlePanelState.Open;
                    }
                    break;
                case PuzzlePanelState.Open:
                    parentScreen.HelpPanel.On();

                    panelState = PuzzlePanelState.Startup;
                    startupState = PanelStartupState.Phase1;

                    AudioManager.PlayCue("slideCogs");

                    break;
                case PuzzlePanelState.Startup:

                    switch (startupState)
                    {
                        case PanelStartupState.Phase1:
                            if (isPhase1Finished)
                            {
                                startupState = PanelStartupState.Extend;
                                isPhase1Finished = false;

                                AudioManager.PlayCue("resizeCogs");
                            }
                            break;
                        case PanelStartupState.Extend:
                            if (isPhaseExtendFinished)
                            {
                                startupState = PanelStartupState.Phase2;
                                isPhaseExtendFinished = false;

                                AudioManager.PlayCue("clampCogs");
                            }
                            break;
                        case PanelStartupState.Phase2:
                            if (isPhase2Finished)
                            {
                                panelState = PuzzlePanelState.Running;
                                runningState = PuzzleRunningState.FadeInName;
                                startupState = PanelStartupState.Phase1;

                                isPhase2Finished = false;
                            }
                            break;
                    }

                    break;
                case PuzzlePanelState.Running:

                    /*
                    if (PanelState == PuzzlePanel.PuzzlePanelState.Running &&
                        !Session.ScreenManager.Game.IsActive)
                    {
                        if (!parentScreen.IsPaused)
                        {
                            parentScreen.PausePuzzlePanel();
                        }
//                        return;
                    }
                    */

                    if (PuzzleEngine.IsCursorModeActive && !PuzzleEngine.IsAShapeSelected)
                    {
                        updateFocus(PuzzleEngine.Cursor.ScreenPosition);
                    }

                    if (PuzzleEngine.IsAShapeSelected)
                    {
                        if (PuzzleEngine.SelectedShape.IsReadyToUnsnap)
                        {
                            PuzzleEngine.SelectedShape.IsReadyToUnsnap = false;
                            PuzzleEngine.SelectedShape.IsSnapped = false;
                        }
                        else if (PuzzleEngine.SelectedShape.IsPlaySnapCue)
                        {
                            PuzzleEngine.SelectedShape.IsPlaySnapCue = false;
                            AudioManager.PlayCue("snap");
                        }

                    }

                    PuzzleEngine.UpdatePuzzle(gameTime);

                    switch (runningState)
                    {
                        case PuzzleRunningState.FadeInName:
                            puzzleNameAlpha += PUZZLE_NAME_ALPHA_STEP;
                            if (puzzleNameAlpha >= 1)
                            {
                                puzzleNameAlpha = 1;
                                runningState = PuzzleRunningState.ShowName;
                            }
                            break;
                        case PuzzleRunningState.ShowName:
                            puzzleNameShowTimer--;
                            if (puzzleNameShowTimer <= 0)
                            {
                                puzzleNameShowTimer = PUZZLE_NAME_SHOW_TIME;
                                runningState = PuzzleRunningState.FadeOutName;
                            }
                            break;
                        case PuzzleRunningState.FadeOutName:
                            puzzleNameAlpha -= PUZZLE_NAME_ALPHA_STEP;
                            if (puzzleNameAlpha <= 0)
                            {
                                puzzleNameAlpha = 0;
                                runningState = PuzzleRunningState.NameFinished;
                            }
                            break;
                        case PuzzleRunningState.NameFinished:

                            break;
                    }
                    break;

                case PuzzlePanelState.Cleared:
                    switch (clearedState)
                    {
                        case PuzzleClearedState.Prepare:
                            PuzzleEngine.UpdatePuzzle(gameTime);
                            if (PuzzleEngine.CurrentPuzzleSet.AreAllInvalidShapesWaiting)
                            {
                                PuzzleEngine.MainScreen.PuzzlePanel.ClearedState = PuzzlePanel.PuzzleClearedState.FlashSolution;
                                clearedMessagePosition = PuzzleEngine.ViewportCenter + CLEARED_MESSAGE_START_OFFSET;
                            }
                            break;
                        case PuzzleClearedState.FlashSolution:
                            flashTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
                            if (flashTimer >= FLASH_TIMER_MAX)
                            {
                                flashTimer = 0;
                                if (!isFlashSolutionComplete)
                                {
                                    foreach (KeyValuePair<int, bool> shapeKvp in flashShapeDict)
                                    {
                                        if (!shapeKvp.Value) // shape has not been flashed yet
                                        {
                                            PuzzleEngine.CurrentPuzzleSet.ShapesDict[shapeKvp.Key].IsDrawHighlighted = true;
                                            AudioManager.PlayCue(PuzzleEngine.CurrentPuzzleSet.ShapesDict[shapeKvp.Key].MusicCueName_Loud);
                                            flashShapeDict[shapeKvp.Key] = true;
                                            return;
                                        }
                                    }
                                }
                                else
                                {
                                    PuzzleEngine.MainScreen.PuzzlePanel.ClearedState = PuzzlePanel.PuzzleClearedState.FadeInMessage;
                                    AudioManager.PlayCue("completePuzzle");
                                    return;
                                }
                            }

                            break;
                        case PuzzleClearedState.FadeInMessage:
                            clearedMessageAlpha += CLEARED_MESSAGE_ALPHA_STEP;
                            clearedMessagePosition += CLEARED_MESSAGE_POSITION_STEP;
                            if (clearedMessageAlpha >= 1)
                            {
                                clearedMessageAlpha = 1;
                                clearedState = PuzzleClearedState.ShowMessage;
                            }
                            break;
                        case PuzzleClearedState.ShowMessage:
                            clearedMessagePosition += CLEARED_MESSAGE_POSITION_STEP;
                            clearedMessageShowTimer--;
                            if (clearedMessageShowTimer <= 0)
                            {
                                clearedMessageShowTimer = CLEARED_MESSAGE_SHOW_TIME;
                                clearedState = PuzzleClearedState.FadeOutMessage;
                            }
                            break;
                        case PuzzleClearedState.FadeOutMessage:
                            clearedMessageAlpha -= CLEARED_MESSAGE_ALPHA_STEP;
                            clearedMessagePosition += CLEARED_MESSAGE_POSITION_STEP;
                            if (clearedMessageAlpha <= 0)
                            {
                                clearedMessageAlpha = 0;
                                clearedState = PuzzleClearedState.Finished;
                            }
                            break;
                        case PuzzleClearedState.Finished:
                            panelState = PuzzlePanelState.Stamp;
                            stampState = PuzzleStampState.Deploy;
                            AudioManager.PlayCue("stampSlide");
                            break;
                    }
                    break;
                case PuzzlePanelState.Hint:
                    PuzzleEngine.UpdatePuzzle(gameTime);

                    break;
                case PuzzlePanelState.Stamp:
                    switch (stampState)
                    {
                        case PuzzleStampState.Deploy:
                            stampSprite.Position += STAMP_POSITION_STEP;

                            if (stampSprite.Position.Y <= ANCHOR_STAMP_OPEN_Y)
                            {
                                stampSprite.Position = new Vector2(ANCHOR_STAMP_X, ANCHOR_STAMP_OPEN_Y);
                                stampState = PuzzleStampState.Deployed;
                            }
                            break;
                        case PuzzleStampState.Deployed:
                            stampState = PuzzleStampState.Wait1;
                            AudioManager.PlayCue("clamping");
                            break;
                        case PuzzleStampState.Wait1:
                            stampState = PuzzleStampState.Clamp;
                            break;
                        case PuzzleStampState.Clamp:
                            stampSprite.Scale -= STAMP_CLAMP_SCALE_STEP;

                            if (stampSprite.Scale <= STAMP_SCALE_MIN)
                            {
                                Session.QuickSave();

                                stampSprite.Scale = STAMP_SCALE_MIN;
                                stampState = PuzzleStampState.Wait2_1;
                            }
                            break;
                        case PuzzleStampState.Wait2_1:
                            stampWaitTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;

                            if (stampWaitTimer >= STAMP_WAIT2_LIMIT)
                            {
                                stampWaitTimer = 0;
                                stampState = PuzzleStampState.PuzzleChange;
                            }
                            break;
                        case PuzzleStampState.PuzzleChange:

                            if (PuzzleEngine.CurrentPuzzleSet.Statistics.PuzzlesSolved >= PuzzleEngine.CurrentPuzzleSet.Puzzles.Count)
                            {
                                PuzzleEngine.PuzzleSet.IsCleared = true;
                                isReadyForCredits = true;
                                panelState = PuzzlePanelState.Ending;
                                AudioManager.PlayCue("clampCogsOut");
                                PuzzleEngine.ResetCurrentPuzzleShapes(false, true);
                                return;
                            }

                            changePuzzle();
                            stampState = PuzzleStampState.Wait2_2;
                            break;
                        case PuzzleStampState.Wait2_2:
                            stampWaitTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;

                            if (stampWaitTimer >= STAMP_WAIT2_LIMIT)
                            {
                                stampWaitTimer = 0;
                                stampState = PuzzleStampState.Unclamp;
                                AudioManager.PlayCue("stampRetract");
                            }
                            break;
                        case PuzzleStampState.Unclamp:
                            stampSprite.Scale += STAMP_UNCLAMP_SCALE_STEP;

                            if (stampSprite.Scale >= 1)
                            {
                                stampSprite.Scale = 1;
                                stampState = PuzzleStampState.Wait3;
                            }

                            break;
                        case PuzzleStampState.Wait3:
                            stampState = PuzzleStampState.Retract;
                            AudioManager.PlayCue("stampSlide");
                            break;
                        case PuzzleStampState.Retract:
                            stampSprite.Position -= STAMP_POSITION_STEP;

                            if (stampSprite.Position.Y >= ANCHOR_STAMP_CLOSED_Y)
                            {
                                stampSprite.Position = new Vector2(ANCHOR_STAMP_X, ANCHOR_STAMP_CLOSED_Y);
                                stampState = PuzzleStampState.Finished;
                            }
                            break;
                        case PuzzleStampState.Finished:
                            parentScreen.HelpPanel.On();
                            panelState = PuzzlePanelState.Running;
                            runningState = PuzzleRunningState.FadeInName;
                            break;
                    }
                    break;

                case PuzzlePanelState.Ending:
                    switch (endingState)
                    {
                        case GameEndingState.PanelPhase2Rev:
                            if (isPhase2RevFinished)
                            {
                                endingState = GameEndingState.PanelRetract;
                                isPhase2RevFinished = false;

                                AudioManager.PlayCue("resizeCogs");
                            }
                            break;
                        case GameEndingState.PanelRetract:
                            if (isPhaseRetractFinished)
                            {
                                endingState = GameEndingState.PanelPhase1Rev;
                                isPhaseRetractFinished = false;
                                AudioManager.PlayCue("slideCogs");
                            }
                            break;
                        case GameEndingState.PanelPhase1Rev:
                            if (isPhase1RevFinished)
                            {
                                Close();
                                endingState = GameEndingState.PanelPhase2Rev;
                                isPhase1RevFinished = false;
                            }
                            break;
                    }



                    break;

                case PuzzlePanelState.PreShutdown:
                    if (!PuzzleEngine.CurrentPuzzleSet.AreAllInvalidShapesWaiting)
                    {
                        PuzzleEngine.UpdatePuzzle(gameTime);
                    }
                    else
                    {
                        if (isCogsZeroed)
                        {
                            panelState = PuzzlePanelState.Shutdown;
                            AudioManager.PlayCue("clampCogsOut");
                        }
                    }
                    break;
                case PuzzlePanelState.Shutdown:
                    switch (shutdownState)
                    {
                        case PanelShutdownState.Phase2Rev:
                            if (isPhase2RevFinished)
                            {
                                shutdownState = PanelShutdownState.Retract;
                                isPhase2RevFinished = false;
                                AudioManager.PlayCue("resizeCogs");
                            }
                            break;
                        case PanelShutdownState.Retract:
                            if (isPhaseRetractFinished)
                            {
                                shutdownState = PanelShutdownState.Phase1Rev;
                                isPhaseRetractFinished = false;
                                AudioManager.PlayCue("slideCogs");
                            }
                            break;
                        case PanelShutdownState.Phase1Rev:
                            if (isPhase1RevFinished)
                            {
                                Close();
                                shutdownState = PanelShutdownState.Phase2Rev;
                                isPhase1RevFinished = false;
                            }
                            break;
                    }

                    break;
                case PuzzlePanelState.Closing:
                    basePosition.Y += PANEL_SPEED;

                    if (PuzzleEngine.CurrentPuzzleSet.IsCleared)
                    {
                        stampSprite.Position += new Vector2(0, PANEL_SPEED);
                    }

                    if (basePosition.Y >= ANCHOR_PANELCLOSED_Y)
                    {
                        basePosition.Y = ANCHOR_PANELCLOSED_Y;
                        panelState = PuzzlePanelState.Closed;
                    }
                    break;
                case PuzzlePanelState.Closed:
                    if (parentScreen.Mode == MainGameScreen.MainGameScreenMode.PuzzlePanel && !PuzzleEngine.CurrentPuzzleSet.IsCleared)
                    {
                        parentScreen.IsBackToPicker = true;
                    }

                    if (isReadyForCredits && PuzzleEngine.CurrentPuzzleSet.IsCleared)
                    {
                        parentScreen.Mode = MainGameScreen.MainGameScreenMode.CreditsPanel;
                        parentScreen.CreditsPanel.Open();
                        isReadyForCredits = false;
                    }
                    break;
            }

            // update puzzleCogs then puzzlePanel?
            foreach (PuzzleCog cog in cogs)
            {
                cog.Update(gameTime);
            }

        }

        public void HandleInput()
        {
            switch (panelState)
            {
                case PuzzlePanelState.Running:
                    handleInput();
                    break;
            }
        }

        private void handleInput()
        {
            PuzzleEngine.Cursor.HandleInput();

            if (InputManager.IsActionTriggered(InputManager.Action.Help))
            {
                parentScreen.HelpPanel.PuzzlePanelGo();
            }
            else if (InputManager.IsActionTriggered(InputManager.Action.ResetPuzzleShapes))
            {
                PuzzleEngine.ResetCurrentPuzzleShapes(false, false);
                return;
            }
            else if (InputManager.IsActionTriggered(InputManager.Action.ShowHint) && runningState == PuzzleRunningState.NameFinished)
            {
                AudioManager.PlayCue("hint");

                if (PuzzleEngine.CurrentPuzzle.Statistics.HintsUsed < PuzzleEngine.CurrentPuzzle.OurSolution.SolutionShapes.Count / 2)
                {
                    PuzzleEngine.CurrentPuzzle.CountHintUsed();
                }
                panelState = PuzzlePanelState.Hint;
                hintState = PuzzleHintState.StartHint;

            }
            else if (InputManager.IsActionTriggered(InputManager.Action.ToggleCursorMode))
            {
                PuzzleEngine.IsCursorModeActive = !PuzzleEngine.IsCursorModeActive;

                if (PuzzleEngine.IsCursorModeActive) // just got turned on
                {
                    if (!PuzzleEngine.IsAShapeSelected)
                    {
                        PuzzleEngine.Cursor.ScreenPosition = PuzzleEngine.ViewportCenter;

                        // reset shapeCogIdx
                        SelectedCog.IsSelected = false;
                        selectedCogIdx = -1;
                    }

                }
                else // just got turned off
                {
                    if (selectedCogIdx == -1)
                    {
                        // if null, set shapeCogIdx to 0
                        selectedCogIdx = 0;
                        SelectedCog.IsSelected = true;
                        selectorSprite.Position = SelectedCog.Position;
                    }
                }

            }
            else if (PuzzleEngine.IsAShapeSelected)
            {
                if (!PuzzleEngine.SelectedShape.IsRotating && !PuzzleEngine.SelectedShape.IsFlipping)
                {
                    if (InputManager.IsActionTriggered(InputManager.Action.ResetSelectedShape))
                    {
                        PuzzleEngine.ShapeResetTriggered();
                    }
                    else if (InputManager.IsActionTriggered(InputManager.Action.DropSelectedShape))
                    {
                        PuzzleEngine.ShapeDropTriggered();
                    }
                    else if (InputManager.IsActionTriggered(InputManager.Action.RotateShape90CW))
                    {
                        PuzzleEngine.ShapeRotateTriggered(true);
                    }
                    else if (InputManager.IsActionTriggered(InputManager.Action.RotateShape90CCW))
                    {
                        PuzzleEngine.ShapeRotateTriggered(false);
                    }
                    else if (InputManager.IsActionTriggered(InputManager.Action.FlipShapeHorizontal))
                    {
                        PuzzleEngine.ShapeFlipTriggered(true);
                    }
                    else if (InputManager.IsActionTriggered(InputManager.Action.FlipShapeVertical))
                    {
                        PuzzleEngine.ShapeFlipTriggered(false);
                    }
                }
            }
            else // no shape selected
            {
                if (InputManager.IsActionTriggered(InputManager.Action.Back))  // if no shape is transitioning 
                {
                    if (PuzzleEngine.CurrentPuzzleSet.AreAnyShapesDropped)
                    {
                        List<Shape> shapes = new List<Shape>(PuzzleEngine.CurrentPuzzleSet.ShapesDict.Values);

                        // remove all non dropped shapes
                        for (int i = 0; i < shapes.Count; i++)
                        {
                            if (shapes[i].TimeIdle == 0.0 || shapes[i].State == Shape.ShapeState.TransitionOut)
                            {
                                shapes.Remove(shapes[i]);
                                i--;
                            }
                        }

                        shapes.Sort(new ShapeAgeComparer());
                        shapes.Reverse();
                        AudioManager.PlayCue("replaceShape");
                        shapes[0].State = Shape.ShapeState.TransitionOut;

                        if (shapes[0].IsValid)
                        {
                            PuzzleEngine.CurrentPuzzle.RemoveShape(shapes[0]);
                        }

                        return;
                    }

                    PuzzleEngine.MainScreen.ShowConfirmBackToPickerPanel();
                    return;
                }



                else if (!PuzzleEngine.IsCursorModeActive)
                {

                    if (InputManager.IsActionPressed(InputManager.Action.QuickShapeSelect))
                    {
                        handleQuickSelect();
                        if (InputManager.IsActionTriggered(InputManager.Action.GrabShape))
                        {
                            PuzzleEngine.ShapeGrabTriggered();
                        }
                    }
                    else if (InputManager.IsActionPressed(InputManager.Action.ContinuousPrevious) ||
                             InputManager.IsActionPressed(InputManager.Action.ContinuousNext))
                    {
                        if (InputManager.IsActionPressed(InputManager.Action.ContinuousPrevious))
                        {
                            handlePreviousShuffle();
                        }
                        else if (InputManager.IsActionPressed(InputManager.Action.ContinuousNext))
                        {
                            handleNextShuffle();
                        }

                        if (InputManager.IsActionTriggered(InputManager.Action.GrabShape))
                        {
                            PuzzleEngine.ShapeGrabTriggered();
                        }
                    }
                    // Move to the next menu entry?
                    else if (InputManager.IsActionPressed(InputManager.Action.ContinuousNext))
                    {
                        handleNextShuffle();
                    }
                    else if (InputManager.IsActionTriggered(InputManager.Action.StepPrevious))
                    {
                        goPreviousCog();
                    }
                    // Move to the next menu entry?
                    else if (InputManager.IsActionTriggered(InputManager.Action.StepNext))
                    {
                        goNextCog();
                    }
                    else if (InputManager.IsActionTriggered(InputManager.Action.GrabShape))
                    {
                        PuzzleEngine.ShapeGrabTriggered();
                    }
                }
                else // cursorMode ACTIVE
                {
                    if (InputManager.IsActionTriggered(InputManager.Action.GrabShape))
                    {
                        PuzzleEngine.ShapeGrabTriggered();
                    }
                }


            }

        }

        private void handlePreviousShuffle()
        {
            float triggerValue = InputManager.CurrentGamePadState.Triggers.Left;
            shuffleTimer += triggerValue;
            if (shuffleTimer > SHUFFLE_MAX)
            {
                shuffleTimer = 0;
                goPreviousCog();
            }
        }

        private void handleNextShuffle()
        {
            float triggerValue = InputManager.CurrentGamePadState.Triggers.Right;
            shuffleTimer += triggerValue;
            if (shuffleTimer > SHUFFLE_MAX)
            {
                shuffleTimer = 0;
                goNextCog();
            }
        }

        private void handleQuickSelect()
        {
            int oldCogIdx = selectedCogIdx;

            float angleRads = InputManager.CalculateAngle(InputManager.CurrentGamePadState.ThumbSticks.Left);
            float angle = MathHelper.ToDegrees(angleRads);

            if (SelectedCog != null)
            {
                SelectedCog.IsSelected = false;
            }

            if (angle == 0) { selectedCogIdx = 2; }
            else if (angle == 90) { selectedCogIdx = 5; }
            else if (angle == -90) { selectedCogIdx = 11; }
            else if (angle == 180) { selectedCogIdx = 8; }

            else if (angle > 0 && angle <= 45) { selectedCogIdx = 3; }
            else if (angle > 45 && angle < 90) { selectedCogIdx = 4; }

            else if (angle > 90 && angle <= 135) { selectedCogIdx = 6; }
            else if (angle > 135 && angle < 180) { selectedCogIdx = 7; }

            else if (angle > -180 && angle <= -135) { selectedCogIdx = 9; }
            else if (angle > -135 && angle < -90) { selectedCogIdx = 10; }

            else if (angle > -90 && angle <= -45) { selectedCogIdx = 0; }
            else if (angle > -45 && angle < 0) { selectedCogIdx = 1; }

            SelectedCog.IsSelected = true;
            selectorSprite.Position = SelectedCog.Position;

            if (oldCogIdx != selectedCogIdx)
            {
                AudioManager.PlayCue(SelectedCog.PShape.MusicCueName);
            }
        }

        private void goPreviousCog()
        {
            SelectedCog.IsSelected = false;
            if (selectedCogIdx > 0)
            {
                selectedCogIdx--;
            }
            else
            {
                selectedCogIdx = cogs.Count - 1;
            }
            SelectedCog.IsSelected = true;
            selectorSprite.Position = SelectedCog.Position;
            AudioManager.PlayCue(SelectedCog.PShape.MusicCueName);
        }

        private void goNextCog()
        {
            SelectedCog.IsSelected = false;
            if (selectedCogIdx < cogs.Count - 1) 
            { 
                selectedCogIdx++; 
            }
            else 
            { 
                selectedCogIdx = 0; 
            }
            SelectedCog.IsSelected = true;
            selectorSprite.Position = SelectedCog.Position;
            AudioManager.PlayCue(SelectedCog.PShape.MusicCueName);
        }

        public void startCogs()
        {
            foreach (PuzzleCog pc in cogs)
            {
                pc.IsRotating = true;
            }
        }

        public void stopCogs()
        {
            foreach (PuzzleCog pc in cogs)
            {
                pc.IsRotating = false;
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied);

            foreach (PuzzleCog cog in cogs)
            {
                cog.Draw(spriteBatch, new Vector2(0, basePosition.Y - ANCHOR_PANELOPEN_Y));
            }

            // draw cog selector 
            if (panelState == PuzzlePanelState.Running || panelState == PuzzlePanelState.Hint)
            {
                if (SelectedCog != null)
                {
                    selectorSprite.Draw(spriteBatch);
                }
            }

            if (panelState != PuzzlePanelState.Closed)
            {
                spriteBatch.Draw(PuzzleEngine.CurrentPuzzleSet.GetForegroundTex(PuzzleEngine.CurrentPuzzle.ForegroundTextureName),
                                 basePosition,
                                 null,
                                 Color.White,
                                 0,
                                 Vector2.Zero,
                                 1f,
                                 SpriteEffects.None,
                                 0);

                PuzzleEngine.DrawTileLayers(spriteBatch, PuzzleEngine.CurrentPuzzle, basePosition);

                spriteBatch.Draw(puzzleFrameTexture,
                                 basePosition + puzzleFrameOffset,
                                 Color.White);

                PuzzleEngine.DrawShapes(spriteBatch);

                if (panelState == PuzzlePanelState.Cleared && isClearedMessageState)
                {
                    Fonts.DrawCenteredText(spriteBatch,
                                           Fonts.Console72OutlinedFont,
                                           CLEARED_MESSAGE,
                                           clearedMessagePosition,
                                           new Color(1, 1, 1, clearedMessageAlpha),
                                           1f);                
                }

                if (panelState == PuzzlePanelState.Running)
                {
                    if (runningState != PuzzleRunningState.NameFinished)
                    {
                        float scale = 1f;

                        Vector2 nameSize = Fonts.Console72OutlinedFont.MeasureString(PuzzleEngine.CurrentPuzzle.Name);
                        float len = nameSize.X;

                        // handle scaling spritefont size for puzzlename -- should be refactored and done computationally instead of hard-coding                     
                        if (len >= 900 && len < 1000) { scale = .95f; }
                        else if (len >= 950 && len < 1050) { scale = .90f; }
                        else if (len >= 1050 && len < 1150) { scale = .85f; }
                        else if (len >= 1150 && len < 1250) { scale = .80f; }
                        else if (len >= 1250 && len < 1350) { scale = .75f; }
                        else if (len >= 1350 && len < 1450) { scale = .70f; }
                        else if (len >= 1450 && len < 1550) { scale = .65f; }
                        else if (len >= 1550 && len < 1650) { scale = .60f; }
                        else if (len >= 1650 && len < 1750) { scale = .55f; }
                        else if (len >= 1750 && len < 1850) { scale = .50f; }
                        else if (len >= 1850) { scale = .45f; }

                        Rectangle dstRect = new Rectangle(0, 
                                                          (int)(PUZZLE_NAME_POSITION.Y - ((nameSize.Y * scale) / 2)),
                                                          ShapeShop.PREFERRED_WIDTH, 
                                                          (int)(nameSize.Y * scale));

                        spriteBatch.Draw(Session.ScreenManager.BlankTexture,
                                         dstRect,
                                         null,
                                         new Color(1, 1, 1, puzzleNameAlpha / 2),
                                         0f,
                                         Vector2.Zero,
                                         SpriteEffects.None,
                                         0);

                        Fonts.DrawCenteredText(spriteBatch,
                                               Fonts.Console72OutlinedFont,
                                               PuzzleEngine.CurrentPuzzle.Name,
                                               PUZZLE_NAME_POSITION,
                                               new Color(1, 1, 1, puzzleNameAlpha),
                                               scale);
                    }
                }

                if (panelState == PuzzlePanelState.Stamp || 
                    panelState == PuzzlePanelState.Ending || 
                    (panelState == PuzzlePanelState.Closing && PuzzleEngine.CurrentPuzzleSet.IsCleared))
                {
                    stampSprite.Draw(spriteBatch);
                }

            }
            
            spriteBatch.End();
        }

        public string GetDebugInformation()
        {
            StringBuilder retStr = new StringBuilder();

            retStr.Append("[panelState:" + panelState + "]")
                  .Append("\n");

            switch (panelState)
            {
                case PuzzlePanelState.Closed:
                    break;
                case PuzzlePanelState.Closing:
                    break;
                case PuzzlePanelState.Open:
                    break;
                case PuzzlePanelState.Opening:
                    break;
                case PuzzlePanelState.Startup:
                    retStr.Append(" [startupState:"+startupState+"]");
                    break;
                case PuzzlePanelState.PreShutdown:
                    break;
                case PuzzlePanelState.Shutdown:
                    retStr.Append(" [shutdownState:"+shutdownState+"]");
                    break;
                case PuzzlePanelState.Running:
                    break;
                case PuzzlePanelState.Cleared:
                    retStr.Append(" [clearedState:" + clearedState + "]");
                    break;
                case PuzzlePanelState.Stamp:
                    retStr.Append(" [stampState:" + stampState + "]");
                    break;
                case PuzzlePanelState.Hint:
                    retStr.Append(" [hintState:" + hintState + "]");
                    break;

            }

            retStr.Append('\n')
                  .Append(" [basePosition:" + basePosition+"]")
                  .Append('\n');

            return retStr.ToString();
        }


    }
}
