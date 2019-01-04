using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using ShapeShop.GameEngine;
using System;

namespace ShapeShop.UI
{
    /// A popup message box screen, used to display "are you sure?"
    /// confirmation messages.
    class SignOutMessageBoxScreen : GameScreen
    {
        // Fields
        private const float buttonScale = 0.6f;
        private readonly string okText = "OK";

//        private readonly string message = "You have signed out of your\nprofile. Shape Shop requires\nthat you be signed in so that\nit can save your progress.\nSorry, the game will be reset.";
        private readonly string message = "The primary user has\nsigned out. Shape Shop\nrequires a signed-in\nuser so that it can save\nyour progress.\nThe game will now reset.";

        private Texture2D backgroundTexture;
        private Vector2 backgroundPosition;

        private Texture2D loadingBlackTexture;
        private Rectangle loadingBlackTextureDestination;

        private Texture2D selectTexture;
        private Vector2 okTextPosition;
        private Vector2 selectTextPosition;

        private Vector2 messagePosition;

        // Events
        public event EventHandler<EventArgs> Ok;

        // Initialization
        /// Constructor lets the caller specify the message.
        public SignOutMessageBoxScreen()
        {
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

            backgroundTexture = content.Load<Texture2D>(@"Textures\GameScreens\largePopup");
            selectTexture = content.Load<Texture2D>(@"Textures\Buttons\AButton");
            loadingBlackTexture = content.Load<Texture2D>(@"Textures\GameScreens\FadeScreen");

            Viewport viewport = ScreenManager.GraphicsDevice.Viewport;
            backgroundPosition = new Vector2((ShapeShop.PREFERRED_WIDTH - backgroundTexture.Width) / 2,
                                             (ShapeShop.PREFERRED_HEIGHT - backgroundTexture.Height) / 2);
            loadingBlackTextureDestination = new Rectangle(viewport.X, viewport.Y,
                                                           ShapeShop.PREFERRED_WIDTH, ShapeShop.PREFERRED_HEIGHT);
            float framePadding = 20f;
            okTextPosition = backgroundPosition +
                                      new Vector2(-framePadding, -framePadding) +
                                      new Vector2(backgroundTexture.Width - selectTexture.Width * buttonScale,
                                                  backgroundTexture.Height - selectTexture.Height * buttonScale);
            selectTextPosition = new Vector2(okTextPosition.X - Fonts.MessageBoxButtonFont.MeasureString(okText).X, okTextPosition.Y);

//            backPosition = backgroundPosition + new Vector2(framePadding, -framePadding) + new Vector2(0, backgroundTexture.Height - backTexture.Height * buttonScale);
//            backTextPosition = new Vector2(backPosition.X + backTexture.Width * buttonScale, backPosition.Y);

            messagePosition.X = backgroundPosition.X + framePadding * 2;
            messagePosition.Y = backgroundPosition.Y + framePadding + framePadding / 2 - 4;
        }


        // Handle Input

        /// Responds to user input, accepting or cancelling the message box.
        public override void HandleInput()
        {
            if (InputManager.IsActionTriggered(InputManager.Action.Ok))
            {
                // Raise the accepted event, then exit the message box.
                Ok?.Invoke(this, EventArgs.Empty);

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

            spriteBatch.Draw(loadingBlackTexture, loadingBlackTextureDestination, TransitionColor);
            spriteBatch.Draw(backgroundTexture, backgroundPosition, TransitionColor);
            
            /*
            spriteBatch.Draw(backTexture,
                             backPosition,
                             null,
                             TransitionColor,
                             0f,
                             Vector2.Zero,
                             buttonScale,
                             SpriteEffects.None,
                             0);
            */

            spriteBatch.Draw(selectTexture,
                             okTextPosition,
                             null,
                             TransitionColor,
                             0f,
                             Vector2.Zero,
                             buttonScale,
                             SpriteEffects.None,
                             0);

            /*
            spriteBatch.DrawString(Fonts.MessageBoxButtonFont,
                                   okText,
                                   backTextPosition,
                                   new Color(Fonts.ConsoleTextColor.R, Fonts.ConsoleTextColor.G, Fonts.ConsoleTextColor.B, TransitionAlpha));
            */

            spriteBatch.DrawString(Fonts.MessageBoxButtonFont,
                                   okText,
                                   okTextPosition,
                                   new Color(Fonts.ConsoleTextColor.R, Fonts.ConsoleTextColor.G, Fonts.ConsoleTextColor.B, TransitionAlpha));

            spriteBatch.DrawString(Fonts.MessageBoxMessageFont,
                                   message,
                                   messagePosition,
                                   new Color(Fonts.ConsoleTextColor.R, Fonts.ConsoleTextColor.G, Fonts.ConsoleTextColor.B, TransitionAlpha));

            spriteBatch.End();
        }


    }
}
