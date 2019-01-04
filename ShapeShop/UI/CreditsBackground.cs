using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using ShapeShop.GameEngine;
using ShapeShopData.Models;
using System.Linq;

namespace ShapeShop.UI
{
    public class CreditsBackground
    {
        public enum BackgroundState
        {
            Waiting,
            Scrolling,
            Finished,
        }

        private readonly Vector2 PORTRAIT_START_OFFSET = new Vector2(21, 698); //22);
        private readonly Vector2 PORTRAIT_TEX_SIZE = new Vector2(319, 229);
        private const float PORTRAIT_ROW_MAX = 3;
        private const float SCROLL_SPEED = .885f; //1
        private const float SCROLL_MAX = 7792; //7786; //7557; //6870;

        private Viewport viewport;
        private CreditsPanel parentPanel;
        private float dScroll = 0;

        private BackgroundState state = BackgroundState.Scrolling;
        public BackgroundState State
        {
            get { return state; }
        }

        public CreditsBackground(CreditsPanel parentPanel)
        {
            this.parentPanel = parentPanel;
        }

        public void LoadContent(ContentManager content)
        {
            viewport = parentPanel.Viewport;
        }

        public void NotifyConsoleStartScroll()
        {
            state = BackgroundState.Scrolling;
        }

        public void Update(GameTime gameTime)
        {
            if (parentPanel.PanelState == CreditsPanel.CreditsPanelState.Open)
            {
                switch (state)
                {
                    case BackgroundState.Waiting:
                        break;
                    case BackgroundState.Scrolling:
                        if (dScroll >= SCROLL_MAX)
                        {
                            dScroll = SCROLL_MAX;
                            parentPanel.Console.NotifyBackgroundFinished();
                            state = BackgroundState.Finished;
                        }
                        else
                        {
                            dScroll += SCROLL_SPEED;
                        }
                        break;
                    case BackgroundState.Finished:
                        break;
                }

            }
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 basePosition)
        {
            float dY = PORTRAIT_START_OFFSET.Y - PORTRAIT_TEX_SIZE.Y;
            float dX = PORTRAIT_START_OFFSET.X;
            Rectangle dstRect;
            Puzzle[] puzzles = PuzzleEngine.CurrentPuzzleSet.Puzzles.ToArray<Puzzle>();

            basePosition.Y += dScroll;

            for (int i = 0; i < puzzles.Length; i += 3)
            {
                dstRect = new Rectangle((int)(dX + basePosition.X), (int)(dY + basePosition.Y), (int)PORTRAIT_TEX_SIZE.X, (int)PORTRAIT_TEX_SIZE.Y);
                spriteBatch.Draw(puzzles[i].PortraitTexture, dstRect, new Color(1,1,1,.60f));

                dX += PORTRAIT_TEX_SIZE.X;
                dstRect = new Rectangle((int)(dX + basePosition.X), (int)(dY + basePosition.Y), (int)PORTRAIT_TEX_SIZE.X, (int)PORTRAIT_TEX_SIZE.Y);
                spriteBatch.Draw(puzzles[i + 1].PortraitTexture, dstRect, new Color(1, 1, 1, .60f));

                dX += PORTRAIT_TEX_SIZE.X;
                dstRect = new Rectangle((int)(dX + basePosition.X), (int)(dY + basePosition.Y), (int)PORTRAIT_TEX_SIZE.X, (int)PORTRAIT_TEX_SIZE.Y);
                spriteBatch.Draw(puzzles[i + 2].PortraitTexture, dstRect, new Color(1, 1, 1, .60f));

                dX = PORTRAIT_START_OFFSET.X;
                dY -= PORTRAIT_TEX_SIZE.Y;
            }

        }
    }
}

