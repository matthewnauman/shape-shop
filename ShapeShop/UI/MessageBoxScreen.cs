using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using ShapeShop.GameEngine;
using System;

namespace ShapeShop.UI
{
    /// A popup message box screen, used to display "are you sure?"
    /// confirmation messages.
    class MessageBoxScreen : GameScreen
    {

        // refactor this class for dynamic height on boxes

        // Fields
        private const float buttonScale = 0.6f;
        private readonly string selectText = "Yes";
        private readonly string backText = "No";
        private readonly string continueText = "Continue";

        private readonly string message;
        private readonly bool isMessageOnly;

        private Texture2D backgroundTexture;
        private Vector2 backgroundPosition;

        private Texture2D loadingBlackTexture;
        private Rectangle loadingBlackTextureDestination;

        private Texture2D backTexture;
        private Vector2 backPosition;
        private Vector2 backTextPosition;

        private Texture2D selectTexture;
        private Vector2 selectPosition;
        private Vector2 selectTextPosition;

        private Vector2 continueTextPosition;

        private Vector2 messagePosition;

        // Events
        public event EventHandler<EventArgs> Accepted;
        public event EventHandler<EventArgs> Cancelled;


        // Initialization


        /// Constructor lets the caller specify the message.
        public MessageBoxScreen(string message, bool isMessageOnly)
        {
            this.message = message;
            this.isMessageOnly = isMessageOnly;

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

            backgroundTexture = content.Load<Texture2D>(@"Textures\GameScreens\popup");
            backTexture = content.Load<Texture2D>(@"Textures\Buttons\BButton");
            selectTexture = content.Load<Texture2D>(@"Textures\Buttons\AButton");
            loadingBlackTexture = content.Load<Texture2D>(@"Textures\GameScreens\FadeScreen");

            Viewport viewport = ScreenManager.GraphicsDevice.Viewport;
            backgroundPosition = new Vector2((ShapeShop.PREFERRED_WIDTH - backgroundTexture.Width) / 2,
                                             (ShapeShop.PREFERRED_HEIGHT - backgroundTexture.Height) / 2);
            loadingBlackTextureDestination = new Rectangle(viewport.X, viewport.Y,
                                                           ShapeShop.PREFERRED_WIDTH, ShapeShop.PREFERRED_HEIGHT);

            float framePadding = 20f;

            backPosition = backgroundPosition + new Vector2(framePadding, -framePadding) + new Vector2(0, backgroundTexture.Height - backTexture.Height * buttonScale);

            selectPosition = backgroundPosition + new Vector2(-framePadding, -framePadding) + new Vector2(backgroundTexture.Width - backTexture.Width * buttonScale, backgroundTexture.Height - backTexture.Height * buttonScale);

            backTextPosition = new Vector2(backPosition.X + backTexture.Width * buttonScale, backPosition.Y);
            selectTextPosition = new Vector2(selectPosition.X - Fonts.MessageBoxButtonFont.MeasureString(selectText).X, selectPosition.Y);
            continueTextPosition = new Vector2(selectPosition.X - Fonts.MessageBoxButtonFont.MeasureString(continueText).X, selectPosition.Y);

            messagePosition.X = backgroundPosition.X + framePadding + framePadding / 2;
            messagePosition.Y = backgroundPosition.Y + framePadding;
        }


        // Handle Input

        /// Responds to user input, accepting or cancelling the message box.
        public override void HandleInput()
        {
            if (isMessageOnly)
            {
                if (InputManager.IsActionTriggered(InputManager.Action.Ok))
                {
                    // Raise the accepted event, then exit the message box.
                    Accepted?.Invoke(this, EventArgs.Empty);

                    AudioManager.PlayCue("closeWindow");

                    ExitScreen();
                }
            }
            else
            {
                if (InputManager.IsActionTriggered(InputManager.Action.Ok))
                {
                    // Raise the accepted event, then exit the message box.
                    Accepted?.Invoke(this, EventArgs.Empty);

                    AudioManager.PlayCue("closeWindow");

                    ExitScreen();
                }
                else if (InputManager.IsActionTriggered(InputManager.Action.Back))
                {
                    // Raise the cancelled event, then exit the message box.
                    Cancelled?.Invoke(this, EventArgs.Empty);

                    AudioManager.PlayCue("closeWindow");

                    ExitScreen();
                }
            }
        }


        // Draw

        /// Draws the message box.
        public override void Draw(GameTime gameTime)
        {
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;

            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied);

            spriteBatch.Draw(loadingBlackTexture, loadingBlackTextureDestination, TransitionColor);
            spriteBatch.Draw(backgroundTexture, backgroundPosition, TransitionColor);

            if (isMessageOnly)
            {
                spriteBatch.Draw(selectTexture, 
                                 selectPosition, 
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
            }
            else
            {
                spriteBatch.Draw(backTexture, 
                                 backPosition, 
                                 null, 
                                 TransitionColor,
                                 0f, 
                                 Vector2.Zero, 
                                 buttonScale, 
                                 SpriteEffects.None, 
                                 0);

                spriteBatch.Draw(selectTexture, 
                                 selectPosition, 
                                 null, 
                                 TransitionColor,
                                 0f, 
                                 Vector2.Zero, 
                                 buttonScale, 
                                 SpriteEffects.None, 
                                 0);
                
                spriteBatch.DrawString(Fonts.MessageBoxButtonFont,
                                       backText,
                                       backTextPosition,
                                       new Color(Fonts.ConsoleTextColor.R, Fonts.ConsoleTextColor.G, Fonts.ConsoleTextColor.B, TransitionAlpha));
                
                spriteBatch.DrawString(Fonts.MessageBoxButtonFont,
                                       selectText,
                                       selectTextPosition,
                                       new Color(Fonts.ConsoleTextColor.R, Fonts.ConsoleTextColor.G, Fonts.ConsoleTextColor.B, TransitionAlpha));
            }

            spriteBatch.DrawString(Fonts.MessageBoxMessageFont, 
                                   message, 
                                   messagePosition,
                                   new Color(Fonts.ConsoleTextColor.R, Fonts.ConsoleTextColor.G, Fonts.ConsoleTextColor.B, TransitionAlpha));

            spriteBatch.End();
        }


    }
}
