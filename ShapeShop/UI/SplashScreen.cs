using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using ShapeShop.GameEngine;
using System;

namespace ShapeShop.UI
{
    public class SplashScreen : GameScreen
    {
        private const double MAX_WAIT_MILLIS = 3500;
        private const float scaleDelta = .0004f;
        private const float scaleMax = 1f;
        private Texture2D splashTexture;
        private Vector2 splashPosition;
        private Vector2 splashOrigin;
        private float splashScale = .7f;
        private bool isScaleMax = false;

        private double counter = 0;
        
        private readonly bool createMainGameScreen = true;

        public SplashScreen()
        {
            TransitionOnTime = TimeSpan.FromSeconds(1);
            TransitionOffTime = TimeSpan.FromSeconds(1);

            InputManager.ClearPlayerIndex();
        }

        public SplashScreen(bool createMainGameScreen) : this()
        {
            this.createMainGameScreen = createMainGameScreen;
        }

        public override void LoadContent()
        {
            ContentManager content = ScreenManager.GlobalContent;
            Viewport viewport = ScreenManager.GraphicsDevice.Viewport;

            splashTexture = content.Load<Texture2D>(@"Textures\GameScreens\splash");
            splashPosition = new Vector2(ShapeShop.PREFERRED_WIDTH / 2, ShapeShop.PREFERRED_HEIGHT / 2);
            splashOrigin = new Vector2(splashTexture.Width / 2, splashTexture.Height / 2);
        }

        public override void HandleInput()
        {
        }

        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            if (ScreenState == ScreenState.Active && counter >= MAX_WAIT_MILLIS)
            {
                ExitScreen();
            }

            counter += gameTime.ElapsedGameTime.TotalMilliseconds;

            if (ScreenState == ScreenState.FinishedExiting)
            {
                if (createMainGameScreen)
                {
                    ScreenManager.AddScreen(new MainGameScreen());
                }
                else
                {
                    PuzzleEngine.MainScreen.Mode = MainGameScreen.MainGameScreenMode.ShowSignIn;
                }
            }

            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);

            if (!isScaleMax)
            {
                if (splashScale >= scaleMax)
                {
                    splashScale = scaleMax;
                    isScaleMax = true;
                }
                else
                {
                    splashScale += scaleDelta;
                }
            }
        }

        /// Draws the menu.
        public override void Draw(GameTime gameTime)
        {
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;

            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied);

            spriteBatch.GraphicsDevice.Clear(Color.Black);
            spriteBatch.Draw(splashTexture, splashPosition, null, TransitionColor, 0, splashOrigin, splashScale, SpriteEffects.None, 0);

            spriteBatch.End();
        }

        public override void UnloadContent()
        {
            base.UnloadContent();
        }

    }
}
