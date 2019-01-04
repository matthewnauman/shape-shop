using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using ShapeShop.GameEngine;
using System;

namespace ShapeShop.UI
{
    /// A popup message box screen, used to display "are you sure?"
    /// confirmation messages.
    class SigningInMessageBoxScreen : GameScreen
    {
        // Fields
        private const float buttonScale = 0.6f;

        private readonly string message = "Signing in...";

        private Texture2D backgroundTexture;
        private Vector2 backgroundPosition;

        private Texture2D loadingBlackTexture;
        private Rectangle loadingBlackTextureDestination;

        private Vector2 messagePosition;

        // Initialization
        /// Constructor lets the caller specify the message.
        public SigningInMessageBoxScreen()
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
            loadingBlackTexture = content.Load<Texture2D>(@"Textures\GameScreens\FadeScreen");

            Viewport viewport = ScreenManager.GraphicsDevice.Viewport;
            backgroundPosition = new Vector2((ShapeShop.PREFERRED_WIDTH - backgroundTexture.Width) / 2,
                                             (ShapeShop.PREFERRED_HEIGHT - backgroundTexture.Height) / 2);
            loadingBlackTextureDestination = new Rectangle(viewport.X, viewport.Y,
                                                           ShapeShop.PREFERRED_WIDTH, ShapeShop.PREFERRED_HEIGHT);
            float framePadding = 20f;

//            backPosition = backgroundPosition + new Vector2(framePadding, -framePadding) + new Vector2(0, backgroundTexture.Height - backTexture.Height * buttonScale);
//            backTextPosition = new Vector2(backPosition.X + backTexture.Width * buttonScale, backPosition.Y);

            messagePosition.X = backgroundPosition.X + framePadding * 2;
            messagePosition.Y = backgroundPosition.Y + framePadding + framePadding / 2 - 4;
        }

        /// Draws the message box.
        public override void Draw(GameTime gameTime)
        {
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;

            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied);

            spriteBatch.Draw(loadingBlackTexture, loadingBlackTextureDestination, TransitionColor);
            spriteBatch.Draw(backgroundTexture, backgroundPosition, TransitionColor);
            
            spriteBatch.DrawString(Fonts.MessageBoxMessageFont,
                                   message,
                                   messagePosition,
                                   new Color(Fonts.ConsoleTextColor.R, Fonts.ConsoleTextColor.G, Fonts.ConsoleTextColor.B, TransitionAlpha));

            spriteBatch.End();
        }


    }
}
