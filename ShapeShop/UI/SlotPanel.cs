using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using ShapeShop.GameEngine;
using ShapeShop.GameSession;
using System.Collections.Generic;
using System.Text;

namespace ShapeShop.UI
{
    public class SlotPanel
    {
        public enum SlotPanelState
        {
            Opening,
            Open,
            Closing,
            Closed,
        }

        public enum SelectorMode
        {
            Controlled,
            Transitioning,
            AutoTransitioning,
            Docked,
        }

        private enum AutoTransitionType
        {
            Up,
            Down,
        }

        private const float ANCHOR_PANEL_X = 209;
        private const float ANCHOR_PANELCLOSED_Y = -620; // -581
        private const float ANCHOR_PANELOPEN_Y = 101;
        private const float SPEED_PANEL = 14f;
        private const float SPEED_SELECTOR = 10f;
        private const float SPEED_TRANSITION = 15f;
        private const float SPEED_ROTATION_TRANSITION = SPEED_TRANSITION / 500;
        private const float ANCHOR_SLOT1_SELECTOR_OFFSETY = 45;
        private const float ANCHOR_SLOT1_COG_OFFSETY = 31;

        public const float STEP_OFFSETY = 140f;

        private const float HALFSTEP_OFFSETY = STEP_OFFSETY / 2;

        private const float ANCHOR_BETWEEN_1_2_Y = ANCHOR_SLOT1_SELECTOR_OFFSETY + HALFSTEP_OFFSETY;
        private const float ANCHOR_BETWEEN_2_3_Y = ANCHOR_SLOT1_SELECTOR_OFFSETY + STEP_OFFSETY + HALFSTEP_OFFSETY;

        private Vector2 cog1Offset = new Vector2(-67, ANCHOR_SLOT1_COG_OFFSETY);
        private Vector2 cog2Offset = new Vector2(754, ANCHOR_SLOT1_COG_OFFSETY);
        private Vector2 selectorOffset = new Vector2(-19, ANCHOR_SLOT1_SELECTOR_OFFSETY);
        private float cog1Rotation, cog2Rotation;

        private Texture2D cog1Texture, cog2Texture;
        private Texture2D panelTexture, selectorTexture, plateTexture;
        private Vector2 basePosition, cogOrigin;

        private MainGameScreen parentScreen;
        private SlotPanelState panelState = SlotPanelState.Closed;
        public SlotPanelState PanelState
        {
            get { return panelState; }
            set { panelState = value; }
        }

        private List<SaveLoadSlot> slots;
        public List<SaveLoadSlot> Slots
        {
            get { return slots; }
        }

        private int nextSlotIdx = 1;
        
        private int currentSlotIdx = 0;
        public int CurrentSlotIdx
        {
            get { return currentSlotIdx; }
        }

        public SaveLoadSlot CurrentSlot
        {
            get { return slots[currentSlotIdx]; }
        }

        private SaveLoadSlot nextSlot
        {
            get { return slots[nextSlotIdx]; }
        }

        private AutoTransitionType autoTransitionType = AutoTransitionType.Down;
        private SelectorMode selectMode = SelectorMode.Docked;
        public SelectorMode SelectMode
        {
            get { return selectMode; }
            set { this.selectMode = value; }
        }

        public SlotPanel(MainGameScreen parentScreen)
        {
            this.parentScreen = parentScreen;
        }

        public void RefreshSlots()
        {
            for (int i = 0; i < slots.Count; i++)
            {
                if (StorageManager.Instance.SaveGameDescriptionsDict.ContainsKey(i + 1))
                {
                    slots[i].RefreshSlot(StorageManager.Instance.SaveGameDescriptionsDict[i + 1]);
                }
                else
                {
                    slots[i].RefreshSlot(null);
                }
            }
        }

        public void LoadContent(ContentManager content)
        {
            // create slots
            slots = new List<SaveLoadSlot>(StorageManager.MAX_SAVE_GAMES);
            for (int i = 0; i < slots.Capacity; i++)
            {
                slots.Add(new SaveLoadSlot(parentScreen, i + 1, new Vector2(selectorOffset.X, selectorOffset.Y + (STEP_OFFSETY * i))));
            }

            foreach (SaveLoadSlot slot in slots)
            {
                slot.LoadContent(content);
            }

            // default slot
            slots[currentSlotIdx].IsSelected = true;

            panelTexture = content.Load<Texture2D>(@"Textures\GameScreens\slotPanel");
            basePosition = new Vector2(ANCHOR_PANEL_X, ANCHOR_PANELCLOSED_Y);

            selectorTexture = content.Load<Texture2D>(@"Textures\GameScreens\slotSelector");
            cog1Texture = content.Load<Texture2D>(@"Textures\Cogs\smCog01");
            cog2Texture = content.Load<Texture2D>(@"Textures\Cogs\smCog01");
            plateTexture = content.Load<Texture2D>(@"Textures\Cogs\smCogPlate01");

            cogOrigin = new Vector2(plateTexture.Width / 2, plateTexture.Height / 2);
            cog1Rotation = 0f;
            cog2Rotation = 0f;
        }

        public void Reset()
        {
            foreach (SaveLoadSlot slot in slots)
            {
                slot.IsSelected = false;
            }
            currentSlotIdx = 0;
            slots[currentSlotIdx].IsSelected = true;

            cog1Offset = new Vector2(-67, ANCHOR_SLOT1_COG_OFFSETY);
            cog2Offset = new Vector2(754, ANCHOR_SLOT1_COG_OFFSETY);
            selectorOffset = new Vector2(-19, ANCHOR_SLOT1_SELECTOR_OFFSETY);

            basePosition = new Vector2(ANCHOR_PANEL_X, ANCHOR_PANELCLOSED_Y);
            panelState = SlotPanelState.Closed;
        }

        public void Open()
        {
            AudioManager.PlayCue("trackSlide");
            panelState = SlotPanelState.Opening;
        }

        public void Close()
        {
            AudioManager.PlayCue("trackSlide");
            panelState = SlotPanelState.Closing;
        }

        public void Update(GameTime gameTime)
        {
            switch (panelState)
            {
                case SlotPanelState.Closed:
                    if (parentScreen.IsLoadConfirmed)
                    {
                        parentScreen.IsReadyToLoad = true;                          
                    }
                    break;
                case SlotPanelState.Opening:
                    basePosition.Y += SPEED_PANEL;
                    if (basePosition.Y >= ANCHOR_PANELOPEN_Y)
                    {
                        basePosition.Y = ANCHOR_PANELOPEN_Y;
                        panelState = SlotPanelState.Open;
                    }
                    break;
                case SlotPanelState.Open:
                    parentScreen.HelpPanel.On();
                    break;
                case SlotPanelState.Closing:
                    basePosition.Y -= SPEED_PANEL;
                    if (basePosition.Y <= ANCHOR_PANELCLOSED_Y)
                    {
                        basePosition.Y = ANCHOR_PANELCLOSED_Y;
                        panelState = SlotPanelState.Closed;
                    }
                    break;
            }

        }

        public void HandleInput()
        {
            switch (panelState)
            {
                case SlotPanelState.Open:
                    handleInput();
                    break;
                case SlotPanelState.Opening:
                    break;
                case SlotPanelState.Closed:
                    break;
                case SlotPanelState.Closing:
                    break;
            }
        }

        private void handleInput()
        {
            if (InputManager.IsActionTriggered(InputManager.Action.Ok) && (slots != null) && selectMode == SelectorMode.Docked)
            {
                if ((currentSlotIdx >= 0) && (currentSlotIdx < slots.Count))
                {                    
                    parentScreen.HelpPanel.Off(true);

                    if (CurrentSlot.SaveGameDescription != null)
                    {
//                        parentScreen.ShowConfirmLoad();
                        parentScreen.BypassConfirmLoad();
                    }
                    else
                    {
//                        parentScreen.ShowConfirmNew();
                        parentScreen.BypassConfirmNew();
                    }
                }
            }
            else if (InputManager.IsActionTriggered(InputManager.Action.DeleteFile) && (CurrentSlot.SaveGameDescription != null) && selectMode == SelectorMode.Docked)
            {
                parentScreen.ShowConfirmDelete();
            }
            else if ((InputManager.IsActionTriggered(InputManager.Action.MenuCursorUp) ||
                     InputManager.IsActionTriggered(InputManager.Action.MenuCursorDown)) && selectMode == SelectorMode.Docked)
            {
                if (InputManager.IsActionTriggered(InputManager.Action.MenuCursorUp))
                {
                    if (currentSlotIdx >= 1)
                    {
                        AudioManager.PlayCue("slotSelector");

                        selectMode = SelectorMode.AutoTransitioning;
                        autoTransitionType = AutoTransitionType.Up;
                        nextSlotIdx = currentSlotIdx - 1;
                    }
                }
                else // if (InputManager.IsActionTriggered(InputManager.Action.MenuCursorDownDPadOnly)
                {
                    if (currentSlotIdx < slots.Count - 1)
                    {
                        AudioManager.PlayCue("slotSelector");

                        selectMode = SelectorMode.AutoTransitioning;
                        autoTransitionType = AutoTransitionType.Down;
                        nextSlotIdx = currentSlotIdx + 1;
                    }
                }
            }
            else if (InputManager.IsActionTriggered(InputManager.Action.Help))
            {
                parentScreen.HelpPanel.SlotPanelGo();
            }
            else // no user input from Lstick or Dpad
            {
                if (selectMode == SelectorMode.AutoTransitioning)
                {
                    switch (autoTransitionType)
                    {
                        case AutoTransitionType.Up:
                            selectorOffset.Y -= SPEED_TRANSITION;
                            cog1Offset.Y -= SPEED_TRANSITION;
                            cog2Offset.Y -= SPEED_TRANSITION;

                            cog1Rotation += SPEED_SELECTOR * -SPEED_ROTATION_TRANSITION;
                            cog2Rotation += SPEED_SELECTOR * SPEED_ROTATION_TRANSITION;

                            if (selectorOffset.Y <= nextSlot.SelectorOffset.Y)
                            {
                                selectMode = SelectorMode.Docked;

                                selectorOffset.Y = nextSlot.SelectorOffset.Y;
                                cog1Offset.Y = ANCHOR_SLOT1_COG_OFFSETY + (STEP_OFFSETY * nextSlotIdx);
                                cog2Offset.Y = cog1Offset.Y;

                                currentSlotIdx = nextSlotIdx;
                            }

                            break;
                        case AutoTransitionType.Down:
                            selectorOffset.Y += SPEED_TRANSITION;
                            cog1Offset.Y += SPEED_TRANSITION;
                            cog2Offset.Y += SPEED_TRANSITION;

                            cog1Rotation -= SPEED_SELECTOR * -SPEED_ROTATION_TRANSITION;
                            cog2Rotation -= SPEED_SELECTOR * SPEED_ROTATION_TRANSITION;

                            if (selectorOffset.Y >= nextSlot.SelectorOffset.Y)
                            {
                                selectMode = SelectorMode.Docked;

                                selectorOffset.Y = nextSlot.SelectorOffset.Y;
                                cog1Offset.Y = ANCHOR_SLOT1_COG_OFFSETY + (STEP_OFFSETY * nextSlotIdx);
                                cog2Offset.Y = cog1Offset.Y;

                                currentSlotIdx = nextSlotIdx;
                            }

                            break;
                    }
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied);

            if (panelState != SlotPanelState.Closed)
            {
                spriteBatch.Draw(cog1Texture,
                                 basePosition + cog1Offset + cogOrigin,
                                 null,
                                 parentScreen.TransitionColor,
                                 cog1Rotation,
                                 cogOrigin,
                                 1f,
                                 SpriteEffects.None,
                                 0);
                spriteBatch.Draw(cog2Texture,
                                 basePosition + cog2Offset + cogOrigin,
                                 null,
                                 parentScreen.TransitionColor,
                                 cog2Rotation,
                                 cogOrigin,
                                 1f,
                                 SpriteEffects.FlipVertically,
                                 0);
                spriteBatch.Draw(plateTexture,
                                 basePosition + cog1Offset,
                                 null,
                                 parentScreen.TransitionColor,
                                 0f,
                                 Vector2.Zero,
                                 1f,
                                 SpriteEffects.FlipVertically,
                                 0);
                spriteBatch.Draw(plateTexture, basePosition + cog2Offset, parentScreen.TransitionColor);
                spriteBatch.Draw(panelTexture, basePosition, parentScreen.TransitionColor);

                foreach (SaveLoadSlot slot in slots)
                {
                    slot.Draw(spriteBatch, basePosition);
                }

                spriteBatch.Draw(selectorTexture, basePosition + selectorOffset, parentScreen.TransitionColor);
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
                  .Append(" selectMode : " + selectMode)
                  .Append('\n')
                  .Append("   autoTransitionType : " + autoTransitionType)
                  .Append('\n')
                  .Append("   currentSlotIdx : " + currentSlotIdx)
                  .Append('\n')
                  .Append("   currentSlot : " + (CurrentSlot.SaveGameDescription != null ? CurrentSlot.SaveGameDescription.FileName : "<empty>"))
                  .Append('\n');

            return retStr.ToString();
        }

    }
}
