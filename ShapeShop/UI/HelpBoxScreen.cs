using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using ShapeShop.GameEngine;
using System;

namespace ShapeShop.UI
{
    /// A popup message box screen, used to display "are you sure?"
    /// confirmation messages.
    class HelpBoxScreen : GameScreen
    {

        public enum HelpType
        {
            SlotPanel,
            PickerPanel,
            PuzzlePanel,
        }

        private readonly Vector2 smallPanelStartPosition = new Vector2(400, 300);
        private readonly Vector2 largePanelStartPosition = new Vector2(250, 150);
        
        private const float buttonColumnRightMargin_slotPanel = 555f;
        private const float textColumnLeftMargin_slotPanel = 570f;
        private const float buttonColumnRightMargin_pickerPanel = 545f;
        private const float textColumnLeftMargin_pickerPanel = 560f;        
        private const float buttonColumnRightMargin_puzzlePanel = 530f;
        private const float textColumnLeftMargin_puzzlePanel = 545f;

        private const float smallPanelTopIndent = 240f;
        private const float largePanelTopIndent = 120f;

        private const float buttonScale = 0.6f;
        private static readonly string continueText = "Close Help";
        private static readonly string startButtonPuzzleText = "Pause";
        private static readonly string lbrbltrtPuzzleText1 = "Flip & Rotate Selected Shape";
        private static readonly string lbrbltrtPuzzleText2 = "Navigate Cogs";
        private static readonly string lStickPuzzleText1 = "Move Cursor / Navigate Cogs";
        private static readonly string lStickPuzzleText2 = "Move Selected Shape";
        private static readonly string aButtonPuzzleText = "Select Shape / Drop Shape";
        private static readonly string yButtonPuzzleText = "Get a Hint";
        private static readonly string bButtonPuzzleText1 = "Undo Last Shape / Back";
        private static readonly string rStickPuzzleText = "(Click) Reset All Shapes";
        private static readonly string xButtonPuzzleText = "Toggle Cursor Mode";
        private static readonly string lStickDPadSlotText = "Navigate Slots";
        private static readonly string aButtonSlotText = "Select a Slot";
        private static readonly string xButtonSlotText = "Delete a Save Game";
        private static readonly string bButtonSlotText = "Back to Main Menu";
        private static readonly string lbrbPickerText = "Step Through Puzzles";
        private static readonly string ltrtPickerText1 = "Shuffle Puzzles";
        private static readonly string ltrtPickerText2 = "(10 at a time)";
        private static readonly string aButtonPickerText = "Select Puzzle";
        private static readonly string bButtonPickerText = "Back to Main Menu";

        private Vector2 continueTextPosition;
        private Vector2 startButtonPuzzleTextPosition;
        private Vector2 lbrbltrtPuzzleTextPosition1;
        private Vector2 lbrbltrtPuzzleTextPosition2;
        private Vector2 lStickPuzzleTextPosition1;
        private Vector2 lStickPuzzleTextPosition2;
        private Vector2 rStickPuzzleTextPosition;
        private Vector2 aButtonPuzzleTextPosition;
        private Vector2 yButtonPuzzleTextPosition;
        private Vector2 bButtonPuzzleTextPosition1;
        private Vector2 xButtonPuzzleTextPosition;
        private Vector2 lStickDPadSlotTextPosition;
        private Vector2 aButtonSlotTextPosition;
        private Vector2 xButtonSlotTextPosition;
        private Vector2 bButtonSlotTextPosition;
        private Vector2 lbrbPickerTextPosition;
        private Vector2 ltrtPickerTextPosition1;
        private Vector2 ltrtPickerTextPosition2;
        private Vector2 aButtonPickerTextPosition;
        private Vector2 bButtonPickerTextPosition;

        private Texture2D smallPanelTexture;
        private Vector2 smallPanelPosition;
        private Texture2D largePanelTexture;
        private Vector2 largePanelPosition;

        private Texture2D fadeTexture;
        private Rectangle fadeTextureDestination;

        private Texture2D startButtonTexture;
        private Texture2D dpadTexture;
        private Texture2D lStickTexture;
        private Texture2D rStickTexture;
        private Texture2D aButtonTexture;
        private Texture2D bButtonTexture;
        private Texture2D xButtonTexture;
        private Texture2D yButtonTexture;
        private Texture2D lbTexture;
        private Texture2D rbTexture;
        private Texture2D ltTexture;
        private Texture2D rtTexture;

        private Vector2 continueButtonPosition;
        private Vector2 startButtonPosition;
        private Vector2 dpadPosition;
        private Vector2 lStickPosition;
        private Vector2 rStickPosition;
        private Vector2 aButtonPosition;
        private Vector2 bButtonPosition;
        private Vector2 xButtonPosition;
        private Vector2 yButtonPosition;
        private Vector2 lbButtonPosition;
        private Vector2 rbButtonPosition;
        private Vector2 ltButtonPosition;
        private Vector2 rtButtonPosition;
        
        private readonly HelpType type;

        private float puzzlePanelLineHeight;
        private float pickerPanelLineHeight;

        // Events
        public event EventHandler<EventArgs> Close;

        // Initialization
        /// Constructor lets the caller specify the message.
        public HelpBoxScreen(HelpType type)
        {
            this.type = type;
            IsPopup = true;

            TransitionOnTime = TimeSpan.FromSeconds(.5);
            TransitionOffTime = TimeSpan.FromSeconds(.5);

            AudioManager.PlayCue("openWindow");
        }


        /// Loads graphics content for this screen. This uses the shared ContentManager
        /// provided by the Game class, so the content will remain loaded forever.
        /// Whenever a subsequent MessageBoxScreen tries to load this same content,
        /// it will just get back another reference to the already loaded data.
        public override void LoadContent()
        {
            ContentManager content = ScreenManager.GlobalContent;
            Viewport viewport = ScreenManager.GraphicsDevice.Viewport;

            puzzlePanelLineHeight = Fonts.Console24Font.MeasureString("X").Y;
            pickerPanelLineHeight = Fonts.Console24Font.MeasureString("X").Y;

            // needed for all types
            fadeTexture = content.Load<Texture2D>(@"Textures\GameScreens\FadeScreen");
            bButtonTexture = content.Load<Texture2D>(@"Textures\Buttons\BButton");
            aButtonTexture = content.Load<Texture2D>(@"Textures\Buttons\AButton");

            fadeTextureDestination = new Rectangle(viewport.X, viewport.Y, ShapeShop.PREFERRED_WIDTH, ShapeShop.PREFERRED_HEIGHT);
            float framePadding = 20f;

            switch (type)
            {
                case HelpType.SlotPanel:
                    smallPanelTexture = content.Load<Texture2D>(@"Textures\GameScreens\helpPopupSmall");
                    smallPanelPosition = new Vector2((ShapeShop.PREFERRED_WIDTH - smallPanelTexture.Width) / 2,
                                                     (ShapeShop.PREFERRED_HEIGHT - smallPanelTexture.Height) / 2);

                    lStickTexture = content.Load<Texture2D>(@"Textures\Buttons\LeftStick");
                    dpadTexture = content.Load<Texture2D>(@"Textures\Buttons\dpad");
                    xButtonTexture = content.Load<Texture2D>(@"Textures\Buttons\XButton");

                    dpadPosition = new Vector2(buttonColumnRightMargin_slotPanel - dpadTexture.Width, smallPanelTopIndent);
                    lStickPosition = new Vector2(dpadPosition.X - lStickTexture.Width, smallPanelTopIndent);
                    lStickDPadSlotTextPosition = new Vector2(textColumnLeftMargin_slotPanel, dpadPosition.Y + 17);

                    aButtonPosition = new Vector2(buttonColumnRightMargin_slotPanel - aButtonTexture.Width, smallPanelTopIndent + dpadTexture.Height);
                    aButtonSlotTextPosition = new Vector2(textColumnLeftMargin_slotPanel, aButtonPosition.Y + 7);

                    xButtonPosition = new Vector2(buttonColumnRightMargin_slotPanel - xButtonTexture.Width, aButtonPosition.Y + aButtonTexture.Height);
                    xButtonSlotTextPosition = new Vector2(textColumnLeftMargin_slotPanel, xButtonPosition.Y + 7);
                    
                    bButtonPosition = new Vector2(buttonColumnRightMargin_slotPanel - bButtonTexture.Width, xButtonPosition.Y + xButtonTexture.Height);
                    bButtonSlotTextPosition = new Vector2(textColumnLeftMargin_slotPanel, bButtonPosition.Y + 7);

                    continueButtonPosition = smallPanelPosition + 
                                             new Vector2(framePadding, -framePadding) + 
                                             new Vector2(0, smallPanelTexture.Height - bButtonTexture.Height * buttonScale);

                    break;

                case HelpType.PickerPanel:
                    smallPanelTexture = content.Load<Texture2D>(@"Textures\GameScreens\helpPopupSmall");
                    smallPanelPosition = new Vector2((ShapeShop.PREFERRED_WIDTH - smallPanelTexture.Width) / 2,
                                                     (ShapeShop.PREFERRED_HEIGHT - smallPanelTexture.Height) / 2);

                    lbTexture = content.Load<Texture2D>(@"Textures\Buttons\LeftBumperButton");
                    rbTexture = content.Load<Texture2D>(@"Textures\Buttons\RightBumperButton");
                    ltTexture = content.Load<Texture2D>(@"Textures\Buttons\LeftTriggerButton");
                    rtTexture = content.Load<Texture2D>(@"Textures\Buttons\RightTriggerButton");

                    rbButtonPosition = new Vector2(buttonColumnRightMargin_pickerPanel - rbTexture.Width, smallPanelTopIndent);
                    lbButtonPosition = new Vector2(rbButtonPosition.X - lbTexture.Width, smallPanelTopIndent);
                    lbrbPickerTextPosition = new Vector2(textColumnLeftMargin_pickerPanel, smallPanelTopIndent + 2);

                    rtButtonPosition = new Vector2(buttonColumnRightMargin_pickerPanel - rtTexture.Width, smallPanelTopIndent + lbTexture.Height);
                    ltButtonPosition = new Vector2(rtButtonPosition.X - ltTexture.Width, smallPanelTopIndent + lbTexture.Height);
                    ltrtPickerTextPosition1 = new Vector2(textColumnLeftMargin_pickerPanel, lbrbPickerTextPosition.Y + lbTexture.Height);
                    ltrtPickerTextPosition2 = new Vector2(textColumnLeftMargin_pickerPanel, ltrtPickerTextPosition1.Y + pickerPanelLineHeight);

                    aButtonPosition = new Vector2(buttonColumnRightMargin_pickerPanel - aButtonTexture.Width, ltrtPickerTextPosition2.Y + pickerPanelLineHeight + 10);
                    aButtonPickerTextPosition = new Vector2(textColumnLeftMargin_pickerPanel, ltrtPickerTextPosition2.Y + pickerPanelLineHeight + 10 + 5);

                    bButtonPosition = new Vector2(buttonColumnRightMargin_pickerPanel - bButtonTexture.Width, aButtonPosition.Y + aButtonTexture.Height);
                    bButtonPickerTextPosition = new Vector2(textColumnLeftMargin_pickerPanel, aButtonPickerTextPosition.Y + aButtonTexture.Height);

                    continueButtonPosition = smallPanelPosition +
                                             new Vector2(framePadding, -framePadding) +
                                             new Vector2(0, smallPanelTexture.Height - bButtonTexture.Height * buttonScale);

                    break;

                case HelpType.PuzzlePanel:
                    largePanelTexture = content.Load<Texture2D>(@"Textures\GameScreens\helpPopupLarge");
                    largePanelPosition = new Vector2((ShapeShop.PREFERRED_WIDTH - largePanelTexture.Width) / 2,
                                                     (ShapeShop.PREFERRED_HEIGHT - largePanelTexture.Height) / 2);
    
                    startButtonTexture = content.Load<Texture2D>(@"Textures\Buttons\StartButton");
                    lbTexture = content.Load<Texture2D>(@"Textures\Buttons\LeftBumperButton");
                    rbTexture = content.Load<Texture2D>(@"Textures\Buttons\RightBumperButton");
                    ltTexture = content.Load<Texture2D>(@"Textures\Buttons\LeftTriggerButton");
                    rtTexture = content.Load<Texture2D>(@"Textures\Buttons\RightTriggerButton");

                    lStickTexture = content.Load<Texture2D>(@"Textures\Buttons\LeftStick");
                    rStickTexture = content.Load<Texture2D>(@"Textures\Buttons\RightStick");
                    dpadTexture = content.Load<Texture2D>(@"Textures\Buttons\dpad");

                    yButtonTexture = content.Load<Texture2D>(@"Textures\Buttons\YButton");
                    xButtonTexture = content.Load<Texture2D>(@"Textures\Buttons\XButton");

                    startButtonPosition = new Vector2(buttonColumnRightMargin_puzzlePanel - startButtonTexture.Width, largePanelTopIndent);
                    startButtonPuzzleTextPosition = new Vector2(textColumnLeftMargin_puzzlePanel, largePanelTopIndent);

                    lStickPosition = new Vector2(buttonColumnRightMargin_puzzlePanel - lStickTexture.Width, largePanelTopIndent + startButtonTexture.Height + 10);
                    lStickPuzzleTextPosition1 = new Vector2(textColumnLeftMargin_puzzlePanel, lStickPosition.Y);
                    lStickPuzzleTextPosition2 = new Vector2(textColumnLeftMargin_puzzlePanel, lStickPosition.Y + puzzlePanelLineHeight);

                    rStickPosition = new Vector2(buttonColumnRightMargin_puzzlePanel - rStickTexture.Width, lStickPuzzleTextPosition2.Y + puzzlePanelLineHeight + 10);
                    rStickPuzzleTextPosition = new Vector2(textColumnLeftMargin_puzzlePanel, rStickPosition.Y + 15);

                    rtButtonPosition = new Vector2(buttonColumnRightMargin_puzzlePanel - rtTexture.Width, rStickPosition.Y + rStickTexture.Height + 10);
                    ltButtonPosition = new Vector2(rtButtonPosition.X - ltTexture.Width, rtButtonPosition.Y);
                    rbButtonPosition = new Vector2(ltButtonPosition.X - rbTexture.Width, rtButtonPosition.Y);
                    lbButtonPosition = new Vector2(rbButtonPosition.X - lbTexture.Width, rtButtonPosition.Y);
                    lbrbltrtPuzzleTextPosition1 = new Vector2(textColumnLeftMargin_puzzlePanel, rtButtonPosition.Y);
                    lbrbltrtPuzzleTextPosition2 = new Vector2(textColumnLeftMargin_puzzlePanel, rtButtonPosition.Y + puzzlePanelLineHeight);

                    aButtonPosition = new Vector2(buttonColumnRightMargin_puzzlePanel - aButtonTexture.Width, lbrbltrtPuzzleTextPosition2.Y + puzzlePanelLineHeight + 10);
                    aButtonPuzzleTextPosition = new Vector2(textColumnLeftMargin_puzzlePanel, aButtonPosition.Y + 7);

                    xButtonPosition = new Vector2(buttonColumnRightMargin_puzzlePanel - xButtonTexture.Width, aButtonPosition.Y + aButtonTexture.Height);
                    xButtonPuzzleTextPosition = new Vector2(textColumnLeftMargin_puzzlePanel, xButtonPosition.Y + 7);

                    yButtonPosition = new Vector2(buttonColumnRightMargin_puzzlePanel - yButtonTexture.Width, xButtonPosition.Y + xButtonTexture.Height);
                    yButtonPuzzleTextPosition = new Vector2(textColumnLeftMargin_puzzlePanel, yButtonPosition.Y + 7);

                    bButtonPosition = new Vector2(buttonColumnRightMargin_puzzlePanel - bButtonTexture.Width, yButtonPosition.Y + yButtonTexture.Height);
                    bButtonPuzzleTextPosition1 = new Vector2(textColumnLeftMargin_puzzlePanel, bButtonPosition.Y + 7);

                    continueButtonPosition = largePanelPosition +
                                             new Vector2(framePadding, -framePadding) +
                                             new Vector2(0, largePanelTexture.Height - bButtonTexture.Height * buttonScale);

                    break;

            }

            continueTextPosition = new Vector2(continueButtonPosition.X + bButtonTexture.Width * buttonScale + 3, continueButtonPosition.Y);


        }

        // Handle Input
        /// Responds to user input, accepting or cancelling the message box.
        public override void HandleInput()
        {
            if (InputManager.IsActionTriggered(InputManager.Action.Back) || 
                InputManager.IsActionTriggered(InputManager.Action.Help))
            {
                // Raise the cancelled event, then exit the message box.
                Close?.Invoke(this, EventArgs.Empty);

                AudioManager.PlayCue("closeWindow");

                ExitScreen();
            }
        }


        // Draw
        /// Draws the message box.
        public override void Draw(GameTime gameTime)
        {
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;

            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied);

            spriteBatch.Draw(fadeTexture, fadeTextureDestination, TransitionColor);

            switch (type)
            {
                case HelpType.SlotPanel:
                    spriteBatch.Draw(smallPanelTexture, smallPanelPosition, TransitionColor);
                    break;
                case HelpType.PickerPanel:
                    spriteBatch.Draw(smallPanelTexture, smallPanelPosition, TransitionColor);
                    break;
                case HelpType.PuzzlePanel:
                    spriteBatch.Draw(largePanelTexture, largePanelPosition, TransitionColor);
                    break;
            }

            spriteBatch.Draw(bButtonTexture, 
                             continueButtonPosition, 
                             null, 
                             TransitionColor,
                             0f, 
                             Vector2.Zero, 
                             buttonScale, 
                             SpriteEffects.None, 
                             0);
                
            spriteBatch.DrawString(Fonts.MessageBoxButtonFont,
                                   continueText,
                                   continueTextPosition,
                                   new Color(Fonts.ConsoleTextColor.R, Fonts.ConsoleTextColor.G, Fonts.ConsoleTextColor.B, TransitionAlpha));


            switch (type)
            {
                case HelpType.SlotPanel:
                    
                    spriteBatch.Draw(lStickTexture,
                                     lStickPosition,
                                     TransitionColor);
                    spriteBatch.Draw(dpadTexture,
                                     dpadPosition,
                                     TransitionColor);
                    spriteBatch.DrawString(Fonts.Console24Font,
                                           lStickDPadSlotText,
                                           lStickDPadSlotTextPosition,
                                           new Color(Fonts.ConsoleTextColor.R, Fonts.ConsoleTextColor.G, Fonts.ConsoleTextColor.B, TransitionAlpha));

                    spriteBatch.Draw(aButtonTexture,
                                     aButtonPosition,
                                     TransitionColor);
                    spriteBatch.DrawString(Fonts.Console24Font,
                                           aButtonSlotText,
                                           aButtonSlotTextPosition,
                                           new Color(Fonts.ConsoleTextColor.R, Fonts.ConsoleTextColor.G, Fonts.ConsoleTextColor.B, TransitionAlpha));

                    spriteBatch.Draw(xButtonTexture,
                                     xButtonPosition,
                                     TransitionColor);
                    spriteBatch.DrawString(Fonts.Console24Font,
                                           xButtonSlotText,
                                           xButtonSlotTextPosition,
                                           new Color(Fonts.ConsoleTextColor.R, Fonts.ConsoleTextColor.G, Fonts.ConsoleTextColor.B, TransitionAlpha));
    
                    spriteBatch.Draw(bButtonTexture,
                                     bButtonPosition,
                                     TransitionColor);
                    spriteBatch.DrawString(Fonts.Console24Font,
                                           bButtonSlotText,
                                           bButtonSlotTextPosition,
                                           new Color(Fonts.ConsoleTextColor.R, Fonts.ConsoleTextColor.G, Fonts.ConsoleTextColor.B, TransitionAlpha));
                                     
                    break;

                case HelpType.PickerPanel:

                    spriteBatch.Draw(lbTexture,
                                     lbButtonPosition,
                                     TransitionColor);
                    spriteBatch.Draw(rbTexture,
                                     rbButtonPosition,
                                     TransitionColor);
                    spriteBatch.DrawString(Fonts.Console24Font,
                                           lbrbPickerText,
                                           lbrbPickerTextPosition,
                                           new Color(Fonts.ConsoleTextColor.R, Fonts.ConsoleTextColor.G, Fonts.ConsoleTextColor.B, TransitionAlpha));

                    spriteBatch.Draw(ltTexture,
                                     ltButtonPosition,
                                     TransitionColor);
                    spriteBatch.Draw(rtTexture,
                                     rtButtonPosition,
                                     TransitionColor);
                    spriteBatch.DrawString(Fonts.Console24Font,
                                           ltrtPickerText1,
                                           ltrtPickerTextPosition1,
                                           new Color(Fonts.ConsoleTextColor.R, Fonts.ConsoleTextColor.G, Fonts.ConsoleTextColor.B, TransitionAlpha));
                    spriteBatch.DrawString(Fonts.Console24Font,
                                           ltrtPickerText2,
                                           ltrtPickerTextPosition2,
                                           new Color(Fonts.ConsoleTextColor.R, Fonts.ConsoleTextColor.G, Fonts.ConsoleTextColor.B, TransitionAlpha));

                    spriteBatch.Draw(aButtonTexture,
                                     aButtonPosition,
                                     TransitionColor);
                    spriteBatch.DrawString(Fonts.Console24Font,
                                           aButtonPickerText,
                                           aButtonPickerTextPosition,
                                           new Color(Fonts.ConsoleTextColor.R, Fonts.ConsoleTextColor.G, Fonts.ConsoleTextColor.B, TransitionAlpha));
    
                    spriteBatch.Draw(bButtonTexture,
                                     bButtonPosition,
                                     TransitionColor);
                    spriteBatch.DrawString(Fonts.Console24Font,
                                           bButtonPickerText,
                                           bButtonPickerTextPosition,
                                           new Color(Fonts.ConsoleTextColor.R, Fonts.ConsoleTextColor.G, Fonts.ConsoleTextColor.B, TransitionAlpha));
                                     
                    break;

                case HelpType.PuzzlePanel:

                    spriteBatch.Draw(startButtonTexture,
                                     startButtonPosition,
                                     TransitionColor);
                    spriteBatch.DrawString(Fonts.Console24Font,
                                           startButtonPuzzleText,
                                           startButtonPuzzleTextPosition,
                                           new Color(Fonts.ConsoleTextColor.R, Fonts.ConsoleTextColor.G, Fonts.ConsoleTextColor.B, TransitionAlpha));

                    spriteBatch.Draw(lStickTexture,
                                     lStickPosition,
                                     TransitionColor);
                    spriteBatch.DrawString(Fonts.Console24Font,
                                           lStickPuzzleText1,
                                           lStickPuzzleTextPosition1,
                                           new Color(Fonts.ConsoleTextColor.R, Fonts.ConsoleTextColor.G, Fonts.ConsoleTextColor.B, TransitionAlpha));
                    spriteBatch.DrawString(Fonts.Console24Font,
                                           lStickPuzzleText2,
                                           lStickPuzzleTextPosition2,
                                           new Color(Fonts.ConsoleTextColor.R, Fonts.ConsoleTextColor.G, Fonts.ConsoleTextColor.B, TransitionAlpha));

                    spriteBatch.Draw(rStickTexture,
                                     rStickPosition,
                                     TransitionColor);
                    spriteBatch.DrawString(Fonts.Console24Font,
                                           rStickPuzzleText,
                                           rStickPuzzleTextPosition,
                                           new Color(Fonts.ConsoleTextColor.R, Fonts.ConsoleTextColor.G, Fonts.ConsoleTextColor.B, TransitionAlpha));

                    spriteBatch.Draw(lbTexture,
                                     lbButtonPosition,
                                     TransitionColor);
                    spriteBatch.Draw(rbTexture,
                                     rbButtonPosition,
                                     TransitionColor);
                    spriteBatch.Draw(ltTexture,
                                     ltButtonPosition,
                                     TransitionColor);
                    spriteBatch.Draw(rtTexture,
                                     rtButtonPosition,
                                     TransitionColor);
                    spriteBatch.DrawString(Fonts.Console24Font,
                                           lbrbltrtPuzzleText1,
                                           lbrbltrtPuzzleTextPosition1,
                                           new Color(Fonts.ConsoleTextColor.R, Fonts.ConsoleTextColor.G, Fonts.ConsoleTextColor.B, TransitionAlpha));
                    spriteBatch.DrawString(Fonts.Console24Font,
                                           lbrbltrtPuzzleText2,
                                           lbrbltrtPuzzleTextPosition2,
                                           new Color(Fonts.ConsoleTextColor.R, Fonts.ConsoleTextColor.G, Fonts.ConsoleTextColor.B, TransitionAlpha));

                    spriteBatch.Draw(aButtonTexture,
                                     aButtonPosition,
                                     TransitionColor);
                    spriteBatch.DrawString(Fonts.Console24Font,
                                           aButtonPuzzleText,
                                           aButtonPuzzleTextPosition,
                                           new Color(Fonts.ConsoleTextColor.R, Fonts.ConsoleTextColor.G, Fonts.ConsoleTextColor.B, TransitionAlpha));

                    spriteBatch.Draw(yButtonTexture,
                                     yButtonPosition,
                                     TransitionColor);
                    spriteBatch.DrawString(Fonts.Console24Font,
                                           yButtonPuzzleText,
                                           yButtonPuzzleTextPosition,
                                           new Color(Fonts.ConsoleTextColor.R, Fonts.ConsoleTextColor.G, Fonts.ConsoleTextColor.B, TransitionAlpha));

                    spriteBatch.Draw(xButtonTexture,
                                     xButtonPosition,
                                     TransitionColor);
                    spriteBatch.DrawString(Fonts.Console24Font,
                                           xButtonPuzzleText,
                                           xButtonPuzzleTextPosition,
                                           new Color(Fonts.ConsoleTextColor.R, Fonts.ConsoleTextColor.G, Fonts.ConsoleTextColor.B, TransitionAlpha));

                    spriteBatch.Draw(bButtonTexture,
                                     bButtonPosition,
                                     TransitionColor);
                    spriteBatch.DrawString(Fonts.Console24Font,
                                           bButtonPuzzleText1,
                                           bButtonPuzzleTextPosition1,
                                           new Color(Fonts.ConsoleTextColor.R, Fonts.ConsoleTextColor.G, Fonts.ConsoleTextColor.B, TransitionAlpha));
                                     
                    break; 

            } 

            spriteBatch.End();
        }

    }
}
