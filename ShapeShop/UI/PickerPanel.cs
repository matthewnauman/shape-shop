using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using ShapeShop.GameEngine;
using ShapeShop.GameSession;
using ShapeShopData.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace ShapeShop.UI
{
    public class PickerPanel
    {
        public enum PickerPanelState
        {
            Opening,
            Open,
            Closing,
            Closed,
        }

        public enum RotationState
        {
            StepPrevious,
            StepNext,
            Ready,
            LockedNext,
            LockedPrevious,
            ContinuousPrevious,
            ContinuousNext,
        }

        private enum LockedState
        {
            Clear1,
            Flash1,
            Clear2,
            Flash2,
            Clear3,
        }

        private const int LOCKED_TIMER_CLEAR = 10;
        private const int LOCKED_TIMER_FLASH = 30;

        private const float ROTATION_SPEED_STEP = 0.1f;
        private const float ROTATION_SPEED_CONTINUOUS = 0.75f;
        private readonly float ANCHOR_ROTATE_START = MathHelper.ToRadians(0);
        private readonly float ANCHOR_ROTATE_LOCK = MathHelper.ToRadians(15);
        private readonly float ANCHOR_ROTATE_HALF = MathHelper.ToRadians(180);
        private readonly Vector2 BASE_OFFSET = new Vector2(140, -139);
        private readonly Vector2 LOCK_POSITION = new Vector2(208, 530); // y = 529
        private readonly Vector2 COUNTER_POSITION = new Vector2(460, 203);

        private const int CONTINUOUS_COUNT = 10;

        private PickerConsole pickerConsole = new PickerConsole();
        private int selectedEntryIdx = 0;
        private int unselectedEntryIdx = -1;
        private int destEntryIdx = -1;
        private int lockedTimerClear = 0;
        private int lockedTimerFlash = 0;

        private List<PickerEntry> pickerEntries = new List<PickerEntry>();
        public List<PickerEntry> PickerEntries
        {
            get { return pickerEntries; }
        }

        private bool isReadyForPuzzle = false;

        public PickerEntry SelectedPickerEntry
        {
            get
            {
                if (selectedEntryIdx < 0 || selectedEntryIdx > pickerEntries.Count - 1)
                {
                    return null;
                }
                else
                {
                    return pickerEntries[selectedEntryIdx];
                }
            }
        }

        public PickerEntry UnselectedPickerEntry
        {        
            get 
            {
                if (unselectedEntryIdx < 0 || unselectedEntryIdx > pickerEntries.Count - 1)
                {
                    return null;
                }
                else
                {
                    return pickerEntries[unselectedEntryIdx];
                }
            }
        }


        private const float ANCHOR_PANEL_X = 140;
        private const float ANCHOR_PANELCLOSED_Y = -1139; // -581
        private const float ANCHOR_PANELOPEN_Y = -139;
        private const float SPEED_PANEL = 14f;

        private Texture2D cogTexture, faceTexture, tempTexture, lockTexture, faceBoltTexture;
        private Vector2 basePosition, cogOrigin;
        private readonly float cogRotation = 0;

        private MainGameScreen parentScreen;
        private readonly GameStartDescription gameStartDescription = null;
        private readonly SaveGameDescription saveGameDescription = null;

        private LockedState lockedState = LockedState.Clear1;

        private PickerPanelState panelState = PickerPanelState.Closed;
        public PickerPanelState PanelState
        {
            get { return panelState; }
            set { panelState = value; }
        }

        private RotationState rotState = RotationState.Ready;
        public RotationState RotState
        {
            get { return rotState; }
            set { rotState = value; }
        }

        /// Create a new GameplayScreen object from a new-game description.
        public PickerPanel(MainGameScreen parentScreen, GameStartDescription gameStartDescription)
        {
            this.parentScreen = parentScreen;
            this.gameStartDescription = gameStartDescription;
            this.saveGameDescription = null;
        }

        /// Create a new GameplayScreen object from a new-game description.
        public PickerPanel(MainGameScreen parentScreen, SaveGameDescription saveGameDescription)
        {
            this.parentScreen = parentScreen;
            this.saveGameDescription = saveGameDescription;
            this.gameStartDescription = null;
        }

        public void LoadContent(ContentManager content)
        {
            if (gameStartDescription != null)
            {
                Session.StartNewSession(gameStartDescription, parentScreen.ScreenManager, parentScreen);
                LoadPickerEntries(true);
            }
            else if (saveGameDescription != null)
            {
                Session.LoadSession(saveGameDescription, parentScreen.ScreenManager, parentScreen);
            }
            else
            {
                throw new NullReferenceException("pickerscreen::loadcontent - no gameStartDescription or saveGameDescription");
            }

            cogTexture = content.Load<Texture2D>(@"Textures\Cogs\pickerCog");
            faceTexture = content.Load<Texture2D>(@"Textures\GameScreens\Picker\pickerFace");
            faceBoltTexture = content.Load<Texture2D>(@"Textures\GameScreens\Picker\pickerFaceBolt");
            lockTexture = content.Load<Texture2D>(@"Textures\GameScreens\Picker\pickerLockedConsole");
            tempTexture = content.Load<Texture2D>(@"Textures\GameScreens\Picker\pickerTemp");

            basePosition = new Vector2(ANCHOR_PANEL_X, ANCHOR_PANELCLOSED_Y);
            cogOrigin = new Vector2(cogTexture.Width / 2, cogTexture.Height / 2);

            pickerConsole.LoadContent(content);

        }

        public void LoadPickerEntries(bool isBrandNew)
        {
            List<Puzzle> puzzles = PuzzleEngine.CurrentPuzzleSet.Puzzles;

            PuzzleEngine.IsCheckRender = true;
            foreach (Puzzle puzzle in puzzles)
            {
                PickerEntry pe = new PickerEntry(puzzle);
                pe.IsRenderEntry = true;
                pickerEntries.Add(pe);
            }

            // if a true load, set picker to first unsolved puzzle
            if (!isBrandNew)
            {
                for (int i = 0; i < puzzles.Count; i++)
                {
                    if (!puzzles[i].IsCleared)
                    {
                        selectedEntryIdx = i;
                        SelectedPickerEntry.Rotation = ANCHOR_ROTATE_START;
                        break;
                    }
                }
            }
        }

        public void Open()
        {
            AudioManager.PlayCue("trackSlide");
            panelState = PickerPanelState.Opening;
        }

        public void Close()
        {
            AudioManager.PlayCue("trackSlide");
            panelState = PickerPanelState.Closing;
        }
         
        public void GoToNext(bool nextIncomplete)
        {
            int prevIdx = selectedEntryIdx;
            int nextIdx = selectedEntryIdx;
            while (pickerEntries[nextIdx].Puzzle.IsCleared)
            {
                if (nextIdx == pickerEntries.Count - 1)
                {
                    nextIdx = 0;
                }
                else
                {
                    nextIdx++;
                }
            }
            selectedEntryIdx = nextIdx;
            SelectedPickerEntry.Rotation = ANCHOR_ROTATE_START;
        }

        public void StepNext()
        {
            int nextEntryIdx = -1;
            if (selectedEntryIdx == pickerEntries.Count - 1) { return; }
            else { nextEntryIdx = selectedEntryIdx + 1; }

            if (pickerEntries[nextEntryIdx].Puzzle.IsLocked)
            {
                AudioManager.PlayCue("locked");

                rotState = RotationState.LockedNext;
                lockedState = LockedState.Clear1;
            }
            else
            {
                AudioManager.PlayCue("puzzlePickerRotate");

                rotState = RotationState.StepNext;
                unselectedEntryIdx = nextEntryIdx;
                UnselectedPickerEntry.Rotation = ANCHOR_ROTATE_HALF;
            }
        }

        public void StepPrevious()
        {
            int prevEntryIdx = -1;
            if (selectedEntryIdx == 0) { return; }
            else { prevEntryIdx = selectedEntryIdx - 1; }

            if (pickerEntries[prevEntryIdx].Puzzle.IsLocked)
            {
                rotState = RotationState.LockedPrevious;
            }
            else
            {
                AudioManager.PlayCue("puzzlePickerRotate");

                rotState = RotationState.StepPrevious;
                unselectedEntryIdx = prevEntryIdx;
                UnselectedPickerEntry.Rotation = ANCHOR_ROTATE_HALF;
            }
        }

        public void ContinuousPrevious()
        {
            if (selectedEntryIdx - CONTINUOUS_COUNT >= 0)
            {
                destEntryIdx = selectedEntryIdx - CONTINUOUS_COUNT;
            }
            else
            {
                destEntryIdx = 0;
            }

            int prevEntryIdx = -1;
            if (selectedEntryIdx == 0) { return; }
            else { prevEntryIdx = selectedEntryIdx - 1; }

            if (pickerEntries[prevEntryIdx].Puzzle.IsLocked)
            {
                rotState = RotationState.LockedPrevious;
            }
            else
            {
                AudioManager.PlayCue("puzzlePickerShuffle");

                rotState = RotationState.ContinuousPrevious;
                unselectedEntryIdx = prevEntryIdx;
                UnselectedPickerEntry.Rotation = ANCHOR_ROTATE_HALF;
            }
             
        }

        public void ContinuousNext()
        {
            if (selectedEntryIdx + CONTINUOUS_COUNT <= pickerEntries.Count - 1)
            {
                destEntryIdx = selectedEntryIdx + CONTINUOUS_COUNT;
            }
            else
            {
                destEntryIdx = pickerEntries.Count - 1;
            }

            int nextEntryIdx = -1;
            if (selectedEntryIdx == pickerEntries.Count - 1) { return; }
            else { nextEntryIdx = selectedEntryIdx + 1; }

            if (pickerEntries[nextEntryIdx].Puzzle.IsLocked)
            {
                AudioManager.PlayCue("locked");

                rotState = RotationState.LockedNext;
                lockedState = LockedState.Clear1;
            }
            else
            {
                AudioManager.PlayCue("puzzlePickerShuffle");

                rotState = RotationState.ContinuousNext;
                unselectedEntryIdx = nextEntryIdx;
                UnselectedPickerEntry.Rotation = ANCHOR_ROTATE_HALF;
            }
             
        }

        public void Update(GameTime gameTime)
        {
            switch (panelState)
            {
                case PickerPanelState.Closed:
                    if (StorageManager.Instance.IsRefreshRequired)
                    {
                        StorageManager.Instance.RefreshSaveGameDescriptions();
                        StorageManager.Instance.IsRefreshRequired = false;
                    }

                    if (PuzzleEngine.IsOnReplay)
                    {
                        PuzzleEngine.ResetCurrentPuzzleOnReplay();

                        Session.QuickSave();

                        PuzzleEngine.IsOnReplay = false;
                    }

                    if (isReadyForPuzzle)
                    {
                        isReadyForPuzzle = false;
                        parentScreen.IsReadyForPuzzle = true;                        
                    }
                    break;
                case PickerPanelState.Opening:
                    basePosition.Y += SPEED_PANEL;
                    if (basePosition.Y >= ANCHOR_PANELOPEN_Y)
                    {
                        basePosition.Y = ANCHOR_PANELOPEN_Y;
                        panelState = PickerPanelState.Open;
                    }
                    break;
                case PickerPanelState.Open:
                    parentScreen.HelpPanel.On();
                    updateRotation(gameTime);
                    pickerConsole.Update(gameTime);
                    break;
                case PickerPanelState.Closing:
                    basePosition.Y -= SPEED_PANEL;
                    if (basePosition.Y <= ANCHOR_PANELCLOSED_Y)
                    {
                        basePosition.Y = ANCHOR_PANELCLOSED_Y;
                        panelState = PickerPanelState.Closed;
                    }
                    break;
            }

        }

        private void updateRotation(GameTime gameTime)
        {
            switch (rotState)
            {
                case RotationState.StepNext:

                    SelectedPickerEntry.Rotation += ROTATION_SPEED_STEP;
                    UnselectedPickerEntry.Rotation += ROTATION_SPEED_STEP;

                    if (SelectedPickerEntry.Rotation >= ANCHOR_ROTATE_HALF)
                    {
                        selectedEntryIdx = unselectedEntryIdx;
                        unselectedEntryIdx = -1;
                        SelectedPickerEntry.Rotation = ANCHOR_ROTATE_START;
                        rotState = RotationState.Ready;
                    }
                    break;
                case RotationState.ContinuousNext:

                    SelectedPickerEntry.Rotation += ROTATION_SPEED_CONTINUOUS;
                    UnselectedPickerEntry.Rotation += ROTATION_SPEED_CONTINUOUS;

                    if (SelectedPickerEntry.Rotation >= ANCHOR_ROTATE_HALF)
                    {
                        selectedEntryIdx = unselectedEntryIdx;
                        unselectedEntryIdx = -1;
                        SelectedPickerEntry.Rotation = ANCHOR_ROTATE_START;

                        if (selectedEntryIdx == destEntryIdx)
                        {
                            rotState = RotationState.Ready;
                        }
                        else
                        {
                            AudioManager.PlayCue("puzzlePickerShuffle");

                            int nextEntryIdx = -1;
                            if (selectedEntryIdx == pickerEntries.Count - 1) { return; }
                            else { nextEntryIdx = selectedEntryIdx + 1; }

                            if (pickerEntries[nextEntryIdx].Puzzle.IsLocked)
                            {
                                rotState = RotationState.Ready;
                            }
                            else
                            {
                                unselectedEntryIdx = nextEntryIdx;
                                UnselectedPickerEntry.Rotation = ANCHOR_ROTATE_HALF;
                            }
                        }
                    }
                    break;
                case RotationState.LockedNext:

                    switch (lockedState)
                    {
                        case LockedState.Clear1:
                            lockedTimerClear++;
                            if (lockedTimerClear >= LOCKED_TIMER_CLEAR)
                            {
                                lockedTimerClear = 0;
                                lockedState = LockedState.Flash1;
                            }
                            break;
                        case LockedState.Flash1:
                            lockedTimerFlash++;
                            if (lockedTimerFlash >= LOCKED_TIMER_FLASH)
                            {
                                lockedTimerFlash = 0;
                                lockedState = LockedState.Clear2;
                            }
                            break;
                        case LockedState.Clear2:
                            lockedTimerClear++;
                            if (lockedTimerClear >= LOCKED_TIMER_CLEAR)
                            {
                                lockedTimerClear = 0;
                                lockedState = LockedState.Flash2;
                            }
                            break;
                        case LockedState.Flash2:
                            lockedTimerFlash++;
                            if (lockedTimerFlash >= LOCKED_TIMER_FLASH)
                            {
                                lockedTimerFlash = 0;
                                lockedState = LockedState.Clear3;
                            }
                            break;
                        case LockedState.Clear3:
                            lockedTimerClear++;
                            if (lockedTimerClear >= LOCKED_TIMER_CLEAR)
                            {
                                lockedTimerClear = 0;
                                lockedState = LockedState.Clear1;
                                rotState = RotationState.Ready;
                            }
                            break;
                    }
                    break;
                case RotationState.StepPrevious:
                    SelectedPickerEntry.Rotation -= ROTATION_SPEED_STEP;
                    UnselectedPickerEntry.Rotation -= ROTATION_SPEED_STEP;

                    if (SelectedPickerEntry.Rotation <= -ANCHOR_ROTATE_HALF)
                    {
                        selectedEntryIdx = unselectedEntryIdx;
                        unselectedEntryIdx = -1;
                        SelectedPickerEntry.Rotation = ANCHOR_ROTATE_START;
                        rotState = RotationState.Ready;
                    }
                    break;
                case RotationState.ContinuousPrevious:
                    SelectedPickerEntry.Rotation -= ROTATION_SPEED_CONTINUOUS;
                    UnselectedPickerEntry.Rotation -= ROTATION_SPEED_CONTINUOUS;

                    if (SelectedPickerEntry.Rotation <= -ANCHOR_ROTATE_HALF)
                    {
                        selectedEntryIdx = unselectedEntryIdx;
                        unselectedEntryIdx = -1;
                        SelectedPickerEntry.Rotation = ANCHOR_ROTATE_START;

                        if (selectedEntryIdx == destEntryIdx)
                        {
                            rotState = RotationState.Ready;
                        }
                        else
                        {
                            AudioManager.PlayCue("puzzlePickerShuffle");

                            int prevEntryIdx = -1;
                            if (selectedEntryIdx == 0) { return; }
                            else { prevEntryIdx = selectedEntryIdx - 1; }

                            if (pickerEntries[prevEntryIdx].Puzzle.IsLocked)
                            {
                                rotState = RotationState.LockedPrevious;
                            }
                            else
                            {
                                unselectedEntryIdx = prevEntryIdx;
                                UnselectedPickerEntry.Rotation = ANCHOR_ROTATE_HALF;
                            }
                        }
                    }
                    break;
                case RotationState.LockedPrevious:
                    rotState = RotationState.Ready;
                    break;
                case RotationState.Ready:
                    break;
            }

        }


        public void HandleInput()
        {
            switch (panelState)
            {
                case PickerPanelState.Open:
                    switch (rotState)
                    {
                        case RotationState.Ready:
                            handleInput();
                            break;
                        case RotationState.StepNext:
                            break;
                        case RotationState.StepPrevious:
                            break;
                    }
                    break;
            }
        }

        private void handleInput()
        {

            if (InputManager.IsActionTriggered(InputManager.Action.Pause))
            {                
                return;
            }
            // Move to the previous menu entry
            else if (InputManager.IsActionTriggered(InputManager.Action.StepPrevious))
            {
                StepPrevious();
            }
            // Move to the next menu entry
            else if (InputManager.IsActionTriggered(InputManager.Action.StepNext))
            {
                StepNext();
            }
            else if (InputManager.IsActionTriggered(InputManager.Action.ContinuousPrevious))
            {
                ContinuousPrevious();
            }
            // Move to the next menu entry?
            else if (InputManager.IsActionTriggered(InputManager.Action.ContinuousNext))
            {
                ContinuousNext();
            }
            else if (InputManager.IsActionTriggered(InputManager.Action.Ok))
            {
                if (PuzzleEngine.CurrentPuzzle.IsCleared)
                {
                    parentScreen.ShowConfirmReplay();
                }
                else
                {
                    parentScreen.HelpPanel.Off(true);
                    Close();
                    isReadyForPuzzle = true;
                }
            }

            else if (InputManager.IsActionTriggered(InputManager.Action.Help))
            {
                parentScreen.HelpPanel.PickerPanelGo();
            }

        }

        public void ReplayConfirmed()
        {
            Close();
            isReadyForPuzzle = true;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied);

            if (panelState != PickerPanelState.Closed)
            {
                spriteBatch.Draw(cogTexture,
                                 basePosition + cogOrigin,
                                 null,
                                 parentScreen.TransitionColor,
                                 cogRotation,
                                 cogOrigin,
                                 1f,
                                 SpriteEffects.None,
                                 0);
                spriteBatch.Draw(tempTexture,
                                 basePosition,
                                 null,
                                 parentScreen.TransitionColor,
                                 0,
                                 Vector2.Zero,
                                 1f,
                                 SpriteEffects.None,
                                 0);



                SelectedPickerEntry.Draw(spriteBatch, basePosition - BASE_OFFSET);
                if (UnselectedPickerEntry != null)
                {
                    UnselectedPickerEntry.Draw(spriteBatch, basePosition - BASE_OFFSET);
                }

                spriteBatch.Draw(faceTexture,
                                 basePosition,
                                 null,
                                 parentScreen.TransitionColor,
                                 0f,
                                 Vector2.Zero,
                                 1f,
                                 SpriteEffects.None,
                                 0);

                spriteBatch.Draw(faceBoltTexture,
                                 basePosition,
                                 null,
                                 parentScreen.TransitionColor,
                                 0f,
                                 Vector2.Zero,
                                 1f,
                                 SpriteEffects.None,
                                 0);

                spriteBatch.DrawString(Fonts.Console30BoldFont,
                                       PuzzleEngine.CurrentPuzzle.PlaceText,
                                       basePosition + COUNTER_POSITION,
                                       Fonts.ConsoleTextColor,
                                       0f,
                                       Vector2.Zero,
                                       1f,
                                       SpriteEffects.None,
                                       0);

                if (rotState == RotationState.LockedNext)
                {
                    if (lockedState == LockedState.Flash1 || lockedState == LockedState.Flash2)
                    {
                        spriteBatch.Draw(lockTexture,
                                         basePosition + LOCK_POSITION,
                                         null,
                                         parentScreen.TransitionColor,
                                         0f,
                                         Vector2.Zero,
                                         1f,
                                         SpriteEffects.None,
                                         0);
                    }
                }
                else
                {
                    pickerConsole.Draw(spriteBatch, basePosition - BASE_OFFSET, PuzzleEngine.CurrentPuzzle);
                }
            }

            spriteBatch.End();
        }

        public string GetDebugInformation()
        {
            StringBuilder retStr = new StringBuilder();

            retStr.Append(" panelState : " + panelState)
                  .Append('\n')
                  .Append(" basePosition : " + basePosition)
                  .Append('\n')
                  .Append(" rotState : " + rotState)
                  .Append('\n')
                  .Append(" lockedState : " + lockedState)
                  .Append('\n');

            return retStr.ToString();
        }

    }
}
