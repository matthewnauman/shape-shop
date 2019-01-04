using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using ShapeShop.GameEngine;
using ShapeShop.GameSession;
using System;

namespace ShapeShop.UI
{
    public enum StorageDeviceWarningType
    {
        StorageIsNull,
        AutoSaveWarn,
    }

    class StorageDeviceWarningScreen : GameScreen
    {
        private const float BUTTON_SCALE = 0.6f;
        private readonly string CONTINUE_TEXT = "Continue";

        private readonly StorageDeviceWarningType type;
        private readonly string message;
        private Texture2D backgroundTexture;
        private Vector2 backgroundPosition;
        private Texture2D loadingBlackTexture;
        private Rectangle loadingBlackTextureDestination;
        private Texture2D storageTexture;
        private Vector2 storageTexturePosition;
        private Texture2D continueTexture;
        private Vector2 continueTexturePosition;
        private Vector2 continueTextPosition;
        private Vector2 messagePosition;

        // Events
        public event EventHandler<EventArgs> Continue;

        public StorageDeviceWarningScreen(StorageDeviceWarningType type)
        {
            this.type = type;

            switch (type)
            {
                case StorageDeviceWarningType.AutoSaveWarn:
                    message = $"Hello {SignInManager.Instance.GamerTag}!\nYour progress will be saved\nautomatically. Do not power\noff or reset your console\nwhile loading or saving.";
                    break;
                case StorageDeviceWarningType.StorageIsNull:
                    message = "You chose not to select a\nstorage device.\nYour progress will not be saved.";
                    break;
            }

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
            continueTexture = content.Load<Texture2D>(@"Textures\Buttons\AButton");
            loadingBlackTexture = content.Load<Texture2D>(@"Textures\GameScreens\FadeScreen");
            storageTexture = content.Load<Texture2D>(@"Textures\GameScreens\disk");

            Viewport viewport = ScreenManager.GraphicsDevice.Viewport;
            backgroundPosition = new Vector2((ShapeShop.PREFERRED_WIDTH - backgroundTexture.Width) / 2,
                                             (ShapeShop.PREFERRED_HEIGHT - backgroundTexture.Height) / 2);
            loadingBlackTextureDestination = new Rectangle(viewport.X, viewport.Y,
                                                           ShapeShop.PREFERRED_WIDTH, ShapeShop.PREFERRED_HEIGHT);
            float framePadding = 15f;
            continueTexturePosition = backgroundPosition + 
                                      new Vector2(-framePadding, -framePadding) + 
                                      new Vector2(backgroundTexture.Width - continueTexture.Width * BUTTON_SCALE, 
                                                  backgroundTexture.Height - continueTexture.Height * BUTTON_SCALE);
            continueTextPosition = new Vector2(continueTexturePosition.X - Fonts.MessageBoxButtonFont.MeasureString(CONTINUE_TEXT).X, continueTexturePosition.Y);
            messagePosition.X = backgroundPosition.X + framePadding * 2;
            messagePosition.Y = backgroundPosition.Y + framePadding + framePadding / 2 - 4;

            storageTexturePosition = new Vector2((ShapeShop.PREFERRED_WIDTH - storageTexture.Width) / 2 + 130, 375);
        }


        // Handle Input
        public override void HandleInput()
        {
            if (InputManager.IsActionPressed(InputManager.Action.Ok) && IsActive)
            {
                // Raise the accepted event, then exit the message box.
                Continue?.Invoke(this, EventArgs.Empty);

                AudioManager.PlayCue("closeWindow");

                ExitScreen();
            }

        }

        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
        }

        /// Draws the message box.
        public override void Draw(GameTime gameTime)
        {
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;

            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied);

            spriteBatch.Draw(loadingBlackTexture, loadingBlackTextureDestination, TransitionColor);
            spriteBatch.Draw(backgroundTexture, backgroundPosition, TransitionColor);

            spriteBatch.Draw(continueTexture, 
                             continueTexturePosition, 
                             null, 
                             TransitionColor,
                             0f, 
                             Vector2.Zero, 
                             BUTTON_SCALE, 
                             SpriteEffects.None, 
                             0);

            spriteBatch.DrawString(Fonts.MessageBoxButtonFont,
                                   CONTINUE_TEXT,
                                   continueTextPosition,
                                   new Color(Fonts.ConsoleTextColor.R, Fonts.ConsoleTextColor.G, Fonts.ConsoleTextColor.B, TransitionAlpha));

            spriteBatch.DrawString(Fonts.MessageBoxMessageFont, 
                                   message, 
                                   messagePosition,
                                   new Color(Fonts.ConsoleTextColor.R, Fonts.ConsoleTextColor.G, Fonts.ConsoleTextColor.B, TransitionAlpha));

            // draw storageTexture if autoSaveWarn type
            if (type == StorageDeviceWarningType.AutoSaveWarn)
            {
                spriteBatch.Draw(storageTexture,
                                 storageTexturePosition,
                                 null,
                                 TransitionColor,
                                 0,
                                 Vector2.Zero,
                                 1f,
                                 SpriteEffects.None, 
                                 0);
            }

            spriteBatch.End();
        }


    }
}
