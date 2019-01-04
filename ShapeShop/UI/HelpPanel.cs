using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace ShapeShop.UI
{
    public class HelpPanel
    {
        private enum HelpState
        {
            FadeOut,
            Off,
            FadeIn,
            On,
        }

        private const float ALPHA_STEP = .075f;
        private readonly Vector2 HELP_TEXTURE_POSITION = new Vector2(984, 25);
        private const string HELP_TEXT = "Help";
        private const float HELP_TEXT_SCALE = 1f;
        private readonly Rectangle HELP_RECTANGLE = new Rectangle(985, 20, 95, 35);
        private readonly Vector2 SM_PANEL_POSITION = new Vector2(1042, 30);
        private readonly Vector2 CURSOR_PANEL_POSITION = new Vector2(135, 30);

        private HelpState helpState = HelpState.Off;
        private MainGameScreen parentScreen;

        private float helpAlpha = 0;

        private Texture2D helpButtonTexture;
        private Vector2 helpTextOffset;

        private Texture2D smPanelTexture;

        public HelpPanel(MainGameScreen parentScreen)
        {
            this.parentScreen = parentScreen;
        }

        public void LoadContent(ContentManager content)
        {
            helpButtonTexture = content.Load<Texture2D>(@"Textures\Buttons\StartButton");
            smPanelTexture = content.Load<Texture2D>(@"Textures\GameScreens\smHelpPanel");

            helpTextOffset = new Vector2(helpButtonTexture.Width + 5, -5);
        }

        public void Update(GameTime gameTime)
        {
            switch (helpState)
            {
                case HelpState.FadeOut:
                    if (helpAlpha == 0)
                    {
                        helpState = HelpState.Off;
                        return;
                    }

                    helpAlpha -= ALPHA_STEP;
                    if (helpAlpha <= 0)
                    {
                        helpAlpha = 0;
                        helpState = HelpState.Off;
                    }
                    break;
                case HelpState.Off:
                    break;
                case HelpState.FadeIn:
                    if (helpAlpha == 1)
                    {
                        helpState = HelpState.On;
                        return;
                    }

                    helpAlpha += ALPHA_STEP;
                    if (helpAlpha >= 1)
                    {
                        helpAlpha = 1;
                        helpState = HelpState.On;
                    }
                    break;
                case HelpState.On:
                    break;
            }
        }

        public void HandleInput()
        {
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(smPanelTexture,
                             SM_PANEL_POSITION,
                             new Color(1, 1, 1, helpAlpha));
        }

        public void SlotPanelGo()
        {
            parentScreen.ScreenManager.AddScreen(new HelpBoxScreen(HelpBoxScreen.HelpType.SlotPanel));
        }

        public void PickerPanelGo()
        {
            parentScreen.ScreenManager.AddScreen(new HelpBoxScreen(HelpBoxScreen.HelpType.PickerPanel));
        }

        public void PuzzlePanelGo()
        {
            parentScreen.ScreenManager.AddScreen(new HelpBoxScreen(HelpBoxScreen.HelpType.PuzzlePanel));
        }

        public void On()
        {
            helpState = HelpState.FadeIn;
        }

        public void Off(bool isFade)
        {
            if (isFade)
            {
                helpState = HelpState.FadeOut;
            }
            else
            {
                helpState = HelpState.Off;
                helpAlpha = 0;
            }
        }

    }
}
