using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using ShapeShop.GameEngine;
using System.Text;

namespace ShapeShop.UI
{
    public class CreditsPanel
    {
        public enum CreditsPanelState
        {
            Opening,
            Open,
            Closing,
            Closed,
        }

        private const float ANCHOR_PANEL_X = 140;
        private const float ANCHOR_PANELCLOSED_Y = -1280; //-1280; // -581
        private const float ANCHOR_PANELOPEN_Y = 0;
        private const float SPEED_PANEL = 14f;

        private Texture2D tex_1x1;

        private Viewport viewport;
        public Viewport Viewport
        {
            get { return viewport; }
        }

        private CreditsConsole console;
        public CreditsConsole Console
        {
            get { return console; }
        }

        private CreditsBackground background;
        public CreditsBackground Background
        {
            get { return background; }
        }

        private Rectangle panelBackgroundRect;
        
        private float panelCenter;
        public float PanelCenter
        {
            get { return panelCenter; }
        }

        private Texture2D panelFrameTexture;
        private Vector2 basePosition;

        private MainGameScreen parentScreen;

        private CreditsPanelState panelState = CreditsPanelState.Closed;
        public CreditsPanelState PanelState
        {
            get { return panelState; }
            set { panelState = value; }
        }

        public CreditsPanel(MainGameScreen parentScreen)
        {
            this.parentScreen = parentScreen;
            background = new CreditsBackground(this);
            console = new CreditsConsole(this);
        }

        public void LoadContent(ContentManager content)
        {
            viewport = parentScreen.ScreenManager.GraphicsDevice.Viewport;
            tex_1x1 = content.Load<Texture2D>(@"Textures\1x1");
            panelFrameTexture = content.Load<Texture2D>(@"Textures\GameScreens\endGamePanelFrame");
            basePosition = new Vector2(ANCHOR_PANEL_X, ANCHOR_PANELCLOSED_Y);

            panelCenter = panelFrameTexture.Width / 2;
            panelBackgroundRect = new Rectangle(0, 0, panelFrameTexture.Width, panelFrameTexture.Height);

            background.LoadContent(content);
            console.LoadContent(content);            
        }

        public void Open()
        {
            AudioManager.PlayCue("trackSlideLong");
            panelState = CreditsPanelState.Opening;
            console.LoadStatistics();
        }

        public void Close()
        {
            AudioManager.PlayCue("trackSlideLong");
            panelState = CreditsPanelState.Closing;
        }

        public void Update(GameTime gameTime)
        {
            switch (panelState)
            {
                case CreditsPanelState.Closed:

//                    if (parentScreen.IsLoadConfirmed)
//                    {
//                        parentScreen.IsReadyToLoad = true;                          
//                    }
                    break;
                case CreditsPanelState.Opening:
                    basePosition.Y += SPEED_PANEL;
                    if (basePosition.Y >= ANCHOR_PANELOPEN_Y)
                    {
                        basePosition.Y = ANCHOR_PANELOPEN_Y;
                        panelState = CreditsPanelState.Open;

                        AudioManager.PlayMusic("creditsMusic");
                    }
                    break;
                case CreditsPanelState.Open:
                    handleInput();
                    break;
                case CreditsPanelState.Closing:
                    /*
                    basePosition.Y -= SPEED_PANEL;
                    if (basePosition.Y <= ANCHOR_PANELCLOSED_Y)
                    {
                        basePosition.Y = ANCHOR_PANELCLOSED_Y;
                        panelState = CreditsPanelState.Closed;
                    }
                    */
                    break;
            }

            if (panelState != CreditsPanelState.Closed)
            {
                console.Update(gameTime);
                background.Update(gameTime);
            }

        }

//        public void HandleInput()
//       {
//            handleInput();
//        }

        private void handleInput()
        {
            if (background.State == CreditsBackground.BackgroundState.Finished &&
                console.State == CreditsConsole.ConsoleState.Finished)
            {
                if (InputManager.IsActionTriggered(InputManager.Action.Ok))
                {
                    parentScreen.Mode = MainGameScreen.MainGameScreenMode.Closing;

                    AudioManager.StopMusic();
                    AudioManager.PlayCue("doorClose");
                } 
            }

        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied);

            if (panelState != CreditsPanelState.Closed)
            {
                panelBackgroundRect.X = (int)basePosition.X;
                panelBackgroundRect.Y = (int)basePosition.Y;
                spriteBatch.Draw(tex_1x1, panelBackgroundRect, new Color(1, 1, 1, .5f));//parentScreen.TransitionColor);

                // draw credits, puzzles, stats, etc.
                background.Draw(spriteBatch, basePosition);
                console.Draw(spriteBatch, basePosition);

                // top of frame drawn ontop, to facilitate text scrolling up and off screen
                spriteBatch.Draw(panelFrameTexture, basePosition, parentScreen.TransitionColor);
            }

            spriteBatch.End();
        }

        public string GetDebugInformation()
        {
            StringBuilder retStr = new StringBuilder();

            retStr.Append(" panelState : " + panelState)
                  .Append('\n')
                  .Append(" basePosition : " + basePosition)
                  .Append('\n');

            return retStr.ToString();
        }

    }
}
